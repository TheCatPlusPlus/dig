using System;
using System.IO;

using Dig.Utils.WinAPI;

using SharpDX.Direct3D11;
using SharpDX.DXGI;

using D3D11Texture2D1 = SharpDX.Direct3D11.Texture2D1;
using D3D11Texture2DDescription1 = SharpDX.Direct3D11.Texture2DDescription1;
using D3D11ShaderResourceView1 = SharpDX.Direct3D11.ShaderResourceView1;
using D3D11ShaderResourceViewDescription1 = SharpDX.Direct3D11.ShaderResourceViewDescription1;

namespace Dig.Renderer
{
	public sealed class Texture2D : IDisposable
	{
		public DXContext Parent { get; }
		public D3D11Texture2D1 Texture { get; }
		public D3D11ShaderResourceView1 View { get; }
		public TextureAlphaMode Alpha { get; }
		public int Width { get; }
		public int Height { get; }
		public Format Format { get; }

		public string DebugName
		{
			get => Texture.DebugName;
			set
			{
				Texture.DebugName = value;
				View.DebugName = $"{value}.View";
			}
		}

		public Texture2D(DXContext dx, Span<byte> dds, BindFlags flags = BindFlags.ShaderResource)
		{
			Parent = dx;
			(Texture, View, Alpha) = DX.TextureCreateFromDDS(dx.Device, dds, flags);

			var desc = Texture.Description1;

			Width = desc.Width;
			Height = desc.Height;
			Format = desc.Format;
		}

		public static Texture2D Load(DXContext dx, string filename, BindFlags flags = BindFlags.ShaderResource)
		{
			var bytes = File.ReadAllBytes(filename);
			return new Texture2D(dx, bytes, flags);
		}

		public void Dispose()
		{
			View.Dispose();
			Texture.Dispose();
		}
	}
}
