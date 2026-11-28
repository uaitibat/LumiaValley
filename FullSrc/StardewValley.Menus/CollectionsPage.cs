using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace StardewValley.Menus;

public class CollectionsPage : IClickableMenu
{
	public const int region_sideTabShipped = 7001;

	public const int region_sideTabFish = 7002;

	public const int region_sideTabArtifacts = 7003;

	public const int region_sideTabMinerals = 7004;

	public const int region_sideTabCooking = 7005;

	public const int region_sideTabAchivements = 7006;

	public const int region_sideTabSecretNotes = 7007;

	public const int region_sideTabLetters = 7008;

	public const int region_forwardButton = 707;

	public const int region_backButton = 706;

	public static int widthToMoveActiveTab = 8;

	public const int organicsTab = 0;

	public const int fishTab = 1;

	public const int archaeologyTab = 2;

	public const int mineralsTab = 3;

	public const int cookingTab = 4;

	public const int achievementsTab = 5;

	public const int secretNotesTab = 6;

	public const int lettersTab = 7;

	public const int distanceFromMenuBottomBeforeNewPage = 128;

	private string descriptionText = "";

	private string hoverText = "";

	public ClickableTextureComponent backButton;

	public ClickableTextureComponent forwardButton;

	public Dictionary<int, ClickableTextureComponent> sideTabs = new Dictionary<int, ClickableTextureComponent>();

	public int currentTab;

	public int currentPage;

	public int secretNoteImage = -1;

	public Dictionary<int, List<List<ClickableTextureComponent>>> collections = new Dictionary<int, List<List<ClickableTextureComponent>>>();

	public Dictionary<int, string> secretNotesData;

	public Texture2D secretNoteImageTexture;

	public LetterViewerMenu letterviewerSubMenu;

	private Item hoverItem;

	private CraftingRecipe hoverCraftingRecipe;

	private int value;

