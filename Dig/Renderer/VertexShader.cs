using SharpDX.D3DCompiler;

using D3D11VertexShader = SharpDX.Direct3D11.VertexShader;

namespace Dig.Renderer
{
	public sealed class VertexShader
	{
		public readonly D3D11VertexShader Shader;
		public readonly ShaderSignature Signature;
		public readonly byte[] Bytecode;

		public VertexShader(DXContext ctx, byte[] bytecode)
		{
			Shader = new D3D11VertexShader(ctx.Device, bytecode);
			Signature = ShaderSignature.GetInputSignature(bytecode);
			Bytecode = bytecode;
		}

		public void Dispose()
		{
			Signature.Dispose();
			Shader.Dispose();
		}
	}
}
