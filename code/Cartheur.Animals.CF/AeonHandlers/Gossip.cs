using System.Xml;
using Cartheur.Animals.CF.Core;
using Cartheur.Animals.CF.Utilities;

namespace Cartheur.Animals.CF.AeonHandlers
{
    /// <summary>
    /// The gossip element instructs the interpreter to capture the result of processing the contents of the gossip elements and to store these contents in a manner left up to the implementation. Most common uses of gossip have been to store captured contents in a separate file. 
    /// 
    /// The gossip element does not have any attributes. It may contain any template elements.
    /// </summary>
    public class Gossip : AeonTagHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Gossip"/> class.
        /// </summary>
        /// <param name="aeon">The aeon involved in this request.</param>
        /// <param name="thisUser">The user making the request.</param>
        /// <param name="query">The query that originated this node.</param>
        /// <param name="userRequest">The request sent by the user.</param>
        /// <param name="userResult">The result to be sent back to the user.</param>
        /// <param name="templateNode">The node to be processed.</param>
        public Gossip(Aeon aeon,
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
            if (TemplateNode.Name.ToLower() == "gossip")
            {
                // Gossip is merely logged by aeon and written to the log file.
                if (TemplateNode.InnerText.Length > 0)
                {
                    Logging.WriteLog("GOSSIP from user: " + ThisUser.UserID + ", '" + TemplateNode.InnerText + "'",
                        Logging.LogType.Gossip, Logging.LogCaller.Gossip);
                }
            }
            return string.Empty;
        }
    }
}
