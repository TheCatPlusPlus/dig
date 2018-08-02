using System;

using SharpDX.Direct3D11;
using SharpDX.DXGI;

using D3D11Texture2D1 = SharpDX.Direct3D11.Texture2D1;
using D3D11Texture2DDescription1 = SharpDX.Direct3D11.Texture2DDescription1;
using Resource = SharpDX.Direct3D11.Resource;

namespace Dig.Renderer
{
	public abstract class GPUTexture2D : IDisposable
	{
		public readonly D3D11Texture2D1 Texture;

		public DXContext Parent { get; }
		public Resource Resource { get; }
		public bool IsDynamic { get; }
		public int Count { get; }
		public int ByteSize { get; }
		public int Stride { get; }

		public GPUTexture2D(DXContext dx, int width, int height, BindFlags flags, Format format, bool dynamic, ResourceOptionFlags options = default)
		{
			var desc = new D3D11Texture2DDescription1
			{
				BindFlags = flags,
				ArraySize = 1,
				Format = format,
				OptionFlags = options,
				CpuAccessFlags = dynamic ? CpuAccessFlags.Write : CpuAccessFlags.None,
				Usage = dynamic ? ResourceUsage.Dynamic : ResourceUsage.Default,
				Width = width,
				Height = height,
				MipLevels = 1,
				SampleDescription = new SampleDescription(1, 0),
				TextureLayout = dynamic ? TextureLayout.StandardSwizzle64kb : TextureLayout.Undefined
			};

			Texture = new D3D11Texture2D1(dx.Device, desc);
		}

		public void Dispose()
		{
			Texture.Dispose();
		}
	}
}
