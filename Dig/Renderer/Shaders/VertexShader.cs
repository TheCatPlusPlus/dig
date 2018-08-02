using System;

using SharpDX.D3DCompiler;

using D3D11VertexShader = SharpDX.Direct3D11.VertexShader;

namespace Dig.Renderer.Shaders
{
	public sealed class VertexShader : IDisposable
	{
		public readonly D3D11VertexShader Shader;
		public readonly ShaderSignature Signature;
		public readonly byte[] Bytecode;

		public string DebugName
		{
			get => Shader.DebugName;
			set => Shader.DebugName = value;
		}

		public VertexShader(DXContext ctx, string filename)
		{
			Bytecode = ShaderCompiler.CompileVertexShader(filename);
			Shader = new D3D11VertexShader(ctx.Device, Bytecode);
			Signature = ShaderSignature.GetInputSignature(Bytecode);
		}

		public void Dispose()
		{
			Signature.Dispose();
			Shader.Dispose();
		}
	}
}
