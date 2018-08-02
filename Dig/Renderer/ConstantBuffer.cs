using SharpDX.Direct3D11;

namespace Dig.Renderer
{
	public interface IConstants
	{
	}

	public interface IConstantBuffer
	{
	}

	public sealed class ConstantBuffer<T> : GPUBuffer<T>, IConstantBuffer
		where T : struct, IConstants
	{
		public ConstantBuffer(DXContext ctx, bool dynamic)
			: base(ctx, 1, BindFlags.ConstantBuffer, dynamic)
		{
		}
	}
}
