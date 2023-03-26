//
// This autonomous intelligent system software is the property of Cartheur Robotics, spol. s r.o. Copyright 2017, all rights reserved.
//
using System;
using System.Reflection;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Cartheur.Animals.CF.Core;
using Cartheur.Animals.CF.Learning;
using Cartheur.Animals.CF.Personality;
using Cartheur.Animals.CF.Utilities;

namespace Cartheur.Animals.CF.Gui.Forms
{
    public partial class MainForm : Form
    {
        private ViewResultForm _viewResult;
        private static Aeon _thisAeon;
        private static User _thisUser;
        public DateTime AeonChatStartedOn;
        public TimeSpan AeonChatDuration;
        public static string UserInput { get; set; }
        public static string AeonOutput { get; set; }
        public static Mood AeonMood { get; set; }
        public static string CurrentMood { get; set; }
        public static Result LastResult { get; set; }
        public static Request UserRequest { get; set; }
        public static string LastOutput { get; set; }
        public bool AeonIsAlone { get; set; }
        public static string TestMap = "10x10.map";

        public MainForm()
        {
            InitializeComponent();
            InputBox.Focus();
            _thisAeon = new Aeon();
            _thisAeon.LoadSettings(PathToSettings);
            _thisUser = new User("Chris", _thisAeon);
            _thisAeon.Name = "Rhoda";
            _thisAeon.WrittenToLog += MyAeonWrittenToLog;
            emotionUsedMenuItem.Checked = false;
            OutputBox.Text = @"Use 'File' -> 'Open aeon...' in the menu to load your emotional animal." + "\r\n" + "\r\n";
            _thisAeon.AeonAloneStartedOn = DateTime.Now;
        }

        public void ProcessInput()
        {
            if (_thisAeon.IsAcceptingUserInput)
            {
                AeonChatStartedOn = DateTime.Now;
                Thread.Sleep(250);
                UserInput = InputBox.Text;
                InputBox.Text = string.Empty;
                OutputBox.Text += "User: " + UserInput + "\r\n";
                var myRequest = new Request(UserInput, _thisUser, _thisAeon);
                Cursor.Current = Cursors.WaitCursor;
                var myResult = _thisAeon.Chat(myRequest);
                LastResult = myResult;
                AeonOutput = myResult.Output;
                OutputBox.Text += "Aeon: " + myResult.Output + "\r\n";
                OutputBox.ScrollToCaret();
                Refresh();
                Logging.RecordTranscript("User: " + UserInput);
                Logging.RecordTranscript("Aeon: " + myResult.Output);
                // Record performance vectors for the result.
                AeonChatDuration = DateTime.Now - AeonChatStartedOn;
                Logging.WriteLog(
                    "Result search was conducted in: " + AeonChatDuration.Seconds + @"." + AeonChatDuration.Milliseconds +
                    " seconds", Logging.LogType.Information, Logging.LogCaller.AeonGui);
                //debugOutputBox.Text = myResult.SubQueries[0].FullPath + Environment.NewLine;
                //debugOutputBox.Text += myResult.SubQueries[0].Template;
                if (_thisAeon.EmotionUsed)
                {
                    // Add to the mood dynamic based on the last interaction between user and aeon.

                }
                Cursor.Current = Cursors.Default;
            }
            else
            {
                InputBox.Text = string.Empty;
                OutputBox.Text += "Aeon is not accepting user input." + "\r\n";
            }
        }

