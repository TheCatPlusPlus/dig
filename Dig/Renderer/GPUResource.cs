using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using SharpDX;
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

		public static unsafe void Upload<T>(this IGPUResource<T> resource, Span<T> data)
			where T : struct
		{
			Debug.Assert(data.Length <= resource.Count, "data.Length <= resource.Count");

			if (resource.IsDynamic)
			{
				using (resource.Map(out var target))
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
					var region = new ResourceRegion(0, 0, 0, resource.ByteSize, 1, 1);
					resource.Parent.Context.UpdateSubresource(box, resource.Resource, 0, region);
				}
			}
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
