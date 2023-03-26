using System.Collections.Generic;
using System.IO;
using System.Xml;
using Cartheur.Animals.CF.Normalize;

namespace Cartheur.Animals.CF.Core
{
    /// <summary>
    /// A dictionary for loading, adding, checking, removing, and extracting settings.
    /// </summary>
    public class SettingsDictionary
    {
        /// <summary>
        /// Holds a dictionary of settings.
        /// </summary>
        private readonly Dictionary<string, string> _settingsHash = new Dictionary<string, string>();
        /// <summary>
        /// Contains an ordered collection of all the keys.
        /// </summary>
        private readonly List<string> _orderedKeys = new List<string>();
        /// <summary>
        /// The presence this dictionary is associated with.
        /// </summary>
        protected Aeon TheAeon;
        /// <summary>
        /// The number of items in the dictionary.
        /// </summary>
        public int Count
        {
            get
            {
                return _orderedKeys.Count;
            }
        }
        /// <summary>
        /// An xml representation of the contents of this dictionary.
        /// </summary>
        public XmlDocument DictionaryAsXml
        {
            get
            {
                XmlDocument result = new XmlDocument();
                XmlDeclaration dec = result.CreateXmlDeclaration("1.0", "UTF-8", "");
                result.AppendChild(dec);
                XmlNode root = result.CreateNode(XmlNodeType.Element, "root", "");
                result.AppendChild(root);
                foreach (string key in _orderedKeys)
                {
                    XmlNode item = result.CreateNode(XmlNodeType.Element, "item", "");
                    XmlAttribute name = result.CreateAttribute("name");
                    name.Value = key;
                    XmlAttribute value = result.CreateAttribute("value");
                    value.Value = _settingsHash[key];
                    if (item.Attributes != null)
                    {
                        item.Attributes.Append(name);
                        item.Attributes.Append(value);
                    }
                    root.AppendChild(item);
                }
                return result;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsDictionary"/> class.
        /// </summary>
        /// <param name="theAeon">Aeon settings dictionary.</param>
        public SettingsDictionary(Aeon theAeon)
        {
            TheAeon = theAeon;
        }
        /// <summary>
        /// Loads settings into the class from the file referenced in pathToSettings. The xml should have a declaration with a root tag with child nodes of the form:
        /// <item name="name" value="value"/>
        /// </summary>
        /// <param name="pathToSettings">The file containing the settings.</param>
        public void LoadSettings(string pathToSettings)
        {
            if (pathToSettings.Length > 0)
            {
                FileInfo fi = new FileInfo(pathToSettings);
                if (fi.Exists)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(pathToSettings);
                    LoadSettings(xmlDoc);
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }
            else
            {
                throw new FileNotFoundException();
            }
        }
        /// <summary>
        /// Loads settings into the class from the file referenced in pathToSettings. The xml should have a declaration with a root tag with child nodes of the form:
        /// <item name="name" value="value"/>
        /// </summary>
        /// <param name="settingsAsXml">The settings as an xml document.</param>
        public void LoadSettings(XmlDocument settingsAsXml)
        {
            // Empty the hash.
            ClearSettings();

            if (settingsAsXml.DocumentElement != null)
            {
                XmlNodeList rootChildren = settingsAsXml.DocumentElement.ChildNodes;

                foreach (XmlNode myNode in rootChildren)
                {
                    if (myNode.Attributes != null && (myNode.Name == "item") & (myNode.Attributes.Count == 2))
                    {
                        if ((myNode.Attributes[0].Name == "name") & (myNode.Attributes[1].Name == "value"))
                        {
                            string name = myNode.Attributes["name"].Value;
                            string value = myNode.Attributes["value"].Value;
                            if (name.Length > 0)
                            {
                                AddSetting(name, value);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Adds a setting to the Settings class (accessed via the grabSettings(string name) method.
        /// </summary>
        /// <param name="name">The name of the new setting.</param>
        /// <param name="value">The value associated with this setting.</param>
        public void AddSetting(string name, string value)
        {
            string key = MakeCaseInsensitive.TransformInput(name);
            if (key.Length > 0)
            {
                RemoveSetting(key);
                _orderedKeys.Add(key);
                _settingsHash.Add(MakeCaseInsensitive.TransformInput(key), value);
            }
        }
        /// <summary>
        /// Removes a named setting from this class.
        /// </summary>
        /// <param name="name">The name of the setting to remove.</param>
        public void RemoveSetting(string name)
        {
            string normalizedName = MakeCaseInsensitive.TransformInput(name);
            _orderedKeys.Remove(normalizedName);
            RemoveFromHash(normalizedName);
        }
        /// <summary>
        /// Removes a named setting from the dictionary.
        /// </summary>
        /// <param name="name">The key for the dictionary.</param>
        private void RemoveFromHash(string name)
        {
            string normalizedName = MakeCaseInsensitive.TransformInput(name);
            _settingsHash.Remove(normalizedName);
        }
        /// <summary>
        /// Updates the named setting with a new value while retaining the position in the dictionary.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="value">The new value.</param>
        public void UpdateSetting(string name, string value)
        {
            string key = MakeCaseInsensitive.TransformInput(name);
            if (_orderedKeys.Contains(key))
            {
                RemoveFromHash(key);
                _settingsHash.Add(MakeCaseInsensitive.TransformInput(key), value);
            }
        }
        /// <summary>
        /// Clears the dictionary to an empty state.
        /// </summary>
        public void ClearSettings()
        {
            _orderedKeys.Clear();
            _settingsHash.Clear();
        }
        /// <summary>
        /// Returns the value of a setting given the name of the setting.
        /// </summary>
        /// <param name="name">The name of the setting whose value is of interest.</param>
        /// <returns>The value of the setting.</returns>
        public string GrabSetting(string name)
        {
            string normalizedName = MakeCaseInsensitive.TransformInput(name);
            if (ContainsSettingCalled(normalizedName))
            {
                return _settingsHash[normalizedName];
            }
            return string.Empty;
        }
        /// <summary>
        /// Checks to see if a setting of a particular name exists.
        /// </summary>
        /// <param name="name">The setting name to check.</param>
        /// <returns>Existential truth value.</returns>
        public bool ContainsSettingCalled(string name)
        {
            string normalizedName = MakeCaseInsensitive.TransformInput(name);
            if (normalizedName.Length > 0)
            {
                return _orderedKeys.Contains(normalizedName);
            }
            return false;
        }
        /// <summary>
        /// Returns a collection of the names of all the settings defined in the dictionary.
        /// </summary>
        /// <returns>A collection of the names of all the settings defined in the dictionary.</returns>
        public string[] SettingNames
        {
            get
            {
                string[] result = new string[_orderedKeys.Count];
                _orderedKeys.CopyTo(result, 0);
                return result;
            }
        }
        /// <summary>
        /// Copies the values in the current object into the SettingsDictionary passed as the target.
        /// </summary>
        /// <param name="target">The target to recieve the values from this SettingsDictionary.</param>
        public void Clone(SettingsDictionary target)
        {
            foreach (string key in _orderedKeys)
            {
                target.AddSetting(key, GrabSetting(key));
            }
        }
    }
}
