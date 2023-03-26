using System;
using System.IO;
using System.Text;
using System.Xml;
using Cartheur.Animals.CF.Core;
using Cartheur.Animals.CF.Normalize;

namespace Cartheur.Animals.CF.Utilities
{
    /// <summary>
    /// A class for loading *.aeon files from the file system into the program's architecture that forms an aeon brain.
    /// </summary>
    public class AeonLoader
    {
        private readonly Aeon _aeon;
        /// <summary>
        /// Initializes a new instance of the <see cref="AeonLoader"/> class.
        /// </summary>
        /// <param name="aeon">The presence whose brain is being processed.</param>
        public AeonLoader(Aeon aeon)
        {
            _aeon = aeon;
        }
        /// <summary>
        /// Loads the *.aeon from files found in the path.
        /// </summary>
        /// <param name="path">The path where the aeon code files are.</param>
        public void LoadAeon(string path)
        {
            if (Directory.Exists(path))
            {
                // Log the loading activity.
                _aeon.WriteToLog("Starting to process files found in the directory " + path, Logging.LogType.Information, Logging.LogCaller.AeonLoader);

                string[] fileEntries = Directory.GetFiles(path, "*.aeon");
                if (fileEntries.Length > 0)
                {
                    foreach (string filename in fileEntries)
                    {
                        LoadAeonCodeFile(filename);
                    }
                    _aeon.WriteToLog("Finished processing *.aeon files. " + Convert.ToString(_aeon.Size) + " categories processed.", Logging.LogType.Information, Logging.LogCaller.AeonLoader);
                    Aeon.PersonalityLoaded = true;
                }
                else
                {
                    throw new FileNotFoundException("Could not find any data files in the specified directory (" + path + "). Make sure that these files end in a lowercase extension like *.aeon.");
                }
            }
            else
            {
                throw new FileNotFoundException("The directory specified as the path to the *.aeon files (" + path + ") cannot be found by the AeonLoader object. Please make sure the directory where the *.aeon files are to be found is the same as the directory specified in the settings file.");
            }
        }
        /// <summary>
        /// Given the name of a file in the aeon code path directory, attempts to load it into the program.
        /// </summary>
        /// <param name="filename">The name of the file to process.</param>
        public void LoadAeonCodeFile(string filename)
        {
            _aeon.WriteToLog("Processing *.aeon file: " + filename, Logging.LogType.Information, Logging.LogCaller.AeonLoader);
            // Load the document.
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            LoadAeonFromXml(doc, filename);
        }
        /// <summary>
        /// Given an xml document containing valid aeon code, attempts to load it into the brain.
        /// </summary>
        /// <param name="doc">The xml document containing the aeon-formatted code.</param>
        /// <param name="filename">Where the xml document originated.</param>
        public void LoadAeonFromXml(XmlDocument doc, string filename)
        {
            // Get a list of the nodes that are children of the <aeon> tag these nodes should only be either <topic> or <category> the <topic> nodes will contain more <category> nodes.
            if (doc.DocumentElement == null) return;
            XmlNodeList rootChildren = doc.DocumentElement.ChildNodes;
            // Process each of the child nodes.
            foreach (XmlNode currentNode in rootChildren)
            {
                switch (currentNode.Name)
                {
                    case "topic":
                        ProcessTopic(currentNode, filename);
                        break;
                    case "category":
                        ProcessCategory(currentNode, filename);
                        break;
                }
            }
        }
        /// <summary>
        /// Given a name will try to find a node named "name" in the childnodes or return null.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        /// <param name="node">The node whose children need searching.</param>
        /// <returns>The node (or null).</returns>
        private static XmlNode FindNode(string name, XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == name)
                {
                    return child;
                }
            }
            return null;
        }
        /// <summary>
        /// Given a "topic" node, processes all the categories for the topic and adds them to the brain.
        /// </summary>
        /// <param name="node">The "topic" node.</param>
        /// <param name="filename">The file from which this node is taken.</param>
        private void ProcessTopic(XmlNode node, string filename)
        {
            // Find the name of the topic or set to default "*".
            string topicName = "*";
            if (node.Attributes != null && (node.Attributes.Count == 1) & (node.Attributes[0].Name == "name"))
            {
                topicName = node.Attributes["name"].Value;
            }
            // Process all the category nodes.
            foreach (XmlNode thisNode in node.ChildNodes)
            {
                if (thisNode.Name == "category")
                {
                    ProcessCategory(thisNode, topicName, filename);
                }
            }
        }
        /// <summary>
        /// Adds a category to the program's structure using the default topic ("*").
        /// </summary>
        /// <param name="node">The xml node containing the category.</param>
        /// <param name="filename">The file from which this category was taken.</param>
        private void ProcessCategory(XmlNode node, string filename)
        {
            ProcessCategory(node, "*", filename);
        }
        /// <summary>
        /// Adds a category to the program's structure using the given topic.
        /// </summary>
        /// <param name="node">The xml node containing the category.</param>
        /// <param name="topicName">The topic to be used.</param>
        /// <param name="filename">The file from which this category was taken.</param>
        private void ProcessCategory(XmlNode node, string topicName, string filename)
        {
            // Reference and check the required nodes.
            XmlNode pattern = FindNode("pattern", node);
            XmlNode template = FindNode("template", node);

            if (Equals(null, pattern))
            {
                throw new XmlException("Missing pattern tag in a node found in " + filename);
            }
            if (Equals(null, template))
            {
                throw new XmlException("Missing template tag in the node with pattern: " + pattern.InnerText + " found in " + filename);
            }
            // Generate the path based on its category.
            string categoryPath = GeneratePath(node, topicName, false);
            // Add the processed aeon code to the aeon structure.
            if (categoryPath.Length > 0)
            {
                try
                {
                    _aeon.ThisNode.AddCategory(categoryPath, template.OuterXml, filename);
                    // Keep count of the number of categories that have been processed.
                    _aeon.Size++;
                }
                catch
                {
                    Logging.WriteLog("Failed to load a new category into the brain where the path = " + categoryPath + " and template = " + template.OuterXml + " produced by a category in the file:" + filename, Logging.LogType.Error, Logging.LogCaller.AeonLoader);
                }
            }
            else
            {
                Logging.WriteLog("Attempted to load a new category with an empty pattern where the path = " + categoryPath + " and template = " + template.OuterXml + " produced by a category in the file: " + filename, Logging.LogType.Warning, Logging.LogCaller.AeonLoader);
            }
        }
        /// <summary>
        /// Generates a path from a category xml node and topic name.
        /// </summary>
        /// <param name="node">The category xml node.</param>
        /// <param name="topicName">The topic.</param>
        /// <param name="isUserInput">Marks the path to be created as originating from user input - so normalize out the * and _ wildcards used by *.aeon.</param>
        /// <returns>The appropriately processed path.</returns>
        public string GeneratePath(XmlNode node, string topicName, bool isUserInput)
        {
            // Get the required nodes.
            XmlNode pattern = FindNode("pattern", node);
            XmlNode that = FindNode("that", node);
            string thatText = "*";
            string patternText = Equals(null, pattern) ? string.Empty : pattern.InnerText;

            if (!Equals(null, that))
            {
                thatText = that.InnerText;
            }

            return GeneratePath(patternText, thatText, topicName, isUserInput);
        }
        /// <summary>
        /// Generates a path from the passed arguments.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="that">The that.</param>
        /// <param name="topicName">The topic.</param>
        /// <param name="isUserInput">Marks the path to be created as originating from user input - so normalize out the * and _ wildcards used by the aeon code.</param>
        /// <returns>The appropriately processed path.</returns>
        public string GeneratePath(string pattern, string that, string topicName, bool isUserInput)
        {
            // To hold the normalized path entered.
            StringBuilder normalizedPath = new StringBuilder();
            string normalizedPattern;
            string normalizedThat;
            string normalizedTopic;

            if ((_aeon.TrustCodeFiles) & (!isUserInput))
            {
                normalizedPattern = pattern.Trim();
                normalizedThat = that.Trim();
                normalizedTopic = topicName.Trim();
            }
            else
            {
                normalizedPattern = Normalize(pattern, isUserInput).Trim();
                normalizedThat = Normalize(that, isUserInput).Trim();
                normalizedTopic = Normalize(topicName, isUserInput).Trim();
            }
            // Check sizes.
            if (normalizedPattern.Length > 0)
            {
                if (normalizedThat.Length == 0)
                {
                    normalizedThat = "*";
                }
                if (normalizedTopic.Length == 0)
                {
                    normalizedTopic = "*";
                }
                // This check is in place to avoid overly large "that" elements having to be processed. 
                if (normalizedThat.Length > _aeon.MaxThatSize)
                {
                    normalizedThat = "*";
                }
                // Build the path.
                normalizedPath.Append(normalizedPattern);
                normalizedPath.Append(" <that> ");
                normalizedPath.Append(normalizedThat);
                normalizedPath.Append(" <topic> ");
                normalizedPath.Append(normalizedTopic);

                return normalizedPath.ToString();
            }
            return string.Empty;
        }
        /// <summary>
        /// Generates a path from the passed arguments.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="that">The that.</param>
        /// <param name="topicName">The topic.</param>
        /// <param name="emotion">The emotion of aeon.</param>
        /// <param name="isUserInput">Marks the path to be created as originating from user input - so normalize out the * and _ wildcards used by the aeon code.</param>
        /// <returns>The appropriately processed path.</returns>
        public string GeneratePath(string pattern, string that, string topicName, string emotion, bool isUserInput)
        {
            // To hold the normalized path entered.
            StringBuilder normalizedPath = new StringBuilder();
            string normalizedPattern;
            string normalizedThat;
            string normalizedTopic;
            string normalizedEmotion;

            if ((_aeon.TrustCodeFiles) & (!isUserInput))
            {
                normalizedPattern = pattern.Trim();
                normalizedThat = that.Trim();
                normalizedTopic = topicName.Trim();
                normalizedEmotion = emotion.Trim();
            }
            else
            {
                normalizedPattern = Normalize(pattern, isUserInput).Trim();
                normalizedThat = Normalize(that, isUserInput).Trim();
                normalizedTopic = Normalize(topicName, isUserInput).Trim();
                normalizedEmotion = Normalize(emotion, isUserInput).Trim();
            }
            // Check sizes.
            if (normalizedPattern.Length > 0)
            {
                if (normalizedThat.Length == 0)
                {
                    normalizedThat = "*";
                }
                if (normalizedTopic.Length == 0)
                {
                    normalizedTopic = "*";
                }
                if (normalizedEmotion.Length == 0)
                {
                    normalizedEmotion = "*";
                }
                // This check is in place to avoid overly large "that" elements having to be processed. 
                if (normalizedThat.Length > _aeon.MaxThatSize)
                {
                    normalizedThat = "*";
                }
                // Build the path.
                normalizedPath.Append(normalizedPattern);
                normalizedPath.Append(" <that> ");
                normalizedPath.Append(normalizedThat);
                normalizedPath.Append(" <topic> ");
                normalizedPath.Append(normalizedTopic);
                normalizedPath.Append(" <emotion> ");
                normalizedPath.Append(normalizedEmotion);

                return normalizedPath.ToString();
            }
            return string.Empty;
        }

        #region Utilities
        /// <summary>
        /// Given an input, provide a normalized output.
        /// </summary>
        /// <param name="input">The string to be normalized.</param>
        /// <param name="isUserInput">True if the string being normalized is part of the user input path - flags that we need to normalize out * and _ chars.</param>
        /// <returns>The normalized string.</returns>
        public string Normalize(string input, bool isUserInput)
        {
            StringBuilder result = new StringBuilder();
            // Objects for normalization of the input.
            ApplySubstitutions substitutor = new ApplySubstitutions(_aeon);
            StripIllegalCharacters stripper = new StripIllegalCharacters(_aeon);
            string substitutedInput = substitutor.Transform(input);
            // Split the pattern into its component words.
            string[] substitutedWords = substitutedInput.Split(" \r\n\t".ToCharArray());
            // Normalize all words unless wildcards "*" and "_".
            foreach (string word in substitutedWords)
            {
                string normalizedWord;
                if (isUserInput)
                {
                    normalizedWord = stripper.Transform(word);
                }
                else
                {
                    if ((word == "*") || (word == "_"))
                    {
                        normalizedWord = word;
                    }
                    else
                    {
                        normalizedWord = stripper.Transform(word);
                    }
                }
                result.Append(normalizedWord.Trim() + " ");
            }

            return result.ToString().Replace("  "," "); // Make sure the whitespace is neat.
        }
        /// <summary>
        /// Does the file decrypt.
        /// </summary>
        /// <param name="path">The path where the file should be stored.</param>
        public void DoFileDecrypt(string path)
        {
            try
            {
                Cryptography.Key = _aeon.DefaultPredicates.GrabSetting("password");
                if (Directory.Exists(path))
                {
                    _aeon.WriteToLog("Starting to process encrypted aeon files found in the directory " + path,
                        Logging.LogType.Information, Logging.LogCaller.AeonLoader);

                    string[] fileEntries = Directory.GetFiles(path, "*.aeon");
                    if (fileEntries.Length > 0)
                    {
                        foreach (string filename in fileEntries)
                        {
                            filename.DecryptFile(path + @"output.aeon", Cryptography.Key);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.AeonLoader);
            }
        }
        #endregion
    }
}
