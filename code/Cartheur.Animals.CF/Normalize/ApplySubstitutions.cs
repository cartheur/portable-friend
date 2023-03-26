using System;
using System.Text;
using System.Text.RegularExpressions;
using Cartheur.Animals.CF.Core;
using Cartheur.Animals.CF.Utilities;

namespace Cartheur.Animals.CF.Normalize
{
    /// <summary>
    /// Checks the text for any matches in the presence's substitutions dictionary and makes any appropriate changes.
    /// </summary>
    public class ApplySubstitutions : TextTransformer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplySubstitutions"/> class.
        /// </summary>
        /// <param name="aeon">The aeon this transformer is a part of</param>
        /// <param name="inputString">The input string to be transformed</param>
        public ApplySubstitutions(Aeon aeon, string inputString)
            : base(aeon, inputString)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplySubstitutions"/> class.
        /// </summary>
        /// <param name="aeon">The aeon this transformer is a part of</param>
        public ApplySubstitutions(Aeon aeon)
            : base(aeon)
        { }
        /// <summary>
        /// Gets the marker.
        /// </summary>
        /// <param name="len">The length.</param>
        private static string GetMarker(int len)
        {
            char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            StringBuilder result = new StringBuilder();
            Random r = new Random();
            for (int i = 0; i < len; i++)
            {
                result.Append(chars[r.Next(chars.Length)]);
            }
            return result.ToString();
        }
        /// <summary>
        /// The method that does the actual processing of the text.
        /// </summary>
        protected override string ProcessChange()
        {
            return Substitute(ThisAeon, ThisAeon.Substitutions, InputString);
        }
        /// <summary>
        /// Static helper that applies replacements from the passed dictionary object to the target string.
        /// </summary>
        /// <param name="aeon">The aeon for whom this is being processed</param>
        /// <param name="dictionary">The dictionary containing the substitutions</param>
        /// <param name="target">the target string to which the substitutions are to be applied</param>
        /// <returns>The processed string</returns>
        public static string Substitute(Aeon aeon, SettingsDictionary dictionary, string target)
        {
            string marker = GetMarker(5);
            string result = target;
            foreach (string pattern in dictionary.SettingNames)
            {
                string p2 = MakeRegexSafe(pattern);
                //string match = "\\b"+@p2.Trim().Replace(" ","\\s*")+"\\b";
                string match = "\\b" + p2.TrimEnd().TrimStart() + "\\b";
                string replacement = marker+dictionary.GrabSetting(pattern).Trim()+marker;
                result = Regex.Replace(result, match, replacement, RegexOptions.IgnoreCase);
            }

            return result.Replace(marker, "");
        }
        /// <summary>
        /// Given an input, escapes certain characters so they can be used as part of a regex.
        /// </summary>
        /// <param name="input">The raw input</param>
        /// <returns>the safe version</returns>
        private static string MakeRegexSafe(string input)
        {
            string result = input.Replace("\\","");
            result = result.Replace(")", "\\)");
            result = result.Replace("(", "\\(");
            result = result.Replace(".", "\\.");
            return result;
        }
    }
}
