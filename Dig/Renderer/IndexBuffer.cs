using System;

using SharpDX.Direct3D11;

namespace Dig.Renderer
{
	public sealed class IndexBuffer<T> : GPUBuffer<T>
		where T : struct, IIndex
	{
		public IndexBuffer(DXContext ctx, int count, bool dynamic)
			: base(ctx, count, BindFlags.IndexBuffer, dynamic)
		{
			// TODO assert all the base fields are ints
		}

		public IndexBuffer(DXContext ctx, Span<T> data, bool dynamic)
			: base(ctx, data, BindFlags.IndexBuffer, dynamic)
		{
			// TODO assert all the base fields are ints
		}
	}
}