	public CollectionsPage(int x, int y, int width, int height)
		: base(x, y, width, height)
	{
		sideTabs.Add(0, new ClickableTextureComponent(0.ToString() ?? "", new Rectangle(xPositionOnScreen - 48 + widthToMoveActiveTab, yPositionOnScreen + 64 * (2 + sideTabs.Count), 64, 64), "", Game1.content.LoadString("Strings\\UI:Collections_Shipped"), Game1.mouseCursors, new Rectangle(640, 80, 16, 16), 4f)
		{
			myID = 7001,
			downNeighborID = -99998,
			rightNeighborID = 0
		});
		collections.Add(0, new List<List<ClickableTextureComponent>>());
		sideTabs.Add(1, new ClickableTextureComponent(1.ToString() ?? "", new Rectangle(xPositionOnScreen - 48, yPositionOnScreen + 64 * (2 + sideTabs.Count), 64, 64), "", Game1.content.LoadString("Strings\\UI:Collections_Fish"), Game1.mouseCursors, new Rectangle(640, 64, 16, 16), 4f)
		{
			myID = 7002,
			upNeighborID = -99998,
			downNeighborID = -99998,
			rightNeighborID = 0
		});
		collections.Add(1, new List<List<ClickableTextureComponent>>());
		sideTabs.Add(2, new ClickableTextureComponent(2.ToString() ?? "", new Rectangle(xPositionOnScreen - 48, yPositionOnScreen + 64 * (2 + sideTabs.Count), 64, 64), "", Game1.content.LoadString("Strings\\UI:Collections_Artifacts"), Game1.mouseCursors, new Rectangle(656, 64, 16, 16), 4f)
		{
			myID = 7003,
			upNeighborID = -99998,
			downNeighborID = -99998,
			rightNeighborID = 0
		});
		collections.Add(2, new List<List<ClickableTextureComponent>>());
		sideTabs.Add(3, new ClickableTextureComponent(3.ToString() ?? "", new Rectangle(xPositionOnScreen - 48, yPositionOnScreen + 64 * (2 + sideTabs.Count), 64, 64), "", Game1.content.LoadString("Strings\\UI:Collections_Minerals"), Game1.mouseCursors, new Rectangle(672, 64, 16, 16), 4f)
		{
			myID = 7004,
			upNeighborID = -99998,
			downNeighborID = -99998,
			rightNeighborID = 0
		});
		collections.Add(3, new List<List<ClickableTextureComponent>>());
		sideTabs.Add(4, new ClickableTextureComponent(4.ToString() ?? "", new Rectangle(xPositionOnScreen - 48, yPositionOnScreen + 64 * (2 + sideTabs.Count), 64, 64), "", Game1.content.LoadString("Strings\\UI:Collections_Cooking"), Game1.mouseCursors, new Rectangle(688, 64, 16, 16), 4f)
		{
			myID = 7005,
			upNeighborID = -99998,
			downNeighborID = -99998,
			rightNeighborID = 0
		});
		collections.Add(4, new List<List<ClickableTextureComponent>>());
		sideTabs.Add(5, new ClickableTextureComponent(5.ToString() ?? "", new Rectangle(xPositionOnScreen - 48, yPositionOnScreen + 64 * (2 + sideTabs.Count), 64, 64), "", Game1.content.LoadString("Strings\\UI:Collections_Achievements"), Game1.mouseCursors, new Rectangle(656, 80, 16, 16), 4f)
		{
			myID = 7006,
			upNeighborID = 7005,
			downNeighborID = -99998,
			rightNeighborID = 0
		});
		collections.Add(5, new List<List<ClickableTextureComponent>>());
		sideTabs.Add(7, new ClickableTextureComponent(7.ToString() ?? "", new Rectangle(xPositionOnScreen - 48, yPositionOnScreen + 64 * (2 + sideTabs.Count), 64, 64), "", Game1.content.LoadString("Strings\\UI:Collections_Letters"), Game1.mouseCursors, new Rectangle(688, 80, 16, 16), 4f)
		{
			myID = 7008,
			upNeighborID = -99998,
			downNeighborID = -99998,
			rightNeighborID = 0
		});
		collections.Add(7, new List<List<ClickableTextureComponent>>());
		if (Game1.player.secretNotesSeen.Count > 0)
		{
			sideTabs.Add(6, new ClickableTextureComponent(6.ToString() ?? "", new Rectangle(xPositionOnScreen - 48, yPositionOnScreen + 64 * (2 + sideTabs.Count), 64, 64), "", Game1.content.LoadString("Strings\\UI:Collections_SecretNotes"), Game1.mouseCursors, new Rectangle(672, 80, 16, 16), 4f)
			{
				myID = 7007,
				upNeighborID = -99998,
				rightNeighborID = 0
			});
			collections.Add(6, new List<List<ClickableTextureComponent>>());
		}
		sideTabs[0].upNeighborID = -1;
		sideTabs[0].upNeighborImmutable = true;
		int key = 0;
		int num = 0;
		foreach (int key2 in sideTabs.Keys)
		{
			if (sideTabs[key2].bounds.Y > num)
			{
				num = sideTabs[key2].bounds.Y;
				key = key2;
			}
		}
		sideTabs[key].downNeighborID = -1;
		sideTabs[key].downNeighborImmutable = true;
		widthToMoveActiveTab = 8;
		backButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 48, yPositionOnScreen + height - 80, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f)
		{
			myID = 706,
			rightNeighborID = -7777
		};
		forwardButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 32 - 60, yPositionOnScreen + height - 80, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f)
		{
			myID = 707,
			leftNeighborID = -7777
		};
		int[] array = new int[8];
		int num2 = xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearSideBorder;
		int num3 = yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16;
		int num4 = 10;
		List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>(Game1.objectInformation);
		list.Sort((KeyValuePair<int, string> a, KeyValuePair<int, string> b) => a.Key.CompareTo(b.Key));
		foreach (KeyValuePair<int, string> item in list)
		{
			string text = item.Value.Split('/')[3];
			int num5 = 0;
			bool drawShadow = false;
			bool flag = false;
			if (text.Contains("Arch"))
			{
				num5 = 2;
				if (Game1.player.archaeologyFound.ContainsKey(item.Key))
				{
					drawShadow = true;
				}
			}
			else if (text.Contains("Fish"))
			{
				if ((item.Key >= 167 && item.Key <= 172) || (item.Key >= 898 && item.Key <= 902))
				{
					continue;
				}
				num5 = 1;
				if (Game1.player.fishCaught.ContainsKey(item.Key))
				{
					drawShadow = true;
				}
			}
			else if (text.Contains("Mineral") || text.Substring(text.Length - 3).Equals("-2"))
			{
				num5 = 3;
				if (Game1.player.mineralsFound.ContainsKey(item.Key))
				{
					drawShadow = true;
				}
			}
			else if (text.Contains("Cooking") || text.Substring(text.Length - 3).Equals("-7"))
			{
				num5 = 4;
				string text2 = item.Value.Split('/')[0];
				switch (text2)
				{
				case "Cheese Cauli.":
					text2 = "Cheese Cauliflower";
					break;
				case "Cheese Cauliflower":
					text2 = "Cheese Cauli.";
					break;
				case "Vegetable Medley":
					text2 = "Vegetable Stew";
					break;
				case "Cookie":
					text2 = "Cookies";
					break;
				case "Eggplant Parmesan":
					text2 = "Eggplant Parm.";
					break;
				case "Cranberry Sauce":
					text2 = "Cran. Sauce";
					break;
				case "Dish O' The Sea":
					text2 = "Dish o' The Sea";
					break;
				}
				if (Game1.player.recipesCooked.ContainsKey(item.Key))
				{
					drawShadow = true;
				}
				else if (Game1.player.cookingRecipes.ContainsKey(text2))
				{
					flag = true;
				}
				if (item.Key == 217 || item.Key == 772 || item.Key == 773 || item.Key == 279 || item.Key == 873)
				{
					continue;
				}
			}
			else
			{
				if (!Object.isPotentialBasicShippedCategory(item.Key, text.Substring(text.Length - 3)))
				{
					continue;
				}
				num5 = 0;
				if (Game1.player.basicShipped.ContainsKey(item.Key))
				{
					drawShadow = true;
				}
			}
			int x2 = num2 + array[num5] % num4 * 68;
			int num6 = num3 + array[num5] / num4 * 68;
			if (num6 > yPositionOnScreen + height - 128)
			{
				collections[num5].Add(new List<ClickableTextureComponent>());
				array[num5] = 0;
				x2 = num2;
				num6 = num3;
			}
			if (collections[num5].Count == 0)
			{
				collections[num5].Add(new List<ClickableTextureComponent>());
			}
			collections[num5].Last().Add(new ClickableTextureComponent(item.Key + " " + drawShadow + " " + flag, new Rectangle(x2, num6, 64, 64), null, "", Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, item.Key, 16, 16), 4f, drawShadow)
			{
				myID = collections[num5].Last().Count,
				rightNeighborID = (((collections[num5].Last().Count + 1) % num4 == 0) ? (-1) : (collections[num5].Last().Count + 1)),
				leftNeighborID = ((collections[num5].Last().Count % num4 == 0) ? 7001 : (collections[num5].Last().Count - 1)),
				downNeighborID = ((num6 + 68 > yPositionOnScreen + height - 128) ? (-7777) : (collections[num5].Last().Count + num4)),
				upNeighborID = ((collections[num5].Last().Count < num4) ? 12345 : (collections[num5].Last().Count - num4)),
				fullyImmutable = true
			});
			array[num5]++;
		}
		if (collections[5].Count == 0)
		{
			collections[5].Add(new List<ClickableTextureComponent>());
		}
		foreach (KeyValuePair<int, string> achievement in Game1.achievements)
		{
			bool flag2 = Game1.player.achievements.Contains(achievement.Key);
			string[] array2 = achievement.Value.Split('^');
			if (flag2 || (array2[2].Equals("true") && (array2[3].Equals("-1") || farmerHasAchievements(array2[3]))))
			{
				int x3 = num2 + array[5] % num4 * 68;
				int y2 = num3 + array[5] / num4 * 68;
				collections[5][0].Add(new ClickableTextureComponent(achievement.Key + " " + flag2, new Rectangle(x3, y2, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 25), 1f));
				array[5]++;
			}
		}
		if (Game1.player.secretNotesSeen.Count > 0)
		{
			if (collections[6].Count == 0)
			{
				collections[6].Add(new List<ClickableTextureComponent>());
			}
			secretNotesData = Game1.content.Load<Dictionary<int, string>>("Data\\SecretNotes");
			secretNoteImageTexture = Game1.temporaryContent.Load<Texture2D>("TileSheets\\SecretNotesImages");
			bool flag3 = Game1.player.secretNotesSeen.Contains(GameLocation.JOURNAL_INDEX + 1);
			foreach (int key3 in secretNotesData.Keys)
			{
				if (key3 >= GameLocation.JOURNAL_INDEX)
				{
					if (!flag3)
					{
						continue;
					}
				}
				else if (!Game1.player.hasMagnifyingGlass)
				{
					continue;
				}
				int x4 = num2 + array[6] % num4 * 68;
				int y3 = num3 + array[6] / num4 * 68;
				if (key3 >= GameLocation.JOURNAL_INDEX)
				{
					collections[6][0].Add(new ClickableTextureComponent(key3 + " " + Game1.player.secretNotesSeen.Contains(key3), new Rectangle(x4, y3, 64, 64), null, "", Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 842, 16, 16), 4f, Game1.player.secretNotesSeen.Contains(key3)));
				}
				else
				{
					collections[6][0].Add(new ClickableTextureComponent(key3 + " " + Game1.player.secretNotesSeen.Contains(key3), new Rectangle(x4, y3, 64, 64), null, "", Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 79, 16, 16), 4f, Game1.player.secretNotesSeen.Contains(key3)));
				}
				array[6]++;
			}
		}
		if (collections[7].Count == 0)
		{
			collections[7].Add(new List<ClickableTextureComponent>());
		}
		Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\mail");
		foreach (string item2 in Game1.player.mailReceived)
		{
			if (dictionary.ContainsKey(item2))
			{
				int x5 = num2 + array[7] % num4 * 68;
				int num7 = num3 + array[7] / num4 * 68;
				string[] array3 = dictionary[item2].Split(new string[1] { "[#]" }, StringSplitOptions.None);
				if (num7 > yPositionOnScreen + height - 128)
				{
					collections[7].Add(new List<ClickableTextureComponent>());
					array[7] = 0;
					x5 = num2;
					num7 = num3;
				}
				collections[7].Last().Add(new ClickableTextureComponent(item2 + " true " + ((array3.Count() > 1) ? array3[1] : "???"), new Rectangle(x5, num7, 64, 64), null, "", Game1.mouseCursors, new Rectangle(190, 423, 14, 11), 4f, drawShadow: true)
				{
					myID = collections[7].Last().Count,
					rightNeighborID = (((collections[7].Last().Count + 1) % num4 == 0) ? (-1) : (collections[7].Last().Count + 1)),
					leftNeighborID = ((collections[7].Last().Count % num4 == 0) ? 7008 : (collections[7].Last().Count - 1)),
					downNeighborID = ((num7 + 68 > yPositionOnScreen + height - 128) ? (-7777) : (collections[7].Last().Count + num4)),
					upNeighborID = ((collections[7].Last().Count < num4) ? 12345 : (collections[7].Last().Count - num4)),
					fullyImmutable = true
				});
				array[7]++;
			}
		}
	}

	protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
	{
		base.customSnapBehavior(direction, oldRegion, oldID);
		switch (direction)
		{
		case 2:
			if (currentPage > 0)
			{
				currentlySnappedComponent = getComponentWithID(706);
			}
			else if (currentPage == 0 && collections[currentTab].Count > 1)
			{
				currentlySnappedComponent = getComponentWithID(707);
			}
			backButton.upNeighborID = oldID;
			forwardButton.upNeighborID = oldID;
			break;
		case 3:
			if (oldID == 707 && currentPage > 0)
			{
				currentlySnappedComponent = getComponentWithID(706);
			}
			break;
		case 1:
			if (oldID == 706 && collections[currentTab].Count > currentPage + 1)
			{
				currentlySnappedComponent = getComponentWithID(707);
			}
			break;
		}
	}

	public override void snapToDefaultClickableComponent()
	{
		base.snapToDefaultClickableComponent();
		currentlySnappedComponent = getComponentWithID(0);
		snapCursorToCurrentSnappedComponent();
	}

	private bool farmerHasAchievements(string listOfAchievementNumbers)
	{
		string[] array = listOfAchievementNumbers.Split(' ');
		foreach (string text in array)
		{
			if (!Game1.player.achievements.Contains(Convert.ToInt32(text)))
			{
				return false;
			}
		}
		return true;
	}

	public override bool readyToClose()
	{
		if (letterviewerSubMenu != null)
		{
			return false;
		}
		return base.readyToClose();
	}

	public override void update(GameTime time)
	{
		base.update(time);
		if (letterviewerSubMenu == null)
		{
			return;
		}
		letterviewerSubMenu.update(time);
		if (letterviewerSubMenu.destroy)
		{
			letterviewerSubMenu = null;
			if (Game1.options.SnappyMenus)
			{
				snapCursorToCurrentSnappedComponent();
			}
		}
	}

	public override void receiveKeyPress(Keys key)
	{
		base.receiveKeyPress(key);
		if (letterviewerSubMenu != null)
		{
			letterviewerSubMenu.receiveKeyPress(key);
		}
	}

	public override void receiveLeftClick(int x, int y, bool playSound = true)
	{
		if (letterviewerSubMenu != null)
		{
			letterviewerSubMenu.receiveLeftClick(x, y);
			return;
		}
		foreach (KeyValuePair<int, ClickableTextureComponent> sideTab in sideTabs)
		{
			if (sideTab.Value.containsPoint(x, y) && currentTab != sideTab.Key)
			{
				Game1.playSound("smallSelect");
				sideTabs[currentTab].bounds.X -= widthToMoveActiveTab;
				currentTab = Convert.ToInt32(sideTab.Value.name);
				currentPage = 0;
				sideTab.Value.bounds.X += widthToMoveActiveTab;
			}
		}
		if (currentPage > 0 && backButton.containsPoint(x, y))
		{
			currentPage--;
			Game1.playSound("shwip");
			backButton.scale = backButton.baseScale;
			if (Game1.options.snappyMenus && Game1.options.gamepadControls && currentPage == 0)
			{
				currentlySnappedComponent = forwardButton;
				Game1.setMousePosition(currentlySnappedComponent.bounds.Center);
			}
		}
		if (currentPage < collections[currentTab].Count - 1 && forwardButton.containsPoint(x, y))
		{
			currentPage++;
			Game1.playSound("shwip");
			forwardButton.scale = forwardButton.baseScale;
			if (Game1.options.snappyMenus && Game1.options.gamepadControls && currentPage == collections[currentTab].Count - 1)
			{
				currentlySnappedComponent = backButton;
				Game1.setMousePosition(currentlySnappedComponent.bounds.Center);
			}
		}
		if (currentTab == 7)
		{
			Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\mail");
			{
				foreach (ClickableTextureComponent item in collections[currentTab][currentPage])
				{
					if (item.containsPoint(x, y))
					{
						letterviewerSubMenu = new LetterViewerMenu(dictionary[item.name.Split(' ')[0]], item.name.Split(' ')[0], fromCollection: true);
					}
				}
				return;
			}
		}
		if (currentTab != 6)
		{
			return;
		}
		foreach (ClickableTextureComponent item2 in collections[currentTab][currentPage])
		{
			if (item2.containsPoint(x, y))
			{
				int result = -1;
				string[] array = item2.name.Split(' ');
				if (array[1] == "True" && int.TryParse(array[0], out result))
				{
					letterviewerSubMenu = new LetterViewerMenu(result);
					letterviewerSubMenu.isFromCollection = true;
					break;
				}
			}
		}
	}

	public override bool shouldDrawCloseButton()
	{
		return letterviewerSubMenu == null;
	}

	public override void receiveRightClick(int x, int y, bool playSound = true)
	{
		if (letterviewerSubMenu != null)
		{
			letterviewerSubMenu.receiveRightClick(x, y);
		}
	}

	public override void applyMovementKey(int direction)
	{
		if (letterviewerSubMenu != null)
		{
			letterviewerSubMenu.applyMovementKey(direction);
		}
		else
		{
			base.applyMovementKey(direction);
		}
	}

	public override void gamePadButtonHeld(Buttons b)
	{
		if (letterviewerSubMenu != null)
		{
			letterviewerSubMenu.gamePadButtonHeld(b);
		}
		else
		{
			base.gamePadButtonHeld(b);
		}
	}

	public override void receiveGamePadButton(Buttons b)
	{
		if (letterviewerSubMenu != null)
		{
			letterviewerSubMenu.receiveGamePadButton(b);
		}
		else
		{
			base.receiveGamePadButton(b);
		}
	}

	public override void performHoverAction(int x, int y)
	{
		descriptionText = "";
		hoverText = "";
		value = -1;
		secretNoteImage = -1;
		if (letterviewerSubMenu != null)
		{
			letterviewerSubMenu.performHoverAction(x, y);
			return;
		}
		foreach (ClickableTextureComponent value in sideTabs.Values)
		{
			if (value.containsPoint(x, y))
			{
				hoverText = value.hoverText;
				return;
			}
		}
		bool flag = false;
		foreach (ClickableTextureComponent item in collections[currentTab][currentPage])
		{
			if (item.containsPoint(x, y))
			{
				item.scale = Math.Min(item.scale + 0.02f, item.baseScale + 0.1f);
				string[] array = item.name.Split(' ');
				if (currentTab == 5 || (array.Length > 1 && Convert.ToBoolean(array[1])) || (array.Length > 2 && Convert.ToBoolean(array[2])))
				{
					if (currentTab == 7)
					{
						hoverText = Game1.parseText(item.name.Substring(item.name.IndexOf(' ', item.name.IndexOf(' ') + 1) + 1), Game1.smallFont, 256);
					}
					else
					{
						hoverText = createDescription(Convert.ToInt32(array[0]));
					}
				}
				else
				{
					if (hoverText != "???")
					{
						hoverItem = null;
					}
					hoverText = "???";
				}
				flag = true;
			}
			else
			{
				item.scale = Math.Max(item.scale - 0.02f, item.baseScale);
			}
		}
		if (!flag)
		{
			hoverItem = null;
		}
		forwardButton.tryHover(x, y, 0.5f);
		backButton.tryHover(x, y, 0.5f);
	}

	public string createDescription(int index)
	{
		string text = "";
		if (currentTab == 5)
		{
			string[] array = Game1.achievements[index].Split('^');
			text = text + array[0] + Environment.NewLine + Environment.NewLine;
			text += array[1];
		}
		else if (currentTab == 6)
		{
			if (secretNotesData != null)
			{
				text = ((index >= GameLocation.JOURNAL_INDEX) ? (text + Game1.content.LoadString("Strings\\Locations:Journal_Name") + " #" + (index - GameLocation.JOURNAL_INDEX)) : (text + Game1.content.LoadString("Strings\\Locations:Secret_Note_Name") + " #" + index));
				if (secretNotesData[index][0] == '!')
				{
					secretNoteImage = Convert.ToInt32(secretNotesData[index].Split(' ')[1]);
				}
				else
				{
					string text2 = Game1.parseText(Utility.ParseGiftReveals(secretNotesData[index]).TrimStart(' ', '^').Replace("^", Environment.NewLine)
						.Replace("@", Game1.player.name), Game1.smallFont, 512);
					string[] array2 = text2.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None);
					int num = 15;
					if (array2.Length > num)
					{
						string[] array3 = new string[num];
						for (int i = 0; i < num; i++)
						{
							array3[i] = array2[i];
						}
						text2 = string.Join(Environment.NewLine, array3).Trim() + Environment.NewLine + "(...)";
					}
					text = text + Environment.NewLine + Environment.NewLine + text2;
				}
			}
		}
		else
		{
			string[] array4 = Game1.objectInformation[index].Split('/');
			string text3 = array4[4];
			text = text + text3 + Environment.NewLine + Environment.NewLine + Game1.parseText(array4[5], Game1.smallFont, 256) + Environment.NewLine + Environment.NewLine;
			if (array4[3].Contains("Arch"))
			{
				text += (Game1.player.archaeologyFound.ContainsKey(index) ? Game1.content.LoadString("Strings\\UI:Collections_Description_ArtifactsFound", Game1.player.archaeologyFound[index][0]) : "");
			}
			else if (array4[3].Contains("Cooking"))
			{
				text += (Game1.player.recipesCooked.ContainsKey(index) ? Game1.content.LoadString("Strings\\UI:Collections_Description_RecipesCooked", Game1.player.recipesCooked[index]) : "");
				if (hoverItem == null || hoverItem.ParentSheetIndex != index)
				{
					hoverItem = new Object(index, 1);
					string text4 = hoverItem.Name;
					switch (text4)
					{
					case "Cheese Cauli.":
						text4 = "Cheese Cauliflower";
						break;
					case "Cheese Cauliflower":
						text4 = "Cheese Cauli.";
						break;
					case "Vegetable Medley":
						text4 = "Vegetable Stew";
						break;
					case "Cookie":
						text4 = "Cookies";
						break;
					case "Eggplant Parmesan":
						text4 = "Eggplant Parm.";
						break;
					case "Cranberry Sauce":
						text4 = "Cran. Sauce";
						break;
					case "Dish O' The Sea":
						text4 = "Dish o' The Sea";
						break;
					}
					hoverCraftingRecipe = new CraftingRecipe(text4, isCookingRecipe: true);
				}
			}
			else if (!array4[3].Contains("Fish"))
			{
				text = ((!array4[3].Contains("Minerals") && !array4[3].Substring(array4[3].Length - 3).Equals("-2")) ? (text + Game1.content.LoadString("Strings\\UI:Collections_Description_NumberShipped", Game1.player.basicShipped.ContainsKey(index) ? Game1.player.basicShipped[index] : 0)) : (text + Game1.content.LoadString("Strings\\UI:Collections_Description_MineralsFound", Game1.player.mineralsFound.ContainsKey(index) ? Game1.player.mineralsFound[index] : 0)));
			}
			else
			{
				text += Game1.content.LoadString("Strings\\UI:Collections_Description_FishCaught", Game1.player.fishCaught.ContainsKey(index) ? Game1.player.fishCaught[index][0] : 0);
				if (Game1.player.fishCaught.ContainsKey(index) && Game1.player.fishCaught[index][1] > 0)
				{
					text = text + Environment.NewLine + Game1.content.LoadString("Strings\\UI:Collections_Description_BiggestCatch", Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14083", (LocalizedContentManager.CurrentLanguageCode != 0) ? Math.Round((double)Game1.player.fishCaught[index][1] * 2.54) : ((double)Game1.player.fishCaught[index][1])));
				}
			}
			value = Convert.ToInt32(array4[1]);
		}
		return text;
	}

	public override void draw(SpriteBatch b)
	{
		foreach (ClickableTextureComponent value in sideTabs.Values)
		{
			value.draw(b);
		}
		if (currentPage > 0)
		{
			backButton.draw(b);
		}
		if (currentPage < collections[currentTab].Count - 1)
		{
			forwardButton.draw(b);
		}
		b.End();
		b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);
		foreach (ClickableTextureComponent item in collections[currentTab][currentPage])
		{
			bool flag = Convert.ToBoolean(item.name.Split(' ')[1]);
			bool flag2 = currentTab == 4 && Convert.ToBoolean(item.name.Split(' ')[2]);
			item.draw(b, flag2 ? (Color.DimGray * 0.4f) : (flag ? Color.White : (Color.Black * 0.2f)), 0.86f);
			if (currentTab == 5 && flag)
			{
				int num = new Random(Convert.ToInt32(item.name.Split(' ')[0])).Next(12);
				b.Draw(Game1.mouseCursors, new Vector2(item.bounds.X + 16 + 16, item.bounds.Y + 20 + 16), new Rectangle(256 + num % 6 * 64 / 2, 128 + num / 6 * 64 / 2, 32, 32), Color.White, 0f, new Vector2(16f, 16f), item.scale, SpriteEffects.None, 0.88f);
			}
		}
		b.End();
		b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);
		if (hoverItem != null)
		{
			IClickableMenu.drawToolTip(b, hoverItem.getDescription(), hoverItem.DisplayName, hoverItem, heldItem: false, -1, 0, -1, -1, hoverCraftingRecipe);
		}
		else if (!hoverText.Equals(""))
		{
			IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont, 0, 0, this.value);
			if (secretNoteImage != -1)
			{
				IClickableMenu.drawTextureBox(b, Game1.getOldMouseX(), Game1.getOldMouseY() + 64 + 32, 288, 288, Color.White);
				b.Draw(secretNoteImageTexture, new Vector2(Game1.getOldMouseX() + 16, Game1.getOldMouseY() + 64 + 32 + 16), new Rectangle(secretNoteImage * 64 % secretNoteImageTexture.Width, secretNoteImage * 64 / secretNoteImageTexture.Width * 64, 64, 64), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.865f);
			}
		}
		if (letterviewerSubMenu != null)
		{
			letterviewerSubMenu.draw(b);
		}
	}
}
