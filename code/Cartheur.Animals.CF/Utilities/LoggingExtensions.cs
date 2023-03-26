using System;
using System.IO;
using System.Text;
using Cartheur.Animals.CF.Properties;

namespace Cartheur.Animals.CF.Utilities
{
    /// <summary>
    /// The class which performs extends logging when used in different runtime environments.
    /// </summary>
    public static class LoggingExtensions
    {
        // Used only for core test frameworks.
        private static readonly string PathLiteral = Resources.PathLiteralTestFramework;
        /// <summary>
        /// The file path for executing assemblies in normal runtimes.
        /// </summary>
        public static string FilePath()
        {
            return ""; // Environment.CurrentDirectory;
        }
        /// <summary>
        /// The type of project using the logger class.
        /// </summary>
        public enum ProjectType { Interactive, TestFramework, Undisclosed }
        /// <summary>
        /// The type of log to write.
        /// </summary>
        public enum LogType { Information, Error, Gossip, Temporal, Warning };
        /// <summary>
        /// The classes within the interpreter calling the log.
        /// </summary>
        public enum LogCaller { Aeon, AeonGui, AeonLoader, Cognizance, Condition, Cryptography, Get, Gossip, Input, Interaction, Learn, Me, MonoRuntime, Mood, Node, Presence, Result, Script, Set, SpeechRecognizer, Star, TestFramework, That, ThatStar, Think, TopicStar }
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
        /// Gets or sets the stream.
        /// </summary>
        public static StreamWriter Stream { get; set; }
        /// <summary>
        /// Logs a message sent from the calling application to a file.
        /// </summary>
        /// <param name="message">The message to log. Space between the message and log type enumeration provided.</param>
        /// <param name="logType">Type of the log.</param>
        /// <param name="caller">The class creating the log entry.</param>
        /// <param name="projectType">Type of the project.</param>
        public static void WriteLog(string message, LogType logType, LogCaller caller, ProjectType projectType)
        {
            LastMessage = message;
            // Choose paths based on GUI type.
            if (projectType == ProjectType.TestFramework)
            {
                Stream = new StreamWriter(PathLiteral + @"\logs\logfile.txt", true);
                switch (logType)
                {
                    case LogType.Error:
                        Stream.WriteLine(DateTime.Now + " - " + " ERROR " + " - " + message + " from class " + caller + ".");
                        break;
                    case LogType.Warning:
                        Stream.WriteLine(DateTime.Now + " - " + " WARNING " + " - " + message + " from class " + caller + ".");
                        break;
                    case LogType.Information:
                        Stream.WriteLine(DateTime.Now + " - " + message + " called from the class " + caller + ".");
                        break;
                    case LogType.Temporal:
                        Stream.WriteLine(DateTime.Now + " - " + message + ".");
                        break;
                    case LogType.Gossip:
                        Stream.WriteLine(DateTime.Now + " - " + message + ".");
                        break;
                }
                Stream.Close();
                if (!Equals(null, ReturnedToConsole))
                {
                    ReturnedToConsole();
                }
            }
            else
            {
                Stream = new StreamWriter(FilePath() + @"\logs\logfile.txt", true);
                switch (logType)
                {
                    case LogType.Error:
                        Stream.WriteLine(DateTime.Now + " - " + " ERROR " + " - " + message + " from class " + caller + ".");
                        break;
                    case LogType.Warning:
                        Stream.WriteLine(DateTime.Now + " - " + " WARNING " + " - " + message + " from class " + caller + ".");
                        break;
                    case LogType.Information:
                        Stream.WriteLine(DateTime.Now + " - " + message + " called from the class " + caller + ".");
                        break;
                    case LogType.Temporal:
                        Stream.WriteLine(DateTime.Now + " - " + message + ".");
                        break;
                    case LogType.Gossip:
                        Stream.WriteLine(DateTime.Now + " - " + message + ".");
                        break;
                }
                Stream.Close();
                if (!Equals(null, ReturnedToConsole))
                {
                    ReturnedToConsole();
                }
            }
            //Stream.Dispose();
            
        }
        /// <summary>
        /// Records a transcript of the conversation.
        /// </summary>
        /// <param name="message">The message to save in transcript format.</param>
        /// <param name="projectType">Type of the project.</param>
        public static void RecordTranscript(string message, ProjectType projectType)
        {
            if (projectType == ProjectType.TestFramework)
            {
                Stream = new StreamWriter(PathLiteral + @"\logs\transcript.txt", true);
                Stream.WriteLine(DateTime.Now + " - " + message);
                Stream.Close();
            }
            else
            {
                Stream = new StreamWriter(FilePath() + @"\logs\transcript.txt", true);
                Stream.WriteLine(DateTime.Now + " - " + message);
                Stream.Close();
            }
        }
        /// <summary>
        /// Saves the last result to support analysis of the algorithm.
        /// </summary>
        /// <param name="output">The output from the conversation.</param>
        /// <param name="caller">The caller.</param>
        public static void SaveLastResult(StringBuilder output, LogCaller caller)
        {
            if (caller == LogCaller.TestFramework)
            {
                Stream = new StreamWriter(PathLiteral + @"\db\AnimalAnalytics.txt", true);
                Stream.WriteLine(DateTime.Now + " - " + output);
                Stream.Close();
            }
            else
            {
                Stream = new StreamWriter(FilePath() + @"\db\AnimalAnalytics.txt", true);
                Stream.WriteLine(DateTime.Now + " - " + output);
                Stream.Close();
            }
        }
        /// <summary>
        /// Saves the last result to support analysis of the algorithm to storage.
        /// </summary>
        /// <param name="output">The output from the conversation.</param>
        /// <param name="caller">The caller.</param>
        public static void SaveLastResultToStorage(StringBuilder output, LogCaller caller)
        {
            if (caller == LogCaller.TestFramework)
            {
                Stream = new StreamWriter(PathLiteral + @"\db\AnimalAnalyticsStorage.txt", true);
                Stream.WriteLine("#" + DateTime.Now + ";" + output);
                Stream.Close();
            }
            else
            {
                Stream = new StreamWriter(FilePath() + @"\db\AnimalAnalyticsStorage.txt", true);
                Stream.WriteLine("#" + DateTime.Now + ";" + output);
                Stream.Close();
            }
        }
        /// <summary>
        /// Writes a debug log with object parameters.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="objects">The objects.</param>
        public static void Debug(LogCaller caller, params object[] objects)
        {
            // Use FilePath() when outside of a test framework.
            if (caller == LogCaller.TestFramework)
            {
                Stream = new StreamWriter(PathLiteral + @"\logs\logfile.txt", true);
                foreach (object obj in objects)
                {
                    Stream.WriteLine(obj);
                }
                Stream.WriteLine("--");
                Stream.Close();
            }
            else
            {
                Stream = new StreamWriter(FilePath() + @"\logs\logfile.txt", true);
                foreach (object obj in objects)
                {
                    Stream.WriteLine(obj);
                }
                Stream.WriteLine("--");
                Stream.Close();
            }
            
        }
    }
}
