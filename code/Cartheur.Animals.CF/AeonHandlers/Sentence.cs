using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Cartheur.Animals.CF.Core;
using Cartheur.Animals.CF.Utilities;

namespace Cartheur.Animals.CF.AeonHandlers
{
    /// <summary>
    /// The sentence element tells the interpreter to render the contents of the element such that the first letter of each sentence is in uppercase, as defined (if defined) by the locale indicated by the specified language (if specified). Sentences are interpreted as strings whose last character is the period or full-stop character .. If the string does not contain a ., then the entire string is treated as a sentence. If no character in this string has a different uppercase version, based on the Unicode standard, then the original string is returned. 
    /// </summary>
    public class Sentence : AeonTagHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sentence"/> class.
        /// </summary>
        /// <param name="aeon">The aeon involved in this request.</param>
        /// <param name="thisUser">The user making the request.</param>
        /// <param name="query">The query that originated this node.</param>
        /// <param name="userRequest">The request sent by the user.</param>
        /// <param name="userResult">The result to be sent back to the user.</param>
        /// <param name="templateNode">The node to be processed.</param>
        public Sentence(Aeon aeon,
                        User thisUser,
                        SubQuery query,
                        Request userRequest,
                        Result userResult,
                        XmlNode templateNode)
            : base(aeon, thisUser, query, userRequest, userResult, templateNode)
        {
        }

        protected override string ProcessChange()
        {
            if(TemplateNode.Name.ToLower()=="sentence")
            {
                if (TemplateNode.InnerText.Length > 0)
                {
                    StringBuilder result = new StringBuilder();
                    char[] letters = TemplateNode.InnerText.Trim().ToCharArray();
                    bool doChange = true;
                    for (int i = 0; i < letters.Length; i++)
                    {
                        string letterAsString = Convert.ToString(letters[i]);
                        if (ThisAeon.Splitters.Contains(letterAsString))
                        {
                            doChange = true;
                        }

                        Regex lowercaseLetter = new Regex("[a-zA-Z]");

                        if (lowercaseLetter.IsMatch(letterAsString))
                        {
                            if (doChange)
                            {
                                result.Append(letterAsString.ToUpper(ThisAeon.Locale));
                                doChange = false;
                            }
                            else
                            {
                                result.Append(letterAsString.ToLower(ThisAeon.Locale));
                            }
                        }
                        else
                        {
                            result.Append(letterAsString);
                        }
                    }
                    return result.ToString();
                }
                // Atomic version of the node.
                XmlNode starNode = GetNode("<star/>");
                Star recursiveStar = new Star(ThisAeon, ThisUser, Query, UserRequest, UserResult, starNode);
                TemplateNode.InnerText = recursiveStar.Transform();
                if (!string.IsNullOrEmpty(TemplateNode.InnerText))
                {
                    return ProcessChange();
                }
                return string.Empty;
            }

            return string.Empty;
        }
    }
}
