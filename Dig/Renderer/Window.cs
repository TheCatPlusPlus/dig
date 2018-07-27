using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using Dig.Input;

using static Dig.Utils.WinAPI.User32;
using static Dig.Utils.WinAPI.Kernel32;

namespace Dig.Renderer
{
	public sealed class Window : IDisposable
	{
		private static readonly IntPtr Instance = GetModuleHandle(null);

		private readonly WindowProc _windowProc;
		private readonly InputState _input;
		private short _lastMouseX;
		private short _lastMouseY;

		public readonly IntPtr Handle;
		public readonly int Width;
		public readonly int Height;

		public Window(InputState input, int width = 1920, int height = 1080)
		{
			_input = input;
			_windowProc = OnMessage;

			var wndClass = new WNDCLASSEX
			{
				Size = (uint)Marshal.SizeOf<WNDCLASSEX>(),
				WndProc = Marshal.GetFunctionPointerForDelegate(_windowProc),
				ClassName = $"Dig.{Guid.NewGuid()}",
				Cursor = LoadCursor(IntPtr.Zero, IDC_ARROW),
				Instance = Instance,
				Style = CS_OWNDC | CS_BYTEALIGNCLIENT | CS_VREDRAW | CS_HREDRAW
			};

			var classAtom = RegisterClassEx(ref wndClass);
			if (classAtom == IntPtr.Zero)
			{
				throw new Win32Exception();
			}

			var rect = new RECT
			{
				Bottom = Height = height,
				Right = Width = width
			};

			const uint style = WS_VISIBLE | WS_CAPTION | WS_CLIPCHILDREN | WS_SYSMENU;
			const uint exStyle = WS_EX_OVERLAPPEDWINDOW;
			AdjustWindowRectEx(ref rect, style, false, exStyle);

			Handle = CreateWindowEx(
				exStyle, classAtom, "Dig", style,
				CW_USEDEFAULT, CW_USEDEFAULT, rect.Right - rect.Left, rect.Bottom - rect.Top,
				IntPtr.Zero, IntPtr.Zero, Instance, IntPtr.Zero
			);

			if (Handle == IntPtr.Zero)
			{
				throw new Win32Exception();
			}
		}

		public void Dispose()
		{
			DestroyWindow(Handle);
		}

		private IntPtr OnMessage(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
		{
			// ReSharper disable once SwitchStatementMissingSomeCases
			switch (msg)
			{
				case WM_CLOSE:
					PostQuitMessage(0);
					return (IntPtr)0;
				case WM_KEYDOWN:
				case WM_SYSKEYDOWN:
				{
					var code = MapKey(wParam.ToInt32());
					_input[code].Set(true);
					return (IntPtr)0;
				}
				case WM_KEYUP:
				case WM_SYSKEYUP:
				{
					var code = MapKey(wParam.ToInt32());
					_input[code].Set(false);
					return (IntPtr)0;
				}
				case WM_SETFOCUS:
					_input.Focus.Set(true);

					return (IntPtr)0;
				case WM_KILLFOCUS:
					_input.Focus.Set(false);
					return (IntPtr)0;
				case WM_MOUSEWHEEL:
				{
					var delta = (wParam.ToInt32() >> 16) & ushort.MaxValue;
					_input[InputCode.MouseWheel].Add(delta / 120.0f);
					return (IntPtr)0;
				}
				case WM_MOUSEMOVE:
				{
					var coords = lParam.ToInt32();
					var x = (short)(coords & short.MaxValue);
					var y = (short)((coords >> 16) & short.MaxValue);
					_input[InputCode.MouseX].Add(x - _lastMouseX);
					_input[InputCode.MouseY].Add(y - _lastMouseY);
					_lastMouseX = x;
					_lastMouseY = y;
					return (IntPtr)0;
				}
				case WM_LBUTTONDOWN:
				case WM_LBUTTONUP:
				case WM_RBUTTONDOWN:
				case WM_RBUTTONUP:
				case WM_MBUTTONDOWN:
				case WM_MBUTTONUP:
				{
					var state = wParam.ToInt32();
					_input[InputCode.MouseLeft].Set((state & MK_LBUTTON) != 0);
					_input[InputCode.MouseRight].Set((state & MK_RBUTTON) != 0);
					_input[InputCode.MouseMiddle].Set((state & MK_MBUTTON) != 0);
					_input[InputCode.MouseFourth].Set((state & MK_XBUTTON1) != 0);
					_input[InputCode.MouseFifth].Set((state & MK_XBUTTON2) != 0);
					return (IntPtr)0;
				}
			}

			return DefWindowProc(hwnd, msg, wParam, lParam);
		}

		private static InputCode MapKey(int vk)
		{
			switch (vk)
			{
				case VK_W:
					return InputCode.KeyW;
				case VK_S:
					return InputCode.KeyS;
				case VK_A:
					return InputCode.KeyA;
				case VK_D:
					return InputCode.KeyD;
				case VK_SPACE:
					return InputCode.KeySpace;
				case VK_ESCAPE:
					return InputCode.KeyEscape;
				default:
					return default;
			}
		}

		public static bool ProcessEvents()
		{
			var message = new MSG();

			while (PeekMessage(ref message, IntPtr.Zero, 0, 0, PM_REMOVE))
			{
				if (message.Message == WM_QUIT)
				{
					return false;
				}

				TranslateMessage(ref message);
				DispatchMessage(ref message);
			}

			return true;
		}
	}
}
