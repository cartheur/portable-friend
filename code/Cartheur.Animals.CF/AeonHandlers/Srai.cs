using System.Xml;
using Cartheur.Animals.CF.Core;
using Cartheur.Animals.CF.Utilities;

namespace Cartheur.Animals.CF.AeonHandlers
{
    /// <summary>
    /// The srai element instructs the interpreter to pass the result of processing the contents of the srai element to the matching loop, as if the input had been produced by the user (this includes stepping through the entire input normalization process). The srai element does not have any attributes. It may contain any template elements. 
    /// 
    /// As with all elements, nested forms should be parsed from inside out, so embedded srais are perfectly acceptable. 
    /// </summary>
    public class Srai : AeonTagHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Srai"/> class.
        /// </summary>
        /// <param name="aeon">The aeon involved in this request.</param>
        /// <param name="thisUser">The user making the request.</param>
        /// <param name="query">The query that originated this node.</param>
        /// <param name="userRequest">The request sent by the user.</param>
        /// <param name="userResult">The result to be sent back to the user.</param>
        /// <param name="templateNode">The node to be processed.</param>
        public Srai(Aeon aeon,
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
            if (TemplateNode.Name.ToLower() == "srai")
            {
                if (TemplateNode.InnerText.Length > 0)
                {
                    Request subRequest = new Request(TemplateNode.InnerText, ThisUser, ThisAeon);
                    // Make sure time is not added to the request.
                    subRequest.StartedOn = UserRequest.StartedOn;
                    Result subQuery = ThisAeon.Chat(subRequest);
                    UserRequest.HasTimedOut = subRequest.HasTimedOut;
                    return subQuery.Output;
                }
            }
            return string.Empty;
        }
    }
}
