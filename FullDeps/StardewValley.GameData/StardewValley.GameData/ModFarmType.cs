using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace StardewValley.GameData;

public class ModFarmType
{
	public string ID;

	public string TooltipStringPath;

	public string MapName;

	[ContentSerializer(Optional = true)]
	public string IconTexture;

	[ContentSerializer(Optional = true)]
	public string WorldMapTexture;

	[ContentSerializer(Optional = true)]
	public Dictionary<string, string> ModData;
}
