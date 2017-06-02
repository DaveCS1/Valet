using System;
using System.Xml;

namespace Engine.Utilities
{
    internal static class XmlExtensions
    {
        internal static decimal AttributeAsDecimal(this XmlDocument obj, string xpath)
        {
            XmlNode selectSingleNode = obj.SelectSingleNode(xpath);

            if(selectSingleNode != null)
            {
                return Convert.ToDecimal(selectSingleNode.Value);
            }

            throw new ArgumentException($"XML Path not found: {xpath}");
        }
    }
}