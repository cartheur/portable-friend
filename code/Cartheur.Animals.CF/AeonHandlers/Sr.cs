using System.Xml;
using Cartheur.Animals.CF.Core;
using Cartheur.Animals.CF.Utilities;

namespace Cartheur.Animals.CF.AeonHandlers
{
    /// <summary>
    /// The sr element is a shortcut for: 
    /// 
    /// <srai><star/></srai> 
    /// 
    /// The atomic sr does not have any content. 
    /// </summary>
    public class Sr : AeonTagHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sr"/> class.
        /// </summary>
        /// <param name="aeon">The aeon involved in this request.</param>
        /// <param name="thisUser">The user making the request.</param>
        /// <param name="query">The query that originated this node.</param>
        /// <param name="userRequest">The request sent by the user.</param>
        /// <param name="userResult">The result to be sent back to the user.</param>
        /// <param name="templateNode">The node to be processed.</param>
        public Sr(Aeon aeon,
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
            if (TemplateNode.Name.ToLower() == "sr")
            {
                XmlNode starNode = GetNode("<star/>");
                Star recursiveStar = new Star(ThisAeon, ThisUser, Query, UserRequest, UserResult, starNode);
                string starContent = recursiveStar.Transform();

                XmlNode sraiNode = GetNode("<srai>"+starContent+"</srai>");
                Srai sraiHandler = new Srai(ThisAeon, ThisUser, Query, UserRequest, UserResult, sraiNode);
                return sraiHandler.Transform();
            }
            return string.Empty;
        }
    }
}
