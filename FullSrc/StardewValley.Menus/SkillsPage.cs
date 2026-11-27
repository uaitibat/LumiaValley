using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewValley.Menus;

public class SkillsPage : IClickableMenu
{
	public const int region_special1 = 10201;

	public const int region_special2 = 10202;

	public const int region_special3 = 10203;

	public const int region_special4 = 10204;

	public const int region_special5 = 10205;

	public const int region_special6 = 10206;

	public const int region_special7 = 10207;

	public const int region_special8 = 10208;

	public const int region_special9 = 10209;

	public const int region_special_skullkey = 10210;

	public const int region_special_townkey = 10211;

	public const int region_skillArea1 = 0;

	public const int region_skillArea2 = 1;

	public const int region_skillArea3 = 2;

	public const int region_skillArea4 = 3;

	public const int region_skillArea5 = 4;

	public List<ClickableTextureComponent> skillBars = new List<ClickableTextureComponent>();

	public List<ClickableTextureComponent> skillAreas = new List<ClickableTextureComponent>();

	public List<ClickableTextureComponent> specialItems = new List<ClickableTextureComponent>();

	private string hoverText = "";

	private string hoverTitle = "";

	private int professionImage = -1;

	private int playerPanelIndex;

	private int playerPanelTimer;

	private Rectangle playerPanel;

	private int[] playerPanelFrames = new int[4] { 0, 1, 0, 2 };

