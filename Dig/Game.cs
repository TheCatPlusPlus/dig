using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Dig.Input;
using Dig.Renderer;

using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;

namespace Dig
{
	[StructLayout(LayoutKind.Sequential)]
	public struct SimpleVertex : IVertex
	{
		public Vector3 Position;
		public Color4 Color;

		public SimpleVertex(float x, float y, float z, float r, float g, float b)
			: this()
		{
			Position = new Vector3(x, y, z);
			Color = new Color4(r, g, b, 1.0f);
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

			var triangle = new[]
			{
				new SimpleVertex(0.0f, 0.5f, 0.5f, 1.0f, 0.0f, 0.0f),
				new SimpleVertex(0.5f, -0.5f, 0.5f, 0.0f, 1.0f, 0.0f),
				new SimpleVertex(-0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f)
			};

			_vertexBuffer = new VertexBuffer<SimpleVertex>(_dx, _vertexShader, triangle.Length, false);
			_vertexBuffer.Upload(triangle);
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
			vs.Set(_vertexShader.Shader);
			ps.Set(_pixelShader.Shader);

			dx.Context.Draw(_vertexBuffer.Count, 0);
		}

		public void Render2D(DXWindowContext dx)
		{
		}
	}
}
