using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace StardewValley.GameData.HomeRenovations;

public class HomeRenovation
{
	public string TextStrings;

	public string AnimationType;

	public bool CheckForObstructions;

	public List<RenovationValue> Requirements;

	public List<RenovationValue> RenovateActions;

	[ContentSerializer(Optional = true)]
	public List<RectGroup> RectGroups;

	[ContentSerializer(Optional = true)]
	public string SpecialRect;
}
