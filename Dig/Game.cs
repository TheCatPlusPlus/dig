using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;

using Dig.Input;
using Dig.Renderer;

using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.DXGI;

namespace Dig
{
	[StructLayout(LayoutKind.Sequential)]
	[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
	public struct SimpleVertex : IVertex
	{
		public Vector3 Position;
		public Color4 Color;

		public SimpleVertex(float x, float y, float z, float r, float g, float b)
		{
			Position = new Vector3(x, y, z);
			Color = new Color4(r, g, b, 1.0f);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
	public struct SimpleTriangle : IIndex
	{
		public uint V0;
		public uint V1;
		public uint V2;

		public SimpleTriangle(uint v0, uint v1, uint v2)
		{
			V0 = v0;
			V1 = v1;
			V2 = v2;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
	public struct PerObject : IConstants
	{
		public Matrix Projection;
	}

	[SuppressMessage("ReSharper", "NotAccessedField.Local")]
	public sealed class Game : IDisposable
	{
		private readonly DXContext _dx;
		private readonly InputState _input;

		private readonly Material _unlit;
		private readonly VertexBuffer<SimpleVertex> _vertexBuffer;
		private readonly IndexBuffer<SimpleTriangle> _indexBuffer;
		private readonly ConstantBuffer<PerObject> _perObjectBuffer;

		public Game(DXContext dx, InputState input)
		{
			_dx = dx;
			_input = input;

			var vertices = new[]
			{
				new SimpleVertex(-0.5f, -0.5f, 0.5f, 1.0f, 0.0f, 0.0f),
				new SimpleVertex(-0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f),
				new SimpleVertex(0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f),
				new SimpleVertex(0.5f, -0.5f, 0.5f, 0.0f, 1.0f, 0.0f)
			};

			var triangles = new[]
			{
				new SimpleTriangle(0, 1, 2),
				new SimpleTriangle(0, 2, 3)
			};

			_unlit = new Material(dx, "Unlit");
			_unlit.DebugName = $"{nameof(Game)}.{nameof(_unlit)}";

			_vertexBuffer = new VertexBuffer<SimpleVertex>(_dx, _unlit.VertexShader, vertices, false);
			_vertexBuffer.DebugName = $"{nameof(Game)}.{nameof(_vertexBuffer)}";

			_indexBuffer = new IndexBuffer<SimpleTriangle>(_dx, triangles, false);
			_indexBuffer.DebugName = $"{nameof(Game)}.{nameof(_indexBuffer)}";

			_perObjectBuffer = new ConstantBuffer<PerObject>(dx, false);
			_perObjectBuffer.DebugName = $"{nameof(Game)}.{nameof(_perObjectBuffer)}";
		}

		public void Dispose()
		{
			_vertexBuffer.Dispose();
			_unlit.Dispose();
		}

		public void UpdateFixed(double dt)
		{
		}

		public void Update(double dt)
		{
		}

		public void Render3D(DXWindowContext dx)
		{
			var ia = dx.Context.InputAssembler;
			var vs = dx.Context.VertexShader;
			var ps = dx.Context.PixelShader;

			var eye = new Vector3(0, 0, -5f);
			var target = new Vector3(1f, 0, 0);
			var up = new Vector3(0, 1, 0);
			var model = Matrix.Identity;
			var view = Matrix.LookAtLH(eye, target, up);
			var projection = Matrix.PerspectiveFovLH(75, dx.AspectRatio, 0.03f, 50f);

			var mvp = model * view * projection;
			mvp.Transpose();

			var perObject = new PerObject
			{
				Projection = mvp
			};

			_perObjectBuffer.Upload(ref perObject);

			ia.InputLayout = _vertexBuffer.Layout;
			ia.PrimitiveTopology = PrimitiveTopology.TriangleList;
			ia.SetVertexBuffers(0, _vertexBuffer.Binding());
			ia.SetIndexBuffer(_indexBuffer.Buffer, Format.R32_UInt, 0);
			vs.Set(_unlit.VertexShader.Shader);
			ps.Set(_unlit.PixelShader.Shader);
			vs.SetConstantBuffer(0, _perObjectBuffer.Buffer);

			dx.Context.DrawIndexed(6, 0, 0);
		}

		public void Render2D(DXWindowContext dx)
		{
		}
	}
}
