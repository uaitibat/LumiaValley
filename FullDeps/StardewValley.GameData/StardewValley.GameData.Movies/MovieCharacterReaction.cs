using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace StardewValley.GameData.Movies;

public class MovieCharacterReaction
{
	public string NPCName;

	[ContentSerializer(Optional = true)]
	public List<MovieReaction> Reactions;
}
