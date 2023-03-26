using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Cartheur.Animals.CF.Normalize;
using Cartheur.Animals.CF.Utilities;

namespace Cartheur.Animals.CF.Core
{
    /// <summary>
    /// Encapsulates a node in the brain's tree structure.
    /// </summary>
    [Serializable]
    public class Node
    {
        /// <summary>
        /// Contains the child nodes of this node.
        /// </summary>
        private readonly Dictionary<string, Node> _children = new Dictionary<string, Node>();
        /// <summary>
        /// The number of direct children (non-recursive) of this node.
        /// </summary>
        public int NumberOfChildNodes
        {
            get
            {
                return _children.Count;
            }
        }
        /// <summary>
        /// The template (if any) associated with this node.
        /// </summary>
        public string Template = string.Empty;
        /// <summary>
        /// The aeon source for the category that defines the template.
        /// </summary>
        public string Filename = string.Empty;
        /// <summary>
        /// The word that identifies this node to its parent node.
        /// </summary>
        public string Word = string.Empty;
        /// <summary>
        /// Adds a category to the node.
        /// </summary>
        /// <param name="path">The path for the category.</param>
        /// <param name="template">The template to find at the end of the path.</param>
        /// <param name="filename">The file that was the source of this category.</param>
        public void AddCategory(string path, string template, string filename)
        {
            if (template.Length == 0)
            {
                throw new XmlException("The category with a pattern: " + path + " found in file: " + filename + " has an empty template tag. ABORTING");
            }

            // Check we're not at the leaf node.
            if (path.Trim().Length == 0)
            {
                Template = template;
                Filename = filename;
                return;
            }
            // Otherwise, this sentence requires further child nodemappers in order to be fully mapped within the program's structure.
            // Split the input into its component words.
            string[] words = path.Trim().Split(" ".ToCharArray());
            // Get the first word (to form the key for the child nodemapper).
            string firstWord = MakeCaseInsensitive.TransformInput(words[0]);
            // Concatenate the rest of the sentence into a suffix (to act as the path argument in the child nodemapper).
            string newPath = path.Substring(firstWord.Length, path.Length - firstWord.Length).Trim();
            // Check we don't already have a child with the key from this sentence. If we do then pass the handling of this sentence down the branch to the child nodemapper otherwise the child nodemapper doesn't yet exist, so create a new one.
            if (_children.ContainsKey(firstWord))
            {
                Node childNode = _children[firstWord];
                childNode.AddCategory(newPath, template, filename);
            }
            else
            {
                Node childNode = new Node {Word = firstWord};
                childNode.AddCategory(newPath, template, filename);
                _children.Add(childNode.Word, childNode);
            }
        }
        /// <summary>
        /// Navigates this node (and recusively into child nodes) for a match to the path passed as an argument while processing the referenced request.
        /// </summary>
        /// <param name="path">The normalized path derived from the user's input.</param>
        /// <param name="query">The query that this search is for.</param>
        /// <param name="request">An encapsulation of the request from the user.</param>
        /// <param name="matchstate">The part of the input path the node represents.</param>
        /// <param name="wildcard">The contents of the user input absorbed by wildcards "_" and "*".</param>
        /// <returns>The template to process to generate the output.</returns>
        public string Evaluate(string path, SubQuery query, Request request, MatchState matchstate, StringBuilder wildcard)
        {
            while (true)
            {
                // Check for timeout.
                if (request.StartedOn.AddMilliseconds(request.UserAeon.TimeOut) < DateTime.Now)
                {
                    request.UserAeon.WriteToLog("Request timeout. User: " + request.ThisUser.UserID + " raw input: \"" + request.RawInput + "\"", Logging.LogType.Warning, Logging.LogCaller.Node);
                    request.HasTimedOut = true;
                    return string.Empty;
                }
                // We still have time.
                path = path.Trim();
                // Check if this is the end of a branch and return the Category for this node.
                if (_children.Count == 0)
                {
                    if (path.Length > 0)
                    {
                        // If we get here it means that there is a wildcard in the user input part of the path.
                        StoreWildCard(path, wildcard);
                    }
                    return Template;
                }
                // If we've matched all the words in the input sentence and this is the end of the line then return the Category for this node.
                if (path.Length == 0)
                {
                    return Template;
                }
                // Otherwise split the input into its component words.
                string[] splitPath = path.Split(" \r\n\t".ToCharArray());
                // Get the first word of the sentence.
                string firstWord = MakeCaseInsensitive.TransformInput(splitPath[0]);
                // And concatenate the rest of the input into a new path for child nodes.
                string newPath = path.Substring(firstWord.Length, path.Length - firstWord.Length);
                // First option is to see if this node has a child denoted by the "_" wildcard. "_" comes first in precedence in the alphabet.
                if (_children.ContainsKey("_"))
                {
                    // Look for the path in the child node denoted by "_".
                    Node childNode = _children["_"];
                    // Add the next word to the wildcard match. 
                    StringBuilder newWildcard = new StringBuilder();
                    StoreWildCard(splitPath[0], newWildcard);
                    // Move down into the identified branch of the structure.
                    string result = childNode.Evaluate(newPath, query, request, matchstate, newWildcard);
                    // And if we get a result from the branch process the wildcard matches and return the result.
                    if (result.Length > 0)
                    {
                        if (newWildcard.Length > 0)
                        {
                            // Capture and push the star content appropriate to the current matchstate.
                            switch (matchstate)
                            {
                                case MatchState.UserInput:
                                    query.InputStar.Add(newWildcard.ToString());
                                    // Added due to this match being the end of the line.
                                    newWildcard.Remove(0, newWildcard.Length);
                                    break;
                                case MatchState.That:
                                    query.ThatStar.Add(newWildcard.ToString());
                                    break;
                                case MatchState.Topic:
                                    query.TopicStar.Add(newWildcard.ToString());
                                    break;
                                case MatchState.Emotion:
                                    query.EmotionStar.Add(newWildcard.ToString());
                                    break;
                            }
                        }
                        return result;
                    }
                }
                // Second option - the nodemapper may have contained a "_" child, but led to no match or it didn't contain a "_" child at all. So get the child nodemapper from this nodemapper that matches the first word of the input sentence.
                if (_children.ContainsKey(firstWord))
                {
                    // Process the matchstate - this might not make sense but the matchstate is working with a "backwards" path: "topic <topic> that <that> user input" the "classic" path looks like this: "user input <that> that <topic> topic" but having it backwards is more efficient for searching purposes.
                    MatchState newMatchState = matchstate;
                    if (firstWord == "<THAT>")
                    {
                        newMatchState = MatchState.That;
                    }
                    else if (firstWord == "<TOPIC>")
                    {
                        newMatchState = MatchState.Topic;
                    }
                    else if (firstWord == "<EMOTION>")
                    {
                        newMatchState = MatchState.Emotion;
                    }
					// Look for the path in the child node denoted by the first word.
                    Node childNode = _children[firstWord];
                    // Move down into the identified branch of the tapmaster structure using the new matchstate.
                    StringBuilder newWildcard = new StringBuilder();
                    string result = childNode.Evaluate(newPath, query, request, newMatchState, newWildcard);
                    // And if we get a result from the child return it.
                    if (result.Length > 0)
                    {
                        if (newWildcard.Length > 0)
                        {
                            // Capture and push the star content appropriate to the matchstate if it exists and then clear it for subsequent wildcards.
                            switch (matchstate)
                            {
                                case MatchState.UserInput:
                                    query.InputStar.Add(newWildcard.ToString());
                                    newWildcard.Remove(0, newWildcard.Length);
                                    break;
                                case MatchState.That:
                                    query.ThatStar.Add(newWildcard.ToString());
                                    newWildcard.Remove(0, newWildcard.Length);
                                    break;
                                case MatchState.Topic:
                                    query.TopicStar.Add(newWildcard.ToString());
                                    newWildcard.Remove(0, newWildcard.Length);
                                    break;
                                case MatchState.Emotion:
                                    query.EmotionStar.Add(newWildcard.ToString());
                                    newWildcard.Remove(0, newWildcard.Length);
                                    break;
                            }
                        }
                        return result;
                    }
                }
                // Third option - the input part of the path might have been matched so far but hasn't returned a match, so check to see it contains the "*" wildcard. "*" comes last in precedence in the alphabet.
                if (_children.ContainsKey("*"))
                {
                    // Look for the path in the child node denoted by "*".
                    Node childNode = _children["*"];
                    // Add the next word to the wildcard match.
                    StringBuilder newWildcard = new StringBuilder();
                    StoreWildCard(splitPath[0], newWildcard);
                    string result = childNode.Evaluate(newPath, query, request, matchstate, newWildcard);
                    // And if we get a result from the branch process and return it.
                    if (result.Length > 0)
                    {
                        if (newWildcard.Length > 0)
                        {
                            // Capture and push the star content appropriate to the current matchstate.
                            switch (matchstate)
                            {
                                case MatchState.UserInput:
                                    query.InputStar.Add(newWildcard.ToString());
                                    // Added due to this match being the end of the line.
                                    newWildcard.Remove(0, newWildcard.Length);
                                    break;
                                case MatchState.That:
                                    query.ThatStar.Add(newWildcard.ToString());
                                    break;
                                case MatchState.Topic:
                                    query.TopicStar.Add(newWildcard.ToString());
                                    break;
                                case MatchState.Emotion:
                                    query.EmotionStar.Add(newWildcard.ToString());
                                    break;
                            }
                        }
                        return result;
                    }
                }
                // If the nodemapper has failed to match at all: the input contains neither a "_", the FirstWord text, or "*" as a means of denoting a child node. However, if this node is itself representing a wildcard then the search continues to be valid if we proceed with the tail.
                if ((Word == "_") || (Word == "*"))
                {
                    StoreWildCard(splitPath[0], wildcard);
                    path = newPath;
                    continue;
                }
                // If we get here then we're at a dead end so return an empty string. Hopefully, if the aeon code files have been set up to include a " * <that> * <topic> * <emotion> * " catch-all this state won't be reached. Remember to empty the surplus to requirements wildcard matches wildcard = new StringBuilder();
                return string.Empty;
            }
        }
        /// <summary>
        /// Correctly stores a word in the wildcard slot.
        /// </summary>
        /// <param name="word">The word matched by the wildcard.</param>
        /// <param name="wildcard">The contents of the user input absorbed by wildcards "_" and "*".</param>
        private static void StoreWildCard(string word, StringBuilder wildcard)
        {
            if (wildcard.Length > 0)
            {
                wildcard.Append(" ");
            }
            wildcard.Append(word);
        }
    }
}
