using System.Xml;
using Cartheur.Animals.CF.Core;
using Cartheur.Animals.CF.Utilities;

namespace Cartheur.Animals.CF.AeonHandlers
{
    /// <summary>
    /// The uppercase element tells the interpreter to render the contents of the element in uppercase, as defined (if defined) by the locale indicated by the specified language if specified).
    /// 
    /// If no character in this string has a different uppercase version, based on the Unicode standard, then the original string is returned. 
    /// </summary>
    public class Uppercase : AeonTagHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Uppercase"/> class.
        /// </summary>
        /// <param name="aeon">The aeon involved in this request.</param>
        /// <param name="thisUser">The user making the request.</param>
        /// <param name="query">The query that originated this node.</param>
        /// <param name="userRequest">The request sent by the user.</param>
        /// <param name="userResult">The result to be sent back to the user.</param>
        /// <param name="templateNode">The node to be processed.</param>
        public Uppercase(Aeon aeon,
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
            if (TemplateNode.Name.ToLower() == "uppercase")
            {
                return TemplateNode.InnerText.ToUpper(ThisAeon.Locale);
            }
            return string.Empty;
        }
    }
}
