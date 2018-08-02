using Dig.Renderer.Shaders;

using SharpDX.Direct3D11;

namespace Dig.Renderer.Models
{
	public interface IVertexBuffer
	{
	}

	public sealed class VertexBuffer : GPUBuffer<Vertex>, IVertexBuffer
	{
		public readonly InputLayout Layout;

		public override string DebugName
		{
			get => base.DebugName;
			set
			{
				base.DebugName = value;
				Layout.DebugName = $"{value}.{nameof(Layout)}";
			}
		}

		public VertexBuffer(DXContext ctx, int count, bool dynamic)
			: base(ctx, count, BindFlags.VertexBuffer, dynamic)
		{
			Layout = VertexBufferLayout.CommonLayout;
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
