using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DiscordBot.Util.Logging
{
    /// <summary>
    /// Stellt einen Mechanisums bereit, um die Zeit einer Operation zu messen
    /// </summary>
    public class EntryExitLog : IDisposable
    {
        public delegate void NewLogDelegate(string message);

        /// <summary>
        /// Beschreibung des Tasks
        /// </summary>
        protected string TaskDescription;

        /// <summary>
        /// Logging Method die zum loggen der Zeit genutzt wird
        /// </summary>
        protected NewLogDelegate LoggingMethod;

        /// <summary>
        /// Stoppuhr die die verstrichene Zeit misst
        /// </summary>
        protected Stopwatch Watch { get; }

        /// <summary>
        /// Verstrichene Zeit, seid dem die Zeitmessung begonnen hat
        /// </summary>
        public TimeSpan Elapsed => Watch.Elapsed;

        /// <summary>
        /// Erstellt eine neue <see cref="EntryExitLog"/> Instanz mit der übergebenen Logger Methode und Task Beschreibung
        /// </summary>
        /// <param name="loggingMethod">Methode die zum loggen genutzt werden soll</param>
        /// <param name="taskDescription">Beschreibung des Tasks</param>
        public EntryExitLog(NewLogDelegate loggingMethod, string taskDescription)
        {
            if (string.IsNullOrWhiteSpace(taskDescription))
            {
                throw new ArgumentException(nameof(taskDescription));
            }

            LoggingMethod = loggingMethod;
            TaskDescription = taskDescription;
            WriteEntry();
            Watch = Stopwatch.StartNew();
        }

        protected virtual void WriteEntry()
        {
            LoggingMethod($"Entry: {TaskDescription}");
        }

        /// <summary>
        /// Loggt die verstrichene Zeit
        /// </summary>
        public virtual void WriteStaus()
        {
            LoggingMethod($"Status: {TaskDescription} is running since {GetElapsedTimeRounded()} Sec.");
        }

        protected virtual void WriteExit()
        {
            LoggingMethod($"Exit: {TaskDescription} - {GetElapsedTimeRounded()} Sec.");
        }

        protected double GetElapsedTimeRounded()
        {
            return Math.Round(Watch.Elapsed.TotalSeconds, 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Stoppt die Zeitmessung und loggt die verstrichene Zeit
        /// </summary>
        public virtual void Dispose()
        {
            Watch.Stop();
            WriteExit();
        }
    }
}
