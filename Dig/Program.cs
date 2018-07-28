using System;
using System.IO;
using System.Reflection;

namespace Dig
{
	internal static class Program
	{
		private static void Main()
		{
			var here = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			Environment.CurrentDirectory = here;

			new Loop().Run();
		}
	}
}
