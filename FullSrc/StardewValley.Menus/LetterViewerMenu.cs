using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;

namespace StardewValley.Menus;

public class LetterViewerMenu : IClickableMenu
{
	public const int region_backButton = 101;

	public const int region_forwardButton = 102;

	public const int region_acceptQuestButton = 103;

	public const int region_itemGrabButton = 104;

	public const int letterWidth = 320;

	public const int letterHeight = 180;

	public Texture2D letterTexture;

	public Texture2D secretNoteImageTexture;

	public int moneyIncluded;

	public int questID = -1;

	public int secretNoteImage = -1;

	public int whichBG;

	public string learnedRecipe = "";

	public string cookingOrCrafting = "";

	public string mailTitle;

	public List<string> mailMessage = new List<string>();

	public int page;

	public List<ClickableComponent> itemsToGrab = new List<ClickableComponent>();

	public float scale;

	public bool isMail;

	public bool isFromCollection;

	public new bool destroy;

	public int customTextColor = -1;

	public bool usingCustomBackground;

	public ClickableTextureComponent backButton;

	public ClickableTextureComponent forwardButton;

	public ClickableComponent acceptQuestButton;

	public const float scaleChange = 0.003f;

	public LetterViewerMenu(string text)
		: base((int)Utility.getTopLeftPositionForCenteringOnScreen(1280, 720).X, (int)Utility.getTopLeftPositionForCenteringOnScreen(1280, 720).Y, 1280, 720, showUpperRightCloseButton: true)
	{
		Game1.playSound("shwip");
		backButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 32, yPositionOnScreen + height - 32 - 64, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f)
		{
			myID = 101,
			rightNeighborID = 102
		};
		forwardButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 32 - 48, yPositionOnScreen + height - 32 - 64, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f)
		{
			myID = 102,
			leftNeighborID = 101
		};
		letterTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\letterBG");
		text = ApplyCustomFormatting(text);
		mailMessage = SpriteText.getStringBrokenIntoSectionsOfHeight(text, width - 64, height - 128);
		forwardButton.visible = page < mailMessage.Count - 1;
		backButton.visible = page > 0;
		OnPageChange();
		populateClickableComponentList();
		if (Game1.options.SnappyMenus)
		{
			snapToDefaultClickableComponent();
		}
	}

	public LetterViewerMenu(int secretNoteIndex)
		: base((int)Utility.getTopLeftPositionForCenteringOnScreen(1280, 720).X, (int)Utility.getTopLeftPositionForCenteringOnScreen(1280, 720).Y, 1280, 720, showUpperRightCloseButton: true)
	{
		Game1.playSound("shwip");
		backButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 32, yPositionOnScreen + height - 32 - 64, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f)
		{
			myID = 101,
			rightNeighborID = 102
		};
		forwardButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 32 - 48, yPositionOnScreen + height - 32 - 64, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f)
		{
			myID = 102,
			leftNeighborID = 101
		};
		letterTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\letterBG");
		string text = Game1.content.Load<Dictionary<int, string>>("Data\\SecretNotes")[secretNoteIndex];
		if (text[0] == '!')
		{
			secretNoteImageTexture = Game1.temporaryContent.Load<Texture2D>("TileSheets\\SecretNotesImages");
			secretNoteImage = Convert.ToInt32(text.Split(' ')[1]);
		}
		else
		{
			whichBG = ((secretNoteIndex <= 1000) ? 1 : 0);
			string s = ApplyCustomFormatting(Utility.ParseGiftReveals(text.Replace("@", Game1.player.name)));
			mailMessage = SpriteText.getStringBrokenIntoSectionsOfHeight(s, width - 64, height - 128);
		}
		OnPageChange();
		forwardButton.visible = page < mailMessage.Count - 1;
		backButton.visible = page > 0;
		populateClickableComponentList();
		if (Game1.options.SnappyMenus)
		{
			snapToDefaultClickableComponent();
		}
	}

	public virtual void OnPageChange()
	{
		forwardButton.visible = page < mailMessage.Count - 1;
		backButton.visible = page > 0;
		foreach (ClickableComponent item in itemsToGrab)
		{
			item.visible = ShouldShowInteractable();
		}
		if (acceptQuestButton != null)
		{
			acceptQuestButton.visible = ShouldShowInteractable();
		}
		if (Game1.options.SnappyMenus && (currentlySnappedComponent == null || !currentlySnappedComponent.visible))
		{
			snapToDefaultClickableComponent();
		}
	}

	public LetterViewerMenu(string mail, string mailTitle, bool fromCollection = false)
		: base((int)Utility.getTopLeftPositionForCenteringOnScreen(1280, 720).X, (int)Utility.getTopLeftPositionForCenteringOnScreen(1280, 720).Y, 1280, 720, showUpperRightCloseButton: true)
	{
		isFromCollection = fromCollection;
		mail = mail.Split(new string[1] { "[#]" }, StringSplitOptions.None)[0];
		mail = mail.Replace("@", Game1.player.Name);
		if (mail.Contains("%update"))
		{
			mail = mail.Replace("%update", Utility.getStardewHeroStandingsString());
		}
		isMail = true;
		Game1.playSound("shwip");
		backButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 32, yPositionOnScreen + height - 32 - 64, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f)
		{
			myID = 101,
			rightNeighborID = 102
		};
		forwardButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 32 - 48, yPositionOnScreen + height - 32 - 64, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f)
		{
			myID = 102,
			leftNeighborID = 101
		};
		acceptQuestButton = new ClickableComponent(new Rectangle(xPositionOnScreen + width / 2 - 128, yPositionOnScreen + height - 128, (int)Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest")).X + 24, (int)Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest")).Y + 24), "")
		{
			myID = 103,
			rightNeighborID = 102,
			leftNeighborID = 101
		};
		this.mailTitle = mailTitle;
		letterTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\letterBG");
		if (mail.Contains("¦"))
		{
			mail = (Game1.player.IsMale ? mail.Substring(0, mail.IndexOf("¦")) : mail.Substring(mail.IndexOf("¦") + 1));
		}
		if (mailTitle.Equals("winter_5_2") || mailTitle.Equals("winter_12_1") || mailTitle.ToLower().Contains("wizard"))
		{
			whichBG = 2;
		}
		else if (mailTitle.Equals("Sandy"))
		{
			whichBG = 1;
		}
		else if (mailTitle.Contains("Krobus"))
		{
			whichBG = 3;
		}
		mail = ApplyCustomFormatting(mail);
		if (mail.Contains("%item"))
		{
			string text = mail.Substring(mail.IndexOf("%item"), mail.IndexOf("%%") + 2 - mail.IndexOf("%item"));
			string[] array = text.Split(' ');
			mail = mail.Replace(text, "");
			if (!isFromCollection)
			{
				if (array[1].Equals("object"))
				{
					int maxValue = array.Length - 1;
					int num = Game1.random.Next(2, maxValue);
					num -= num % 2;
					Object item = new Object(Vector2.Zero, Convert.ToInt32(array[num]), Convert.ToInt32(array[num + 1]));
					itemsToGrab.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + width / 2 - 48, yPositionOnScreen + height - 32 - 96, 96, 96), item)
					{
						myID = 104,
						leftNeighborID = 101,
						rightNeighborID = 102
					});
					backButton.rightNeighborID = 104;
					forwardButton.leftNeighborID = 104;
				}
				else if (array[1].Equals("tools"))
				{
					for (int i = 2; i < array.Length; i++)
					{
						Item item2 = null;
						switch (array[i])
						{
						case "Axe":
							item2 = new Axe();
							break;
						case "Hoe":
							item2 = new Hoe();
							break;
						case "Can":
							item2 = new WateringCan();
							break;
						case "Scythe":
							item2 = new MeleeWeapon(47);
							break;
						case "Pickaxe":
							item2 = new Pickaxe();
							break;
						}
						if (item2 != null)
						{
							itemsToGrab.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + width / 2 - 48, yPositionOnScreen + height - 32 - 96, 96, 96), item2));
						}
					}
				}
				else if (array[1].Equals("bigobject"))
				{
					int maxValue2 = array.Length - 1;
					int num2 = Game1.random.Next(2, maxValue2);
					Object item3 = new Object(Vector2.Zero, Convert.ToInt32(array[num2]));
					itemsToGrab.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + width / 2 - 48, yPositionOnScreen + height - 32 - 96, 96, 96), item3)
					{
						myID = 104,
						leftNeighborID = 101,
						rightNeighborID = 102
					});
					backButton.rightNeighborID = 104;
					forwardButton.leftNeighborID = 104;
				}
				else if (array[1].Equals("furniture"))
				{
					int maxValue3 = array.Length - 1;
					int num3 = Game1.random.Next(2, maxValue3);
					Item furnitureInstance = Furniture.GetFurnitureInstance(Convert.ToInt32(array[num3]), null);
					itemsToGrab.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + width / 2 - 48, yPositionOnScreen + height - 32 - 96, 96, 96), furnitureInstance)
					{
						myID = 104,
						leftNeighborID = 101,
						rightNeighborID = 102
					});
					backButton.rightNeighborID = 104;
					forwardButton.leftNeighborID = 104;
				}
				else if (array[1].Equals("money"))
				{
					int num4 = ((array.Length > 4) ? Game1.random.Next(Convert.ToInt32(array[2]), Convert.ToInt32(array[3])) : Convert.ToInt32(array[2]));
					num4 -= num4 % 10;
					Game1.player.Money += num4;
					moneyIncluded = num4;
				}
				else if (array[1].Equals("conversationTopic"))
				{
					string text2 = array[2];
					int value = Convert.ToInt32(array[3].Replace("%%", ""));
					Game1.player.activeDialogueEvents.Add(text2, value);
					if (text2.Equals("ElliottGone3"))
					{
						Utility.getHomeOfFarmer(Game1.player).fridge.Value.addItem(new Object(732, 1));
					}
				}
				else if (array[1].Equals("cookingRecipe"))
				{
					Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\CookingRecipes");
					int num5 = 1000;
					string text3 = "";
					foreach (string key in dictionary.Keys)
					{
						string[] array2 = dictionary[key].Split('/');
						string[] array3 = array2[3].Split(' ');
						if (!array3[0].Equals("f") || !array3[1].Equals(mailTitle.Replace("Cooking", "")) || Game1.player.cookingRecipes.ContainsKey(key))
						{
							continue;
						}
						int num6 = Convert.ToInt32(array3[2]);
						if (num6 <= num5)
						{
							num5 = num6;
							text3 = key;
							learnedRecipe = key;
							if (LocalizedContentManager.CurrentLanguageCode != 0)
							{
								learnedRecipe = array2[^1];
							}
						}
					}
					if (text3 != "")
					{
						if (!Game1.player.cookingRecipes.ContainsKey(text3))
						{
							Game1.player.cookingRecipes.Add(text3, 0);
						}
						cookingOrCrafting = Game1.content.LoadString("Strings\\UI:LearnedRecipe_cooking");
					}
				}
				else if (array[1].Equals("craftingRecipe"))
				{
					learnedRecipe = array[2].Replace('_', ' ');
					if (!Game1.player.craftingRecipes.ContainsKey(learnedRecipe))
					{
						Game1.player.craftingRecipes.Add(learnedRecipe, 0);
					}
					cookingOrCrafting = Game1.content.LoadString("Strings\\UI:LearnedRecipe_crafting");
					if (LocalizedContentManager.CurrentLanguageCode != 0)
					{
						Dictionary<string, string> dictionary2 = Game1.content.Load<Dictionary<string, string>>("Data\\CraftingRecipes");
						if (dictionary2.ContainsKey(learnedRecipe))
						{
							learnedRecipe = dictionary2[learnedRecipe].Split('/')[^1];
						}
					}
				}
				else if (array[1].Equals("itemRecovery"))
				{
					if (Game1.player.recoveredItem != null)
					{
						Item recoveredItem = Game1.player.recoveredItem;
						Game1.player.recoveredItem = null;
						itemsToGrab.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + width / 2 - 48, yPositionOnScreen + height - 32 - 96, 96, 96), recoveredItem)
						{
							myID = 104,
							leftNeighborID = 101,
							rightNeighborID = 102
						});
						backButton.rightNeighborID = 104;
						forwardButton.leftNeighborID = 104;
					}
				}
				else if (array[1].Equals("quest"))
				{
					questID = Convert.ToInt32(array[2].Replace("%%", ""));
					if (array.Length > 4)
					{
						if (!Game1.player.mailReceived.Contains("NOQUEST_" + questID))
						{
							Game1.player.addQuest(questID);
						}
						questID = -1;
					}
					backButton.rightNeighborID = 103;
					forwardButton.leftNeighborID = 103;
				}
			}
		}
		if (mailTitle == "ccBulletinThankYou" && !Game1.player.hasOrWillReceiveMail("ccBulletinThankYouReceived"))
		{
			foreach (NPC allCharacter in Utility.getAllCharacters())
			{
				if (!allCharacter.datable && allCharacter.isVillager())
				{
					Game1.player.changeFriendship(500, allCharacter);
				}
			}
			Game1.addMailForTomorrow("ccBulletinThankYouReceived", noLetter: true);
		}
		Random r = new Random((int)(Game1.uniqueIDForThisGame / 2) ^ Game1.year ^ (int)Game1.player.UniqueMultiplayerID);
		bool flag = fromCollection;
		if (Game1.currentSeason == "winter" && Game1.dayOfMonth >= 18 && Game1.dayOfMonth <= 25)
		{
			flag = false;
		}
		mail = mail.Replace("%secretsanta", flag ? "???" : Utility.getRandomTownNPC(r).displayName);
		int num7 = height - 128;
		if (HasInteractable())
		{
			num7 = height - 128 - 32;
		}
		mailMessage = SpriteText.getStringBrokenIntoSectionsOfHeight(mail, width - 64, num7);
		forwardButton.visible = page < mailMessage.Count - 1;
		backButton.visible = page > 0;
		if (Game1.options.SnappyMenus)
		{
			populateClickableComponentList();
			snapToDefaultClickableComponent();
			if (mailMessage != null && mailMessage.Count <= 1)
			{
				backButton.myID = -100;
				forwardButton.myID = -100;
			}
		}
	}

	public virtual string ApplyCustomFormatting(string text)
	{
		int num = -1;
		for (num = text.IndexOf("["); num >= 0; num = text.IndexOf("[", num + 1))
		{
			int num2 = text.IndexOf("]", num);
			if (num2 >= 0)
			{
				bool flag = false;
				try
				{
					string[] array = text.Substring(num + 1, num2 - num - 1).Split(' ');
					if (array[0] == "letterbg")
					{
						if (array.Length == 2)
						{
							whichBG = int.Parse(array[1]);
						}
						else if (array.Length == 3)
						{
							usingCustomBackground = true;
							letterTexture = Game1.temporaryContent.Load<Texture2D>(array[1]);
							whichBG = int.Parse(array[2]);
						}
						flag = true;
					}
					else if (array[0] == "textcolor")
					{
						string text2 = array[1].ToLower();
						string[] array2 = new string[9] { "black", "blue", "red", "purple", "white", "orange", "green", "cyan", "gray" };
						customTextColor = -1;
						for (int i = 0; i < array2.Length; i++)
						{
							if (text2 == array2[i])
							{
								customTextColor = i;
								break;
							}
						}
						flag = true;
					}
				}
				catch (Exception)
				{
				}
				if (flag)
				{
					text = text.Remove(num, num2 - num + 1);
					num--;
				}
			}
		}
		return text;
	}

	public override void snapToDefaultClickableComponent()
	{
		if (questID != -1 && ShouldShowInteractable())
		{
			currentlySnappedComponent = getComponentWithID(103);
		}
		else if (itemsToGrab != null && itemsToGrab.Count > 0 && ShouldShowInteractable())
		{
			currentlySnappedComponent = getComponentWithID(104);
		}
		else if (currentlySnappedComponent == null || (currentlySnappedComponent != backButton && currentlySnappedComponent != forwardButton))
		{
			currentlySnappedComponent = forwardButton;
		}
		snapCursorToCurrentSnappedComponent();
	}

	public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
	{
		xPositionOnScreen = (int)Utility.getTopLeftPositionForCenteringOnScreen(1280, 720).X;
		yPositionOnScreen = (int)Utility.getTopLeftPositionForCenteringOnScreen(1280, 720).Y;
		backButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 32, yPositionOnScreen + height - 32 - 64, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f)
		{
			myID = 101,
			rightNeighborID = 102
		};
		forwardButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 32 - 48, yPositionOnScreen + height - 32 - 64, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f)
		{
			myID = 102,
			leftNeighborID = 101
		};
		acceptQuestButton = new ClickableComponent(new Rectangle(xPositionOnScreen + width / 2 - 128, yPositionOnScreen + height - 128, (int)Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest")).X + 24, (int)Game1.dialogueFont.MeasureString(Game1.content.LoadString("Strings\\UI:AcceptQuest")).Y + 24), "")
		{
			myID = 103,
			rightNeighborID = 102,
			leftNeighborID = 101
		};
		foreach (ClickableComponent item in itemsToGrab)
		{
			item.bounds = new Rectangle(xPositionOnScreen + width / 2 - 48, yPositionOnScreen + height - 32 - 96, 96, 96);
		}
	}

	public override void receiveKeyPress(Keys key)
	{
		if (key != 0)
		{
			if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && readyToClose())
			{
				exitThisMenu(ShouldPlayExitSound());
			}
			else
			{
				base.receiveKeyPress(key);
			}
		}
	}

	public override void receiveGamePadButton(Buttons b)
	{
		base.receiveGamePadButton(b);
		if (isFromCollection && b == Buttons.B)
		{
			exitThisMenu(playSound: false);
		}
		else if (b == Buttons.LeftTrigger && page > 0)
		{
			page--;
			Game1.playSound("shwip");
			OnPageChange();
		}
		else if (b == Buttons.RightTrigger && page < mailMessage.Count - 1)
		{
			page++;
			Game1.playSound("shwip");
			OnPageChange();
		}
	}

	public override void receiveLeftClick(int x, int y, bool playSound = true)
	{
		if (scale < 1f)
		{
			return;
		}
		if (upperRightCloseButton != null && readyToClose() && upperRightCloseButton.containsPoint(x, y))
		{
			if (playSound)
			{
				Game1.playSound("bigDeSelect");
			}
			if (!isFromCollection)
			{
				exitThisMenu(ShouldPlayExitSound());
			}
			else
			{
				destroy = true;
			}
		}
		if (Game1.activeClickableMenu == null && Game1.currentMinigame == null)
		{
			unload();
			return;
		}
		if (ShouldShowInteractable())
		{
			foreach (ClickableComponent item in itemsToGrab)
			{
				if (item.containsPoint(x, y) && item.item != null)
				{
					Game1.playSound("coin");
					Game1.player.addItemByMenuIfNecessary(item.item);
					item.item = null;
					return;
				}
			}
		}
		if (backButton.containsPoint(x, y) && page > 0)
		{
			page--;
			Game1.playSound("shwip");
			OnPageChange();
		}
		else if (forwardButton.containsPoint(x, y) && page < mailMessage.Count - 1)
		{
			page++;
			Game1.playSound("shwip");
			OnPageChange();
		}
		else if (ShouldShowInteractable() && acceptQuestButton != null && acceptQuestButton.containsPoint(x, y))
		{
			AcceptQuest();
		}
		else if (isWithinBounds(x, y))
		{
			if (page < mailMessage.Count - 1)
			{
				page++;
				Game1.playSound("shwip");
				OnPageChange();
			}
			else if (!isMail)
			{
				exitThisMenuNoSound();
				Game1.playSound("shwip");
			}
			else if (isFromCollection)
			{
				destroy = true;
			}
		}
		else if (!itemsLeftToGrab())
		{
			if (!isFromCollection)
			{
				exitThisMenuNoSound();
				Game1.playSound("shwip");
			}
			else
			{
				destroy = true;
			}
		}
	}

	public virtual bool ShouldPlayExitSound()
	{
		if (questID != -1)
		{
			return false;
		}
		if (isFromCollection)
		{
			return false;
		}
		return true;
	}

	public bool itemsLeftToGrab()
	{
		if (itemsToGrab == null)
		{
			return false;
		}
		foreach (ClickableComponent item in itemsToGrab)
		{
			if (item.item != null)
			{
				return true;
			}
		}
		return false;
	}

	public void AcceptQuest()
	{
		if (questID != -1)
		{
			Game1.player.addQuest(questID);
			if (questID == 20)
			{
				MineShaft.CheckForQiChallengeCompletion();
			}
			questID = -1;
			Game1.playSound("newArtifact");
		}
	}

	public override void performHoverAction(int x, int y)
	{
		base.performHoverAction(x, y);
		if (ShouldShowInteractable())
		{
			foreach (ClickableComponent item in itemsToGrab)
			{
				if (item.containsPoint(x, y))
				{
					item.scale = Math.Min(item.scale + 0.03f, 1.1f);
				}
				else
				{
					item.scale = Math.Max(1f, item.scale - 0.03f);
				}
			}
		}
		backButton.tryHover(x, y, 0.6f);
		forwardButton.tryHover(x, y, 0.6f);
		if (ShouldShowInteractable() && questID != -1)
		{
			float num = acceptQuestButton.scale;
			acceptQuestButton.scale = (acceptQuestButton.bounds.Contains(x, y) ? 1.5f : 1f);
			if (acceptQuestButton.scale > num)
			{
				Game1.playSound("Cowboy_gunshot");
			}
		}
	}

	public override void update(GameTime time)
	{
		base.update(time);
		forwardButton.visible = page < mailMessage.Count - 1;
		backButton.visible = page > 0;
		if (scale < 1f)
		{
			scale += (float)time.ElapsedGameTime.Milliseconds * 0.003f;
			if (scale >= 1f)
			{
				scale = 1f;
			}
		}
		if (page < mailMessage.Count - 1 && !forwardButton.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
		{
			forwardButton.scale = 4f + (float)Math.Sin((double)(float)time.TotalGameTime.Milliseconds / (Math.PI * 64.0)) / 1.5f;
		}
	}

	public virtual int getTextColor()
	{
		if (customTextColor >= 0)
		{
			return customTextColor;
		}
		if (usingCustomBackground)
		{
			return -1;
		}
		return whichBG switch
		{
			1 => 8, 
			2 => 7, 
			3 => 4, 
			_ => -1, 
		};
	}

	public override void draw(SpriteBatch b)
	{
		b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
		b.Draw(letterTexture, new Vector2(xPositionOnScreen + width / 2, yPositionOnScreen + height / 2), new Rectangle(whichBG * 320, 0, 320, 180), Color.White, 0f, new Vector2(160f, 90f), 4f * scale, SpriteEffects.None, 0.86f);
		if (scale == 1f)
		{
			if (secretNoteImage != -1)
			{
				b.Draw(secretNoteImageTexture, new Vector2(xPositionOnScreen + width / 2 - 128 - 4, yPositionOnScreen + height / 2 - 128 + 8), new Rectangle(secretNoteImage * 64 % secretNoteImageTexture.Width, secretNoteImage * 64 / secretNoteImageTexture.Width * 64, 64, 64), Color.Black * 0.4f, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.865f);
				b.Draw(secretNoteImageTexture, new Vector2(xPositionOnScreen + width / 2 - 128, yPositionOnScreen + height / 2 - 128), new Rectangle(secretNoteImage * 64 % secretNoteImageTexture.Width, secretNoteImage * 64 / secretNoteImageTexture.Width * 64, 64, 64), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.865f);
				b.Draw(secretNoteImageTexture, new Vector2(xPositionOnScreen + width / 2 - 40, yPositionOnScreen + height / 2 - 192), new Rectangle(193, 65, 14, 21), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.867f);
			}
			else
			{
				SpriteText.drawString(b, mailMessage[page], xPositionOnScreen + 32, yPositionOnScreen + 32, 999999, width - 64, 999999, 0.75f, 0.865f, junimoText: false, -1, "", getTextColor());
			}
			if (ShouldShowInteractable())
			{
				foreach (ClickableComponent item in itemsToGrab)
				{
					b.Draw(letterTexture, item.bounds, new Rectangle(whichBG * 24, 180, 24, 24), Color.White);
					if (item.item != null)
					{
						item.item.drawInMenu(b, new Vector2(item.bounds.X + 16, item.bounds.Y + 16), item.scale);
					}
				}
				if (moneyIncluded > 0)
				{
					string s = Game1.content.LoadString("Strings\\UI:LetterViewer_MoneyIncluded", moneyIncluded);
					SpriteText.drawString(b, s, xPositionOnScreen + width / 2 - SpriteText.getWidthOfString(s) / 2, yPositionOnScreen + height - 96, 999999, -1, 9999, 0.75f, 0.865f);
				}
				else if (learnedRecipe != null && learnedRecipe.Length > 0)
				{
					string s2 = Game1.content.LoadString("Strings\\UI:LetterViewer_LearnedRecipe", cookingOrCrafting);
					SpriteText.drawStringHorizontallyCenteredAt(b, s2, xPositionOnScreen + width / 2, yPositionOnScreen + height - 32 - SpriteText.getHeightOfString(s2) * 2, 999999, -1, 9999, 0.65f, 0.865f, junimoText: false, getTextColor());
					SpriteText.drawStringHorizontallyCenteredAt(b, Game1.content.LoadString("Strings\\UI:LetterViewer_LearnedRecipeName", learnedRecipe), xPositionOnScreen + width / 2, yPositionOnScreen + height - 32 - SpriteText.getHeightOfString("t"), 999999, -1, 9999, 0.9f, 0.865f, junimoText: false, getTextColor());
				}
			}
			base.draw(b);
			forwardButton.draw(b);
			backButton.draw(b);
			if (ShouldShowInteractable() && questID != -1)
			{
				IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 373, 9, 9), acceptQuestButton.bounds.X, acceptQuestButton.bounds.Y, acceptQuestButton.bounds.Width, acceptQuestButton.bounds.Height, (acceptQuestButton.scale > 1f) ? Color.LightPink : Color.White, 4f * acceptQuestButton.scale);
				Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:AcceptQuest"), Game1.dialogueFont, new Vector2(acceptQuestButton.bounds.X + 12, acceptQuestButton.bounds.Y + (LocalizedContentManager.CurrentLanguageLatin ? 16 : 12)), Game1.textColor);
			}
		}
		if ((!Game1.options.SnappyMenus || !(scale < 1f)) && (!Game1.options.SnappyMenus || forwardButton.visible || backButton.visible || questID != -1 || itemsLeftToGrab()))
		{
			drawMouse(b);
		}
	}

	public virtual bool ShouldShowInteractable()
	{
		if (!HasInteractable())
		{
			return false;
		}
		return page == mailMessage.Count - 1;
	}

	public virtual bool HasInteractable()
	{
		if (isFromCollection)
		{
			return false;
		}
		if (questID != -1)
		{
			return true;
		}
		if (moneyIncluded > 0)
		{
			return true;
		}
		if (itemsToGrab.Count > 0)
		{
			return true;
		}
		if (learnedRecipe != null && learnedRecipe.Length > 0)
		{
			return true;
		}
		return false;
	}

	public void unload()
	{
	}

	protected override void cleanupBeforeExit()
	{
		if (questID != -1)
		{
			AcceptQuest();
		}
		if (itemsLeftToGrab())
		{
			List<Item> list = new List<Item>();
			foreach (ClickableComponent item in itemsToGrab)
			{
				if (item.item != null)
				{
					list.Add(item.item);
					item.item = null;
				}
			}
			if (list.Count > 0)
			{
				Game1.playSound("coin");
				Game1.player.addItemsByMenuIfNecessary(list);
			}
		}
		if (isFromCollection)
		{
			destroy = true;
			Game1.oldKBState = Game1.GetKeyboardState();
			Game1.oldMouseState = Game1.input.GetMouseState();
			Game1.oldPadState = Game1.input.GetGamePadState();
		}
		base.cleanupBeforeExit();
	}

	public override void receiveRightClick(int x, int y, bool playSound = true)
	{
		if (isFromCollection)
		{
			destroy = true;
		}
		else
		{
			receiveLeftClick(x, y, playSound);
		}
	}
}
