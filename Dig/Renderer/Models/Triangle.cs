using System;
using System.Runtime.InteropServices;

namespace Dig.Renderer.Models
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Triangle
	{
		private uint _v0;
		private uint _v1;
		private uint _v2;

		public uint this[int idx]
		{
			get => Get(idx);
			set => Set(idx, value);
		}

		public Triangle(uint v0, uint v1, uint v2)
			: this()
		{
			Set(0, v0);
			Set(1, v1);
			Set(2, v2);
		}

		private uint Get(int idx)
		{
			switch (idx)
			{
				case 0:
					return _v0;
				case 1:
					return _v1;
				case 2:
					return _v2;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void Set(int idx, uint value)
		{
			switch (idx)
			{
				case 0:
					_v0 = value;
					break;
				case 1:
					_v1 = value;
					break;
				case 2:
					_v2 = value;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
