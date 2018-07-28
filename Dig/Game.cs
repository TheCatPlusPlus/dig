using System;
using System.Diagnostics.CodeAnalysis;
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

	[SuppressMessage("ReSharper", "NotAccessedField.Local")]
	public sealed class Game : IDisposable
	{
		private readonly DXContext _dx;
		private readonly InputState _input;

		private readonly VertexShader _vertexShader;
		private readonly PixelShader _pixelShader;
		private readonly VertexBuffer<SimpleVertex> _vertexBuffer;
		private readonly IndexBuffer<SimpleTriangle> _indexBuffer;

		public Game(DXContext dx, InputState input)
		{
			_dx = dx;
			_input = input;

			var shader = @"
struct VSInput
{
	float3 Position : Position;
	float4 Color : Color;
};

struct VSOutput
{
	float4 Position : SV_POSITION;
	float4 Color : Color;
};

VSOutput VSMain(VSInput input)
{
	VSOutput output;
	output.Position = float4(input.Position.xyz, 1);
	output.Color = input.Color;
	return output;
}

float4 PSMain(VSOutput input) : SV_TARGET
{
	return input.Color;
}
";

			var vsCode = ShaderBytecode.Compile(shader, "VSMain", "vs_5_0", ShaderFlags.Debug, sourceFileName: "vertex-shader");
			var psCode = ShaderBytecode.Compile(shader, "PSMain", "ps_5_0", ShaderFlags.Debug, sourceFileName: "pixel-shader");
			_vertexShader = new VertexShader(_dx, vsCode);
			_pixelShader = new PixelShader(_dx, psCode);

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

			_vertexBuffer = new VertexBuffer<SimpleVertex>(_dx, _vertexShader, vertices, false);
			_indexBuffer = new IndexBuffer<SimpleTriangle>(_dx, triangles, false);
		}

		public void Dispose()
		{
			_vertexBuffer.Dispose();
			_pixelShader.Dispose();
			_vertexShader.Dispose();
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

			ia.InputLayout = _vertexBuffer.Layout;
			ia.PrimitiveTopology = PrimitiveTopology.TriangleList;
			ia.SetVertexBuffers(0, _vertexBuffer.Binding());
			ia.SetIndexBuffer(_indexBuffer.Buffer, Format.R32_UInt, 0);
			vs.Set(_vertexShader.Shader);
			ps.Set(_pixelShader.Shader);

			dx.Context.DrawIndexed(6, 0, 0);
		}

		public void Render2D(DXWindowContext dx)
		{
		}
	}
}
