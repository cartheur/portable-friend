using System;
using System.Xml;
using Cartheur.Animals.CF.Core;
using Cartheur.Animals.CF.Utilities;

namespace Cartheur.Animals.CF.AeonHandlers
{
    /// <summary>
    /// The star element indicates that an interpreter should substitute the value "captured" by a particular wildcard from the pattern-specified portion of the match path when returning the template. 
    /// 
    /// The star element has an optional integer index attribute that indicates which wildcard to use. The minimum acceptable value for the index is "1" (the first wildcard), and the maximum acceptable value is equal to the number of wildcards in the pattern. 
    /// 
    /// An interpreter should raise an error if the index attribute of a star specifies a wildcard that does not exist in the category element's pattern. Not specifying the index is the same as specifying an index of "1". 
    /// 
    /// The star element does not have any content. 
    /// </summary>
    public class Star : AeonTagHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Star"/> class.
        /// </summary>
        /// <param name="aeon">The aeon involved in this request.</param>
        /// <param name="thisUser">The user making the request.</param>
        /// <param name="query">The query that originated this node.</param>
        /// <param name="userRequest">The request sent by the user.</param>
        /// <param name="userResult">The result to be sent back to the user.</param>
        /// <param name="templateNode">The node to be processed.</param>
        public Star(Aeon aeon,
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
            if (TemplateNode.Name.ToLower() == "star")
            {
                if (Query.InputStar.Count > 0)
                {
                    if (TemplateNode.Attributes != null && TemplateNode.Attributes.Count == 0)
                    {
                        // Return the first (latest) star in the List<>.
                        return Query.InputStar[0];
                    }
                    if (TemplateNode.Attributes != null && TemplateNode.Attributes.Count == 1)
                    {
                        if (TemplateNode.Attributes[0].Name.ToLower() == "index")
                        {
                            try
                            {
                                int index = Convert.ToInt32(TemplateNode.Attributes[0].Value);
                                index--;
                                if ((index >= 0) & (index < Query.InputStar.Count))
                                {
                                    return Query.InputStar[index];
                                }
                                ThisAeon.WriteToLog("InputStar out of bounds reference caused by input: " + UserRequest.RawInput, Logging.LogType.Error, Logging.LogCaller.Star);
                            }
                            catch
                            {
                                ThisAeon.WriteToLog("Index set to non-integer value while processing star tag in response to the input: " + UserRequest.RawInput, Logging.LogType.Error, Logging.LogCaller.Star);
                            }
                        }
                    }
                }
                else
                {
                    ThisAeon.WriteToLog("A star tag tried to reference an empty InputStar collection when processing the input: " + UserRequest.RawInput, Logging.LogType.Error, Logging.LogCaller.Star);
                }
            }
            return string.Empty;
        }
    }
}
