using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;

using Dig.Entities;
using Dig.Input;
using Dig.Renderer;
using Dig.Renderer.Models;
using Dig.Renderer.Texturing;

using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

using D3D11Buffer = SharpDX.Direct3D11.Buffer;
using D3D11RasterizerState2 = SharpDX.Direct3D11.RasterizerState2;
using D3D11RasterizerStateDescription2 = SharpDX.Direct3D11.RasterizerStateDescription2;
using D3D11SamplerState = SharpDX.Direct3D11.SamplerState;
using D3D11SamplerStateDescription = SharpDX.Direct3D11.SamplerStateDescription;
using Texture2D = SharpDX.Direct3D11.Texture2D;

namespace Dig
{
	[StructLayout(LayoutKind.Sequential)]
	[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
	public struct PerObject : IConstants
	{
		public Matrix Model;
	}

	[StructLayout(LayoutKind.Sequential)]
	[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
	public struct PerFrame : IConstants
	{
		public Matrix ViewProjection;
	}

	[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
	public sealed class SimpleObject
	{
		public readonly Mesh Mesh;
		public readonly Material Material;

		public PerObject PerObject => MakePerObject();

		public Transform Transform;
		public int TriangleOffset;
		public int VertexOffset;

		public SimpleObject(Mesh mesh, Material material)
		{
			Mesh = mesh;
			Material = material;
		}

		private PerObject MakePerObject()
		{
			var model = Transform.Matrix;
			model.Transpose();

			return new PerObject
			{
				Model = model
			};
		}
	}

	[SuppressMessage("ReSharper", "NotAccessedField.Local")]
	public sealed class Game : IDisposable
	{
		private readonly DXContext _dx;
		private readonly InputState _input;

		private readonly Material _unlit;
		private readonly List<SimpleObject> _objects;
		private readonly int _vertexCount;
		private readonly int _triangleCount;

		private readonly VertexBuffer _vertexBuffer;
		private readonly IndexBuffer _indexBuffer;
		private readonly ConstantBuffer<PerObject> _perObjectBuffer;
		private readonly ConstantBuffer<PerFrame> _perFrameBuffer;
		private readonly D3D11Buffer[] _cbuffers;
		private readonly D3D11RasterizerState2 _rsWireframe;
		private readonly TextureAtlas _blocksAtlas;
		private readonly D3D11SamplerState _tsCube;

		private double _rotation;

		public Game(DXContext dx, InputState input)
		{
			_dx = dx;
			_input = input;

			_unlit = new Material(dx, "Unlit");
			_unlit.DebugName = $"{nameof(Game)}.{nameof(_unlit)}";

			_blocksAtlas = TextureAtlas.Load(dx, "Assets/Atlases/Blocks");
			_blocksAtlas.DebugName = $"{nameof(Game)}.{nameof(_blocksAtlas)}";

			var dirt = _blocksAtlas["dirt"];
			var sand = _blocksAtlas["sand"];
			var stone = _blocksAtlas["stone"];

			var dirtCube = MeshBuilder.Cube(dirt, dirt, dirt);
			var stoneCube = MeshBuilder.Cube(stone, stone, stone);
			var sandCube = MeshBuilder.Cube(sand, sand, sand);

			_objects = new List<SimpleObject>
			{
				new SimpleObject(stoneCube, _unlit)
				{
					Transform =
					{
						Position = new Vector3(2, 0, 0),
						Scale = Vector3.One
					}
				},
				new SimpleObject(sandCube, _unlit)
				{
					Transform =
					{
						Position = new Vector3(-2, 0, 0),
						Scale = new Vector3(1.3f, 1.3f, 1.3f)
					}
				}
			};

			_vertexCount = _objects.Sum(o => o.Mesh.Vertices.Length);
			_triangleCount = _objects.Sum(o => o.Mesh.Triangles.Length);

			_vertexBuffer = new VertexBuffer(_dx, _vertexCount, false);
			_vertexBuffer.DebugName = $"{nameof(Game)}.{nameof(_vertexBuffer)}";

			_indexBuffer = new IndexBuffer(_dx, _triangleCount, false);
			_indexBuffer.DebugName = $"{nameof(Game)}.{nameof(_indexBuffer)}";

			_perObjectBuffer = new ConstantBuffer<PerObject>(dx, false);
			_perObjectBuffer.DebugName = $"{nameof(Game)}.{nameof(_perObjectBuffer)}";

			_perFrameBuffer = new ConstantBuffer<PerFrame>(dx, false);
			_perFrameBuffer.DebugName = $"{nameof(Game)}.{nameof(_perFrameBuffer)}";

			_cbuffers = new[]
			{
				_perFrameBuffer.Buffer,
				_perObjectBuffer.Buffer
			};

			var rsWireframe = new D3D11RasterizerStateDescription2
			{
				CullMode = CullMode.None,
				FillMode = FillMode.Wireframe
			};

			_rsWireframe = new D3D11RasterizerState2(dx.Device, rsWireframe);
			_rsWireframe.DebugName = $"{nameof(Game)}.{nameof(_rsWireframe)}";

			var tsCube = new D3D11SamplerStateDescription
			{
				Filter = Filter.MinMagMipPoint,
				MaximumAnisotropy = 8,
				AddressU = TextureAddressMode.Clamp,
				AddressV = TextureAddressMode.Clamp,
				AddressW = TextureAddressMode.Clamp,
				MinimumLod = 0,
				MaximumLod = 0
			};

			// TODO put on material
			_tsCube = new D3D11SamplerState(dx.Device, tsCube);
			_tsCube.DebugName = $"{nameof(Game)}.{nameof(_tsCube)}";

			var vertexOffset = 0;
			var triangleOffset = 0;
			foreach (var obj in _objects)
			{
				obj.TriangleOffset = triangleOffset;
				obj.VertexOffset = vertexOffset;
				_vertexBuffer.Upload(obj.Mesh.Vertices, vertexOffset);
				_indexBuffer.Upload(obj.Mesh.Triangles, triangleOffset);
				vertexOffset += obj.Mesh.Vertices.Length;
				triangleOffset += obj.Mesh.Triangles.Length;
			}
		}

		public void Dispose()
		{
			_rsWireframe.Dispose();
			_tsCube.Dispose();
			_blocksAtlas.Dispose();
			_indexBuffer.Dispose();
			_vertexBuffer.Dispose();
			_rsWireframe.Dispose();
			_perObjectBuffer.Dispose();
			_perFrameBuffer.Dispose();
			_unlit.Dispose();
		}

		public void UpdateFixed(double dt)
		{
			_rotation += dt * 0.5;

			if (_rotation >= Math.PI * 2)
			{
				_rotation = 0;
			}

			_objects[0].Transform.Rotation = Quaternion.RotationAxis(Vector3.Up, (float)_rotation);
			_objects[1].Transform.Rotation = Quaternion.RotationAxis(Vector3.Down, (float)_rotation);
		}

		public void Update(double dt)
		{
		}

		public void Render3D(DXWindowContext dx)
		{
			var ia = dx.Context.InputAssembler;
			var vs = dx.Context.VertexShader;
			var ps = dx.Context.PixelShader;
			var rs = dx.Context.Rasterizer;

			var eye = new Vector3(0, 3.0f, -12f);
			var target = new Vector3(0, 0, 0);
			var view = Matrix.LookAtLH(eye, target, Vector3.Up);
			var projection = Matrix.PerspectiveFovLH(75, dx.AspectRatio, 0.03f, 50f);

			var vp = view * projection;
			vp.Transpose();

			var perFrame = new PerFrame
			{
				ViewProjection = vp
			};

			_perFrameBuffer.Upload(ref perFrame);

			ia.InputLayout = _vertexBuffer.Layout;
			ia.PrimitiveTopology = PrimitiveTopology.TriangleList;
			ia.SetVertexBuffers(0, _vertexBuffer.Binding());
			ia.SetIndexBuffer(_indexBuffer.Buffer, Format.R32_UInt, 0);

			vs.Set(_unlit.VertexShader.Shader);
			vs.SetConstantBuffers(0, _cbuffers.Length, _cbuffers);

			ps.Set(_unlit.PixelShader.Shader);
			ps.SetSampler(0, _tsCube);
			ps.SetShaderResource(0, _blocksAtlas.View);

			// rs.State = _rsWireframe;

			foreach (var obj in _objects)
			{
				var perObject = obj.PerObject;
				_perObjectBuffer.Upload(ref perObject);

				var count = obj.Mesh.Triangles.Length * 3;
				var offset = obj.TriangleOffset * 3;
				dx.Context.DrawIndexed(count, offset, obj.VertexOffset);
			}
		}

		public void Render2D(DXWindowContext dx)
		{
		}
	}
}
