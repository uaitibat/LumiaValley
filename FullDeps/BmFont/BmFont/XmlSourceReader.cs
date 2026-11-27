using Microsoft.Xna.Framework.Content;

namespace BmFont;

public class XmlSourceReader : ContentTypeReader<XmlSource>
{
	protected override XmlSource Read(ContentReader input, XmlSource existingInstance)
	{
		return new XmlSource(input.ReadString());
	}
}
