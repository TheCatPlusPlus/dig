using System;

using SharpDX.Direct3D11;

namespace Dig.Renderer
{
	public sealed class VertexBuffer<T> : GPUBuffer<T>
		where T : struct, IVertex
	{
		public readonly InputLayout Layout;

		public VertexBuffer(DXContext ctx, VertexShader shader, int count, bool dynamic)
			: base(ctx, count, BindFlags.VertexBuffer, dynamic)
		{
			Layout = VertexBufferLayout.Create<T>(ctx, shader);
		}

		public VertexBuffer(DXContext ctx, VertexShader shader, Span<T> data, bool dynamic)
			: base(ctx, data, BindFlags.VertexBuffer, dynamic)
		{
			Layout = VertexBufferLayout.Create<T>(ctx, shader);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				Layout.Dispose();
			}
		}

		public VertexBufferBinding Binding(int first = 0)
		{
			return new VertexBufferBinding(Buffer, Stride, first * Stride);
		}
	}
}
