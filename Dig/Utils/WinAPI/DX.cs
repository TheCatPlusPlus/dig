using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

using Dig.Renderer;
using Dig.Renderer.Texturing;

using JetBrains.Annotations;

using NLog;

using SharpDX.Direct3D11;

using static Dig.Utils.WinAPI.Kernel32;

using D3D11Device = SharpDX.Direct3D11.Device;
using D3D11Resource = SharpDX.Direct3D11.Resource;
using D3D11Texture2D1 = SharpDX.Direct3D11.Texture2D1;
using D3D11ShaderResourceView = SharpDX.Direct3D11.ShaderResourceView;
using D3D11ShaderResourceView1 = SharpDX.Direct3D11.ShaderResourceView1;

namespace Dig.Utils.WinAPI
{
	public static unsafe class DX
	{
		[StructLayout(LayoutKind.Sequential)]
		private struct TextureParams
		{
			public IntPtr Device;
			public byte* Buffer;
			public ulong BufferSize;
			public int Usage;
			public uint BindFlags;
			public uint CPUAccessFlags;
			public uint MiscFlags;
			[MarshalAs(UnmanagedType.I1)]
			public bool ForceSRGB;
			public ulong MaxSize;
		}

		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		private delegate void LogFunc(int level, string message);

		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		private delegate string InitFunc(IntPtr log);

		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		private delegate string TextureCreateFromDDSFunc(
			in TextureParams @params,
			out IntPtr texture,
			out IntPtr view,
			out TextureAlphaMode alpha
		);

		private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
		private static readonly LogFunc LogPtr;
		private static readonly TextureCreateFromDDSFunc TextureCreateFromDDSPtr;

		static DX()
		{
			// <root>/Dig/bin/Debug/Dig.dll
			var path = Path.GetFullPath(Assembly.GetExecutingAssembly().Location);
			path = Path.GetDirectoryName(path);
			path = Path.GetDirectoryName(path);
			path = Path.GetDirectoryName(path);
			path = Path.GetDirectoryName(path);
			// <root>/Dig.DX/bin/Debug
			path = Path.Combine(path, "Dig.DX", "bin", "Debug");
			AddDllDirectory(path);

			var dll = LoadLibraryEx("Dig.DX.dll", default, LOAD_LIBRARY_SEARCH_DEFAULT_DIRS | LOAD_LIBRARY_SEARCH_USER_DIRS);
			if (dll == default)
			{
				throw new Win32Exception();
			}

			LogPtr = (l, m) => Log.Log(LogLevel.FromOrdinal(l), m);
			Get<InitFunc>(dll, "Init", out var init);
			C(init(Marshal.GetFunctionPointerForDelegate(LogPtr)));

			Get(dll, nameof(TextureCreateFromDDS), out TextureCreateFromDDSPtr);
		}

		public static (D3D11Texture2D1, D3D11ShaderResourceView1, TextureAlphaMode) TextureCreateFromDDS(
			D3D11Device device, Span<byte> buffer, BindFlags bindFlags = BindFlags.ShaderResource,
			ResourceUsage usage = ResourceUsage.Default, CpuAccessFlags cpuFlags = CpuAccessFlags.None,
			ResourceOptionFlags options = ResourceOptionFlags.None, bool forceSRGB = false, ulong maxSize = 0
		)
		{
			fixed (byte* ptr = &buffer[0])
			{
				var @params = new TextureParams
				{
					Device = device.NativePointer,
					Buffer = ptr,
					BufferSize = (ulong)buffer.Length,
					BindFlags = (uint)bindFlags,
					Usage = (int)usage,
					CPUAccessFlags = (uint)cpuFlags,
					ForceSRGB = forceSRGB,
					MaxSize = maxSize,
					MiscFlags = (uint)options
				};

				C(TextureCreateFromDDSPtr(in @params, out var rawTexture, out var rawView, out var alpha));

				using (var texture = new D3D11Resource(rawTexture))
				using (var view = new D3D11ShaderResourceView(rawView))
				{
					var texture1 = texture.QueryInterface<D3D11Texture2D1>();
					var view1 = view.QueryInterface<D3D11ShaderResourceView1>();
					return (texture1, view1, alpha);
				}
			}
		}

		private static void Get<T>(IntPtr dll, string name, out T ptr)
		{
			var raw = GetProcAddress(dll, name);
			if (raw == default)
			{
				throw new Win32Exception();
			}

			ptr = Marshal.GetDelegateForFunctionPointer<T>(raw);
		}

		private static void C([CanBeNull] string result)
		{
			if (result != null)
			{
				throw new Exception(result);
			}
		}
	}
}
