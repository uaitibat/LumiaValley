using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Characters;

namespace StardewValley.Menus;

public class SocialPage : IClickableMenu
{
	public const int slotsOnPage = 5;

	public static readonly string[] defaultFriendships = new string[2] { "Robin", "Lewis" };

	private string descriptionText = "";

	private string hoverText = "";

	private ClickableTextureComponent upButton;

	private ClickableTextureComponent downButton;

	private ClickableTextureComponent scrollBar;

	private Rectangle scrollBarRunner;

	public List<object> names;

	private List<ClickableTextureComponent> sprites;

	private int slotPosition;

	private int numFarmers;

	private List<string> kidsNames = new List<string>();

	private Dictionary<string, string> npcNames;

	public List<ClickableTextureComponent> characterSlots;

	private bool scrolling;

	public Friendship emptyFriendship = new Friendship();

	public SocialPage(int x, int y, int width, int height)
		: base(x, y, width, height)
	{
		string[] array = defaultFriendships;
		foreach (string key in array)
		{
			if (!Game1.player.friendshipData.ContainsKey(key))
			{
				Game1.player.friendshipData.Add(key, new Friendship());
			}
		}
		characterSlots = new List<ClickableTextureComponent>();
		Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
		npcNames = new Dictionary<string, string>();
		foreach (string key2 in Game1.player.friendshipData.Keys)
		{
			string value = key2;
			if (dictionary.ContainsKey(key2) && dictionary[key2].Split('/').Length > 11)
			{
				value = dictionary[key2].Split('/')[11];
			}
			if (!npcNames.ContainsKey(key2))
			{
				npcNames.Add(key2, value);
			}
		}
		foreach (NPC allCharacter in Utility.getAllCharacters())
		{
			if (allCharacter.CanSocialize)
			{
				if (Game1.player.friendshipData.ContainsKey(allCharacter.Name) && allCharacter is Child)
				{
					kidsNames.Add(allCharacter.Name);
					npcNames[allCharacter.Name] = allCharacter.Name.Trim();
				}
				else if (Game1.player.friendshipData.ContainsKey(allCharacter.Name))
				{
					npcNames[allCharacter.Name] = allCharacter.displayName;
				}
				else
				{
					npcNames[allCharacter.Name] = "???";
				}
			}
		}
		names = new List<object>();
		sprites = new List<ClickableTextureComponent>();
		foreach (Farmer allFarmer in Game1.getAllFarmers())
		{
			if (((bool)allFarmer.isCustomized || allFarmer == Game1.MasterPlayer) && allFarmer != Game1.player)
			{
				names.Add(allFarmer.UniqueMultiplayerID);
				sprites.Add(new ClickableTextureComponent("", new Rectangle(x + IClickableMenu.borderWidth + 4, 0, width, 64), null, "", allFarmer.Sprite.Texture, Rectangle.Empty, 4f));
				numFarmers++;
			}
		}
		foreach (KeyValuePair<string, string> item in npcNames.OrderBy((KeyValuePair<string, string> p) => -Game1.player.getFriendshipLevelForNPC(p.Key)))
		{
			NPC nPC = null;
			if (kidsNames.Contains(item.Key))
			{
				nPC = Game1.getCharacterFromName<Child>(item.Key, mustBeVillager: false);
			}
			else if (dictionary.ContainsKey(item.Key))
			{
				string[] array2 = dictionary[item.Key].Split('/')[10].Split(' ');
				string textureNameForCharacter = NPC.getTextureNameForCharacter(item.Key);
				if (array2.Length > 2)
				{
					nPC = new NPC(new AnimatedSprite("Characters\\" + textureNameForCharacter, 0, 16, 32), new Vector2(Convert.ToInt32(array2[1]) * 64, Convert.ToInt32(array2[2]) * 64), array2[0], 0, item.Key, null, Game1.content.Load<Texture2D>("Portraits\\" + textureNameForCharacter), eventActor: false);
				}
			}
			if (nPC != null)
			{
				names.Add(nPC.Name);
				sprites.Add(new ClickableTextureComponent("", new Rectangle(x + IClickableMenu.borderWidth + 4, 0, width, 64), null, "", nPC.Sprite.Texture, nPC.getMugShotSourceRect(), 4f));
			}
		}
		for (int j = 0; j < names.Count; j++)
		{
			ClickableTextureComponent clickableTextureComponent = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + IClickableMenu.borderWidth, 0, width - IClickableMenu.borderWidth * 2, rowPosition(1) - rowPosition(0)), null, new Rectangle(0, 0, 0, 0), 4f)
			{
				myID = j,
				downNeighborID = j + 1,
				upNeighborID = j - 1
			};
			if (clickableTextureComponent.upNeighborID < 0)
			{
				clickableTextureComponent.upNeighborID = 12342;
			}
			characterSlots.Add(clickableTextureComponent);
		}
		upButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f);
		downButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + height - 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f);
		scrollBar = new ClickableTextureComponent(new Rectangle(upButton.bounds.X + 12, upButton.bounds.Y + upButton.bounds.Height + 4, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f);
		scrollBarRunner = new Rectangle(scrollBar.bounds.X, upButton.bounds.Y + upButton.bounds.Height + 4, scrollBar.bounds.Width, height - 128 - upButton.bounds.Height - 8);
		int num = 0;
		for (int k = 0; k < names.Count; k++)
		{
			if (!(names[k] is long))
			{
				num = k;
				break;
			}
		}
		slotPosition = num;
		setScrollBarToCurrentIndex();
		updateSlots();
	}

	public static bool isRoommateOfAnyone(string name)
	{
		return Game1.getCharacterFromName(name)?.isRoommate() ?? false;
	}

	public static bool isDatable(string name)
	{
		Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
		if (!dictionary.ContainsKey(name))
		{
			return false;
		}
		return dictionary[name].Split('/')[5] == "datable";
	}

	public Friendship getFriendship(string name)
	{
		if (Game1.player.friendshipData.ContainsKey(name))
		{
			return Game1.player.friendshipData[name];
		}
		return emptyFriendship;
	}

	public override void snapToDefaultClickableComponent()
	{
		if (slotPosition < characterSlots.Count)
		{
			currentlySnappedComponent = characterSlots[slotPosition];
		}
		snapCursorToCurrentSnappedComponent();
	}

	public int getGender(string name)
	{
		if (kidsNames.Contains(name))
		{
			return Game1.getCharacterFromName<Child>(name, mustBeVillager: false).Gender;
		}
		Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
		if (!dictionary.ContainsKey(name))
		{
			return 0;
		}
		if (!(dictionary[name].Split('/')[4] == "female"))
		{
			return 0;
		}
		return 1;
	}

	public bool isMarriedToAnyone(string name)
	{
		foreach (Farmer allFarmer in Game1.getAllFarmers())
		{
			if (allFarmer.spouse == name && allFarmer.isMarried())
			{
				return true;
			}
		}
		return false;
	}

	public void updateSlots()
	{
		for (int i = 0; i < characterSlots.Count; i++)
		{
			characterSlots[i].bounds.Y = rowPosition(i - 1);
		}
		int num = 0;
		for (int j = slotPosition; j < slotPosition + 5; j++)
		{
			if (sprites.Count > j)
			{
				int y = yPositionOnScreen + IClickableMenu.borderWidth + 32 + 112 * num + 32;
				sprites[j].bounds.Y = y;
			}
			num++;
		}
		populateClickableComponentList();
		addTabsToClickableComponents();
	}

	public void addTabsToClickableComponents()
	{
		if (Game1.activeClickableMenu is GameMenu && !allClickableComponents.Contains((Game1.activeClickableMenu as GameMenu).tabs[0]))
		{
			allClickableComponents.AddRange((Game1.activeClickableMenu as GameMenu).tabs);
		}
	}

	protected void _SelectSlot(ClickableComponent slot_component)
	{
		if (slot_component != null && characterSlots.Contains(slot_component))
		{
			int num = characterSlots.IndexOf(slot_component as ClickableTextureComponent);
			currentlySnappedComponent = slot_component;
			if (num < slotPosition)
			{
				slotPosition = num;
			}
			else if (num >= slotPosition + 5)
			{
				slotPosition = num - 5 + 1;
			}
			setScrollBarToCurrentIndex();
			updateSlots();
			if (Game1.options.snappyMenus && Game1.options.gamepadControls)
			{
				snapCursorToCurrentSnappedComponent();
			}
		}
	}

	public void ConstrainSelectionToVisibleSlots()
	{
		if (characterSlots.Contains(currentlySnappedComponent))
		{
			int num = characterSlots.IndexOf(currentlySnappedComponent as ClickableTextureComponent);
			if (num < slotPosition)
			{
				num = slotPosition;
			}
			else if (num >= slotPosition + 5)
			{
				num = slotPosition + 5 - 1;
			}
			currentlySnappedComponent = characterSlots[num];
			if (Game1.options.snappyMenus && Game1.options.gamepadControls)
			{
				snapCursorToCurrentSnappedComponent();
			}
		}
	}

	public override void snapCursorToCurrentSnappedComponent()
	{
		if (currentlySnappedComponent != null && characterSlots.Contains(currentlySnappedComponent))
		{
			Game1.setMousePosition(currentlySnappedComponent.bounds.Left + 64, currentlySnappedComponent.bounds.Center.Y);
		}
		else
		{
			base.snapCursorToCurrentSnappedComponent();
		}
	}

	public override void applyMovementKey(int direction)
	{
		base.applyMovementKey(direction);
		if (characterSlots.Contains(currentlySnappedComponent))
		{
			_SelectSlot(currentlySnappedComponent);
		}
	}

	public override void leftClickHeld(int x, int y)
	{
		base.leftClickHeld(x, y);
		if (scrolling)
		{
			int y2 = scrollBar.bounds.Y;
			scrollBar.bounds.Y = Math.Min(yPositionOnScreen + height - 64 - 12 - scrollBar.bounds.Height, Math.Max(y, yPositionOnScreen + upButton.bounds.Height + 20));
			float num = (float)(y - scrollBarRunner.Y) / (float)scrollBarRunner.Height;
			slotPosition = Math.Min(sprites.Count - 5, Math.Max(0, (int)((float)sprites.Count * num)));
			setScrollBarToCurrentIndex();
			if (y2 != scrollBar.bounds.Y)
			{
				Game1.playSound("shiny4");
			}
		}
	}

	public override void releaseLeftClick(int x, int y)
	{
		base.releaseLeftClick(x, y);
		scrolling = false;
	}

	private void setScrollBarToCurrentIndex()
	{
		if (sprites.Count > 0)
		{
			scrollBar.bounds.Y = scrollBarRunner.Height / Math.Max(1, sprites.Count - 5 + 1) * slotPosition + upButton.bounds.Bottom + 4;
			if (slotPosition == sprites.Count - 5)
			{
				scrollBar.bounds.Y = downButton.bounds.Y - scrollBar.bounds.Height - 4;
			}
		}
		updateSlots();
	}

	public override void receiveScrollWheelAction(int direction)
	{
		base.receiveScrollWheelAction(direction);
		if (direction > 0 && slotPosition > 0)
		{
			upArrowPressed();
			ConstrainSelectionToVisibleSlots();
			Game1.playSound("shiny4");
		}
		else if (direction < 0 && slotPosition < Math.Max(0, sprites.Count - 5))
		{
			downArrowPressed();
			ConstrainSelectionToVisibleSlots();
			Game1.playSound("shiny4");
		}
	}

	public void upArrowPressed()
	{
		slotPosition--;
		updateSlots();
		upButton.scale = 3.5f;
		setScrollBarToCurrentIndex();
	}

	public void downArrowPressed()
	{
		slotPosition++;
		updateSlots();
		downButton.scale = 3.5f;
		setScrollBarToCurrentIndex();
	}

	public override void receiveLeftClick(int x, int y, bool playSound = true)
	{
		if (upButton.containsPoint(x, y) && slotPosition > 0)
		{
			upArrowPressed();
			Game1.playSound("shwip");
			return;
		}
		if (downButton.containsPoint(x, y) && slotPosition < sprites.Count - 5)
		{
			downArrowPressed();
			Game1.playSound("shwip");
			return;
		}
		if (scrollBar.containsPoint(x, y))
		{
			scrolling = true;
			return;
		}
		if (!downButton.containsPoint(x, y) && x > xPositionOnScreen + width && x < xPositionOnScreen + width + 128 && y > yPositionOnScreen && y < yPositionOnScreen + height)
		{
			scrolling = true;
			leftClickHeld(x, y);
			releaseLeftClick(x, y);
			return;
		}
		for (int i = 0; i < characterSlots.Count; i++)
		{
			if (i < slotPosition || i >= slotPosition + 5 || !characterSlots[i].bounds.Contains(x, y))
			{
				continue;
			}
			bool flag = true;
			if (names[i] is string)
			{
				Character characterFromName = Game1.getCharacterFromName((string)names[i]);
				if (characterFromName != null && Game1.player.friendshipData.ContainsKey(characterFromName.name))
				{
					flag = false;
					Game1.playSound("bigSelect");
					int cached_slot_position = slotPosition;
					ProfileMenu menu = new ProfileMenu(characterFromName);
					menu.exitFunction = delegate
					{
						if (((GameMenu)(Game1.activeClickableMenu = new GameMenu(2, -1, playOpeningSound: false))).GetCurrentPage() is SocialPage socialPage)
						{
							Character character = menu.GetCharacter();
							if (character != null)
							{
								for (int j = 0; j < socialPage.names.Count; j++)
								{
									if (socialPage.names[j] is string && character.Name == (string)socialPage.names[j])
									{
										socialPage.slotPosition = cached_slot_position;
										socialPage._SelectSlot(socialPage.characterSlots[j]);
										break;
									}
								}
							}
						}
					};
					Game1.activeClickableMenu = menu;
					if (Game1.options.SnappyMenus)
					{
						menu.snapToDefaultClickableComponent();
					}
					return;
				}
			}
			if (flag)
			{
				Game1.playSound("shiny4");
			}
			break;
		}
		slotPosition = Math.Max(0, Math.Min(sprites.Count - 5, slotPosition));
	}

	public override void receiveRightClick(int x, int y, bool playSound = true)
	{
	}

	public override void performHoverAction(int x, int y)
	{
		descriptionText = "";
		hoverText = "";
		upButton.tryHover(x, y);
		downButton.tryHover(x, y);
	}

	private bool isCharacterSlotClickable(int i)
	{
		if (names[i] is string)
		{
			Character characterFromName = Game1.getCharacterFromName((string)names[i]);
			if (characterFromName != null && Game1.player.friendshipData.ContainsKey(characterFromName.name))
			{
				return true;
			}
		}
		return false;
	}

	private void drawNPCSlot(SpriteBatch b, int i)
	{
		if (isCharacterSlotClickable(i) && characterSlots[i].bounds.Contains(Game1.getMouseX(), Game1.getMouseY()))
		{
			b.Draw(Game1.staminaRect, new Rectangle(xPositionOnScreen + IClickableMenu.borderWidth - 4, sprites[i].bounds.Y - 4, characterSlots[i].bounds.Width, characterSlots[i].bounds.Height - 12), Color.White * 0.25f);
		}
		sprites[i].draw(b);
		string text = names[i] as string;
		int friendshipHeartLevelForNPC = Game1.player.getFriendshipHeartLevelForNPC(text);
		bool flag = isDatable(text);
		Friendship friendship = getFriendship(text);
		bool flag2 = friendship.IsMarried();
		bool flag3 = flag2 && isRoommateOfAnyone(text);
		float y = Game1.smallFont.MeasureString("W").Y;
		float num = ((LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko) ? ((0f - y) / 2f) : 0f);
		b.DrawString(Game1.dialogueFont, npcNames[text], new Vector2((float)(xPositionOnScreen + IClickableMenu.borderWidth * 3 / 2 + 64 - 20 + 96) - Game1.dialogueFont.MeasureString(npcNames[text]).X / 2f, (float)(sprites[i].bounds.Y + 48) + num - (float)(flag ? 24 : 20)), Game1.textColor);
		for (int j = 0; j < Math.Max(Utility.GetMaximumHeartsForCharacter(Game1.getCharacterFromName(text)), 10); j++)
		{
			int x = ((j < friendshipHeartLevelForNPC) ? 211 : 218);
			if (flag && !friendship.IsDating() && !flag2 && j >= 8)
			{
				x = 211;
			}
			if (j < 10)
			{
				b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen + 320 - 4 + j * 32, sprites[i].bounds.Y + 64 - 28), new Rectangle(x, 428, 7, 6), (flag && !friendship.IsDating() && !flag2 && j >= 8) ? (Color.Black * 0.35f) : Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
			}
			else
			{
				b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen + 320 - 4 + (j - 10) * 32, sprites[i].bounds.Y + 64), new Rectangle(x, 428, 7, 6), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
			}
		}
		if (flag || flag3)
		{
			string text2 = ((!Game1.content.ShouldUseGenderedCharacterTranslations()) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635") : ((getGender(text) == 0) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635").Split('/').First() : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635").Split('/').Last()));
			if (flag3)
			{
				text2 = Game1.content.LoadString("Strings\\StringsFromCSFiles:Housemate");
			}
			else if (flag2)
			{
				text2 = ((getGender(text) == 0) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11636") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11637"));
			}
			else if (isMarriedToAnyone(text))
			{
				text2 = ((getGender(text) == 0) ? Game1.content.LoadString("Strings\\UI:SocialPage_MarriedToOtherPlayer_MaleNPC") : Game1.content.LoadString("Strings\\UI:SocialPage_MarriedToOtherPlayer_FemaleNPC"));
			}
			else if (!Game1.player.isMarried() && friendship.IsDating())
			{
				text2 = ((getGender(text) == 0) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11639") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11640"));
			}
			else if (getFriendship(text).IsDivorced())
			{
				text2 = ((getGender(text) == 0) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11642") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11643"));
			}
			int num2 = (IClickableMenu.borderWidth * 3 + 128 - 40 + 192) / 2;
			text2 = Game1.parseText(text2, Game1.smallFont, num2);
			Vector2 vector = Game1.smallFont.MeasureString(text2);
			b.DrawString(Game1.smallFont, text2, new Vector2((float)(xPositionOnScreen + 192 + 8) - vector.X / 2f, (float)sprites[i].bounds.Bottom - (vector.Y - y)), Game1.textColor);
		}
		if (!getFriendship(text).IsMarried() && !kidsNames.Contains(text))
		{
			Utility.drawWithShadow(b, Game1.mouseCursors2, new Vector2(xPositionOnScreen + 384 + 304, sprites[i].bounds.Y - 4), new Rectangle(166, 174, 14, 12), Color.White, 0f, Vector2.Zero, 4f, flipped: false, 0.88f, 0, -1, 0.2f);
			b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen + 384 + 296, sprites[i].bounds.Y + 32 + 20), new Rectangle(227 + ((getFriendship(text).GiftsThisWeek >= 2) ? 9 : 0), 425, 9, 9), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
			b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen + 384 + 336, sprites[i].bounds.Y + 32 + 20), new Rectangle(227 + ((getFriendship(text).GiftsThisWeek >= 1) ? 9 : 0), 425, 9, 9), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
			Utility.drawWithShadow(b, Game1.mouseCursors2, new Vector2(xPositionOnScreen + 384 + 424, sprites[i].bounds.Y), new Rectangle(180, 175, 13, 11), Color.White, 0f, Vector2.Zero, 4f, flipped: false, 0.88f, 0, -1, 0.2f);
			b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen + 384 + 432, sprites[i].bounds.Y + 32 + 20), new Rectangle(227 + (getFriendship(text).TalkedToToday ? 9 : 0), 425, 9, 9), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
		}
		if (flag2)
		{
			if (!flag3 || text == "Krobus")
			{
				b.Draw(Game1.objectSpriteSheet, new Vector2(xPositionOnScreen + IClickableMenu.borderWidth * 7 / 4 + 192, sprites[i].bounds.Y), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, flag3 ? 808 : 460, 16, 16), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.88f);
			}
		}
		else if (friendship.IsDating())
		{
			b.Draw(Game1.objectSpriteSheet, new Vector2(xPositionOnScreen + IClickableMenu.borderWidth * 7 / 4 + 192, sprites[i].bounds.Y), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, flag3 ? 808 : 458, 16, 16), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.88f);
		}
	}

	private int rowPosition(int i)
	{
		int num = i - slotPosition;
		int num2 = 112;
		return yPositionOnScreen + IClickableMenu.borderWidth + 160 + 4 + num * num2;
	}

	private void drawFarmerSlot(SpriteBatch b, int i)
	{
		long num = (long)names[i];
		Farmer farmerMaybeOffline = Game1.getFarmerMaybeOffline(num);
		if (farmerMaybeOffline != null)
		{
			int num2 = ((!farmerMaybeOffline.IsMale) ? 1 : 0);
			ClickableTextureComponent clickableTextureComponent = sprites[i];
			int x = clickableTextureComponent.bounds.X;
			int y = clickableTextureComponent.bounds.Y;
			Rectangle scissorRectangle = b.GraphicsDevice.ScissorRectangle;
			Rectangle scissorRectangle2 = scissorRectangle;
			scissorRectangle2.Height = Math.Min(scissorRectangle2.Bottom, rowPosition(i)) - scissorRectangle2.Y - 4;
			b.GraphicsDevice.ScissorRectangle = scissorRectangle2;
			FarmerRenderer.isDrawingForUI = true;
			try
			{
				farmerMaybeOffline.FarmerRenderer.draw(b, new FarmerSprite.AnimationFrame(farmerMaybeOffline.bathingClothes ? 108 : 0, 0, secondaryArm: false, flip: false), farmerMaybeOffline.bathingClothes ? 108 : 0, new Rectangle(0, farmerMaybeOffline.bathingClothes ? 576 : 0, 16, 32), new Vector2(x, y), Vector2.Zero, 0.8f, 2, Color.White, 0f, 1f, farmerMaybeOffline);
			}
			finally
			{
				b.GraphicsDevice.ScissorRectangle = scissorRectangle;
			}
			FarmerRenderer.isDrawingForUI = false;
			Friendship friendship = Game1.player.team.GetFriendship(Game1.player.UniqueMultiplayerID, num);
			bool num3 = friendship.IsMarried();
			float y2 = Game1.smallFont.MeasureString("W").Y;
			float num4 = ((LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru) ? ((0f - y2) / 2f) : 0f);
			b.DrawString(Game1.dialogueFont, farmerMaybeOffline.Name, new Vector2(xPositionOnScreen + IClickableMenu.borderWidth * 3 / 2 + 96 - 20, (float)(sprites[i].bounds.Y + 48) + num4 - 24f), Game1.textColor);
			string text = ((!Game1.content.ShouldUseGenderedCharacterTranslations()) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635") : ((num2 == 0) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635").Split('/').First() : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635").Split('/').Last()));
			if (num3)
			{
				text = ((num2 == 0) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11636") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11637"));
			}
			else if (farmerMaybeOffline.isMarried() && !farmerMaybeOffline.hasRoommate())
			{
				text = ((num2 == 0) ? Game1.content.LoadString("Strings\\UI:SocialPage_MarriedToOtherPlayer_MaleNPC") : Game1.content.LoadString("Strings\\UI:SocialPage_MarriedToOtherPlayer_FemaleNPC"));
			}
			else if (!Game1.player.isMarried() && friendship.IsDating())
			{
				text = ((num2 == 0) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11639") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11640"));
			}
			else if (friendship.IsDivorced())
			{
				text = ((num2 == 0) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11642") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11643"));
			}
			text = Game1.parseText(width: (IClickableMenu.borderWidth * 3 + 128 - 40 + 192) / 2, text: text, whichFont: Game1.smallFont);
			Vector2 vector = Game1.smallFont.MeasureString(text);
			b.DrawString(Game1.smallFont, text, new Vector2((float)(xPositionOnScreen + 192 + 8) - vector.X / 2f, (float)sprites[i].bounds.Bottom - (vector.Y - y2)), Game1.textColor);
			if (num3)
			{
				b.Draw(Game1.objectSpriteSheet, new Vector2(xPositionOnScreen + IClickableMenu.borderWidth * 7 / 4 + 192, sprites[i].bounds.Y), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 801, 16, 16), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.88f);
			}
			else if (friendship.IsDating())
			{
				b.Draw(Game1.objectSpriteSheet, new Vector2(xPositionOnScreen + IClickableMenu.borderWidth * 7 / 4 + 192, sprites[i].bounds.Y), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 458, 16, 16), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.88f);
			}
		}
	}

	public override void draw(SpriteBatch b)
	{
		b.End();
		b.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, Utility.ScissorEnabled, null, null);
		drawHorizontalPartition(b, yPositionOnScreen + IClickableMenu.borderWidth + 128 + 4, small: true);
		drawHorizontalPartition(b, yPositionOnScreen + IClickableMenu.borderWidth + 192 + 32 + 20, small: true);
		drawHorizontalPartition(b, yPositionOnScreen + IClickableMenu.borderWidth + 320 + 36, small: true);
		drawHorizontalPartition(b, yPositionOnScreen + IClickableMenu.borderWidth + 384 + 32 + 52, small: true);
		for (int i = slotPosition; i < slotPosition + 5; i++)
		{
			if (i < sprites.Count)
			{
				if (names[i] is string)
				{
					drawNPCSlot(b, i);
				}
				else if (names[i] is long)
				{
					drawFarmerSlot(b, i);
				}
			}
		}
		Rectangle scissorRectangle = b.GraphicsDevice.ScissorRectangle;
		Rectangle scissorRectangle2 = scissorRectangle;
		scissorRectangle2.Y = Math.Max(0, rowPosition(numFarmers - 1));
		scissorRectangle2.Height -= scissorRectangle2.Y;
		if (scissorRectangle2.Height > 0)
		{
			b.GraphicsDevice.ScissorRectangle = scissorRectangle2;
			try
			{
				drawVerticalPartition(b, xPositionOnScreen + 256 + 12, small: true);
				drawVerticalPartition(b, xPositionOnScreen + 384 + 368, small: true);
				drawVerticalPartition(b, xPositionOnScreen + 256 + 12 + 352, small: true);
			}
			finally
			{
				b.GraphicsDevice.ScissorRectangle = scissorRectangle;
			}
		}
		upButton.draw(b);
		downButton.draw(b);
		IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), scrollBarRunner.X, scrollBarRunner.Y, scrollBarRunner.Width, scrollBarRunner.Height, Color.White, 4f);
		scrollBar.draw(b);
		if (!hoverText.Equals(""))
		{
			IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont);
		}
		b.End();
		b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);
	}
}
