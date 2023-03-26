using System.IO;
using System.Xml;
using Cartheur.Animals.CF.Core;
using Cartheur.Animals.CF.Utilities;

namespace Cartheur.Animals.CF.AeonHandlers
{
    /// <summary>
    /// The learn element instructs the interpreter to retrieve a resource specified by a URI, and to process its aeon code object contents.
    /// </summary>
    public class Learn : AeonTagHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Learn"/> class.
        /// </summary>
        /// <param name="aeon">The aeon involved in this request.</param>
        /// <param name="thisUser">The user making the request.</param>
        /// <param name="query">The query that originated this node.</param>
        /// <param name="userRequest">The request sent by the user.</param>
        /// <param name="userResult">The result to be sent back to the user.</param>
        /// <param name="templateNode">The node to be processed.</param>
        public Learn(Aeon aeon,
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
            if (TemplateNode.Name.ToLower() == "learn")
            {
                // Currently only *.aeon files in the local filesystem can be referenced.
                // ToDo: Network HTTP and web service based learning
                if (TemplateNode.InnerText.Length > 0)
                {
                    string path = TemplateNode.InnerText;
                    FileInfo fi = new FileInfo(path);
                    if (fi.Exists)
                    {
                        XmlDocument doc = new XmlDocument();
                        try
                        {
                            doc.Load(path);
                            ThisAeon.LoadAeonFromXml(doc, path);
                        }
                        catch
                        {
                            ThisAeon.WriteToLog("Attempted (but failed) to <learn> some new aeon code from the following URI: " + path, Logging.LogType.Error, Logging.LogCaller.Learn);
                        }
                    }
                }
            }
            return string.Empty;
        }
    }
}