	public SkillsPage(int x, int y, int width, int height)
		: base(x, y, width, height)
	{
		int num = xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 80;
		int y2 = yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + (int)((float)height / 2f) + 80;
		playerPanel = new Rectangle(xPositionOnScreen + 64, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder, 128, 192);
		if (Game1.player.canUnderstandDwarves)
		{
			specialItems.Add(new ClickableTextureComponent("", new Rectangle(num, y2, 64, 64), null, Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11587"), Game1.mouseCursors, new Rectangle(129, 320, 16, 16), 4f, drawShadow: true)
			{
				myID = 10201,
				rightNeighborID = -99998,
				leftNeighborID = -99998
			});
		}
		else
		{
			specialItems.Add(null);
		}
		if (Game1.player.hasRustyKey)
		{
			specialItems.Add(new ClickableTextureComponent("", new Rectangle(num + 68, y2, 64, 64), null, Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11588"), Game1.mouseCursors, new Rectangle(145, 320, 16, 16), 4f, drawShadow: true)
			{
				myID = 10202,
				rightNeighborID = -99998,
				leftNeighborID = -99998,
				upNeighborID = 4
			});
		}
		else
		{
			specialItems.Add(null);
		}
		if (Game1.player.hasClubCard)
		{
			specialItems.Add(new ClickableTextureComponent("", new Rectangle(num + 136, y2, 64, 64), null, Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11589"), Game1.mouseCursors, new Rectangle(161, 320, 16, 16), 4f, drawShadow: true)
			{
				myID = 10203,
				rightNeighborID = -99998,
				leftNeighborID = -99998,
				upNeighborID = 4
			});
		}
		else
		{
			specialItems.Add(null);
		}
		if (Game1.player.hasSpecialCharm)
		{
			specialItems.Add(new ClickableTextureComponent("", new Rectangle(num + 204, y2, 64, 64), null, Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11590"), Game1.mouseCursors, new Rectangle(176, 320, 16, 16), 4f, drawShadow: true)
			{
				myID = 10204,
				rightNeighborID = -99998,
				leftNeighborID = -99998,
				upNeighborID = 4
			});
		}
		else
		{
			specialItems.Add(null);
		}
		if (Game1.player.hasSkullKey)
		{
			specialItems.Add(new ClickableTextureComponent("", new Rectangle(num + 272, y2, 64, 64), null, Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11591"), Game1.mouseCursors, new Rectangle(192, 320, 16, 16), 4f, drawShadow: true)
			{
				myID = 10210,
				rightNeighborID = -99998,
				leftNeighborID = -99998,
				upNeighborID = 4
			});
		}
		else
		{
			specialItems.Add(null);
		}
		if (Game1.player.hasMagnifyingGlass)
		{
			specialItems.Add(new ClickableTextureComponent("", new Rectangle(num + 340, y2, 64, 64), null, Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.magnifyingglass"), Game1.mouseCursors, new Rectangle(208, 320, 16, 16), 4f, drawShadow: true)
			{
				myID = 10205,
				rightNeighborID = -99998,
				leftNeighborID = -99998,
				upNeighborID = 4
			});
		}
		else
		{
			specialItems.Add(null);
		}
		if (Game1.player.hasDarkTalisman)
		{
			specialItems.Add(new ClickableTextureComponent("", new Rectangle(num + 408, y2, 64, 64), null, Game1.content.LoadString("Strings\\Objects:DarkTalisman"), Game1.mouseCursors, new Rectangle(225, 320, 16, 16), 4f, drawShadow: true)
			{
				myID = 10206,
				rightNeighborID = -99998,
				leftNeighborID = -99998,
				upNeighborID = 4
			});
		}
		else
		{
			specialItems.Add(null);
		}
		if (Game1.player.hasMagicInk)
		{
			specialItems.Add(new ClickableTextureComponent("", new Rectangle(num + 476, y2, 64, 64), null, Game1.content.LoadString("Strings\\Objects:MagicInk"), Game1.mouseCursors, new Rectangle(241, 320, 16, 16), 4f, drawShadow: true)
			{
				myID = 10207,
				rightNeighborID = -99998,
				leftNeighborID = -99998,
				upNeighborID = 4
			});
		}
		else
		{
			specialItems.Add(null);
		}
		if (Game1.player.eventsSeen.Contains(2120303))
		{
			specialItems.Add(new ClickableTextureComponent("", new Rectangle(num + 544, y2, 64, 64), null, Game1.content.LoadString("Strings\\Objects:BearPaw"), Game1.mouseCursors, new Rectangle(192, 336, 16, 16), 4f, drawShadow: true)
			{
				myID = 10208,
				rightNeighborID = -99998,
				leftNeighborID = -99998,
				upNeighborID = 4
			});
		}
		else
		{
			specialItems.Add(null);
		}
		if (Game1.player.eventsSeen.Contains(3910979))
		{
			specialItems.Add(new ClickableTextureComponent("", new Rectangle(num + 612, y2, 64, 64), null, Game1.content.LoadString("Strings\\Objects:SpringOnionBugs"), Game1.mouseCursors, new Rectangle(208, 336, 16, 16), 4f, drawShadow: true)
			{
				myID = 10209,
				rightNeighborID = -99998,
				leftNeighborID = -99998,
				upNeighborID = 4
			});
		}
		else
		{
			specialItems.Add(null);
		}
		if (Game1.player.HasTownKey)
		{
			specialItems.Add(new ClickableTextureComponent("", new Rectangle(num, y2, 64, 64), null, Game1.content.LoadString("Strings\\StringsFromCSFiles:KeyToTheTown"), Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 912, 16, 16), 4f, drawShadow: true)
			{
				myID = 10211,
				rightNeighborID = -99998,
				leftNeighborID = -99998,
				upNeighborID = 4
			});
		}
		else
		{
			specialItems.Add(null);
		}
		int num2 = 680 / specialItems.Count;
		for (int i = 0; i < specialItems.Count; i++)
		{
			if (specialItems[i] != null)
			{
				specialItems[i].bounds.X = num + i * num2;
			}
		}
		ClickableComponent.SetUpNeighbors(specialItems, 4);
		ClickableComponent.ChainNeighborsLeftRight(specialItems);
		int num3 = 0;
		int num4 = ((LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.it) ? (xPositionOnScreen + width - 448 - 48 + 4) : (xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 256 - 4));
		int num5 = yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth - 12;
		for (int j = 4; j < 10; j += 5)
		{
			for (int k = 0; k < 5; k++)
			{
				string professionBlurb = "";
				string professionTitle = "";
				bool flag = false;
				int whichProfession = -1;
				switch (k)
				{
				case 0:
					flag = Game1.player.FarmingLevel > j;
					whichProfession = Game1.player.getProfessionForSkill(0, j + 1);
					parseProfessionDescription(ref professionBlurb, ref professionTitle, LevelUpMenu.getProfessionDescription(whichProfession));
					break;
				case 1:
					flag = Game1.player.MiningLevel > j;
					whichProfession = Game1.player.getProfessionForSkill(3, j + 1);
					parseProfessionDescription(ref professionBlurb, ref professionTitle, LevelUpMenu.getProfessionDescription(whichProfession));
					break;
				case 2:
					flag = Game1.player.ForagingLevel > j;
					whichProfession = Game1.player.getProfessionForSkill(2, j + 1);
					parseProfessionDescription(ref professionBlurb, ref professionTitle, LevelUpMenu.getProfessionDescription(whichProfession));
					break;
				case 3:
					flag = Game1.player.FishingLevel > j;
					whichProfession = Game1.player.getProfessionForSkill(1, j + 1);
					parseProfessionDescription(ref professionBlurb, ref professionTitle, LevelUpMenu.getProfessionDescription(whichProfession));
					break;
				case 4:
					flag = Game1.player.CombatLevel > j;
					whichProfession = Game1.player.getProfessionForSkill(4, j + 1);
					parseProfessionDescription(ref professionBlurb, ref professionTitle, LevelUpMenu.getProfessionDescription(whichProfession));
					break;
				case 5:
					flag = Game1.player.LuckLevel > j;
					whichProfession = Game1.player.getProfessionForSkill(5, j + 1);
					parseProfessionDescription(ref professionBlurb, ref professionTitle, LevelUpMenu.getProfessionDescription(whichProfession));
					break;
				}
				if (flag && (j + 1) % 5 == 0)
				{
					skillBars.Add(new ClickableTextureComponent(whichProfession.ToString() ?? "", new Rectangle(num3 + num4 - 4 + j * 36, num5 + k * 56, 56, 36), null, professionBlurb, Game1.mouseCursors, new Rectangle(159, 338, 14, 9), 4f, drawShadow: true)
					{
						myID = ((j + 1 == 5) ? (100 + k) : (200 + k)),
						leftNeighborID = ((j + 1 == 5) ? k : (100 + k)),
						rightNeighborID = ((j + 1 == 5) ? (200 + k) : (-1)),
						downNeighborID = -99998
					});
				}
			}
			num3 += 24;
		}
		for (int l = 0; l < skillBars.Count; l++)
		{
			if (l < skillBars.Count - 1 && Math.Abs(skillBars[l + 1].myID - skillBars[l].myID) < 50)
			{
				skillBars[l].downNeighborID = skillBars[l + 1].myID;
				skillBars[l + 1].upNeighborID = skillBars[l].myID;
			}
		}
		if (skillBars.Count > 1 && skillBars.Last().myID >= 200 && skillBars[skillBars.Count - 2].myID >= 200)
		{
			skillBars.Last().upNeighborID = skillBars[skillBars.Count - 2].myID;
		}
		for (int m = 0; m < 5; m++)
		{
			int num6 = m;
			switch (num6)
			{
			case 1:
				num6 = 3;
				break;
			case 3:
				num6 = 1;
				break;
			}
			string text = "";
			switch (num6)
			{
			case 0:
				if (Game1.player.FarmingLevel > 0)
				{
					text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11592", Game1.player.FarmingLevel) + Environment.NewLine + Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11594", Game1.player.FarmingLevel);
				}
				break;
			case 2:
				if (Game1.player.ForagingLevel > 0)
				{
					text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11596", Game1.player.ForagingLevel);
				}
				break;
			case 1:
				if (Game1.player.FishingLevel > 0)
				{
					text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11598", Game1.player.FishingLevel);
				}
				break;
			case 3:
				if (Game1.player.MiningLevel > 0)
				{
					text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11600", Game1.player.MiningLevel);
				}
				break;
			case 4:
				if (Game1.player.CombatLevel > 0)
				{
					text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11602", Game1.player.CombatLevel * 5);
				}
				break;
			}
			skillAreas.Add(new ClickableTextureComponent(num6.ToString() ?? "", new Rectangle(num4 - 128 - 48, num5 + m * 56, 148, 36), num6.ToString() ?? "", text, null, Rectangle.Empty, 1f)
			{
				myID = m,
				downNeighborID = ((m < 4) ? (m + 1) : (-99998)),
				upNeighborID = ((m > 0) ? (m - 1) : 12341),
				rightNeighborID = 100 + m
			});
		}
	}

	private void parseProfessionDescription(ref string professionBlurb, ref string professionTitle, List<string> professionDescription)
	{
		if (professionDescription.Count <= 0)
		{
			return;
		}
		professionTitle = professionDescription[0];
		for (int i = 1; i < professionDescription.Count; i++)
		{
			professionBlurb += professionDescription[i];
			if (i < professionDescription.Count - 1)
			{
				professionBlurb += Environment.NewLine;
			}
		}
	}

	public override void snapToDefaultClickableComponent()
	{
		currentlySnappedComponent = ((skillAreas.Count > 0) ? getComponentWithID(0) : null);
		snapCursorToCurrentSnappedComponent();
	}

	public override void receiveLeftClick(int x, int y, bool playSound = true)
	{
	}

	public override void receiveRightClick(int x, int y, bool playSound = true)
	{
	}

	public override void performHoverAction(int x, int y)
	{
		hoverText = "";
		hoverTitle = "";
		professionImage = -1;
		foreach (ClickableTextureComponent specialItem in specialItems)
		{
			if (specialItem != null && specialItem.containsPoint(x, y))
			{
				hoverText = specialItem.hoverText;
				break;
			}
		}
		foreach (ClickableTextureComponent skillBar in skillBars)
		{
			skillBar.scale = 4f;
			if (skillBar.containsPoint(x, y) && skillBar.hoverText.Length > 0 && !skillBar.name.Equals("-1"))
			{
				hoverText = skillBar.hoverText;
				hoverTitle = LevelUpMenu.getProfessionTitleFromNumber(Convert.ToInt32(skillBar.name));
				professionImage = Convert.ToInt32(skillBar.name);
				skillBar.scale = 0f;
			}
		}
		foreach (ClickableTextureComponent skillArea in skillAreas)
		{
			if (skillArea.containsPoint(x, y) && skillArea.hoverText.Length > 0)
			{
				hoverText = skillArea.hoverText;
				hoverTitle = Farmer.getSkillDisplayNameFromIndex(Convert.ToInt32(skillArea.name));
				break;
			}
		}
		if (playerPanel.Contains(x, y))
		{
			playerPanelTimer -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;
			if (playerPanelTimer <= 0)
			{
				playerPanelIndex = (playerPanelIndex + 1) % 4;
				playerPanelTimer = 150;
			}
		}
		else
		{
			playerPanelIndex = 0;
		}
	}

	public override void draw(SpriteBatch b)
	{
		int num = xPositionOnScreen + 64 - 12;
		int num2 = yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder;
		b.Draw((Game1.timeOfDay >= 1900) ? Game1.nightbg : Game1.daybg, new Vector2(num, num2), Color.White);
		FarmerRenderer.isDrawingForUI = true;
		Game1.player.FarmerRenderer.draw(b, new FarmerSprite.AnimationFrame(Game1.player.bathingClothes ? 108 : playerPanelFrames[playerPanelIndex], 0, secondaryArm: false, flip: false), Game1.player.bathingClothes ? 108 : playerPanelFrames[playerPanelIndex], new Rectangle(playerPanelFrames[playerPanelIndex] * 16, Game1.player.bathingClothes ? 576 : 0, 16, 32), new Vector2(num + 32, num2 + 32), Vector2.Zero, 0.8f, 2, Color.White, 0f, 1f, Game1.player);
		if (Game1.timeOfDay >= 1900)
		{
			Game1.player.FarmerRenderer.draw(b, new FarmerSprite.AnimationFrame(playerPanelFrames[playerPanelIndex], 0, secondaryArm: false, flip: false), playerPanelFrames[playerPanelIndex], new Rectangle(playerPanelFrames[playerPanelIndex] * 16, 0, 16, 32), new Vector2(num + 32, num2 + 32), Vector2.Zero, 0.8f, 2, Color.DarkBlue * 0.3f, 0f, 1f, Game1.player);
		}
		FarmerRenderer.isDrawingForUI = false;
		b.DrawString(Game1.smallFont, Game1.player.Name, new Vector2((float)(num + 64) - Game1.smallFont.MeasureString(Game1.player.Name).X / 2f, num2 + 192 + 4), Game1.textColor);
		b.DrawString(Game1.smallFont, Game1.player.getTitle(), new Vector2((float)(num + 64) - Game1.smallFont.MeasureString(Game1.player.getTitle()).X / 2f, num2 + 256 - 32), Game1.textColor);
		num = ((LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.it) ? (xPositionOnScreen + width - 448 - 48) : (xPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 256 - 8));
		num2 = yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth - 8;
		int num3 = 0;
		for (int i = 0; i < 10; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				bool flag = false;
				bool flag2 = false;
				string text = "";
				int num4 = 0;
				Rectangle value = Rectangle.Empty;
				switch (j)
				{
				case 0:
					flag = Game1.player.FarmingLevel > i;
					if (i == 0)
					{
						text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11604");
					}
					num4 = Game1.player.FarmingLevel;
					flag2 = (int)Game1.player.addedFarmingLevel > 0;
					value = new Rectangle(10, 428, 10, 10);
					break;
				case 1:
					flag = Game1.player.MiningLevel > i;
					if (i == 0)
					{
						text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11605");
					}
					num4 = Game1.player.MiningLevel;
					flag2 = (int)Game1.player.addedMiningLevel > 0;
					value = new Rectangle(30, 428, 10, 10);
					break;
				case 2:
					flag = Game1.player.ForagingLevel > i;
					if (i == 0)
					{
						text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11606");
					}
					num4 = Game1.player.ForagingLevel;
					flag2 = (int)Game1.player.addedForagingLevel > 0;
					value = new Rectangle(60, 428, 10, 10);
					break;
				case 3:
					flag = Game1.player.FishingLevel > i;
					if (i == 0)
					{
						text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11607");
					}
					num4 = Game1.player.FishingLevel;
					flag2 = (int)Game1.player.addedFishingLevel > 0 || (Game1.player.CurrentTool != null && Game1.player.CurrentTool.hasEnchantmentOfType<MasterEnchantment>());
					value = new Rectangle(20, 428, 10, 10);
					break;
				case 4:
					flag = Game1.player.CombatLevel > i;
					if (i == 0)
					{
						text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11608");
					}
					num4 = Game1.player.CombatLevel;
					flag2 = (int)Game1.player.addedCombatLevel > 0;
					value = new Rectangle(120, 428, 10, 10);
					break;
				case 5:
					flag = Game1.player.LuckLevel > i;
					if (i == 0)
					{
						text = Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11609");
					}
					num4 = Game1.player.LuckLevel;
					flag2 = (int)Game1.player.addedLuckLevel > 0;
					value = new Rectangle(50, 428, 10, 10);
					break;
				}
				if (!text.Equals(""))
				{
					b.DrawString(Game1.smallFont, text, new Vector2((float)num - Game1.smallFont.MeasureString(text).X + 4f - 64f, num2 + 4 + j * 56), Game1.textColor);
					b.Draw(Game1.mouseCursors, new Vector2(num - 56, num2 + j * 56), value, Color.Black * 0.3f, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.85f);
					b.Draw(Game1.mouseCursors, new Vector2(num - 52, num2 - 4 + j * 56), value, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
				}
				if (!flag && (i + 1) % 5 == 0)
				{
					b.Draw(Game1.mouseCursors, new Vector2(num3 + num - 4 + i * 36, num2 + j * 56), new Rectangle(145, 338, 14, 9), Color.Black * 0.35f, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
					b.Draw(Game1.mouseCursors, new Vector2(num3 + num + i * 36, num2 - 4 + j * 56), new Rectangle(145 + (flag ? 14 : 0), 338, 14, 9), Color.White * (flag ? 1f : 0.65f), 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
				}
				else if ((i + 1) % 5 != 0)
				{
					b.Draw(Game1.mouseCursors, new Vector2(num3 + num - 4 + i * 36, num2 + j * 56), new Rectangle(129, 338, 8, 9), Color.Black * 0.35f, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.85f);
					b.Draw(Game1.mouseCursors, new Vector2(num3 + num + i * 36, num2 - 4 + j * 56), new Rectangle(129 + (flag ? 8 : 0), 338, 8, 9), Color.White * (flag ? 1f : 0.65f), 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
				}
				if (i == 9)
				{
					NumberSprite.draw(num4, b, new Vector2(num3 + num + (i + 2) * 36 + 12 + ((num4 >= 10) ? 12 : 0), num2 + 16 + j * 56), Color.Black * 0.35f, 1f, 0.85f, 1f, 0);
					NumberSprite.draw(num4, b, new Vector2(num3 + num + (i + 2) * 36 + 16 + ((num4 >= 10) ? 12 : 0), num2 + 12 + j * 56), (flag2 ? Color.LightGreen : Color.SandyBrown) * ((num4 == 0) ? 0.75f : 1f), 1f, 0.87f, 1f, 0);
				}
			}
			if ((i + 1) % 5 == 0)
			{
				num3 += 24;
			}
		}
		foreach (ClickableTextureComponent skillBar in skillBars)
		{
			skillBar.draw(b);
		}
		foreach (ClickableTextureComponent skillBar2 in skillBars)
		{
			if (skillBar2.scale == 0f)
			{
				IClickableMenu.drawTextureBox(b, skillBar2.bounds.X - 16 - 8, skillBar2.bounds.Y - 16 - 16, 96, 96, Color.White);
				b.Draw(Game1.mouseCursors, new Vector2(skillBar2.bounds.X - 8, skillBar2.bounds.Y - 32 + 16), new Rectangle(professionImage % 6 * 16, 624 + professionImage / 6 * 16, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
			}
		}
		num = xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 32 + 16;
		num2 = yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + 320 - 8;
		if ((int)Game1.netWorldState.Value.GoldenWalnuts > 0)
		{
			b.Draw(Game1.objectSpriteSheet, new Vector2(num, num2), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 73, 16, 16), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
			num += 48;
			b.DrawString(Game1.smallFont, Game1.netWorldState.Value.GoldenWalnuts?.ToString() ?? "", new Vector2(num, num2), Game1.textColor);
			num += 64;
		}
		if (Game1.player.QiGems > 0)
		{
			b.Draw(Game1.objectSpriteSheet, new Vector2(num, num2), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 858, 16, 16), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
			num += 48;
			b.DrawString(Game1.smallFont, Game1.player.QiGems.ToString() ?? "", new Vector2(num, num2), Game1.textColor);
			num += 64;
		}
		Game1.drawDialogueBox(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 32, yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + (int)((float)height / 2f) - 32, width - 64 - IClickableMenu.spaceToClearSideBorder * 2, height / 4 + 64, speaker: false, drawOnlyBox: true);
		drawBorderLabel(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:SkillsPage.cs.11610"), Game1.smallFont, xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 96, yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + (int)((float)height / 2f) - 32);
		foreach (ClickableTextureComponent specialItem in specialItems)
		{
			specialItem?.draw(b);
		}
		if (hoverText.Length > 0)
		{
			IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont, 0, 0, -1, (hoverTitle.Length > 0) ? hoverTitle : null);
		}
	}
}
