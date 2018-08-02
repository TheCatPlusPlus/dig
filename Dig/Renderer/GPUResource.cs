using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using SharpDX.Direct3D11;

using D3D11DeviceContext4 = SharpDX.Direct3D11.DeviceContext4;
using D3D11Resource = SharpDX.Direct3D11.Resource;

namespace Dig.Renderer
{
	public static class GPUResource
	{
		public struct Mapping : IDisposable
		{
			public readonly D3D11DeviceContext4 Context;
			public readonly D3D11Resource Resource;
			public readonly int Subresource;

			public Mapping(D3D11DeviceContext4 context, D3D11Resource resource, int subresource)
			{
				Context = context;
				Resource = resource;
				Subresource = subresource;
			}

			public void Dispose()
			{
				Context.UnmapSubresource(Resource, Subresource);
			}
		}

		public static unsafe void Upload<T>(
			this IGPUResource<T> resource, Span<T> data, int offset = 0, int subresource = 0, MapMode mode = MapMode.WriteDiscard,
			MapFlags flags = MapFlags.None)
			where T : struct
		{
			Debug.Assert(data.Length <= resource.Count, "data.Length <= resource.Count");

			if (resource.IsDynamic)
			{
				using (resource.Map(out var target, subresource, mode, flags))
				{
					data.CopyTo(target.Slice(offset, target.Length - offset));
				}
			}
			else
			{
				var bytes = MemoryMarshal.AsBytes(data);
				fixed (void* ptr = &bytes[0])
				{
					var byteOffset = offset * resource.Stride;
					var byteLength = bytes.Length;

					Debug.Assert(byteOffset + byteLength <= resource.ByteSize, "byteOffset + byteLength <= resource.ByteSize");
					ResourceRegion? region = new ResourceRegion(byteOffset, 0, 0, byteOffset + byteLength, 1, 1);

					// TODO this will probably be needed for textures
					var rowPitch = 0;
					var depthPitch = 0;

					if (resource is IConstantBuffer)
					{
						Debug.Assert(data.Length == resource.Count, "data.Length == resource.Count (constant buffers must be updated in whole)");
						Debug.Assert(offset == 0, "offset == 0 (constant buffers must be updated in whole)");
						region = null;
					}

					resource.Parent.Context.UpdateSubresource(resource.Resource, subresource, region, (IntPtr)ptr, rowPitch, depthPitch);
				}
			}
		}

		public static void Upload<T>(
			this IGPUResource<T> resource, ref T data, int offset = 0, int subresource = 0, MapMode mode = MapMode.WriteDiscard, MapFlags flags = MapFlags.None)
			where T : struct
		{
			resource.Upload(MemoryMarshal.CreateSpan(ref data, 1), offset, subresource, mode, flags);
		}

		public static unsafe Mapping Map<T>(
			this IGPUResource<T> resource, out Span<T> span, int subresource = 0, MapMode mode = MapMode.WriteDiscard, MapFlags flags = MapFlags.None)
			where T : struct
		{
			var targetSize = resource.ByteSize;
			var target = resource.Parent.Context.MapSubresource(resource.Resource, subresource, mode, flags);
			span = new Span<T>(target.DataPointer.ToPointer(), targetSize);
			return new Mapping(resource.Parent.Context, resource.Resource, subresource);
		}
	}
}
