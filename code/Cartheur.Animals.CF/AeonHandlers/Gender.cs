using System.Xml;
using Cartheur.Animals.CF.Core;
using Cartheur.Animals.CF.Normalize;
using Cartheur.Animals.CF.Utilities;

namespace Cartheur.Animals.CF.AeonHandlers
{
    /// <summary>
    /// The atomic version of the gender element is a shortcut for:
    /// 
    /// <gender><star/></gender> 
    ///
    /// The atomic gender does not have any content. 
    /// 
    /// The non-atomic gender element instructs the interpreter to: 
    /// 
    /// 1. replace male-gendered words in the result of processing the contents of the gender element with the grammatically-corresponding female-gendered words; and 
    /// 2. replace female-gendered words in the result of processing the contents of the gender element with the grammatically-corresponding male-gendered words. 
    /// 
    /// The definition of "grammatically-corresponding" is left up to the implementation. Historically, implementations of gender have exclusively dealt with pronouns, 
	/// likely due to the fact that most code is written in English. However, the decision about whether to transform gender of other words is left up to the implementation.
    /// </summary>
    public class Gender : AeonTagHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Gender"/> class.
        /// </summary>
        /// <param name="aeon">The aeon involved in this request.</param>
        /// <param name="thisUser">The user making the request.</param>
        /// <param name="query">The query that originated this node.</param>
        /// <param name="userRequest">The request sent by the user.</param>
        /// <param name="userResult">The result to be sent back to the user.</param>
        /// <param name="templateNode">The node to be processed.</param>
        public Gender(Aeon aeon,
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
            if (TemplateNode.Name.ToLower() == "gender")
            {
                if (TemplateNode.InnerText.Length > 0)
                {
                    // Non-atomic version of the node.
                    return ApplySubstitutions.Substitute(ThisAeon, ThisAeon.GenderSubstitutions, TemplateNode.InnerText);
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
