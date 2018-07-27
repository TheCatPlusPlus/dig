using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Dig.Utils;

using SharpDX;
using SharpDX.Direct3D11;

using D3D11Buffer = SharpDX.Direct3D11.Buffer;

namespace Dig.Renderer
{
	public class GPUBuffer<T> : IDisposable
		where T : struct
	{
		private readonly Guard _mapGuard;

		public readonly DXContext Parent;
		public readonly D3D11Buffer Buffer;
		public readonly bool IsDynamic;
		public readonly int ByteSize;
		public readonly int Count;
		public readonly int Stride;

		protected GPUBuffer(
			DXContext ctx, int count, BindFlags flags, bool dynamic, ResourceOptionFlags options = default)
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
			_mapGuard = new Guard(Unmap);
		}

		public unsafe void Upload(Span<T> data)
		{
			Debug.Assert(data.Length <= Count, "data.Length <= Length");

			if (IsDynamic)
			{
				using (Map(out var target))
				{
					data.CopyTo(target);
				}
			}
			else
			{
				var bytes = MemoryMarshal.AsBytes(data);
				fixed (void* ptr = &bytes[0])
				{
					var box = new DataBox((IntPtr)ptr, 0, 0);
					var region = new ResourceRegion(0, 0, 0, ByteSize, 1, 1);
					Parent.Context.UpdateSubresource(box, Buffer, 0, region);
				}
			}
		}

		public unsafe Guard Map(out Span<T> span)
		{
			var targetSize = Buffer.Description.SizeInBytes;
			var target = Parent.Context.MapSubresource(Buffer, 0, MapMode.WriteDiscard, MapFlags.None);
			span = new Span<T>(target.DataPointer.ToPointer(), targetSize);
			return _mapGuard;
		}

		public void Unmap()
		{
			Parent.Context.UnmapSubresource(Buffer, 0);
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
