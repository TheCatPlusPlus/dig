using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

using Dig.Input;
using Dig.Renderer;
using Dig.Renderer.Shaders;
using Dig.Utils.WinAPI;

namespace Dig
{
	internal sealed class Loop
	{
		private const long FrameInterval = 1000 / 60;
		private const long FixedUpdateInterval = 1000 / 50;

		private bool _running;

		public Loop()
		{
			_running = true;
		}

		public void Stop()
		{
			_running = false;
		}

		public void Run()
		{
			_running = true;

			var input = new InputState();
			using (var window = new Window(input))
			using (var dx = new DXContext())
			{
				VertexBufferLayout.CreateCommon(dx);

				using (var dxWindow = new DXWindowContext(window, dx))
				using (var game = new Game(dx, dxWindow, input))
				{
					RunLoop(dx, dxWindow, game, input);
				}
			}
		}

		private void RunLoop(DXContext dx, DXWindowContext dxWindow, Game game, InputState input)
		{
			var watch = new Stopwatch();
			watch.Start();

			var prevTime = watch.ElapsedMilliseconds;
			var prevSleepTime = 0L;
			var fixedUpdateTime = 0L;

			while (_running)
			{
				var newTime = watch.ElapsedMilliseconds;
				var timeDelta = newTime - prevTime;
				fixedUpdateTime += timeDelta;
				prevTime = newTime;

				if (!Window.ProcessEvents())
				{
					Stop();
					return;
				}

				input.Commit();

				while (fixedUpdateTime >= FixedUpdateInterval)
				{
					game.UpdateFixed(FixedUpdateInterval / 1000.0);
					fixedUpdateTime -= FixedUpdateInterval;
				}

				game.Update(timeDelta / 1000.0);

				dxWindow.Begin3D();
				game.Render3D(dxWindow);
				dxWindow.Begin2D();
				game.Render2D(dxWindow);
				dxWindow.Finish();

				prevSleepTime = LimitFPS(timeDelta, prevSleepTime);
			}
		}

		private static long LimitFPS(long timeDelta, long prevSleepTime)
		{
			if (timeDelta <= FrameInterval + prevSleepTime)
			{
				prevSleepTime = FrameInterval + prevSleepTime - timeDelta;
				Thread.Sleep(TimeSpan.FromMilliseconds(prevSleepTime));
			}
			else
			{
				prevSleepTime = 0;
			}

			return prevSleepTime;
		}
	}
}
