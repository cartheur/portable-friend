using System;
using System.Collections.Generic;
using System.Text;
using Cartheur.Animals.CF.Utilities;

namespace Cartheur.Animals.CF.Core
{
    /// <summary>
    /// Encapsulates information about the result of a request to the brain.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// The user's aeon that is providing the answer.
        /// </summary>
        public Aeon UserAeon;
        /// <summary>
        /// The user for whom this is a result.
        /// </summary>
        public User ThisUser;
        /// <summary>
        /// The request from the user.
        /// </summary>
        public Request UserRequest;
        /// <summary>
        /// The raw input from the user.
        /// </summary>
        public string RawInput
        {
            get
            {
                return UserRequest.RawInput;
            }
        }
        /// <summary>
        /// The normalized sentence(s) (paths) fed into the brain.
        /// </summary>
        public List<string> NormalizedPaths = new List<string>();
        /// <summary>
        /// The amount of time the request took to process.
        /// </summary>
        public TimeSpan Duration;
        /// <summary>
        /// The last message time.
        /// </summary>
        public DateTime LastMessageTime;
        /// <summary>
        /// The result from aeon.
        /// </summary>
        public string Output
        {
            get
            {
                if (OutputSentences.Count > 0)
                {
                    return RawOutput;
                }
                if (UserRequest.HasTimedOut)
                {
                    return UserAeon.TimeOutMessage;
                }
                StringBuilder paths = new StringBuilder();
                foreach (string pattern in NormalizedPaths)
                {
                    paths.Append(pattern + "\r\n");
                }
                LastMessageTime = DateTime.Now;
                Logging.WriteLog("The brain could not find any response for the input: " + RawInput + " with the path: " + paths + " from the user: " + ThisUser.UserID, Logging.LogType.Warning, Logging.LogCaller.Result);

                return string.Empty;
            }
        }
        /// <summary>
        /// Returns the raw sentences.
        /// </summary>
        public string RawOutput
        {
            get
            {
                StringBuilder result = new StringBuilder();
                foreach (string sentence in OutputSentences)
                {
                    string sentenceForOutput = sentence.Trim();
                    if (!CheckEndsAsSentence(sentenceForOutput))
                    {
                        sentenceForOutput += ".";
                    }
                    result.Append(sentenceForOutput + " ");
                    LastMessageTime = DateTime.Now;
                }
                return result.ToString().Trim();
            }
        }
        /// <summary>
        /// The SubQuery objects processed by the brain which contain the templates that are to be converted into the collection of Sentences.
        /// </summary>
        public List<SubQuery> SubQueries = new List<SubQuery>();
        /// <summary>
        /// The individual sentences produced by the brain that form the complete response.
        /// </summary>
        public List<string> OutputSentences = new List<string>();
        /// <summary>
        /// The individual sentences that constitute the raw input from the user.
        /// </summary>
        public List<string> InputSentences = new List<string>();
        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        /// <param name="thisUser">The user for whom this is a result.</param>
        /// <param name="aeon">The brain providing the result.</param>
        /// <param name="userRequest">The request that originated this result.</param>
        public Result(User thisUser, Aeon aeon, Request userRequest)
        {
            ThisUser = thisUser;
            UserAeon = aeon;
            UserRequest = userRequest;
            UserRequest.UserResult = this;
        }
        /// <summary>
        /// Returns the raw output from the brain.
        /// </summary>
        /// <returns>The raw output from the brain.</returns>
        public override string ToString()
        {
            return Output;
        }
        /// <summary>
        /// Checks that the provided sentence ends with a sentence splitter.
        /// </summary>
        /// <param name="sentence">The sentence to check.</param>
        /// <returns>True if ends with an appropriate sentence splitter.</returns>
        private bool CheckEndsAsSentence(string sentence)
        {
            foreach (string splitter in UserAeon.Splitters)
            {
                if (sentence.Trim().EndsWith(splitter))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
