using System.Threading;

using NLog;
using NLog.Config;
using NLog.Targets;

namespace Dig
{
	internal static class Program
	{
		private static void Main()
		{
			const string layout =
				@"[${date:format=yyyy-MM-dd HH\:mm\:ss}] [T:${threadid}:${threadname}] [${logger}] [${level}] " +
				"${message} ${exception:format=toString,Data:maxInnerExceptionLevel=10}";

			var logConfig = new LoggingConfiguration();
			var logConsole = new ColoredConsoleTarget("console")
			{
				DetectConsoleAvailable = true,
				ErrorStream = true,
				Layout = layout,
				OptimizeBufferReuse = true
			};

			logConfig.AddTarget(logConsole);
			logConfig.AddRuleForAllLevels(logConsole);

			LogManager.Configuration = logConfig;

			Thread.CurrentThread.Name = "Main Game Loop";
			new Loop().Run();
		}
	}
}
