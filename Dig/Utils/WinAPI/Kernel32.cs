using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using JetBrains.Annotations;

namespace Dig.Utils.WinAPI
{
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	public static class Kernel32
	{
		private const string Library = "kernel32.dll";

		public const uint LOAD_LIBRARY_SEARCH_APPLICATION_DIR = 0x00000200;
		public const uint LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000;
		public const uint LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800;
		public const uint LOAD_LIBRARY_SEARCH_USER_DIRS = 0x00000400;

		[DllImport(Library, SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern IntPtr GetModuleHandle([CanBeNull] string name);

		[DllImport(Library, SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern IntPtr AddDllDirectory(string path);

		[DllImport(Library, SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern bool SetDefaultDllDirectories(uint flags);
	}
}
