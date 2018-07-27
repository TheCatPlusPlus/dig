using System;

using SharpDX;

namespace Dig.Utils.WinAPI
{
	public static class COM
	{
		public static TTarget Create<TTarget>(Func<ComObject> makeSource)
			where TTarget : ComObject
		{
			using (var source = makeSource())
			{
				return source.QueryInterface<TTarget>();
			}
		}

		public static void Create<TTarget>(out TTarget target, Func<ComObject> makeSource)
			where TTarget : ComObject
		{
			target = Create<TTarget>(makeSource);
		}
	}
}
