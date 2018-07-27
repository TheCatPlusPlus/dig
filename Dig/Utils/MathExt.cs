using System;

namespace Dig.Utils
{
	public static class MathExt
	{
		public static float Clamp(float value, float min, float max)
		{
			if (value > max)
			{
				return max;
			}

			return value < min ? min : value;
		}

		public static int Mod(int value, int mod)
		{
			return (value % mod + mod) % mod;
		}

		public static int RoundToInt(float value)
		{
			return (int)Math.Round(value);
		}

		public static bool ApproxEquals(float x, float y)
		{
			return MathF.Abs(x - y) <= 0.00001f;
		}
	}
}
