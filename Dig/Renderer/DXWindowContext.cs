using System;

using Dig.Utils.WinAPI;

using JetBrains.Annotations;

using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;

using D3D11Texture2D1 = SharpDX.Direct3D11.Texture2D1;
using D3D11Texture2DDescription1 = SharpDX.Direct3D11.Texture2DDescription1;
using D3D11RenderTargetView1 = SharpDX.Direct3D11.RenderTargetView1;
using D3D11Device5 = SharpDX.Direct3D11.Device5;
using D3D11Device = SharpDX.Direct3D11.Device;
using D3D11DeviceContext4 = SharpDX.Direct3D11.DeviceContext4;
using D3D11DepthStencilView = SharpDX.Direct3D11.DepthStencilView;
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
using DXGISwapChain1 = SharpDX.DXGI.SwapChain1;
using DXGISwapChain4 = SharpDX.DXGI.SwapChain4;

namespace Dig.Renderer
{
	public sealed class DXWindowContext : IDisposable
	{
		public readonly Window Window;
		public readonly D3D11DeviceContext4 Context;
		public readonly D2D1DeviceContext5 Context2D;

		public readonly DXGISwapChain4 SwapChain;
		public readonly D3D11RenderTargetView1 RenderTargetView;
		public readonly D3D11Texture2D1 DepthBuffer;
		public readonly D3D11DepthStencilView DepthView;

		public DXWindowContext(Window window, DXContext ctx)
		{
			Window = window;

			var chainDesc = new SwapChainDescription1
			{
				Width = window.Width,
				Height = window.Height,
				Format = Format.B8G8R8A8_UNorm,
				BufferCount = 2,
				Scaling = Scaling.None,
				Usage = Usage.RenderTargetOutput | Usage.ShaderInput,
				SwapEffect = SwapEffect.FlipSequential,
				Flags = SwapChainFlags.AllowTearing | SwapChainFlags.AllowModeSwitch |
					SwapChainFlags.FrameLatencyWaitAbleObject,
				SampleDescription = new SampleDescription(1, 0)
			};

			var swapChain = new DXGISwapChain1(ctx.Factory, ctx.Device, window.Handle, ref chainDesc);
			COM.Create(out SwapChain, () => swapChain);

			using (var buffer = SwapChain.GetBackBuffer<D3D11Texture2D1>(0))
			{
				RenderTargetView = new D3D11RenderTargetView1(ctx.Device, buffer);
			}

			Context = ctx.Context;
			Context2D = new D2D1DeviceContext5(ctx.Device2D, DeviceContextOptions.EnableMultithreadedOptimizations);

			using (var surface = SwapChain.GetBackBuffer<Surface2>(0))
			using (var bitmap = new D2D1Bitmap1(Context2D, surface))
			{
				Context2D.Target = bitmap;
			}

			var depthDesc = new D3D11Texture2DDescription1
			{
				Width = window.Width,
				Height = window.Height,
				MipLevels = 1,
				ArraySize = 1,
				Format = Format.D24_UNorm_S8_UInt,
				SampleDescription = new SampleDescription(1, 0),
				Usage = ResourceUsage.Default,
				BindFlags = BindFlags.DepthStencil,
				OptionFlags = ResourceOptionFlags.None,
				CpuAccessFlags = CpuAccessFlags.None,
				TextureLayout = TextureLayout.Undefined
			};

			DepthBuffer = new D3D11Texture2D1(ctx.Device, depthDesc);
			DepthView = new D3D11DepthStencilView(ctx.Device, DepthBuffer);
		}

		public void Dispose()
		{
			Context2D.Dispose();
			DepthView.Dispose();
			DepthBuffer.Dispose();
			RenderTargetView.Dispose();
			SwapChain.Dispose();
		}

		public void Begin3D()
		{
			var target = RenderTargetView;

			Context.Rasterizer.SetViewport(0, 0, Window.Width, Window.Height);
			Context.OutputMerger.SetRenderTargets(DepthView, target);
			Context.ClearRenderTargetView(target, new RawColor4(0, 0, 0, 1f));
			Context.ClearDepthStencilView(DepthView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
		}

		public void Begin2D()
		{
			Context2D.BeginDraw();
		}

		public void Finish()
		{
			Context2D.EndDraw();
			SwapChain.Present(1, PresentFlags.None);
		}
	}
}
