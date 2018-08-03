using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using SharpDX.Direct3D11;

using D3D11Buffer = SharpDX.Direct3D11.Buffer;
using D3D11Resource = SharpDX.Direct3D11.Resource;

namespace Dig.Renderer
{
	public class GPUBuffer<T> : IDisposable
		where T : struct
	{
		public DXContext Parent { get; }
		public D3D11Buffer Buffer { get; }
		public bool IsDynamic { get; }
		public int ByteSize { get; }
		public int Capacity { get; }
		public int Stride { get; }

		public int Count { get; private set; }

		public virtual string DebugName
		{
			get => Buffer.DebugName;
			set => Buffer.DebugName = value;
		}

		protected GPUBuffer(DXContext ctx, int capacity, BindFlags flags, bool dynamic, ResourceOptionFlags options = default)
		{
			Parent = ctx;
			Stride = Marshal.SizeOf<T>();
			Capacity = capacity;
			ByteSize = Stride * Capacity;
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

		public void Upload(ref T data)
		{
			var span = MemoryMarshal.CreateSpan(ref data, 1);
			Upload(span);
		}

		public unsafe void Upload(Span<T> data, int offset = 0)
		{
			if (IsDynamic)
			{
				throw new NotImplementedException();
			}

			Count = data.Length;

			var bytes = MemoryMarshal.AsBytes(data);
			fixed (void* ptr = &bytes[0])
			{
				Upload(ptr, bytes.Length, offset * Stride);
			}
		}

		private unsafe void Upload(void* ptr, int byteSize, int byteOffset)
		{
			Debug.Assert(byteOffset + byteSize <= ByteSize, "offset + size <= ByteCapacity");
			var region = new ResourceRegion(byteOffset, 0, 0, byteOffset + byteSize, 1, 1);
			Upload(region, (IntPtr)ptr);
		}

		protected virtual void Upload(ResourceRegion? region, IntPtr data)
		{
			Parent.Context.UpdateSubresource(Buffer, 0, region, data, 0, 0);
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
