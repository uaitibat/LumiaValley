using System.IO;
using BmFont;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace CPExtBmFont;

[ContentTypeWriter]
public class XmlSourceWriter : ContentTypeWriter<XmlSource>
{
	protected override void Write(ContentWriter output, XmlSource value)
	{
		((BinaryWriter)(object)output).Write(value.Source);
	}

	public override string GetRuntimeType(TargetPlatform targetPlatform)
	{
		return typeof(XmlSource).AssemblyQualifiedName;
	}

	public override string GetRuntimeReader(TargetPlatform targetPlatform)
	{
		return typeof(XmlSourceReader).AssemblyQualifiedName;
	}
}
