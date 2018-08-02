using System;

using D3D11PixelShader = SharpDX.Direct3D11.PixelShader;

namespace Dig.Renderer.Shaders
{
	public sealed class PixelShader : IDisposable
	{
		public readonly D3D11PixelShader Shader;
		public readonly byte[] Bytecode;

		public string DebugName
		{
			get => Shader.DebugName;
			set => Shader.DebugName = value;
		}

		public PixelShader(DXContext ctx, string filename)
		{
			Bytecode = ShaderCompiler.CompilePixelShader(filename);
			Shader = new D3D11PixelShader(ctx.Device, Bytecode);
		}

		public void Dispose()
		{
			Shader.Dispose();
		}
	}
}
