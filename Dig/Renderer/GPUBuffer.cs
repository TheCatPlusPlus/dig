using System;
using System.Runtime.InteropServices;

using SharpDX.Direct3D11;

using D3D11Buffer = SharpDX.Direct3D11.Buffer;
using D3D11Resource = SharpDX.Direct3D11.Resource;

namespace Dig.Renderer
{
	public class GPUBuffer<T> : IDisposable, IGPUResource<T>
		where T : struct
	{
		public DXContext Parent { get; }
		public D3D11Buffer Buffer { get; }
		public D3D11Resource Resource => Buffer;
		public bool IsDynamic { get; }
		public int ByteSize { get; }
		public int Count { get; }
		public int Stride { get; }

		protected GPUBuffer(DXContext ctx, int count, BindFlags flags, bool dynamic, ResourceOptionFlags options = default)
		{
			Parent = ctx;
			Stride = Marshal.SizeOf<T>();
			Count = count;
			ByteSize = Stride * Count;
			IsDynamic = dynamic;

			var desc = new BufferDescription
			{
				SizeInBytes = ByteSize,
				StructureByteStride = Stride,
				BindFlags = flags,
				OptionFlags = options,
				CpuAccessFlags = dynamic ? CpuAccessFlags.Write : CpuAccessFlags.None,
				Usage = dynamic ? ResourceUsage.Dynamic : ResourceUsage.Default
			};

			Buffer = new D3D11Buffer(Parent.Device, desc);
		}

		protected GPUBuffer(DXContext ctx, Span<T> data, BindFlags flags, bool dynamic, ResourceOptionFlags options = default)
			: this(ctx, data.Length, flags, dynamic, options)
		{
			this.Upload(data);
		}

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				Buffer.Dispose();
			}
		}
	}
}
