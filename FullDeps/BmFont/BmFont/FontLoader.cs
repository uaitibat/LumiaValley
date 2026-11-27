using System.IO;
using System.Xml.Serialization;

namespace BmFont;

public class FontLoader
{
	public static FontFile Parse(string xml)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(FontFile));
		TextReader textReader = new StringReader(xml);
		FontFile result = (FontFile)xmlSerializer.Deserialize(textReader);
		textReader.Close();
		return result;
	}
}
