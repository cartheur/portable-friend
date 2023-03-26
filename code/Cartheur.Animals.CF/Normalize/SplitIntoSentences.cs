using System.Collections.Generic;
using Cartheur.Animals.CF.Core;

namespace Cartheur.Animals.CF.Normalize
{
    /// <summary>
    /// Splits the raw input into its constituent sentences. Split using the tokens found in aeon's Splitters string array.
    /// </summary>
    public class SplitIntoSentences
    {
        /// <summary>
        /// The aeon this sentence splitter is associated with
        /// </summary>
        private readonly Aeon _aeon;
        /// <summary>
        /// The raw input string
        /// </summary>
        private string _inputString;
        /// <summary>
        /// Initializes a new instance of the <see cref="SplitIntoSentences"/> class.
        /// </summary>
        /// <param name="aeon">The aeon this sentence splitter is associated with</param>
        /// <param name="inputString">The raw input string to be processed</param>
        public SplitIntoSentences(Aeon aeon, string inputString)
        {
            _aeon = aeon;
            _inputString = inputString;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SplitIntoSentences"/> class.
        /// </summary>
        /// <param name="aeon">The aeon this sentence splitter is associated with</param>
        public SplitIntoSentences(Aeon aeon)
        {
            _aeon = aeon;
        }
        /// <summary>
        /// Splits the supplied raw input into an array of strings according to the tokens found in aeon's Splitters list.
        /// </summary>
        /// <param name="inputString">The raw input to split</param>
        /// <returns>An array of strings representing the constituent "sentences"</returns>
        public string[] Transform(string inputString)
        {
            _inputString = inputString;
            return Transform();
        }
        /// <summary>
        /// Splits the raw input supplied via the constructor into an array of strings according to the tokens found in aeon's Splitters list.
        /// </summary>
        /// <returns>An array of strings representing the constituent "sentences"</returns>
        public string[] Transform()
        {
            string[] tokens = _aeon.Splitters.ToArray();
            string[] rawResult = {};
            char[] charTokens = {};
            char charToken = new char();
            foreach (string token in tokens)
            {
                charTokens = token.ToCharArray();
            }
            for (int i = 0; i < charTokens.Length; i++)
            {
                rawResult = _inputString.Split(charToken);
            }

            List<string> tidyResult = new List<string>();
            foreach (string rawSentence in rawResult)
            {
                string tidySentence = rawSentence.Trim();
                if (tidySentence.Length > 0)
                {
                    tidyResult.Add(tidySentence);
                }
            }
            return tidyResult.ToArray();
        }
    }
}
