using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Cartheur.Animals.CF.AeonHandlers;
using Cartheur.Animals.CF.Learning;
using Cartheur.Animals.CF.Normalize;
using Cartheur.Animals.CF.Personality;
using Cartheur.Animals.CF.Utilities;
using Gender = Cartheur.Animals.CF.AeonHandlers.Gender;
using Random = Cartheur.Animals.CF.AeonHandlers.Random;
using Version = Cartheur.Animals.CF.AeonHandlers.Version;

namespace Cartheur.Animals.CF.Core
{
    /// <summary>
    /// The intuitive presence to which the functions and behavior are attached.
    /// </summary>
    public class Aeon
    {
        /// <summary>
        /// Holds information about the available custom tag handling classes (if loaded).
        /// <param>The class name.
        ///     <name>Key</name>
        /// </param>
        /// <param>The TagHandler class that provides information about the class.
        ///     <name>Value</name>
        /// </param>
        /// </summary>
        private Dictionary<string, TagHandler> _customTags;
        /// <summary>
        /// Holds references to the assemblies that hold the custom tag handling code.
        /// </summary>
        private readonly Dictionary<string, Assembly> _lateBindingAssemblies = new Dictionary<string, Assembly>();
        /// <summary>
        /// The message to show if a user tries to use aeon whilst set to not process user input.
        /// </summary>
        private string NotAcceptingUserInputMessage
        {
            get
            {
                return GlobalSettings.GrabSetting("notacceptinguserinputmessage");
            }
        }
        /// <summary>
        /// Flag to show if aeon is accepting user input.
        /// </summary>
        public bool IsAcceptingUserInput = true;
        /// <summary>
        /// The number of categories aeon has in her brain.
        /// </summary>
        public int Size;
        /// <summary>
        /// If set to false the input from aeon code files will undergo the same normalization process that user input goes through. If true aeon will assume the code structure is correct. Defaults to true.
        /// </summary>
        public bool TrustCodeFiles = true;
        /// <summary>
        /// The maximum number of characters a "that" element of a path is allowed to be. Anything above this length will cause "that" to be "*". This is to avoid having the core process huge "that" elements in the path that might have been caused by aeon reporting third party data.
        /// </summary>
        public int MaxThatSize = 256;
        /// <summary>
        /// The maximum amount of time the aeon should be left alone before prompting the user (in milliseconds).
        /// </summary>
        public double AloneTime
        {
            get
            {
                return Convert.ToDouble(GlobalSettings.GrabSetting("alonetime"));
            }
        }
        /// <summary>
        /// The maximum amount of time a request should take (in milliseconds).
        /// </summary>
        public double TimeOut
        {
            get
            {
                return Convert.ToDouble(GlobalSettings.GrabSetting("timeout"));
            }
        }
        /// <summary>
        /// The message to display in the event of a timeout.
        /// </summary>
        public string TimeOutMessage
        {
            get
            {
                return GlobalSettings.GrabSetting("timeoutmessage");
            }
        }
        /// <summary>
        /// The locale of aeon as a CultureInfo object.
        /// </summary>
        public CultureInfo Locale
        {
            get
            {
                return new CultureInfo(GlobalSettings.GrabSetting("culture"));
            }
        }
        /// <summary>
        /// Will match all the illegal characters that might be input by the user.
        /// </summary>
        public Regex Strippers
        {
            get
            {
                return new Regex(GlobalSettings.GrabSetting("stripperregex"), RegexOptions.IgnorePatternWhitespace);
            }
        }
        /// <summary>
        /// The email address to correspond with.
        /// </summary>
        public string AdminEmail
        {
            get
            {
                return GlobalSettings.GrabSetting("adminemail");
            }
            set
            {
                if (value.Length > 0)
                {
                    // Check that the email adress is in a valid format.
                    const string patternStrict = @"^(([^<>()[\]\\.,;:\s@\""]+"
                                                 + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                                                 + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                                                 + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                                                 + @"[a-zA-Z]{2,}))$";
                    Regex reStrict = new Regex(patternStrict);

                    if (reStrict.IsMatch(value))
                    {
                        // Update settings.
                        GlobalSettings.AddSetting("adminemail", value);
                    }
                    else
                    {
                        throw (new Exception("The AdminEmail is not a valid email address"));
                    }
                }
                else
                {
                    GlobalSettings.AddSetting("adminemail", "");
                }
            }
        }
        /// <summary>
        /// Flag to denote if aeon is logging.
        /// </summary>
        public bool IsLogging
        {
            get
            {
                string islogging = GlobalSettings.GrabSetting("islogging");
                if (islogging.ToLower() == "true")
                {
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// The time when this aeon was started.
        /// </summary>
        public DateTime AeonStartedOn;
        /// <summary>
        /// The time when the alone time started.
        /// </summary>
        public DateTime AeonAloneStartedOn { get; set; }
        /// <summary>
        /// The last message to be entered into the log (for testing purposes)
        /// </summary>
        public string LastLogMessage = string.Empty;
        /// <summary>
        /// Returns the persistence of the aeon (in milliseconds).
        /// </summary>
        public int Persistence()
        {
            if (PersonalityLoaded)
                return (DateTime.Now - AeonStartedOn).Milliseconds;
            return 0;
        }
        /// <summary>
        /// Gets or sets a value indicating whether [emotion used].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [emotion used]; otherwise, <c>false</c>.
        /// </value>
        public bool EmotionUsed { get; set; }
        /// <summary>
        /// Flag to indicate if a personality is loaded.
        /// </summary>
        public static bool PersonalityLoaded;
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [about me].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [about me]; otherwise, <c>false</c>.
        /// </value>
        public bool AboutMe { get; set; }
        /// <summary>
        /// The splitters object.
        /// </summary>
        public List<string> Splitters = new List<string>();
        /// <summary>
        /// The brain of aeon.
        /// </summary>
        public Node ThisNode;
        /// <summary>
        /// Determines if learning is active or not.
        /// </summary>
        public enum LearningMode
        {
            True,
            False
        };
        public LearningThreads Tidbit { get; set; }
        /// <summary>
        /// Gets the alone threshold.
        /// </summary>
        public TimeSpan AloneThreshold
        {
            get
            {
                return new TimeSpan(0, 0, 0, Convert.ToInt32(GlobalSettings.GrabSetting("alonethreshold")));
            }
        }
        /// <summary>
        /// Computes the alone time duration.
        /// </summary>
        /// <returns>The duration.</returns>
        public TimeSpan AeonAloneDuration()
        {
            return (AeonStartedOn - DateTime.Now);
        }
        /// <summary>
        /// Determines whether the aeon is alone.
        /// </summary>
        /// <returns>If this aeon is feeling alone.</returns>
        public bool IsAlone()
        {
            if (AeonAloneDuration() > AloneThreshold)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// The supposed sex of aeon.
        /// </summary>
        public AeonGender Sex
        {
            get
            {
                int sex = Convert.ToInt32(GlobalSettings.GrabSetting("gender"));
                AeonGender result;
                switch (sex)
                {
                    case -1:
                        result = AeonGender.Unknown;
                        break;
                    case 0:
                        result = AeonGender.Female;
                        break;
                    case 1:
                        result = AeonGender.Male;
                        break;
                    default:
                        result = AeonGender.Unknown;
                        break;
                }
                return result;
            }
        }
        /// <summary>
        /// A dictionary object that looks after all the settings associated with this aeon.
        /// </summary>
        public SettingsDictionary GlobalSettings;
        /// <summary>
        /// A dictionary of all the gender based substitutions used by this aeon.
        /// </summary>
        public SettingsDictionary GenderSubstitutions;
        /// <summary>
        /// A dictionary of all the first person to second-person (and back) substitutions.
        /// </summary>
        public SettingsDictionary Person2Substitutions;
        /// <summary>
        /// A dictionary of first to third-person substitutions.
        /// </summary>
        public SettingsDictionary PersonSubstitutions;
        /// <summary>
        /// Generic substitutions that take place during the normalization process.
        /// </summary>
        public SettingsDictionary Substitutions;
        /// <summary>
        /// The default predicates to set up for a user.
        /// </summary>
        public SettingsDictionary DefaultPredicates;
        /// <summary>
        /// Instantiates the dictionary objects and collections associated with this class.
        /// </summary>
        private void Setup()
        {
            GlobalSettings = new SettingsDictionary(this);
            GenderSubstitutions = new SettingsDictionary(this);
            Person2Substitutions = new SettingsDictionary(this);
            PersonSubstitutions = new SettingsDictionary(this);
            Substitutions = new SettingsDictionary(this);
            DefaultPredicates = new SettingsDictionary(this);
            _customTags = new Dictionary<string, TagHandler>();
            ThisNode = new Node();
            AeonStartedOn = DateTime.Now;
            WrittenToLog += AeonWrittenToLog;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Aeon"/> class.
        /// </summary>
        public Aeon()
        {
            Setup();
        }

        #region Mood settings from config file
        /// <summary>
        /// Gets the emotion engine seed value.
        /// </summary>
        public string MoodEngineSeedValue
        {
            get
            {
                return GlobalSettings.GrabSetting("seedmoodengine");
            }
        }
        #endregion

        #region Delegates & events

        public delegate void LogMessageDelegate();
        public event LogMessageDelegate WrittenToLog;

        static void AeonWrittenToLog()
        {

        }

        #endregion

        #region Settings and load methods
        /// <summary>
        /// Allows aeon to load a new xml version of some aeon xms data.
        /// </summary>
        /// <param name="xmlCode">The xml document containing xms.</param>
        /// <param name="filename">The originator of the xml document.</param>
        public void LoadAeonFromXml(XmlDocument xmlCode, string filename)
        {
            AeonLoader loader = new AeonLoader(this);
            loader.LoadAeonFromXml(xmlCode, filename);
        }
        /// <summary>
        /// Loads settings and configuration info from various xml files referenced in the settings file passed in the args. Also generates some default values if such values have not been set by the settings file.
        /// </summary>
        /// <param name="pathToSettings">Path to the settings xml file.</param>
        public void LoadSettings(string pathToSettings)
        {
            GlobalSettings.LoadSettings(pathToSettings);

            // Checks for some critical default settings.
            if (!GlobalSettings.ContainsSettingCalled("version"))
            {
                GlobalSettings.AddSetting("version", Environment.Version.ToString());
            }
            if (!GlobalSettings.ContainsSettingCalled("name"))
            {
                GlobalSettings.AddSetting("name", "Unknown");
            }
            if (!GlobalSettings.ContainsSettingCalled("botmaster"))
            {
                GlobalSettings.AddSetting("botmaster", "Unknown");
            }
            if (!GlobalSettings.ContainsSettingCalled("master"))
            {
                GlobalSettings.AddSetting("botmaster", "Unknown");
            }
            if (!GlobalSettings.ContainsSettingCalled("author"))
            {
                GlobalSettings.AddSetting("author", "malfactor");
            }
            if (!GlobalSettings.ContainsSettingCalled("location"))
            {
                GlobalSettings.AddSetting("location", "Unknown");
            }
            if (!GlobalSettings.ContainsSettingCalled("gender"))
            {
                GlobalSettings.AddSetting("gender", "-1");
            }
            if (!GlobalSettings.ContainsSettingCalled("birthday"))
            {
                GlobalSettings.AddSetting("birthday", "2008/11/08");
            }
            if (!GlobalSettings.ContainsSettingCalled("birthplace"))
            {
                GlobalSettings.AddSetting("birthplace", "somewhere");
            }
            if (!GlobalSettings.ContainsSettingCalled("website"))
            {
                GlobalSettings.AddSetting("website", "http://emotional.toys");
            }
            if (GlobalSettings.ContainsSettingCalled("adminemail"))
            {
                string emailToCheck = GlobalSettings.GrabSetting("adminemail");
                AdminEmail = emailToCheck;
            }
            else
            {
                GlobalSettings.AddSetting("adminemail", "");
            }
            if (!GlobalSettings.ContainsSettingCalled("islogging"))
            {
                GlobalSettings.AddSetting("islogging", "true");
            }
            if (!GlobalSettings.ContainsSettingCalled("willcallhome"))
            {
                GlobalSettings.AddSetting("willcallhome", "False");
            }
            if (!GlobalSettings.ContainsSettingCalled("timeout"))
            {
                GlobalSettings.AddSetting("timeout", "2000");
            }
            if (!GlobalSettings.ContainsSettingCalled("timeoutmessage"))
            {
                GlobalSettings.AddSetting("timeoutmessage", "ERROR: The request has timed out.");
            }
            if (!GlobalSettings.ContainsSettingCalled("culture"))
            {
                GlobalSettings.AddSetting("culture", "en-UK");
            }
            if (!GlobalSettings.ContainsSettingCalled("splittersfile"))
            {
                GlobalSettings.AddSetting("splittersfile", "Splitters.xml");
            }
            if (!GlobalSettings.ContainsSettingCalled("person2substitutionsfile"))
            {
                GlobalSettings.AddSetting("person2substitutionsfile", "Person2Substitutions.xml");
            }
            if (!GlobalSettings.ContainsSettingCalled("personsubstitutionsfile"))
            {
                GlobalSettings.AddSetting("personsubstitutionsfile", "PersonSubstitutions.xml");
            }
            if (!GlobalSettings.ContainsSettingCalled("gendersubstitutionsfile"))
            {
                GlobalSettings.AddSetting("gendersubstitutionsfile", "GenderSubstitutions.xml");
            }
            if (!GlobalSettings.ContainsSettingCalled("defaultpredicates"))
            {
                GlobalSettings.AddSetting("defaultpredicates", "DefaultPredicates.xml");
            }
            if (!GlobalSettings.ContainsSettingCalled("substitutionsfile"))
            {
                GlobalSettings.AddSetting("substitutionsfile", "Substitutions.xml");
            }
            if (!GlobalSettings.ContainsSettingCalled("configdirectory"))
            {
                GlobalSettings.AddSetting("configdirectory", "config");
            }
            if (!GlobalSettings.ContainsSettingCalled("logdirectory"))
            {
                GlobalSettings.AddSetting("logdirectory", "logs");
            }
            if (!GlobalSettings.ContainsSettingCalled("maxlogbuffersize"))
            {
                GlobalSettings.AddSetting("maxlogbuffersize", "64");
            }
            if (!GlobalSettings.ContainsSettingCalled("notacceptinguserinputmessage"))
            {
                GlobalSettings.AddSetting("notacceptinguserinputmessage", "Aeon is currently set to not accept user input.");
            }
            if (!GlobalSettings.ContainsSettingCalled("stripperregex"))
            {
                GlobalSettings.AddSetting("stripperregex", "[^0-9a-zA-Z]");
            }
            if (!GlobalSettings.ContainsSettingCalled("password"))
            {
                GlobalSettings.AddSetting("password", "XhUkIjUnYvTqIjUj");
            }
        }
        /// <summary>
        /// Updates a settings entry.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void UpdateSetting(string key, string value)
        {
            GlobalSettings.UpdateSetting(key, value);
        }
        /// <summary>
        /// Adds an entry to settings.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void AddSetting(string key, string value)
        {
            GlobalSettings.AddSetting(key, value);
        }
        /// <summary>
        /// Loads splitters for this aeon from the supplied config file (or sets up some appropriate defaults).
        /// </summary>
        /// <param name="pathToSplitters">Path to the config file.</param>
        public void LoadSplitters(string pathToSplitters)
        {
            FileInfo splittersFile = new FileInfo(pathToSplitters);
            if (splittersFile.Exists)
            {
                XmlDocument splittersXmlDoc = new XmlDocument();
                splittersXmlDoc.Load(pathToSplitters);
                // The XML should have an XML declaration like this:
                // <?xml version="1.0" encoding="utf-8" ?> 
                // followed by a <root> tag with children of the form:
                // <item value="value"/>
                if (splittersXmlDoc.ChildNodes.Count == 2)
                {
                    if (splittersXmlDoc.LastChild.HasChildNodes)
                    {
                        foreach (XmlNode myNode in splittersXmlDoc.LastChild.ChildNodes)
                        {
                            if (myNode.Attributes != null && (myNode.Name == "item") & (myNode.Attributes.Count == 1))
                            {
                                string value = myNode.Attributes["value"].Value;
                                Splitters.Add(value);
                            }
                        }
                    }
                }
            }
            if (Splitters.Count == 0)
            {
                // We don't have any splitters, so lets make do with the following.
                Splitters.Add(".");
                Splitters.Add("!");
                Splitters.Add("?");
                Splitters.Add(";");
            }
        }
        /// <summary>
        /// Writes a message to aeon's log. Encapsulates the logger functionality.
        /// </summary>
        public void WriteToLog(string message, Logging.LogType logType, Logging.LogCaller logCaller)
        {
            LastLogMessage = message;
            if (IsLogging)
            {
                Logging.WriteLog(message, logType, logCaller);
            }
            if (!Equals(null, WrittenToLog))
            {
                WrittenToLog();
            }
        }
        #endregion

        #region Conversation methods

        /// <summary>
        /// Given some raw input and a unique ID creates a response for a new user.
        /// </summary>
        /// <param name="rawInput">the raw input.</param>
        /// <param name="userGuid">The ID for the user (referenced in the result object).</param>
        /// <returns>Result to the user.</returns>
        public Result Chat(string rawInput, string userGuid)
        {
            Request request = new Request(rawInput, new User(userGuid, this), this);
            return Chat(request);
        }
        /// <summary>
        /// Given a request containing user input, produces a result from aeon.
        /// </summary>
        /// <param name="request">The request from the user.</param>
        /// <returns>The result to be output to the user.</returns>
        public Result Chat(Request request)
        {
            Result result = new Result(request.ThisUser, this, request);
            // Set the emotion, where used. It is now known to the core.
            result.ThisUser.Predicates.UpdateSetting("EMOTION", Mood.CurrentMood);

            if (IsAcceptingUserInput)
            {
                // Normalize the input.
                AeonLoader loader = new AeonLoader(this);
                SplitIntoSentences splitter = new SplitIntoSentences(this);
                string[] rawSentences = splitter.Transform(request.RawInput);
                foreach (string sentence in rawSentences)
                {
                    result.InputSentences.Add(sentence);
                    string pathGenerated;
                    if (EmotionUsed)
                    {
                        pathGenerated = loader.GeneratePath(sentence, request.ThisUser.GetLastAeonOutput(), request.ThisUser.Topic, request.ThisUser.Emotion, true);
                        result.NormalizedPaths.Add(pathGenerated);
                    }
                    else
                    {
                        pathGenerated = loader.GeneratePath(sentence, request.ThisUser.GetLastAeonOutput(), request.ThisUser.Topic, true);
                        result.NormalizedPaths.Add(pathGenerated);
                    }
                    
                }
                // Grab the templates for the various sentences.
                foreach (string path in result.NormalizedPaths)
                {
                    SubQuery query = new SubQuery(path);
                    query.Template = ThisNode.Evaluate(path, query, request, MatchState.UserInput, new StringBuilder());
                    result.SubQueries.Add(query);
                }
                // Influence the type of response based on the value of the mood (if set to true).
                if (request.RawInput.Contains("you feeling") && EmotionUsed)
                {
                    AboutMe = true;
                    Mood.Compliment++;

                    #region
                    // Get the I am feeling "".
                    string newPath = "";
                    foreach (string normalizedPath in result.NormalizedPaths)
                    {
                        string emotionalOutput = "I am feeling " + Mood.CurrentMood + ".";
                        newPath = normalizedPath.Replace("<emotion> " + Mood.CurrentMood, "<emotion> " + emotionalOutput);
                    }
                    for (int i = 0; i < result.NormalizedPaths.Count; i++)
                    {
                        result.NormalizedPaths[i] = newPath;
                    }
                    //result.ThisUser.Predicates.UpdateSetting("EMOTION", Mood.CurrentMood);// doesn't do anything.
                    var hold = 0;
                    #endregion
                }
                if (rawSentences.Contains("love you") && EmotionUsed)
                {
                    AboutMe = true;
                    Mood.Compliment++;
                    Mood.Love++;
                }
                // Process the templates into appropriate output.
                foreach (SubQuery query in result.SubQueries)
                {
                    if (query.Template.Length > 0)
                    {
                        try
                        {
                            XmlNode templateNode = AeonTagHandler.GetNode(query.Template);
                            string outputSentence = ProcessNode(templateNode, query, request, result, request.ThisUser);
                            // Integrate the learned output with this query response.
                            if (outputSentence.Length > 0)
                            {
                                result.OutputSentences.Add(outputSentence);
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteToLog("A problem was encountered when trying to process the input: " + request.RawInput + " with the template: \"" + query.Template + ". The following exception message was noted: " + ex.Message, Logging.LogType.Warning, Logging.LogCaller.Aeon);
                        }
                    }
                }
            }
            else
            {
                result.OutputSentences.Add(NotAcceptingUserInputMessage);
            }
            // Populate the result object and note the performance.
            result.Duration = DateTime.Now - request.StartedOn;
            request.ThisUser.AddResult(result);

            return result;
        }
        /// <summary>
        /// Recursively evaluates the template nodes returned from aeon.
        /// </summary>
        /// <param name="node">The node to evaluate.</param>
        /// <param name="query">The query that produced this node.</param>
        /// <param name="request">The request from the user.</param>
        /// <param name="result">The result to be sent to the user.</param>
        /// <param name="user">The user who originated the request.</param>
        /// <returns>The output string.</returns>
        protected string ProcessNode(XmlNode node, SubQuery query, Request request, Result result, User user)
        {
            // Check for timeout (to avoid infinite loops).
            if (request.StartedOn.AddMilliseconds(request.UserAeon.TimeOut) < DateTime.Now)
            {
                request.UserAeon.WriteToLog("Request timeout. User: " + request.ThisUser.UserID + " raw input: \"" + request.RawInput + "\" processing template: \"" + query.Template + "\"", Logging.LogType.Warning, Logging.LogCaller.Aeon);
                request.HasTimedOut = true;
                return string.Empty;
            }

            // Process the node.
            string tagName = node.Name.ToLower();
            if (tagName == "template")
            {
                StringBuilder templateResult = new StringBuilder();
                if (node.HasChildNodes)
                {
                    // Recursively check. Stepping in here, how does it get four child nodes after parsing only one?
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        templateResult.Append(ProcessNode(childNode, query, request, result, user));
                        // Does this really step to the next child node?
                    }
                }
                return templateResult.ToString();
            }
            AeonTagHandler tagHandler = GetBespokeTags(user, query, request, result, node);

            #region switch case fold
            if (Equals(null, tagHandler))
            {
                switch (tagName)
                {
                    case "condition":
                        tagHandler = new Condition(this, user, query, request, result, node);
                        break;
                    case "date":
                        tagHandler = new Date(this, user, query, request, result, node);
                        break;
                    case "formal":
                        tagHandler = new Formal(this, user, query, request, result, node);
                        break;
                    case "gender":
                        tagHandler = new Gender(this, user, query, request, result, node);
                        break;
                    case "get":
                        tagHandler = new Get(this, user, query, request, result, node);
                        break;
                    case "gossip":
                        tagHandler = new Gossip(this, user, query, request, result, node);
                        break;
                    case "id":
                        tagHandler = new Id(this, user, query, request, result, node);
                        break;
                    case "input":
                        tagHandler = new Input(this, user, query, request, result, node);
                        break;
                    case "learn":
                        tagHandler = new Learn(this, user, query, request, result, node);
                        break;
                    case "lowercase":
                        tagHandler = new Lowercase(this, user, query, request, result, node);
                        break;
                    case "person":
                        tagHandler = new Person(this, user, query, request, result, node);
                        break;
                    case "person2":
                        tagHandler = new Person2(this, user, query, request, result, node);
                        break;
                    case "presence":
                        tagHandler = new Presence(this, user, query, request, result, node);
                        break;
                    case "random":
                        tagHandler = new Random(this, user, query, request, result, node);
                        break;
                    case "script":
                        tagHandler = new Script(this, user, query, request, result, node);
                        // Reserved for on-the-fly script execution.
                        break;
                    case "sentence":
                        tagHandler = new Sentence(this, user, query, request, result, node);
                        break;
                    case "set":
                        tagHandler = new Set(this, user, query, request, result, node);
                        break;
                    case "size":
                        tagHandler = new Size(this, user, query, request, result, node);
                        break;
                    case "sr":
                        tagHandler = new Sr(this, user, query, request, result, node);
                        break;
                    case "srai":
                        tagHandler = new Srai(this, user, query, request, result, node);
                        break;
                    case "star":
                        tagHandler = new Star(this, user, query, request, result, node);
                        break;
                    case "that":
                        tagHandler = new That(this, user, query, request, result, node);
                        break;
                    case "thatstar":
                        tagHandler = new ThatStar(this, user, query, request, result, node);
                        break;
                    case "think":
                        tagHandler = new Think(this, user, query, request, result, node);
                        break;
                    case "topicstar":
                        tagHandler = new TopicStar(this, user, query, request, result, node);
                        break;
                    case "uppercase":
                        tagHandler = new Uppercase(this, user, query, request, result, node);
                        break;
                    case "version":
                        tagHandler = new Version(this, user, query, request, result, node);
                        break;
                }
            }
            #endregion

            if (Equals(null, tagHandler))
            {
                return node.InnerText;
            }
            if (tagHandler.IsRecursive)
            {
                if (node.HasChildNodes)
                {
                    // Recursively check.
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        if (childNode.NodeType != XmlNodeType.Text)
                        {
                            childNode.InnerXml = ProcessNode(childNode, query, request, result, user);
                        }
                    }
                }
                return tagHandler.Transform();
            }
            string resultNodeInnerXml = tagHandler.Transform();
            XmlNode resultNode = AeonTagHandler.GetNode("<node>" + resultNodeInnerXml + "</node>");
            if (resultNode.HasChildNodes)
            {
                StringBuilder recursiveResult = new StringBuilder();
                // Recursively check.
                foreach (XmlNode childNode in resultNode.ChildNodes)
                {
                    recursiveResult.Append(ProcessNode(childNode, query, request, result, user));
                }
                return recursiveResult.ToString();
            }
            return resultNode.InnerXml;
        }
        /// <summary>
        /// Searches the custom tag collection and processes the aeon files if an appropriate tag handler is found.
        /// </summary>
        /// <param name="user">The user who originated the request.</param>
        /// <param name="query">The query that produced this node.</param>
        /// <param name="request">The request from the user.</param>
        /// <param name="result">The result to be sent to the user.</param>
        /// <param name="node">The node to evaluate.</param>
        /// <returns>The output string.</returns>
        public AeonTagHandler GetBespokeTags(User user, SubQuery query, Request request, Result result, XmlNode node)
        {
            if (_customTags.ContainsKey(node.Name.ToLower()))
            {
                TagHandler customTagHandler = _customTags[node.Name.ToLower()];
                AeonTagHandler newCustomTag = customTagHandler.Instantiate(_lateBindingAssemblies);
                if (Equals(null, newCustomTag))
                {
                    return null;
                }
                newCustomTag.ThisUser = user;
                newCustomTag.Query = query;
                newCustomTag.UserRequest = request;
                newCustomTag.UserResult = result;
                newCustomTag.TemplateNode = node;
                newCustomTag.ThisAeon = this;
                return newCustomTag;
            }
            return null;
        }

        #endregion

        #region Serialization

        /// <summary>
        /// Saves the tapmaster node (and children) to a binary file to avoid processing the AIML each time the 
        /// presence starts
        /// </summary>
        /// <param name="path">the path to the file for saving</param>
        public void SaveToBinaryFile(string path)
        {
            // check to delete an existing version of the file
            FileInfo fi = new FileInfo(path);
            if (fi.Exists)
            {
                fi.Delete();
            }

            FileStream saveFile = File.Create(path);
            //BinaryFormatter bf = new BinaryFormatter();
            //bf.Serialize(saveFile, ThisNode);
            saveFile.Close();
        }
        /// <summary>
        /// Loads a dump of the tapmaster into memory so avoiding processing the AIML files again
        /// </summary>
        /// <param name="path">the path to the dump file</param>
        public void LoadFromBinaryFile(string path)
        {
            FileStream loadFile = File.OpenRead(path);
            //BinaryFormatter bf = new BinaryFormatter();
            //ThisNode = (Node)bf.Deserialize(loadFile);
            loadFile.Close();
        }

        #endregion

        #region Latebinding custom-tag dll handlers

        /// <summary>
        /// Loads any custom tag handlers found in the dll referenced in the argument.
        /// </summary>
        /// <param name="pathToDll">The path to the dll containing the custom tag handling code.</param>
        public void LoadCustomTagHandlers(string pathToDll)
        {
            Assembly tagDll = Assembly.LoadFrom(pathToDll);
            Type[] tagDllTypes = tagDll.GetTypes();
            for (int i = 0; i < tagDllTypes.Length; i++)
            {
                object[] typeCustomAttributes = tagDllTypes[i].GetCustomAttributes(false);
                for (int j = 0; j < typeCustomAttributes.Length; j++)
                {
                    if (typeCustomAttributes[j] is CustomTagAttribute)
                    {
                        // We've found a custom tag handling class so store the assembly and store it away in the Dictionary as a TagHandler class for later usage.                      
                        // Store the assembly.
                        if (!_lateBindingAssemblies.ContainsKey(tagDll.FullName))
                        {
                            _lateBindingAssemblies.Add(tagDll.FullName, tagDll);
                        }
                        // Create the TagHandler representation.
                        TagHandler newTagHandler = new TagHandler();
                        newTagHandler.AssemblyName = tagDll.FullName;
                        newTagHandler.ClassName = tagDllTypes[i].FullName;
                        newTagHandler.TagName = tagDllTypes[i].Name.ToLower();
                        if (_customTags.ContainsKey(newTagHandler.TagName))
                        {
                            throw new Exception("Unable to add the custom tag: <" + newTagHandler.TagName + ">, found in: " + pathToDll + " as a handler for this tag already exists.");
                        }
                        _customTags.Add(newTagHandler.TagName, newTagHandler);
                    }
                }
            }
        }
        #endregion
    }
}
