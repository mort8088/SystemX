using System.Globalization;

namespace mort8088.XML
{
    using System.IO;
    using System.Xml;

    /// <summary>
    /// A static class containing methods to complete a number of common tasks while working with XmlDocuments.
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// Creates an empty XmlDocument object with an Xml Declaration assigned as version 1.0 encoding UTF-8
        /// </summary>
        /// <returns>Empty XmlDocument object</returns>
        public static XmlDocument NewDoc()
        {
            XmlDocument outputFile = new XmlDocument();
            XmlDeclaration dec = outputFile.CreateXmlDeclaration("1.0", "UTF-8", null);
            outputFile.AppendChild(dec);// Create the root element
            return outputFile;
        }

        /// <summary>
        /// Create a root node on a given document
        /// </summary>
        /// <param name="addTo">Xml document to add the new root node to</param>
        /// <param name="nodeName">Name of the new root node</param>
        /// <returns>A reference to the newly created root node</returns>
        public static XmlElement AddRootNode(XmlDocument addTo, string nodeName)
        {
            XmlElement newRoot = addTo.CreateElement(nodeName);
            addTo.AppendChild(newRoot);

            return newRoot;
        }

        /// <summary>
        /// Adds the Named attribute with a given value to an existing Node
        /// </summary>
        /// <param name="addTo">Node to add the new attribute</param>
        /// <param name="attribName">New attribute name</param>
        /// <param name="attribValue">Value to set</param>
        /// <returns>a link to the new attribute created</returns>
        public static XmlAttribute AddAttrib(XmlElement addTo, string attribName, string attribValue)
        {
            XmlAttribute node = addTo.OwnerDocument.CreateAttribute(attribName);
            node.Value = attribValue;
            addTo.Attributes.Append(node);
            return node;
        }

        /// <summary>
        /// Adds the Named attribute with a given value to an existing Node
        /// </summary>
        /// <param name="addTo">Node to add the new attribute</param>
        /// <param name="attribName">New attribute name</param>
        /// <param name="attribValue">Value to set</param>
        /// <returns>a link to the new attribute created</returns>
        public static XmlAttribute AddAttrib(XmlElement addTo, string attribName, bool attribValue)
        {
            return AddAttrib(addTo, attribName, attribValue.ToString());
        }

