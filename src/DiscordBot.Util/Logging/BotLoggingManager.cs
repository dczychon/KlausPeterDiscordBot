using System;
using System.IO;
using System.Reflection;
using NLog;
using NLog.Conditions;
using NLog.Config;
using NLog.Targets;

namespace DiscordBot.Util.Logging
{
    public static class BotLoggingManager
    {
        private const string ConsoleLayout = @"${level:uppercase=true:padding=-5}    ${message}    ${exception}";

        public static string NLogConfigFileName
        {
            get
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string entryAssemblyName = Assembly.GetEntryAssembly().GetName().Name;
                return Path.Combine(baseDir, $"{entryAssemblyName}.logconfig");
            }
        }

        /// <summary>
        /// Loads a NLog configuration file or configures console logging, if no file exists
        /// </summary>
        public static void InitLogging()
        {
            if (File.Exists(NLogConfigFileName))
            {
                LoadNlogConfigFIle(NLogConfigFileName);
            }
            else
            {
                ConfigureConsoleLogging();
            }
        }

        /// <summary>
        /// Replaces the current logging configuration with console logging
        /// </summary>
        public static void ConfigureConsoleLogging()
        {
            LoggingConfiguration logConfig = new LoggingConfiguration();

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                //Colored console on Windows
                ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget("consolelogger");
                consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule
                {
                    Condition = ConditionParser.ParseExpression("level == LogLevel.Debug"),
                    ForegroundColor = ConsoleOutputColor.DarkGray
                });
                consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule
                {
                    Condition = ConditionParser.ParseExpression("level == LogLevel.Info"),
                    ForegroundColor = ConsoleOutputColor.Gray
                });
                consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule
                {
                    Condition = ConditionParser.ParseExpression("level == LogLevel.Warn"),
                    ForegroundColor = ConsoleOutputColor.Yellow
                });
                consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule
                {
                    Condition = ConditionParser.ParseExpression("level == LogLevel.Error"),
                    ForegroundColor = ConsoleOutputColor.Red
                });
                consoleTarget.RowHighlightingRules.Add(new ConsoleRowHighlightingRule
                {
                    Condition = ConditionParser.ParseExpression("level == LogLevel.Fatal"),
                    ForegroundColor = ConsoleOutputColor.Red,
                    BackgroundColor = ConsoleOutputColor.Yellow
                });
                consoleTarget.Layout = ConsoleLayout;
                logConfig.AddTarget(consoleTarget);
            }
            else
            {
                ConsoleTarget consoleTarget = new ConsoleTarget("consolelogger") {Layout = ConsoleLayout};
                logConfig.AddTarget(consoleTarget);
            }

            
            logConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, "consolelogger");
            LogManager.Configuration = logConfig;
            LogManager.ReconfigExistingLoggers();
        }

        /// <summary>
        /// Load´s a NLog configuration file and replaces the current configuration
        /// </summary>
        /// <param name="path">Path to the configuration file</param>
        public static void LoadNlogConfigFIle(string path)
        {
            if (File.Exists(path))
            {
                LogManager.Configuration = new XmlLoggingConfiguration(path, true);
                LogManager.ReconfigExistingLoggers();
            }
        }
    }
}
