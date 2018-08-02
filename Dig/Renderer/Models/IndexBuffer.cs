using System;

using SharpDX.Direct3D11;

namespace Dig.Renderer.Models
{
	public interface IIndexBuffer
	{
	}

	public sealed class IndexBuffer : GPUBuffer<Triangle>, IIndexBuffer
	{
		public IndexBuffer(DXContext ctx, int count, bool dynamic)
			: base(ctx, count, BindFlags.IndexBuffer, dynamic)
		{
			// TODO assert all the base fields are ints
		}

		public IndexBuffer(DXContext ctx, Span<Triangle> data, bool dynamic)
			: base(ctx, data, BindFlags.IndexBuffer, dynamic)
		{
			// TODO assert all the base fields are ints
		}
	}
}