        /// <summary>
        /// Adds the Named attribute with a given value to an existing Node
        /// </summary>
        /// <param name="addTo">Node to add the new attribute</param>
        /// <param name="attribName">New attribute name</param>
        /// <param name="attribValue">Value to set</param>
        /// <returns>a link to the new attribute created</returns>
        public static XmlAttribute AddAttrib(XmlElement addTo, string attribName, int attribValue)
        {
            return AddAttrib(addTo, attribName, attribValue.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Adds the Named attribute with a given value to an existing Node
        /// </summary>
        /// <param name="addTo">Node to add the new attribute</param>
        /// <param name="attribName">New attribute name</param>
        /// <param name="attribValue">Value to set</param>
        /// <returns>a link to the new attribute created</returns>
        public static XmlAttribute AddAttrib(XmlElement addTo, string attribName, float attribValue)
        {
            return AddAttrib(addTo, attribName, attribValue.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Create a new node on a given node
        /// </summary>
        /// <param name="addTo">Node to add the new node to</param>
        /// <param name="nodeName">Name of the new node</param>
        /// <returns>A reference to the newly created node</returns>
        public static XmlElement AddNode(XmlElement addTo, string nodeName)
        {
            XmlElement newChild = addTo.OwnerDocument.CreateElement(nodeName);
            addTo.AppendChild(newChild);
            return newChild;
        }

        /// <summary>
        /// Create a new comment node in a given node
        /// </summary>
        /// <param name="addTo">Node to add the comment to</param>
        /// <param name="comment">String to go in the Comment section</param>
        public static void AddComment(XmlElement addTo, string comment)
        {
            XmlComment newComment = addTo.OwnerDocument.CreateComment(comment);
            addTo.AppendChild(newComment);
        }

        /// <summary>
        /// Create a new text node on a given node with a given value
        /// </summary>
        /// <param name="addTo">Node to add the new node to</param>
        /// <param name="nodeName">Name of the new node</param>
        /// <param name="nodeValue">Text value to set the new node to</param>
        /// <returns>A reference to the newly created node</returns>
        public static XmlElement AddTextNode(XmlElement addTo, string nodeName, string nodeValue)
        {
            XmlElement newChild = addTo.OwnerDocument.CreateElement(nodeName);
            XmlText text = addTo.OwnerDocument.CreateTextNode(nodeValue);
            newChild.AppendChild(text);
            addTo.AppendChild(newChild);
            return newChild;
        }

        /// <summary>
        /// Attractively format the XML with consistent indentation.
        /// </summary>
        /// <param name="strXml">A well formed XML string</param>
        /// <returns>An XML string with carriage returns and indentations</returns>
        public static string PrettyPrint(string strXml)
        {
            using (StringWriter writer = new StringWriter())
            {
                XmlDocument node = new XmlDocument();
                node.LoadXml(strXml);
                XmlNodeReader reader = new XmlNodeReader(node);
                XmlTextWriter writer2 = new XmlTextWriter(writer)
                    {
                        Formatting = Formatting.Indented,
                        Indentation = 1,
                        IndentChar = '\t'
                    };
                writer2.WriteNode(reader, true);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Attractively format the XML with consistent indentation.
        /// </summary>
        /// <param name="doc">The Xml Document you want to convert</param>
        /// <returns>An XML string with carriage returns and indentations</returns>
        public static string PrettyPrint(XmlDocument doc)
        {
            using (StringWriter writer = new StringWriter())
            {
                XmlNodeReader reader = new XmlNodeReader(doc);
                XmlTextWriter writer2 = new XmlTextWriter(writer)
                    {
                        Formatting = Formatting.Indented,
                        Indentation = 1,
                        IndentChar = '\t'
                    };
                writer2.WriteNode(reader, true);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Read a value from a given attribute on an existing node
        /// </summary>
        /// <param name="readFrom">node that has the attribute</param>
        /// <param name="attribName">Name of the Attribute</param>
        /// <returns>value of the attribute or NULL if not found</returns>
        public static string ReadAttrib(XmlElement readFrom, string attribName)
        {
            return readFrom.Attributes[attribName] != null ? readFrom.Attributes[attribName].Value : null;
        }

        /// <summary>
        /// Read a bool value from a given attribute on an existing node
        /// </summary>
        /// <param name="readFrom">node that has the attribute</param>
        /// <param name="attribName">Name of the Attribute</param>
        /// <param name="defaultValue">The default value to return if the attribute is not found</param>
        /// <returns>value of the attribute or value of defaultValue if not found</returns>
        public static bool ReadAttrib(XmlElement readFrom, string attribName, bool defaultValue)
        {
            bool output = defaultValue;
            if (readFrom.Attributes[attribName] != null)
            {
                bool.TryParse(readFrom.Attributes[attribName].Value, out output);
            }
            return output;
        }

        /// <summary>
        /// Read a int value from a given attribute on an existing node
        /// </summary>
        /// <param name="readFrom">node that has the attribute</param>
        /// <param name="attribName">Name of the Attribute</param>
        /// <param name="defaultValue">The default value to return if the attribute is not found</param>
        /// <returns>value of the attribute or value of defaultValue if not found</returns>
        public static int ReadAttrib(XmlElement readFrom, string attribName, int defaultValue)
        {
            int output = defaultValue;
            if (readFrom.Attributes[attribName] != null)
            {
                int.TryParse(readFrom.Attributes[attribName].Value, out output);
            }
            return output;
        }

        /// <summary>
        /// Read a float value from a given attribute on an existing node
        /// </summary>
        /// <param name="readFrom">node that has the attribute</param>
        /// <param name="attribName">Name of the Attribute</param>
        /// <param name="defaultValue">The default value to return if the attribute is not found</param>
        /// <returns>value of the attribute or value of defaultValue if not found</returns>
        public static float ReadAttrib(XmlElement readFrom, string attribName, float defaultValue)
        {
            float output = defaultValue;
            if (readFrom.Attributes[attribName] != null)
            {
                float.TryParse(readFrom.Attributes[attribName].Value, out output);
            }
            return output;
        }

        /// <summary>
        /// Read a string value from a given attribute on an existing node
        /// </summary>
        /// <param name="readFrom">node that has the attribute</param>
        /// <param name="attribName">Name of the Attribute</param>
        /// <param name="defaultValue">The default value to return if the attribute is not found</param>
        /// <returns>value of the attribute or value of defaultValue if not found</returns>
        public static string ReadAttrib(XmlElement readFrom, string attribName, string defaultValue)
        {
            string output = defaultValue;
            if (readFrom.Attributes[attribName] != null)
            {
                output = readFrom.Attributes[attribName].Value;
            }
            return output;
        }

        /// <summary>
        /// Read a text node from an existing node
        /// </summary>
        /// <param name="readFrom">Node with the node to read</param>
        /// <param name="nodeName">Name of the node to read the value</param>
        /// <returns>inner text of the node or null if not found.</returns>        
        public static string ReadTextNode(XmlElement readFrom, string nodeName)
        {
            XmlNode node = readFrom.SelectSingleNode($"./{nodeName}");
            return node?.InnerText;
        }

        /// <summary>
        /// Read a Boolean value text node from an existing node returns a default value if not found
        /// </summary>
        /// <param name="readFrom">node that has the attribute</param>
        /// <param name="nodeName">Name of the Attribute</param>
        /// <param name="defaultValue">The default value to return if the attribute is not found</param>
        /// <returns>value of the attribute or value of defaultValue if not found</returns>
        public static bool ReadTextNode(XmlElement readFrom, string nodeName, bool defaultValue)
        {

            if (readFrom.SelectSingleNode($"./{nodeName}") == null)
                return defaultValue;

            bool output;
            bool.TryParse(ReadTextNode(readFrom, nodeName), out output);
            return output;
        }

        /// <summary>
        /// Read an integer text node from an existing node returns a default value if not found
        /// </summary>
        /// <param name="readFrom">node that has the attribute</param>
        /// <param name="nodeName">Name of the Attribute</param>
        /// <param name="defaultValue">The default value to return if the attribute is not found</param>
        /// <returns>value of the attribute or value of defaultValue if not found</returns>
        public static int ReadTextNode(XmlElement readFrom, string nodeName, int defaultValue)
        {
            int output = defaultValue;
            if (readFrom.SelectSingleNode($"./{nodeName}") != null)
            {
                int.TryParse(ReadTextNode(readFrom, nodeName), out output);
            }
            return output;
        }

        /// <summary>
        /// Read a float text node from an existing node returns a default value if not found
        /// </summary>
        /// <param name="readFrom">node that has the attribute</param>
        /// <param name="nodeName">Name of the Attribute</param>
        /// <param name="defaultValue">The default value to return if the attribute is not found</param>
        /// <returns>value of the attribute or value of defaultValue if not found</returns>
        public static float ReadTextNode(XmlElement readFrom, string nodeName, float defaultValue)
        {
            float output = defaultValue;
            if (readFrom.SelectSingleNode($"./{nodeName}") != null)
            {
                float.TryParse(ReadTextNode(readFrom, nodeName), out output);
            }
            return output;
        }

        /// <summary>
        /// Read a text node from an existing node returns a default value if not found
        /// </summary>
        /// <param name="readFrom">node that has the attribute</param>
        /// <param name="nodeName">Name of the Attribute</param>
        /// <param name="defaultValue">The default value to return if the attribute is not found</param>
        /// <returns>value of the attribute or value of defaultValue if not found</returns>
        public static string ReadTextNode(XmlElement readFrom, string nodeName, string defaultValue)
        {
            XmlNode node = readFrom.SelectSingleNode($"./{nodeName}");
            return node != null ? node.InnerText : defaultValue;
        }
    }
}
