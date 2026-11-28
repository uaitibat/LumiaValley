// Compat UWP para Vector2Reader/Writer/Serializer do original.
// No perfil UWP nao existem XmlSerializationReader/Writer, nem os overrides
// CreateReader/CreateWriter/Serialize(object, ...)/Deserialize(XmlSerializationReader).
// A conversao XML e a mesma do original, usando apenas a API publica disponivel.
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace StardewValley;

public class Vector2Serializer : XmlSerializer
{
    public override bool CanDeserialize(XmlReader xmlReader)
    {
        return xmlReader.IsStartElement("Vector2");
    }

    // UWP: Serialize/Deserialize nao sao virtuais; "new" mantem a API do original.
    public new void Serialize(XmlWriter xmlWriter, object o)
    {
        Vector2 vec = (Vector2)o;
        xmlWriter.WriteStartElement("Vector2");
        xmlWriter.WriteStartElement("X");
        xmlWriter.WriteValue(vec.X);
        xmlWriter.WriteEndElement();
        xmlWriter.WriteStartElement("Y");
        xmlWriter.WriteValue(vec.Y);
        xmlWriter.WriteEndElement();
        xmlWriter.WriteEndElement();
    }

    public new object Deserialize(XmlReader xmlReader)
    {
        xmlReader.ReadStartElement("Vector2");
        xmlReader.ReadStartElement("X");
        float x = xmlReader.ReadContentAsFloat();
        xmlReader.ReadEndElement();
        xmlReader.ReadStartElement("Y");
        float y = xmlReader.ReadContentAsFloat();
        xmlReader.ReadEndElement();
        xmlReader.ReadEndElement();
        return new Vector2(x, y);
    }
}
