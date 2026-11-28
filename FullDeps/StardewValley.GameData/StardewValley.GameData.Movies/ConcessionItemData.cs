using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace StardewValley.GameData.Movies;

public class ConcessionItemData
{
	public int ID;

	public string Name;

	public string DisplayName;

	public string Description;

	public int Price;

	[ContentSerializer(Optional = true)]
	public List<string> ItemTags;
}
