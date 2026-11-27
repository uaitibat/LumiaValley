using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.GameData;

namespace StardewValley.Menus;

public class LanguageSelectionMenu : IClickableMenu
{
	public new static int width = 500;

	public new static int height = 728;

	private Texture2D texture;

	protected int _currentPage;

	protected int _pageCount = 1;

	public List<ClickableComponent> languages = new List<ClickableComponent>();

	public List<ModLanguage> modLanguageData;

	public Dictionary<string, ModLanguage> modLanguageLookup = new Dictionary<string, ModLanguage>();

	public List<string> languageList = new List<string>();

	public ClickableTextureComponent nextPageButton;

	public ClickableTextureComponent previousPageButton;

	public LanguageSelectionMenu()
	{
		texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\LanguageButtons");
		List<ModLanguage> list = Game1.content.Load<List<ModLanguage>>("Data\\AdditionalLanguages");
		languageList.Clear();
		modLanguageLookup.Clear();
		languageList.AddRange(new string[12]
		{
			"English", "Russian", "Chinese", "German", "Portuguese", "French", "Spanish", "Japanese", "Korean", "Italian",
			"Turkish", "Hungarian"
		});
		if (list != null)
		{
			foreach (ModLanguage item in list)
			{
				languageList.Add("ModLanguage_" + item.ID);
				modLanguageLookup["ModLanguage_" + item.ID] = item;
			}
		}
		_pageCount = (int)Math.Floor((float)(languageList.Count - 1) / 12f) + 1;
		SetupButtons();
	}