        #region Commonly-used methods across the platform and result construction
        private void LoadDictionaries()
        {
            // Load dictionaries from the various configuration files.
            _thisAeon.Person2Substitutions.LoadSettings(Path.Combine(PathToConfigFiles, _thisAeon.GlobalSettings.GrabSetting("person2substitutionsfile")));
            _thisAeon.PersonSubstitutions.LoadSettings(Path.Combine(PathToConfigFiles, _thisAeon.GlobalSettings.GrabSetting("personsubstitutionsfile")));
            _thisAeon.GenderSubstitutions.LoadSettings(Path.Combine(PathToConfigFiles, _thisAeon.GlobalSettings.GrabSetting("gendersubstitutionsfile")));
            _thisAeon.DefaultPredicates.LoadSettings(Path.Combine(PathToConfigFiles, _thisAeon.GlobalSettings.GrabSetting("defaultpredicates")));
            _thisAeon.Substitutions.LoadSettings(Path.Combine(PathToConfigFiles, _thisAeon.GlobalSettings.GrabSetting("substitutionsfile")));
            // Grab the splitters.
            _thisAeon.LoadSplitters(Path.Combine(PathToConfigFiles, _thisAeon.GlobalSettings.GrabSetting("splittersfile")));
        }
        private void LoadPersonality()
        {
            // Let's check out this late-binding assembly feature in this code.
            //_thisAeon.LoadCustomTagHandlers(PathToLibraries + "\\" + "Cartheur.Animals.Extras.dll");

            Cursor.Current = Cursors.WaitCursor;
            try
            {
                var loader = new AeonLoader(_thisAeon);
                _thisAeon.IsAcceptingUserInput = false;
                // Load in the proper order.
                loader.LoadAeon(PathToReductions);
                loader.LoadAeon(PathToMindpixel);
                loader.LoadAeon(PathToPersonality);
                loader.LoadAeon(PathToUpdate);
                Logging.WriteLog(@"--Rhoda personality loaded.--", Logging.LogType.Information, Logging.LogCaller.AeonGui);
                _thisAeon.IsAcceptingUserInput = true;
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.AeonGui);
                MessageBox.Show(@"Unable to load personality: " + ex.Message, @"Loading error", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
            AeonChatDuration = DateTime.Now - AeonChatStartedOn;
            Logging.WriteLog("Personality loaded in: " + AeonChatDuration.Minutes + @" minutes and " + AeonChatDuration.Seconds + @"." + AeonChatDuration.Milliseconds + " seconds", Logging.LogType.Information, Logging.LogCaller.AeonGui);
            if (_thisAeon.EmotionUsed)
            {
                // Start the animals mood, when an emotional response is desired.
                var mood = Convert.ToInt32(StaticRandom.Next(0, 20));
                AeonMood = new Mood(mood);
                CurrentMood = AeonMood.GetCurrentMood();
                Logging.WriteLog("Emotion active from:  " + DateTime.Now, Logging.LogType.Information, Logging.LogCaller.AeonGui);
            }
            // Learning startup check is not implemented. Is the mood check really needed?
            OutputBox.Text = @"Personality loaded.";
            Cursor.Current = Cursors.Default;

        }
        private void SetMood()
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                _thisAeon.EmotionUsed = true;
                // Start the animals mood, when an emotional response is desired.
                var mood = Convert.ToInt32(StaticRandom.Next(0, 20));
                AeonMood = new Mood(mood);
                CurrentMood = AeonMood.GetCurrentMood();
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.AeonGui);
            }
            OutputBox.Text = @"Mood engine active.";
            Cursor.Current = Cursors.Default;
        }
        private void PrepareLearning()
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                _thisAeon.Tidbit = new LearningThreads(_thisAeon, LearningTypes.QLearning);
                _thisAeon.Tidbit.ImportMap(PathToLearningMap, TestMap);
                _thisAeon.Tidbit.QLearningThread();
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.AeonGui);
            }
            OutputBox.Text = @"Learning initialized.";
            Cursor.Current = Cursors.Default;
        }
        protected StringBuilder ConstructResult()
        {
            var result = new StringBuilder();
            if (LastResult == null)
            {
                OutputBox.Text = "";
                OutputBox.Text = "Result is empty. Load a personality and start a conversation first.";
            }
            if (!Equals(null, LastResult))
            {
                result.Append("Query path execution --");
                result.Append("\r\n");

                foreach (var query in LastResult.SubQueries)
                {
                    result.Append("Template: " + query.Template + "\r\n");
                    result.Append("Path: " + query.FullPath + "\r\n");
                    result.Append("Input stars: ");

                    foreach (var star in query.InputStar)
                    {
                        result.Append(star);
                    }
                    result.Append("\r\n");
                    result.Append("That stars: ");

                    foreach (var that in query.ThatStar)
                    {
                        result.Append(that);
                    }
                    result.Append("\r\n");
                    result.Append("Topic stars: ");
                    foreach (var topic in query.TopicStar)
                    {
                        result.Append(topic);
                    }
                    result.Append("\r\n");
                    result.Append("Emotion: ");
                    foreach (var emotion in query.EmotionStar)
                    {
                        result.Append(emotion);
                    }
                    result.Append("\r\n");
                }
                result.Append("Details of the last result --" + "\r\n");
                result.Append("Input: " + LastResult.RawInput + "\r\n");
                result.Append("Output: " + LastResult.Output + "\r\n");
                result.Append("Duration: " + LastResult.Duration + "\r\n");
                result.Append("Sentences: ");
                foreach (var sentence in LastResult.InputSentences)
                {
                    result.Append(sentence + "\r\n");
                }
                result.Append("\r\n");
            }
            return result;
        }
        protected StringBuilder ConstructResultStorage()
        {
            var result = new StringBuilder();

            if (LastResult == null)
            {
                OutputBox.Text = "";
                OutputBox.Text = "Result is empty. Load a personality and start a conversation first.";
            }
            if (!Equals(null, LastResult))
            {
                result.Append("#RawInput:" + LastResult.RawInput + ";");
                if (LastResult.RawInput == "")
                    result.Append("[empty];");
                result.Append("#Output:" + LastResult.Output + ";");
                if (LastResult.Output == "")
                    result.Append("[empty];");
                result.Append("#RawOutput:" + LastResult.RawOutput + ";");
                if (LastResult.RawOutput == "")
                    result.Append("[empty];");
                result.Append("#Duration:" + LastResult.Duration + ";");
                if (LastResult.Duration == TimeSpan.Zero)
                    result.Append("[empty];");

                result.Append("#Sentences:");
                foreach (var sentence in LastResult.InputSentences)
                {
                    result.Append(sentence + ";");
                    if (sentence == "")
                        result.Append("[empty];");
                }
                foreach (var query in LastResult.SubQueries)
                {
                    result.Append("#Path:" + query.FullPath + ";");
                    if (query.FullPath == "")
                        result.Append("[empty];");
                    result.Append("#Template:" + query.Template + ";");
                    if (query.Template == "")
                        result.Append("[empty];");

                    result.Append("#InputStars:");
                    foreach (var star in query.InputStar)
                    {
                        result.Append(star + ";");
                        if (star == "")
                            result.Append("[empty];");
                    }
                    result.Append("#ThatStars:");
                    foreach (var that in query.ThatStar)
                    {
                        result.Append(that + ";");
                        if (that == "")
                            result.Append("[empty];");
                    }
                    result.Append("#TopicStars:");
                    foreach (var topic in query.TopicStar)
                    {
                        result.Append(topic + ";");
                        if (topic == "")
                            result.Append("[empty];");
                    }
                }
                result.Append("#OutputSentences:");
                foreach (var outputSentence in LastResult.OutputSentences)
                {
                    result.Append(outputSentence + ";");
                    if (outputSentence == "")
                        result.Append("[empty];");
                }
            }
            return result;
        }
        protected void ShowInformation(StringBuilder result)
        {
            _viewResult = new ViewResultForm { OutputMessage = result.ToString() };
            _viewResult.ShowDialog();
        }
        #endregion

        #region Intriguing incremental program enhancements
        private void addExtraCodeFragmentsMenuItem_Click(object sender, EventArgs e)
        {
            // Add extra personality traits from new aeon files without relaunching the application.
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                AeonLoader loader = new AeonLoader(_thisAeon);
                _thisAeon.IsAcceptingUserInput = false;
                loader.LoadAeon(PathToExtras);
                OutputBox.Text += "Extras loaded.";
                Logging.RecordTranscript("---Extras loaded---");
                _thisAeon.IsAcceptingUserInput = true;
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.AeonGui);
                MessageBox.Show(@"Unable to load extras feature aeon file(s): " + ex.Message, @"File loading error", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                Cursor.Current = Cursors.Default;
            }

        }
        private void addExtraAssembliesMenuItem_Click(object sender, EventArgs e)
        {
            // Add new personality traits from libraries without relaunching the application.
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                AeonLoader loader = new AeonLoader(_thisAeon);
                _thisAeon.IsAcceptingUserInput = false;
                loader.LoadAeon(PathToLibraries);
                OutputBox.Text += "Additional libraries loaded.";
                Logging.RecordTranscript("---Additional libraries loaded---");
                _thisAeon.IsAcceptingUserInput = true;
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.AeonGui);
                MessageBox.Show(@"Unable to load extras feature libraries: " + ex.Message, @"Library loading error", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                Cursor.Current = Cursors.Default;
            }
        }
        #endregion

        #region Events
        // Click.
        private void EmotionUsedMenuItemClick(object sender, EventArgs e)
        {
            emotionUsedMenuItem.Checked = true;
            SetMood();
            Logging.WriteLog("Emotion active from:  " + DateTime.Now, Logging.LogType.Information, Logging.LogCaller.AeonGui);
            emotionUsedMenuItem.Enabled = false;
        }
        private void LearningUsedMenuItemClick(object sender, EventArgs e)
        {
            learningUsedMenuItem.Checked = true;
            PrepareLearning();
            Logging.WriteLog("Learning active from:  " + DateTime.Now, Logging.LogType.Information, Logging.LogCaller.AeonGui);
            learningUsedMenuItem.Enabled = false;
        }
        private void GoMenuItemClick(object sender, EventArgs e)
        {
            ProcessInput();
        }
        private void FromRhodaMenuItemClick(object sender, EventArgs e)
        {
            Thread.Sleep(250);
            try
            {
                InputBox.BringToFront();
                LoadDictionaries();
                LoadPersonality();
                InputBox.Focus();
            }
            catch (Exception ex)
            {
                OutputBox.Text = "";
                OutputBox.Text += ex.Message + "\r\n";
                Cursor.Current = Cursors.Default;
            }
            AeonChatDuration = DateTime.Now - AeonChatStartedOn;
            Logging.WriteLog("Rhoda personality loaded in: " + AeonChatDuration.Minutes + @" minutes and " + AeonChatDuration.Seconds + @"." + AeonChatDuration.Milliseconds + " seconds", Logging.LogType.Information, Logging.LogCaller.AeonGui);

        }
        private void ClearConsoleMenuItemClick(object sender, EventArgs e)
        {
            OutputBox.Text = "";
            Refresh();
        }
        private void ShutdownMenuItemClick(object sender, EventArgs e)
        {
            DialogResult exitQuery = MessageBox.Show(@"Do you really want to terminate your aeon?", @"Termination request",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

            if (exitQuery == DialogResult.OK)
            {
                Application.Exit();
            }
        }
        private void LastResultMenuItemClick(object sender, EventArgs e)
        {
            if (LastResult == null)
            {
                OutputBox.Text = "";
                OutputBox.Text = "Result is empty. Load a personality and start a conversation first.";
            }
            if (!Equals(null, LastResult))
            {
                var result = ConstructResult();
                ShowInformation(result);
            }
        }
        private void LastRequestMenuItemClick(object sender, EventArgs e)
        {
            if (UserRequest == null)
            {
                OutputBox.Text = "";
                OutputBox.Text = "Request is empty, load a personality first.";
            }
            if (!Equals(null, UserRequest))
            {
                var result = new StringBuilder();
                result.Append("Last request:" + "\r\n" + "\r\n");
                result.Append("Raw input: " + UserRequest.RawInput.Replace("\r\n", "") + "\r\n");
                result.Append("Started on: " + UserRequest.StartedOn + "\r\n");
                result.Append("Has timed out: " + Convert.ToString(UserRequest.HasTimedOut) + "\r\n" + "\r\n");
                ShowInformation(result);
            }
        }
        // Ordinal.
        private void MyAeonWrittenToLog()
        {
            OutputBox.Text += _thisAeon.LastLogMessage + "\r\n" + "\r\n";
            OutputBox.ScrollToCaret();
        }
        private void OutputBox_TextChanged(object sender, EventArgs e)
        {
            // How can this be creatively used? I think for learning.
        }
        #endregion

        #region Local implementation of directory paths
        /// <summary>
        /// The directory to look in for the aeon files for the personality.
        /// </summary>
        public string PathToPersonality
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), _thisAeon.GlobalSettings.GrabSetting("personalitydirectory"));
            }
        }
        /// <summary>
        /// The directory to look in for the aeon files for additions at runtime.
        /// </summary>
        public string PathToExtras
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), _thisAeon.GlobalSettings.GrabSetting("extrasdirectory"));
            }
        }
        /// <summary>
        /// The directory to look in for the aeon files for additions at runtime.
        /// </summary>
        public string PathToLibraries
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), _thisAeon.GlobalSettings.GrabSetting("librariesdirectory"));
            }
        }
        /// <summary>
        /// The directory to look in for the files for the reductions.
        /// </summary>
        public string PathToReductions
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), _thisAeon.GlobalSettings.GrabSetting("reductionsdirectory"));
            }
        }
        /// <summary>
        /// The directory to look in for the files for the mindpixel.
        /// </summary>
        public string PathToMindpixel
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), _thisAeon.GlobalSettings.GrabSetting("mindpixeldirectory"));
            }
        }
        /// <summary>
        /// The directory to look in for the files for the update.
        /// </summary>
        public string PathToUpdate
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), _thisAeon.GlobalSettings.GrabSetting("updatedirectory"));
            }
        }
        /// <summary>
        /// The directory to look in for the various XML configuration files.
        /// </summary>
        public string PathToConfigFiles
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), _thisAeon.GlobalSettings.GrabSetting("configdirectory"));
            }
        }
        /// <summary>
        /// The directory into which the various log files will be written.
        /// </summary>
        public string PathToLogs
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), _thisAeon.GlobalSettings.GrabSetting("logdirectory"));
            }
        }
        /// <summary>
        /// Gets the path to settings file.
        /// </summary>
        public static string PathToSettings
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), Path.Combine("config", "Settings.xml"));
            }
        }
        /// <summary>
        /// Gets the path to learning map.
        /// </summary>
        public static string PathToLearningMap
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), _thisAeon.GlobalSettings.GrabSetting("mapdirectory"));
            }
        }
        #endregion

        #region Invoke WM_flow for voicing
        // NOTE: This seems less and less likely to achieve on this platform.
        // For help here: http://www.informit.com/articles/article.aspx?p=27219&seqNum=8
        // private readonly SpVoiceClass _speaking = new SpVoiceClass();
        // private ISpeechAudio speakAudio;
        // .....
        //Type t = typeof(SpVoice);
        //Type t = Type.GetTypeFromProgID("SAPI.SpVoice");
        //Type t = Type.GetTypeFromCLSID(new Guid("96749377-3391-11D2-9EE3-00C04F797396"));
        //Type t = Type.GetType("SpeechLib.SpVoiceClass, SpeechLib, Version=5.0.0.0, Culture=neutral, PublicKeyToken=null");
        //object voice = Activator.CreateInstance(t);
        //t.InvokeMember("Speak", BindingFlags.InvokeMethod, null, voice, new object[] { OutputBox.Text, 0 });
        //SpVoiceClass speaking = new SpVoiceClass();
        //if (speakMenuItem.Checked)
        //{
        //    speaking.Speak(OutputBox.Text, SpeechVoiceSpeakFlags.SVSFDefault);
        //}
        #endregion

    }
}
