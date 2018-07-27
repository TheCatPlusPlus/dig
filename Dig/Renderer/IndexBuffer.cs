using SharpDX.Direct3D11;

namespace Dig.Renderer
{
	public sealed class IndexBuffer : GPUBuffer<int>
	{
		public IndexBuffer(DXContext ctx, int count, bool dynamic)
			: base(ctx, count, BindFlags.IndexBuffer, dynamic)
		{
		}
	}
}