	private void SetupButtons()
	{
		Vector2 topLeftPositionForCenteringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen((int)((float)width * 2.5f), height);
		languages.Clear();
		int num = 83;
		int num2 = 12 * _currentPage;
		if (num2 < languageList.Count)
		{
			languages.Add(new ClickableComponent(new Rectangle((int)topLeftPositionForCenteringOnScreen.X + 64, (int)topLeftPositionForCenteringOnScreen.Y + height - 30 - num * 6 - 16, width - 128, num), languageList[num2], null)
			{
				myID = 0,
				downNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				upNeighborID = -99998
			});
			num2++;
		}
		if (num2 < languageList.Count)
		{
			languages.Add(new ClickableComponent(new Rectangle((int)topLeftPositionForCenteringOnScreen.X + 448, (int)topLeftPositionForCenteringOnScreen.Y + height - 30 - num * 6 - 16, width - 128, num), languageList[num2], null)
			{
				myID = 3,
				downNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				upNeighborID = -99998
			});
			num2++;
		}
		if (num2 < languageList.Count)
		{
			languages.Add(new ClickableComponent(new Rectangle((int)topLeftPositionForCenteringOnScreen.X + 832, (int)topLeftPositionForCenteringOnScreen.Y + height - 30 - num * 6 - 16, width - 128, num), languageList[num2], null)
			{
				myID = 6,
				downNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				upNeighborID = -99998
			});
			num2++;
		}
		if (num2 < languageList.Count)
		{
			languages.Add(new ClickableComponent(new Rectangle((int)topLeftPositionForCenteringOnScreen.X + 64, (int)topLeftPositionForCenteringOnScreen.Y + height - 30 - num * 5 - 16, width - 128, num), languageList[num2], null)
			{
				myID = 1,
				downNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				upNeighborID = -99998
			});
			num2++;
		}
		if (num2 < languageList.Count)
		{
			languages.Add(new ClickableComponent(new Rectangle((int)topLeftPositionForCenteringOnScreen.X + 448, (int)topLeftPositionForCenteringOnScreen.Y + height - 30 - num * 5 - 16, width - 128, num), languageList[num2], null)
			{
				myID = 4,
				downNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				upNeighborID = -99998
			});
			num2++;
		}
		if (num2 < languageList.Count)
		{
			languages.Add(new ClickableComponent(new Rectangle((int)topLeftPositionForCenteringOnScreen.X + 832, (int)topLeftPositionForCenteringOnScreen.Y + height - 30 - num * 5 - 16, width - 128, num), languageList[num2], null)
			{
				myID = 7,
				downNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				upNeighborID = -99998
			});
			num2++;
		}
		if (num2 < languageList.Count)
		{
			languages.Add(new ClickableComponent(new Rectangle((int)topLeftPositionForCenteringOnScreen.X + 64, (int)topLeftPositionForCenteringOnScreen.Y + height - 30 - num * 4 - 16, width - 128, num), languageList[num2], null)
			{
				myID = 2,
				downNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				upNeighborID = -99998
			});
			num2++;
		}
		if (num2 < languageList.Count)
		{
			languages.Add(new ClickableComponent(new Rectangle((int)topLeftPositionForCenteringOnScreen.X + 448, (int)topLeftPositionForCenteringOnScreen.Y + height - 30 - num * 4 - 16, width - 128, num), languageList[num2], null)
			{
				myID = 5,
				downNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				upNeighborID = -99998
			});
			num2++;
		}
		if (num2 < languageList.Count)
		{
			languages.Add(new ClickableComponent(new Rectangle((int)topLeftPositionForCenteringOnScreen.X + 832, (int)topLeftPositionForCenteringOnScreen.Y + height - 30 - num * 4 - 16, width - 128, num), languageList[num2], null)
			{
				myID = 8,
				downNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				upNeighborID = -99998
			});
			num2++;
		}
		if (num2 < languageList.Count)
		{
			languages.Add(new ClickableComponent(new Rectangle((int)topLeftPositionForCenteringOnScreen.X + 64, (int)topLeftPositionForCenteringOnScreen.Y + height - 30 - num * 3 - 16, width - 128, num), languageList[num2], null)
			{
				myID = 9,
				downNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				upNeighborID = -99998
			});
			num2++;
		}
		if (num2 < languageList.Count)
		{
			languages.Add(new ClickableComponent(new Rectangle((int)topLeftPositionForCenteringOnScreen.X + 448, (int)topLeftPositionForCenteringOnScreen.Y + height - 30 - num * 3 - 16, width - 128, num), languageList[num2], null)
			{
				myID = 10,
				downNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				upNeighborID = -99998
			});
			num2++;
		}
		if (num2 < languageList.Count)
		{
			languages.Add(new ClickableComponent(new Rectangle((int)topLeftPositionForCenteringOnScreen.X + 832, (int)topLeftPositionForCenteringOnScreen.Y + height - 30 - num * 3 - 16, width - 128, num), languageList[num2], null)
			{
				myID = 11,
				downNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				upNeighborID = -99998
			});
			num2++;
		}
		previousPageButton = null;
		if (_currentPage > 0)
		{
			previousPageButton = new ClickableTextureComponent(new Rectangle((int)topLeftPositionForCenteringOnScreen.X + 4, (int)topLeftPositionForCenteringOnScreen.Y + height / 2 - 25, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f)
			{
				myID = 554,
				downNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				upNeighborID = -99998
			};
		}
		nextPageButton = null;
		if (_currentPage < _pageCount - 1)
		{
			nextPageButton = new ClickableTextureComponent(new Rectangle((int)(topLeftPositionForCenteringOnScreen.X + (float)width * 2.5f) - 32, (int)topLeftPositionForCenteringOnScreen.Y + height / 2 - 25, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f)
			{
				myID = 555,
				downNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				upNeighborID = -99998
			};
		}
		if (Game1.options.SnappyMenus)
		{
			int id = ((currentlySnappedComponent != null) ? currentlySnappedComponent.myID : 0);
			populateClickableComponentList();
			currentlySnappedComponent = getComponentWithID(id);
			snapCursorToCurrentSnappedComponent();
		}
	}

	public override void snapToDefaultClickableComponent()
	{
		currentlySnappedComponent = getComponentWithID(0);
		snapCursorToCurrentSnappedComponent();
	}

	public override void receiveLeftClick(int x, int y, bool playSound = true)
	{
		base.receiveLeftClick(x, y, playSound);
		if (nextPageButton != null && nextPageButton.containsPoint(x, y))
		{
			Game1.playSound("shwip");
			_currentPage++;
			SetupButtons();
			return;
		}
		if (previousPageButton != null && previousPageButton.containsPoint(x, y))
		{
			Game1.playSound("shwip");
			_currentPage--;
			SetupButtons();
			return;
		}
		foreach (ClickableComponent language in languages)
		{
			if (!language.containsPoint(x, y))
			{
				continue;
			}
			Game1.playSound("select");
			bool flag = true;
			if (language.name.StartsWith("ModLanguage_"))
			{
				flag = true;
				LocalizedContentManager.SetModLanguage(modLanguageLookup[language.name]);
			}
			else
			{
				switch (language.name)
				{
				case "English":
					LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.en;
					break;
				case "German":
					LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.de;
					break;
				case "Russian":
					LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.ru;
					break;
				case "Chinese":
					LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.zh;
					break;
				case "Japanese":
					LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.ja;
					break;
				case "Spanish":
					LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.es;
					break;
				case "Portuguese":
					LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.pt;
					break;
				case "French":
					LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.fr;
					break;
				case "Korean":
					LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.ko;
					break;
				case "Italian":
					LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.it;
					break;
				case "Turkish":
					LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.tr;
					break;
				case "Hungarian":
					LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.hu;
					break;
				default:
					flag = false;
					break;
				}
			}
			if (Game1.options.SnappyMenus)
			{
				Game1.activeClickableMenu.setCurrentlySnappedComponentTo(81118);
				Game1.activeClickableMenu.snapCursorToCurrentSnappedComponent();
			}
			if (flag)
			{
				ApplyLanguageChange();
				exitThisMenu();
			}
		}
		isWithinBounds(x, y);
	}

	public virtual void ApplyLanguageChange()
	{
	}

	public override void performHoverAction(int x, int y)
	{
		base.performHoverAction(x, y);
		foreach (ClickableComponent language in languages)
		{
			if (language.containsPoint(x, y))
			{
				if (language.label == null)
				{
					Game1.playSound("Cowboy_Footstep");
					language.label = "hovered";
				}
			}
			else
			{
				language.label = null;
			}
		}
		if (previousPageButton != null)
		{
			previousPageButton.tryHover(x, y);
		}
		if (nextPageButton != null)
		{
			nextPageButton.tryHover(x, y);
		}
	}

	public override void receiveRightClick(int x, int y, bool playSound = true)
	{
	}

	public override void draw(SpriteBatch b)
	{
		Vector2 topLeftPositionForCenteringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen((int)((float)width * 2.5f), height);
		b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.6f);
		IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(473, 36, 24, 24), (int)topLeftPositionForCenteringOnScreen.X + 32, (int)topLeftPositionForCenteringOnScreen.Y + 156, (int)((float)width * 2.55f) - 64, height / 2 + 25, Color.White, 4f);
		foreach (ClickableComponent language in languages)
		{
			int num = 0;
			switch (language.name)
			{
			case "English":
				num = 0;
				break;
			case "Spanish":
				num = 1;
				break;
			case "Portuguese":
				num = 2;
				break;
			case "Russian":
				num = 3;
				break;
			case "Chinese":
				num = 4;
				break;
			case "Japanese":
				num = 5;
				break;
			case "German":
				num = 6;
				break;
			case "French":
				num = 7;
				break;
			case "Korean":
				num = 8;
				break;
			case "Italian":
				num = 10;
				break;
			case "Turkish":
				num = 9;
				break;
			case "Hungarian":
				num = 11;
				break;
			}
			if (modLanguageLookup.ContainsKey(language.name))
			{
				ModLanguage modLanguage = modLanguageLookup[language.name];
				Texture2D texture2D = Game1.temporaryContent.Load<Texture2D>(modLanguage.ButtonTexture);
				Rectangle value = new Rectangle(0, 0, 174, 39);
				if (language.label != null)
				{
					value.Y += 39;
				}
				b.Draw(texture2D, language.bounds, value, Color.White, 0f, new Vector2(0f, 0f), SpriteEffects.None, 0f);
			}
			else
			{
				int num2 = ((num <= 6) ? (num * 78) : ((num - 7) * 78));
				num2 += ((language.label != null) ? 39 : 0);
				int x = ((num > 6) ? 174 : 0);
				b.Draw(texture, language.bounds, new Rectangle(x, num2, 174, 40), Color.White, 0f, new Vector2(0f, 0f), SpriteEffects.None, 0f);
			}
		}
		if (previousPageButton != null)
		{
			previousPageButton.draw(b);
		}
		if (nextPageButton != null)
		{
			nextPageButton.draw(b);
		}
		if (Game1.activeClickableMenu == this)
		{
			drawMouse(b);
		}
	}

	public override bool readyToClose()
	{
		return true;
	}

	public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
	{
		base.gameWindowSizeChanged(oldBounds, newBounds);
		SetupButtons();
	}
}
