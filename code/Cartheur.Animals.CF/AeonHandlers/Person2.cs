using System.Xml;
using Cartheur.Animals.CF.Core;
using Cartheur.Animals.CF.Normalize;
using Cartheur.Animals.CF.Utilities;

namespace Cartheur.Animals.CF.AeonHandlers
{
    /// <summary>
    /// The atomic version of the person2 element is a shortcut for: 
    /// 
    /// <person2><star/></person2> 
    /// 
    /// The atomic person2 does not have any content.
    /// 
    /// The non-atomic person2 element instructs the interpreter to: 
    /// 
    /// 1. Replace words with first-person aspect in the result of processing the contents of the person2 element with words with the grammatically-corresponding second-person aspect; and,
    /// 2. Replace words with second-person aspect in the result of processing the contents of the person2 element with words with the grammatically-corresponding first-person aspect. 
    /// 
    /// The definition of "grammatically-corresponding" is left up to the implementation. Historically, implementations of person2 have dealt with pronouns, likely due to the fact that 
	/// most code is written in English. However, the decision about whether to transform the person aspect of other words is left up to the implementation.
    /// </summary>
    public class Person2 : AeonTagHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Person2"/> class.
        /// </summary>
        /// <param name="aeon">The aeon involved in this request.</param>
        /// <param name="thisUser">The user making the request.</param>
        /// <param name="query">The query that originated this node.</param>
        /// <param name="userRequest">The request sent by the user.</param>
        /// <param name="userResult">The result to be sent back to the user.</param>
        /// <param name="templateNode">The node to be processed.</param>
        public Person2(Aeon aeon,
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
            if (TemplateNode.Name.ToLower() == "person2")
            {
                if (TemplateNode.InnerText.Length > 0)
                {
                    // Non-atomic version of the node.
                    return ApplySubstitutions.Substitute(ThisAeon, ThisAeon.Person2Substitutions, TemplateNode.InnerText);
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
