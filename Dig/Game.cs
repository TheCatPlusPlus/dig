using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using Dig.Entities;
using Dig.Input;
using Dig.Renderer;
using Dig.Renderer.Models;

using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

using D3D11Buffer = SharpDX.Direct3D11.Buffer;
using D3D11RasterizerState2 = SharpDX.Direct3D11.RasterizerState2;
using D3D11RasterizerStateDescription2 = SharpDX.Direct3D11.RasterizerStateDescription2;
using Texture2D = Dig.Renderer.Texture2D;

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
		private readonly Texture2D _atlas;

		private double _rotation;

		public Game(DXContext dx, InputState input)
		{
			_dx = dx;
			_input = input;

			_unlit = new Material(dx, "Unlit");
			_unlit.DebugName = $"{nameof(Game)}.{nameof(_unlit)}";

			var green = new Color4(0, 1, 0, 1);
			var blue = new Color4(0, 0, 1, 0);
			var red = new Color4(1, 0, 0, 1);

			var cube = MeshBuilder.Cube();
			cube.GetVertex(0).Color = green;
			cube.GetVertex(1).Color = red;
			cube.GetVertex(2).Color = blue;
			cube.GetVertex(3).Color = red;
			cube.GetVertex(4).Color = green;
			cube.GetVertex(5).Color = blue;
			cube.GetVertex(6).Color = green;
			cube.GetVertex(7).Color = red;

			_objects = new List<SimpleObject>
			{
				new SimpleObject(new Mesh(cube), _unlit)
				{
					Transform =
					{
						Position = new Vector3(2, 0, 0),
						Scale = Vector3.One
					}
				},
				new SimpleObject(new Mesh(cube), _unlit)
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

			var vertexOffset = 0;
			var triangleOffset = 0;
			foreach (var obj in _objects)
			{
				obj.TriangleOffset = triangleOffset;
				_vertexBuffer.Upload(obj.Mesh.Vertices, vertexOffset);
				_indexBuffer.Upload(obj.Mesh.Triangles, triangleOffset);
				vertexOffset += obj.Mesh.Vertices.Length;
				triangleOffset += obj.Mesh.Triangles.Length;
			}

			_atlas = Texture2D.Load(dx, "Assets/Temp/texture-atlas.dds");
			_atlas.DebugName = $"{nameof(Game)}.{nameof(_atlas)}";
		}

		public void Dispose()
		{
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
			ps.Set(_unlit.PixelShader.Shader);
			vs.SetConstantBuffers(0, _cbuffers.Length, _cbuffers);
//			rs.State = _rsWireframe;

			foreach (var obj in _objects)
			{
				var perObject = obj.PerObject;
				_perObjectBuffer.Upload(ref perObject);
				dx.Context.DrawIndexed(obj.Mesh.Triangles.Length * Marshal.SizeOf<Triangle>(), obj.TriangleOffset, 0);
			}
		}

		public void Render2D(DXWindowContext dx)
		{
		}
	}
}
