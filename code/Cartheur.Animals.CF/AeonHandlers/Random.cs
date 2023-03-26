using System.Collections.Generic;
using System.Xml;
using Cartheur.Animals.CF.Core;
using Cartheur.Animals.CF.Utilities;

namespace Cartheur.Animals.CF.AeonHandlers
{
    /// <summary>
    /// The random element instructs the interpreter to return exactly one of its contained li elements randomly. The random element must contain one or more li elements of type defaultListItem, and cannot contain any other elements.
    /// </summary>
    public class Random : AeonTagHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Random"/> class.
        /// </summary>
        /// <param name="aeon">The aeon involved in this request.</param>
        /// <param name="thisUser">The user making the request.</param>
        /// <param name="query">The query that originated this node.</param>
        /// <param name="userRequest">The request sent by the user.</param>
        /// <param name="userResult">The result to be sent back to the user.</param>
        /// <param name="templateNode">The node to be processed.</param>
        public Random(Aeon aeon,
                        User thisUser,
                        SubQuery query,
                        Request userRequest,
                        Result userResult,
                        XmlNode templateNode)
            : base(aeon, thisUser, query, userRequest, userResult, templateNode)
        {
            IsRecursive = false;
        }

        protected override string ProcessChange()
        {
            if (TemplateNode.Name.ToLower() == "random")
            {
                if (TemplateNode.HasChildNodes)
                {
                    // Only grab <li> nodes.
                    List<XmlNode> listNodes = new List<XmlNode>();
                    foreach (XmlNode childNode in TemplateNode.ChildNodes)
                    {
                        if (childNode.Name == "li")
                        {
                            listNodes.Add(childNode);
                        }
                    }
                    if (listNodes.Count > 0)
                    {
                        System.Random r = new System.Random();
                        XmlNode chosenNode = listNodes[r.Next(listNodes.Count)];
                        return chosenNode.InnerXml;
                    }
                }
            }
            return string.Empty;
        }
    }
}
