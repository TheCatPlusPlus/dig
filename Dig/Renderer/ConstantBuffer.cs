using System;

using SharpDX.Direct3D11;

namespace Dig.Renderer
{
	public interface IConstants
	{
	}

	public sealed class ConstantBuffer<T> : GPUBuffer<T>
		where T : struct, IConstants
	{
		public ConstantBuffer(DXContext ctx, bool dynamic)
			: base(ctx, 1, BindFlags.ConstantBuffer, dynamic)
		{
		}

		protected override void Upload(ResourceRegion? region, IntPtr data)
		{
			base.Upload(null, data);
		}
	}
}
