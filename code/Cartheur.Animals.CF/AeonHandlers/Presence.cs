using System.Xml;
using Cartheur.Animals.CF.Core;
using Cartheur.Animals.CF.Utilities;

namespace Cartheur.Animals.CF.AeonHandlers
{
    /// <summary>
    /// An element called presence, which may be considered a restricted version of get, is used to tell the interpreter that it should substitute the contents of a "presence predicate". The value of a presence predicate is set at load-time, and cannot be changed at run-time. The interpreter may decide how to set the values of presence predicate at load-time. If the presence predicate has no value defined, the interpreter should substitute an empty string.
    /// 
    /// The presence element has a required name attribute that identifies the presence predicate. 
    /// 
    /// The presence element does not have any content. 
    /// </summary>
    public class Presence : AeonTagHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Presence"/> class.
        /// </summary>
        /// <param name="aeon">The aeon involved in this request.</param>
        /// <param name="thisUser">The user making the request.</param>
        /// <param name="query">The query that originated this node.</param>
        /// <param name="userRequest">The request sent by the user.</param>
        /// <param name="userResult">The result to be sent back to the user.</param>
        /// <param name="templateNode">The node to be processed.</param>
        public Presence(Aeon aeon,
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
            if (TemplateNode.Name.ToLower() == "presence")
            {
                if (TemplateNode.Attributes != null && TemplateNode.Attributes.Count == 1)
                {
                    if (TemplateNode.Attributes[0].Name.ToLower() == "name")
                    {
                        string key = TemplateNode.Attributes["name"].Value;
                        return ThisAeon.GlobalSettings.GrabSetting(key);
                    }
                }
            }
            return string.Empty;
        }
    }
}
