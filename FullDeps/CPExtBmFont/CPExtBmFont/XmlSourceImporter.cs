using System.IO;
using BmFont;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace CPExtBmFont;

[ContentImporter(".xml", DisplayName = "Xml Source Importer")]
public class XmlSourceImporter : ContentImporter<XmlSource>
{
	public override XmlSource Import(string filename, ContentImporterContext context)
	{
		return new XmlSource(File.ReadAllText(filename));
	}
}
