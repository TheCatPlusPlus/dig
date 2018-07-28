using System;

using Dig.Utils.WinAPI;

using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

using D3D11Device5 = SharpDX.Direct3D11.Device5;
using D3D11Device = SharpDX.Direct3D11.Device;
using D3D11DeviceContext4 = SharpDX.Direct3D11.DeviceContext4;
using D3D11Texture2D1 = SharpDX.Direct3D11.Texture2D1;
using D3D11Buffer = SharpDX.Direct3D11.Buffer;
using D2D1Factory = SharpDX.Direct2D1.Factory;
using D2D1Factory6 = SharpDX.Direct2D1.Factory6;
using D2D1Device5 = SharpDX.Direct2D1.Device5;
using D2D1DeviceContext5 = SharpDX.Direct2D1.DeviceContext5;
using D2D1Bitmap1 = SharpDX.Direct2D1.Bitmap1;
using DXGIDevice4 = SharpDX.DXGI.Device4;
using DXGIFactory5 = SharpDX.DXGI.Factory5;
using DXGIAdapter4 = SharpDX.DXGI.Adapter4;
using DXGIOutput6 = SharpDX.DXGI.Output6;
using FeatureLevel = SharpDX.Direct3D.FeatureLevel;

namespace Dig.Renderer
{
	public sealed class DXContext : IDisposable
	{
		public readonly DXGIDevice4 DeviceDXGI;
		public readonly DXGIFactory5 Factory;
		public readonly DXGIAdapter4 Adapter;
		public readonly DXGIOutput6 Output;
		public readonly D3D11Device5 Device;
		public readonly D3D11DeviceContext4 Context;
		public readonly D2D1Factory6 Factory2D;
		public readonly D2D1Device5 Device2D;

		public DXContext()
		{
			const DeviceCreationFlags flags = DeviceCreationFlags.BgraSupport | DeviceCreationFlags.Debug;

			var device = new D3D11Device(DriverType.Hardware, flags, FeatureLevel.Level_12_1);
			COM.Create(out Device, () => device);
			Device.DebugName = $"{nameof(DXContext)}.{nameof(Device)}";

			COM.Create(out Context, () => Device.ImmediateContext3);
			Context.DebugName = $"{nameof(DXContext)}.{nameof(Context)}";

			DeviceDXGI = Device.QueryInterface<DXGIDevice4>();
			Adapter = DeviceDXGI.GetParent<DXGIAdapter4>();
			Factory = Adapter.GetParent<DXGIFactory5>();
			COM.Create(out Output, () => Adapter.GetOutput(0));

			var factory2D = new D2D1Factory(FactoryType.MultiThreaded, DebugLevel.Information);
			COM.Create(out Factory2D, () => factory2D);
			Device2D = new D2D1Device5(Factory2D, DeviceDXGI);
		}

		public void Dispose()
		{
			Device2D.Dispose();
			Factory2D.Dispose();
			Context.Dispose();
			DeviceDXGI.Dispose();
			Device.Dispose();
			Output.Dispose();
			Adapter.Dispose();
			Factory.Dispose();
		}
	}
}
