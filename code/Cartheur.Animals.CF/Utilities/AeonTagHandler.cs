using System.Xml;
using Cartheur.Animals.CF.Core;

namespace Cartheur.Animals.CF.Utilities 
{
    /// <summary>
    /// The template for all classes that handle the <aeon/> tags found within template nodes of a category.
    /// </summary>
    abstract public class AeonTagHandler : TextTransformer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AeonTagHandler"/> class.
        /// </summary>
        /// <param name="aeon">The aeon involved in this request.</param>
        /// <param name="thisUser">The user making the request.</param>
        /// <param name="query">The query that originated this node.</param>
        /// <param name="userRequest">The request sent by the user.</param>
        /// <param name="userResult">The result to be sent back to the user.</param>
        /// <param name="templateNode">The node to be processed.</param>
        protected AeonTagHandler   (Aeon aeon, 
                                    User thisUser, 
                                    SubQuery query,
                                    Request userRequest, 
                                    Result userResult, 
                                    XmlNode templateNode) :base(aeon,templateNode.OuterXml)
        {
            ThisUser = thisUser;
            Query = query;
            UserRequest = userRequest;
            UserResult = userResult;
            TemplateNode = templateNode;
            XmlAttributeCollection xmlAttributeCollection = TemplateNode.Attributes;
            if (xmlAttributeCollection != null) xmlAttributeCollection.RemoveNamedItem("xmlns");
        }
        /// <summary>
        /// Default to use when late binding.
        /// </summary>
        protected AeonTagHandler() { }
        /// <summary>
        /// A flag to denote if inner tags are to be processed recursively before processing this tag.
        /// </summary>
        public bool IsRecursive = true;
        /// <summary>
        /// A representation of the user making the request.
        /// </summary>
        public User ThisUser;
        /// <summary>
        /// The query that produced this node containing the wildcard matches.
        /// </summary>
        public SubQuery Query;
        /// <summary>
        /// A representation of the input made by the user.
        /// </summary>
        public Request UserRequest;
        /// <summary>
        /// A representation of the result to be returned to the user.
        /// </summary>
        public Result UserResult;
        /// <summary>
        /// The template node to be processed by the class.
        /// </summary>
        public XmlNode TemplateNode;
        /// <summary>
        /// Helper method that turns the passed string into an xml node.
        /// </summary>
        /// <param name="outerXml">The string to xmlize.</param>
        /// <returns>The xml node.</returns>
        public static XmlNode GetNode(string outerXml)
        {
            XmlDocument temp = new XmlDocument();
            temp.LoadXml(outerXml);
            return temp.FirstChild;
        }
    }
}
