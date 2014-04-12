using System;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace ThinkBinary.SchemaToPoco.Console
{
	class Program
	{
		private static Logger _log;

		static Int32 Main(string[] args)
		{
			try
			{
				ConfigureLoging();
				return (int)ExitCodes.Ok;
			}
			catch (Exception e)
			{
				_log.Fatal(e);
				return (int)ExitCodes.AbnormalExit;
			}
		}

		private static void ConfigureLoging()
		{
			var coloredConsoleTarget = new ColoredConsoleTarget
			{
				Layout = "${date:format=yyyy-MM-dd} ${time:format=hh:mm:ss} [${level}] ${message}"
			};
			var loggingRule =  new LoggingRule("*", LogLevel.Debug, coloredConsoleTarget);
			LogManager.Configuration = new LoggingConfiguration();
			LogManager.Configuration.AddTarget("Console", coloredConsoleTarget);
			LogManager.Configuration.LoggingRules.Add(loggingRule);
			LogManager.ReconfigExistingLoggers();

			_log = LogManager.GetCurrentClassLogger();
		}
	}
}
