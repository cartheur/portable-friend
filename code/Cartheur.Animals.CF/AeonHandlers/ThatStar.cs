using System;
using System.Xml;
using Cartheur.Animals.CF.Core;
using Cartheur.Animals.CF.Utilities;

namespace Cartheur.Animals.CF.AeonHandlers
{
    /// <summary>
    /// The thatstar element tells the interpreter that it should substitute the contents of a wildcard from a pattern-side that element. 
    /// 
    /// The thatstar element has an optional integer index attribute that indicates which wildcard to use; the minimum acceptable value for the index is "1" (the first wildcard). 
    /// 
    /// An interpreter should raise an error if the index attribute of a star specifies a wildcard that does not exist in the that element's pattern content. Not specifying the index is the same as specifying an index of "1". 
    /// 
    /// The thatstar element does not have any content. 
    /// </summary>
    public class ThatStar : AeonTagHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThatStar"/> class.
        /// </summary>
        /// <param name="aeon">The aeon involved in this request.</param>
        /// <param name="thisUser">The user making the request.</param>
        /// <param name="query">The query that originated this node.</param>
        /// <param name="userRequest">The request sent by the user.</param>
        /// <param name="userResult">The result to be sent back to the user.</param>
        /// <param name="templateNode">The node to be processed.</param>
        public ThatStar(Aeon aeon,
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
            if (TemplateNode.Name.ToLower() == "thatstar")
            {
                if (TemplateNode.Attributes != null && TemplateNode.Attributes.Count == 0)
                {
                    if (Query.ThatStar.Count > 0)
                    {
                        return Query.ThatStar[0];
                    }
                    ThisAeon.WriteToLog("An out of bounds index to thatstar was encountered when processing the input: " + UserRequest.RawInput, Logging.LogType.Error, Logging.LogCaller.ThatStar);
                }
                else if (TemplateNode.Attributes != null && TemplateNode.Attributes.Count == 1)
                {
                    if (TemplateNode.Attributes[0].Name.ToLower() == "index")
                    {
                        if (TemplateNode.Attributes[0].Value.Length > 0)
                        {
                            try
                            {
                                int result = Convert.ToInt32(TemplateNode.Attributes[0].Value.Trim());
                                if (Query.ThatStar.Count > 0)
                                {
                                    if (result > 0)
                                    {
                                        return Query.ThatStar[result - 1];
                                    }
                                    ThisAeon.WriteToLog("An input tag with a badly formed index (" + TemplateNode.Attributes[0].Value + ") was encountered processing the input: " + UserRequest.RawInput, Logging.LogType.Error, Logging.LogCaller.ThatStar);
                                }
                                else
                                {
                                    ThisAeon.WriteToLog("An out of bounds index to thatstar was encountered when processing the input: " + UserRequest.RawInput, Logging.LogType.Error, Logging.LogCaller.ThatStar);
                                }
                            }
                            catch
                            {
                                ThisAeon.WriteToLog("A thatstar tag with a badly formed index (" + TemplateNode.Attributes[0].Value + ") was encountered processing the input: " + UserRequest.RawInput, Logging.LogType.Error, Logging.LogCaller.ThatStar);
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }
    }
}
