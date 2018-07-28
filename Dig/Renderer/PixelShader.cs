using D3D11PixelShader = SharpDX.Direct3D11.PixelShader;

namespace Dig.Renderer
{
	public sealed class PixelShader
	{
		public readonly D3D11PixelShader Shader;

		public string DebugName
		{
			get => Shader.DebugName;
			set => Shader.DebugName = value;
		}

		public PixelShader(DXContext ctx, byte[] bytecode)
		{
			Shader = new D3D11PixelShader(ctx.Device, bytecode);
		}

		public void Dispose()
		{
			Shader.Dispose();
		}
	}
}
