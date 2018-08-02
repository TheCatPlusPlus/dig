using SharpDX.Direct3D11;

namespace Dig.Renderer.Models
{
	public sealed class IndexBuffer : GPUBuffer<Triangle>
	{
		public IndexBuffer(DXContext ctx, int capacity, bool dynamic)
			: base(ctx, capacity, BindFlags.IndexBuffer, dynamic)
		{
			// TODO assert all the base fields are ints
		}
	}
}
