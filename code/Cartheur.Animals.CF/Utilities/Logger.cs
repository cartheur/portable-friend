using System;
using System.IO;
using System.Text;
using Cartheur.Animals.CF.Properties;

namespace Cartheur.Animals.CF.Utilities
{
    /// <summary>
    /// The class which performs logging for the library. Originated in MACOS 9.0.4 (via Code Warrior in SheepShaver, 2014).
    /// </summary>
    public static class Logging
    {
        // Used only for core test frameworks.
        private static readonly string PathLiteral = Resources.PathLiteralDevice; //.PathLiteralTestFramework;
        /// <summary>
        /// The file path for executing assemblies in normal runtimes.
        /// </summary>
        public static string FilePath()
        {
            return "";//nvironment.CurrentDirectory;
        }
        /// <summary>
        /// The type of log to write.
        /// </summary>
        public enum LogType { Information, Error, Gossip, Temporal, Warning };
        /// <summary>
        /// The classes within the interpreter calling the log.
        /// </summary>
        public enum LogCaller { Aeon, AeonGui, AeonLoader, Cognizance, Condition, Cryptography, Get, Gossip, Input, Interaction, Learn, LearningThread, Me, MonoRuntime, Mood, Node, Presence, Result, Script, Set, SpeechRecognizer, Star, TestFramework, That, ThatStar, Think, TopicStar }
        /// <summary>
        /// The last message passed to logging.
        /// </summary>
        public static string LastMessage = "";
        /// <summary>
        /// The delegate for returning the last log message to the calling application.
        /// </summary>
        public delegate void LoggingDelegate();
        /// <summary>
        /// Occurs when [returned to console] is called.
        /// </summary>
        public static event LoggingDelegate ReturnedToConsole;
        /// <summary>
        /// Logs a message sent from the calling application to a file.
        /// </summary>
        /// <param name="message">The message to log. Space between the message and log type enumeration provided.</param>
        /// <param name="logType">Type of the log.</param>
        /// <param name="caller">The class creating the log entry.</param>
        public static void WriteLog(string message, LogType logType, LogCaller caller)
        {
            LastMessage = message;
            // Use FilePath() when outside of a test framework.
            StreamWriter stream = new StreamWriter(PathLiteral + @"\logs\logfile.txt", true);
            switch (logType)
            {
                case LogType.Error:
                    stream.WriteLine(DateTime.Now + " - " + " ERROR " + " - " + message + " from class " + caller + ".");
                    break;
                case LogType.Warning:
                    stream.WriteLine(DateTime.Now + " - " + " WARNING " + " - " + message + " from class " + caller + ".");
                    break;
                case LogType.Information:
                    stream.WriteLine(DateTime.Now + " - " + message + " called from the class " + caller + ".");
                    break;
                    case LogType.Temporal:
                    stream.WriteLine(DateTime.Now + " - " + message + ".");
                    break;
                case LogType.Gossip:
                    stream.WriteLine(DateTime.Now + " - " + message + ".");
                    break;
            }
            stream.Close();
            stream.Dispose();
            if (!Equals(null, ReturnedToConsole))
            {
                ReturnedToConsole();
            }
        }
        /// <summary>
        /// Records a transcript of the conversation.
        /// </summary>
        /// <param name="message">The message to save in transcript format.</param>
        public static void RecordTranscript(string message)
        {
            try
            {
                // Use FilePath() when outside of a test framework.
                StreamWriter stream = new StreamWriter(PathLiteral + @"\logs\transcript.txt", true);
                stream.WriteLine(DateTime.Now + " - " + message);
                stream.Close();
                stream.Dispose();
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message, LogType.Error, LogCaller.Me);
            }
            
        }
        /// <summary>
        /// Saves the last result to support analysis of the algorithm.
        /// </summary>
        /// <param name="output">The output from the conversation.</param>
        public static void SaveLastResult(StringBuilder output)
        {
            try
            {
                // Use FilePath() when outside of a test framework.
                StreamWriter stream = new StreamWriter(PathLiteral + @"\db\AnimalAnalytics.txt", true);
                stream.WriteLine(DateTime.Now + " - " + output);
                stream.Close();
                stream.Dispose();
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message, LogType.Error, LogCaller.Me);
            }

        }
        /// <summary>
        /// Saves the last result to support analysis of the algorithm to storage.
        /// </summary>
        /// <param name="output">The output from the conversation.</param>
        public static void SaveLastResultToStorage(StringBuilder output)
        {
            try
            {
                // Use FilePath() when outside of a test framework.
                StreamWriter stream = new StreamWriter(PathLiteral + @"\db\AnimalAnalyticsStorage.txt", true);
                stream.WriteLine("#" + DateTime.Now + ";" + output);
                stream.Close();
                stream.Dispose();
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message, LogType.Error, LogCaller.Me);
            }

        }
        /// <summary>
        /// Writes a debug log with object parameters.
        /// </summary>
        /// <param name="objects">The objects.</param>
        public static void Debug(params object[] objects)
        {
            // Use FilePath() when outside of a test framework.
            StreamWriter stream = new StreamWriter(PathLiteral + @"\logs\logfile.txt", true);
            foreach (object obj in objects)
            {
                stream.WriteLine(obj);
            }
            stream.WriteLine("--");
            stream.Close();
            stream.Dispose();
        }
    }
}
