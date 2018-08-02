using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Dig.Utils.WinAPI
{
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	public static class User32
	{
		private const string Library = "user32.dll";

		public const uint CS_HREDRAW = 0x0002;
		public const uint CS_VREDRAW = 0x0001;
		public const uint CS_NOCLOSE = 0x0200;
		public const uint CS_OWNDC = 0x0020;
		public const uint CS_BYTEALIGNCLIENT = 0x1000;

		public const uint WS_CHILD = 0x40000000;
		public const uint WS_VISIBLE = 0x10000000;
		public const uint WS_POPUP = 0x80000000;
		public const uint WS_CAPTION = 0x00C00000;
		public const uint WS_CLIPSIBLINGS = 0x04000000;
		public const uint WS_CLIPCHILDREN = 0x02000000;
		public const uint WS_SYSMENU = 0x00080000;

		public const uint WS_EX_WINDOWEDGE = 0x00000100;
		public const uint WS_EX_APPWINDOW = 0x00040000;
		public const uint WS_EX_CLIENTEDGE = 0x00000200;
		public const uint WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE;

		public const int CW_USEDEFAULT = unchecked((int)0x80000000);

		public const uint WM_ACTIVATE = 0x0006;
		public const uint WM_CLOSE = 0x0010;
		public const uint WM_USER = 0x0400;
		public const uint WM_QUIT = 0x0012;
		public const uint WM_KEYDOWN = 0x0100;
		public const uint WM_KEYUP = 0x0101;
		public const uint WM_SYSKEYDOWN = 0x0104;
		public const uint WM_SYSKEYUP = 0x0105;
		public const uint WM_SETFOCUS = 0x0007;
		public const uint WM_KILLFOCUS = 0x0008;
		public const uint WM_MOUSEWHEEL = 0x020E;
		public const uint WM_MOUSEMOVE = 0x0200;
		public const uint WM_LBUTTONDOWN = 0x0201;
		public const uint WM_LBUTTONUP = 0x0202;
		public const uint WM_RBUTTONDOWN = 0x0204;
		public const uint WM_RBUTTONUP = 0x0205;
		public const uint WM_MBUTTONDOWN = 0x0207;
		public const uint WM_MBUTTONUP = 0x0208;
		public const uint WM_CHAR = 0x0102;

		public const ushort MK_LBUTTON = 0x0001;
		public const ushort MK_RBUTTON = 0x0002;
		public const ushort MK_MBUTTON = 0x0010;
		public const ushort MK_XBUTTON1 = 0x0020;
		public const ushort MK_XBUTTON2 = 0x0040;

		public const byte VK_W = 0x57;
		public const byte VK_S = 0x53;
		public const byte VK_A = 0x41;
		public const byte VK_D = 0x44;
		public const byte VK_SPACE = 0x20;
		public const byte VK_ESCAPE = 0x18;

		public const uint PM_REMOVE = 0x0001;

		public const int SM_CXSCREEN = 0;
		public const int SM_CYSCREEN = 1;

		public static readonly IntPtr IDC_ARROW = new IntPtr(32512);

		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		public delegate IntPtr WindowProc(IntPtr window, uint message, IntPtr wParam, IntPtr lParam);

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int X;
			public int Y;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct WNDCLASSEX
		{
			public uint Size;
			public uint Style;
			public IntPtr WndProc;
			public int ClsExtra;
			public int WndExtra;
			public IntPtr Instance;
			public IntPtr Icon;
			public IntPtr Cursor;
			public IntPtr Background;
			public string MenuName;
			public string ClassName;
			public IntPtr IconSmall;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct MSG
		{
			public IntPtr Window;
			public uint Message;
			public IntPtr WParam;
			public IntPtr LParam;
			public uint Time;
			public POINT Point;
		}

		[DllImport(Library, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr CreateWindowEx(
			uint exStyle,
			string className,
			string windowName,
			uint style,
			int x, int y,
			int width, int height,
			IntPtr parent,
			IntPtr menu,
			IntPtr instance,
			IntPtr userData
		);

		[DllImport(Library, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr CreateWindowEx(
			uint exStyle,
			IntPtr classAtom,
			string windowName,
			uint style,
			int x, int y,
			int width, int height,
			IntPtr parent,
			IntPtr menu,
			IntPtr instance,
			IntPtr userData
		);

		[DllImport(Library, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool DestroyWindow(IntPtr handle);

		[DllImport(Library, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr LoadIcon(IntPtr instance, string name);

		[DllImport(Library, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr LoadIcon(IntPtr instance, IntPtr id);

		[DllImport(Library, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr LoadCursor(IntPtr instance, string name);

		[DllImport(Library, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr LoadCursor(IntPtr instance, IntPtr id);

		[DllImport(Library, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr RegisterClassEx(ref WNDCLASSEX wndClass);

		[DllImport(Library, CharSet = CharSet.Unicode)]
		public static extern IntPtr DefWindowProc(IntPtr window, uint message, IntPtr wParam, IntPtr lParam);

		[DllImport(Library, CharSet = CharSet.Unicode)]
		public static extern void PostQuitMessage(int code);

		[DllImport(Library, CharSet = CharSet.Unicode)]
		public static extern bool PeekMessage(ref MSG msg, IntPtr window, uint min, uint max, uint remove);

		[DllImport(Library, CharSet = CharSet.Unicode)]
		public static extern bool TranslateMessage(ref MSG msg);

		[DllImport(Library, CharSet = CharSet.Unicode)]
		public static extern bool DispatchMessage(ref MSG msg);

		[DllImport(Library, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern bool SetWindowText(IntPtr window, string text);

		[DllImport(Library, CharSet = CharSet.Unicode)]
		public static extern bool AdjustWindowRectEx(ref RECT rect, uint style, bool menu, uint exStyle);

		[DllImport(Library, CharSet = CharSet.Unicode)]
		public static extern int GetSystemMetrics(int metric);
	}
}
