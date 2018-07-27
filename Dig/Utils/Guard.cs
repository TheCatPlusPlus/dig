using System;

namespace Dig.Utils
{
	public sealed class Guard : IDisposable
	{
		internal readonly Action Action;

		public Guard(Action action)
		{
			Action = action;
		}

		public void Dispose()
		{
			Action();
		}
	}
}
