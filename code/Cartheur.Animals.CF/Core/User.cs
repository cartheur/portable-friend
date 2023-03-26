using System;
using System.Collections.Generic;

namespace Cartheur.Animals.CF.Core
{
    /// <summary>
    /// Encapsulates information and history of a user who has interacted with aeon.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The local instance of the GUID that identifies this user.
        /// </summary>
        private readonly string _id;
        /// <summary>
        /// The aeon this user is using.
        /// </summary>
        public Aeon UserAeon;
        /// <summary>
        /// The GUID that identifies this user.
        /// </summary>
        public string UserID
        {
            get{return _id;}
        }
        /// <summary>
        /// A collection of all the result objects returned to the user in this session.
        /// </summary>
        public readonly List<Result> AeonReplies = new List<Result>();
        /// <summary>
        /// The value of the "topic" predicate.
        /// </summary>
        public string Topic
        {
            get
            {
                return Predicates.GrabSetting("topic");
            }
        }
        /// <summary>
        /// The value of the "emotion" predicate.
        /// </summary>
        public string Emotion
        {
            get
            {
                return Predicates.GrabSetting("emotion");
            }
        }
        /// <summary>
		/// The predicates associated with this particular user.
		/// </summary>
        public SettingsDictionary Predicates;
        /// <summary>
        /// The most recent result to be returned by aeon.
        /// </summary>
        public Result LastAeonReply
        {
            get
            {
                return AeonReplies.Count > 0 ? AeonReplies[0] : null;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="userId">The GUID of the user.</param>
        /// <param name="aeon">The aeon the user is connected to.</param>
        /// <exception cref="System.Exception">The UserID cannot be empty.</exception>
		public User(string userId, Aeon aeon)
		{
            if (userId.Length > 0)
            {
                _id = userId;
                UserAeon = aeon;
                Predicates = new SettingsDictionary(UserAeon);
                UserAeon.DefaultPredicates.Clone(Predicates);
                Predicates.AddSetting("topic", "*");
                Predicates.AddSetting("emotion", "*");
            }
            else
            {
                throw new Exception("The UserID cannot be empty.");
            }
		}
        /// <summary>
        /// Returns the string to use for the next that part of a subsequent path.
        /// </summary>
        /// <returns>The string to use for that.</returns>
        public string GetLastAeonOutput()
        {
            if (AeonReplies.Count > 0)
            {
                return AeonReplies[0].RawOutput;
            }
            return "*";
        }
        /// <summary>
        /// Returns the first sentence of the last output from aeon.
        /// </summary>
        /// <returns>The first sentence of the last output from aeon.</returns>
        public string GetThat()
        {
            return GetThat(0,0);
        }
        /// <summary>
        /// Returns the first sentence of the output n-steps ago from aeon.
        /// </summary>
        /// <param name="n">The number of steps back to go.</param>
        /// <returns>The first sentence of the output n-steps ago from aeon.</returns>
        public string GetThat(int n)
        {
            return GetThat(n, 0);
        }
        /// <summary>
        /// Returns the sentence numbered by "sentence" of the output n-steps ago from aeon.
        /// </summary>
        /// <param name="n">The number of steps back to go.</param>
        /// <param name="sentence">The sentence number to get.</param>
        /// <returns>The sentence numbered by "sentence" of the output n-steps ago from aeon.</returns>
        public string GetThat(int n, int sentence)
        {
            if ((n >= 0) & (n < AeonReplies.Count))
            {
                Result historicResult = AeonReplies[n];
                if ((sentence >= 0) & (sentence < historicResult.OutputSentences.Count))
                {
                    return historicResult.OutputSentences[sentence];
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// Returns the first sentence of the last output from aeon.
        /// </summary>
        /// <returns>The first sentence of the last output from aeon.</returns>
        public string GetAeonReply()
        {
            return GetAeonReply(0, 0);
        }
        /// <summary>
        /// Returns the first sentence from the output from aeon n-steps ago.
        /// </summary>
        /// <param name="n">The number of steps back to go.</param>
        /// <returns>The first sentence from the output from aeon n-steps ago.</returns>
        public string GetAeonReply(int n)
        {
            return GetAeonReply(n, 0);
        }
        /// <summary>
        /// Returns the identified sentence number from the output from aeon n-steps ago.
        /// </summary>
        /// <param name="n">The number of steps back to go.</param>
        /// <param name="sentence">The sentence number to return.</param>
        /// <returns>The identified sentence number from the output from aeon n-steps ago.</returns>
        public string GetAeonReply(int n, int sentence)
        {
            if ((n >= 0) & (n < AeonReplies.Count))
            {
                Result historicResult = AeonReplies[n];
                if ((sentence >= 0) & (sentence < historicResult.InputSentences.Count))
                {
                    return historicResult.InputSentences[sentence];
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// Adds the latest result from aeon to the results collection.
        /// </summary>
        /// <param name="latestResult">The latest result from aeon.</param>
        public void AddResult(Result latestResult)
        {
            AeonReplies.Insert(0, latestResult);
        }
    }
}
