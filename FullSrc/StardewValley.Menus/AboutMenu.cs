using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StardewValley.Menus;

public class AboutMenu : IClickableMenu
{
	public const int region_linkToTwitter = 91111;

	public const int region_linkToSVSite = 92222;

	public const int region_linkToChucklefish = 93333;

	public const int region_upArrow = 94444;

	public const int region_downArrow = 95555;

	public const int minWidth = 950;

	public const int maxWidth = 1200;

	public new const int height = 700;

	public ClickableComponent backButton;

	public ClickableTextureComponent upButton;

	public ClickableTextureComponent downButton;

	public List<ICreditsBlock> credits = new List<ICreditsBlock>();

	private int currentCreditsIndex;

	public AboutMenu()
	{
		width = 1280;
		base.height = 700;
		SetUpCredits();
		if (Game1.options.snappyMenus && Game1.options.gamepadControls)
		{
			populateClickableComponentList();
			snapToDefaultClickableComponent();
		}
	}

	public void SetUpCredits()
	{
		foreach (string item in Game1.temporaryContent.Load<List<string>>("Strings\\credits"))
		{
			if (item != "" && item.Length >= 6 && item.Substring(0, 6) == "[image")
			{
				string[] array = item.Split(' ');
				string assetName = array[1];
				int x = Convert.ToInt32(array[2]);
				int y = Convert.ToInt32(array[3]);
				int num = Convert.ToInt32(array[4]);
				int num2 = Convert.ToInt32(array[5]);
				int pixelZoom = Convert.ToInt32(array[6]);
				int animationFrames = ((array.Count() <= 7) ? 1 : Convert.ToInt32(array[7]));
				Texture2D texture2D = null;
				try
				{
					texture2D = Game1.temporaryContent.Load<Texture2D>(assetName);
				}
				catch (Exception)
				{
				}
				if (texture2D != null)
				{
					if (num == -1)
					{
						num = texture2D.Width;
						num2 = texture2D.Height;
					}
					credits.Add(new ImageCreditsBlock(texture2D, new Rectangle(x, y, num, num2), pixelZoom, animationFrames));
				}
			}
			else if (item != "" && item.Length >= 6 && item.Substring(0, 5) == "[link")
			{
				string url = item.Split(' ')[1];
				string text = item.Substring(item.IndexOf(' ') + 1);
				text = text.Substring(text.IndexOf(' ') + 1);
				credits.Add(new LinkCreditsBlock(text, url));
			}
			else
			{
				credits.Add(new TextCreditsBlock(item));
			}
		}
		if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.de && LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.es)
		{
			_ = LocalizedContentManager.CurrentLanguageCode;
			_ = 4;
		}
		Vector2 topLeftPositionForCenteringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(width, 700);
		xPositionOnScreen = (int)topLeftPositionForCenteringOnScreen.X;
		yPositionOnScreen = (int)topLeftPositionForCenteringOnScreen.Y;
		upButton = new ClickableTextureComponent(new Rectangle((int)topLeftPositionForCenteringOnScreen.X + width - 80, (int)topLeftPositionForCenteringOnScreen.Y + 64 + 16, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 12), 0.8f)
		{
			myID = 94444,
			downNeighborID = 95555,
			rightNeighborID = -99998,
			leftNeighborID = -99998
		};
		downButton = new ClickableTextureComponent(new Rectangle((int)topLeftPositionForCenteringOnScreen.X + width - 80, (int)topLeftPositionForCenteringOnScreen.Y + 700 - 32, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 11), 0.8f)
		{
			myID = 95555,
			upNeighborID = -99998,
			rightNeighborID = -99998,
			leftNeighborID = -99998
		};
		backButton = new ClickableComponent(new Rectangle(Game1.uiViewport.Width + -198 - 48, Game1.uiViewport.Height - 81 - 24, 198, 81), "")
		{
			myID = 81114,
			leftNeighborID = -99998,
			rightNeighborID = -99998,
			upNeighborID = 95555
		};
	}

	public override void snapToDefaultClickableComponent()
	{
		currentlySnappedComponent = getComponentWithID(81114);
		snapCursorToCurrentSnappedComponent();
	}

	private static void LaunchBrowser(string url)
	{
		Game1.playSound("bigSelect");
		try
		{
			Process.Start(url);
		}
		catch (Exception)
		{
		}
	}

	public override void receiveLeftClick(int x, int y, bool playSound = true)
	{
		base.receiveLeftClick(x, y, playSound);
		if (upButton.containsPoint(x, y))
		{
			if (currentCreditsIndex > 0)
			{
				currentCreditsIndex--;
				Game1.playSound("shiny4");
				upButton.scale = upButton.baseScale;
			}
		}
		else if (downButton.containsPoint(x, y))
		{
			if (currentCreditsIndex < credits.Count - 1)
			{
				currentCreditsIndex++;
				Game1.playSound("shiny4");
				downButton.scale = downButton.baseScale;
			}
		}
		else
		{
			if (!isWithinBounds(x, y))
			{
				return;
			}
			int num = yPositionOnScreen + 96;
			int num2 = num;
			int num3 = 0;
			while (num < yPositionOnScreen + 700 - 64 && credits.Count() > currentCreditsIndex + num3)
			{
				num += credits[currentCreditsIndex + num3].getHeight(width - 64) + ((credits.Count() <= currentCreditsIndex + num3 + 1 || !(credits[currentCreditsIndex + num3 + 1] is ImageCreditsBlock)) ? 8 : 0);
				if (y >= num2 && y < num)
				{
					credits[currentCreditsIndex + num3].clicked();
					break;
				}
				num3++;
				num2 = num;
			}
		}
	}

	public override void update(GameTime time)
	{
		base.update(time);
		upButton.visible = currentCreditsIndex > 0;
		downButton.visible = currentCreditsIndex < credits.Count() - 1;
	}

	public override void receiveScrollWheelAction(int direction)
	{
		if (direction > 0 && currentCreditsIndex > 0)
		{
			currentCreditsIndex--;
			Game1.playSound("shiny4");
		}
		else if (direction < 0 && currentCreditsIndex < credits.Count - 1)
		{
			currentCreditsIndex++;
			Game1.playSound("shiny4");
		}
	}

	public override void performHoverAction(int x, int y)
	{
		base.performHoverAction(x, y);
		upButton.tryHover(x, y);
		downButton.tryHover(x, y);
		if (!isWithinBounds(x, y))
		{
			return;
		}
		int num = yPositionOnScreen + 96;
		int num2 = num;
		int num3 = 0;
		while (num < yPositionOnScreen + 700 - 64 && credits.Count() > currentCreditsIndex + num3)
		{
			num += credits[currentCreditsIndex + num3].getHeight(width - 64) + ((credits.Count() <= currentCreditsIndex + num3 + 1 || !(credits[currentCreditsIndex + num3 + 1] is ImageCreditsBlock)) ? 8 : 0);
			if (y >= num2 && y < num)
			{
				credits[currentCreditsIndex + num3].hovered();
				break;
			}
			num3++;
			num2 = num;
		}
	}

	public override void receiveRightClick(int x, int y, bool playSound = true)
	{
	}

	public override void draw(SpriteBatch b)
	{
		Vector2 topLeftPositionForCenteringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(width, 600);
		b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
		IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(473, 36, 24, 24), (int)topLeftPositionForCenteringOnScreen.X, (int)topLeftPositionForCenteringOnScreen.Y, width, 700, Color.White, 4f, drawShadow: false);
		int num = yPositionOnScreen + 96;
		int num2 = 0;
		while (num < yPositionOnScreen + 700 - 64 && credits.Count() > currentCreditsIndex + num2)
		{
			credits[currentCreditsIndex + num2].draw(xPositionOnScreen + 32, num, width - 64, b);
			num += credits[currentCreditsIndex + num2].getHeight(width - 64) + ((credits.Count() <= currentCreditsIndex + num2 + 1 || !(credits[currentCreditsIndex + num2 + 1] is ImageCreditsBlock)) ? 8 : 0);
			num2++;
		}
		if (currentCreditsIndex > 0)
		{
			upButton.draw(b);
		}
		if (currentCreditsIndex < credits.Count() - 1)
		{
			downButton.draw(b);
		}
		if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is TitleMenu && (Game1.activeClickableMenu as TitleMenu).startupMessage.Length > 0)
		{
			b.DrawString(Game1.smallFont, Game1.parseText((Game1.activeClickableMenu as TitleMenu).startupMessage, Game1.smallFont, 640), new Vector2(8f, (float)Game1.uiViewport.Height - Game1.smallFont.MeasureString(Game1.parseText((Game1.activeClickableMenu as TitleMenu).startupMessage, Game1.smallFont, 640)).Y - 4f), Color.White);
		}
		else
		{
			b.DrawString(Game1.smallFont, "v" + Game1.GetVersionString(), new Vector2(16f, (float)Game1.uiViewport.Height - Game1.smallFont.MeasureString("v" + Game1.version).Y - 8f), Color.White);
		}
	}

	public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
	{
		base.gameWindowSizeChanged(oldBounds, newBounds);
		SetUpCredits();
		if (Game1.options.snappyMenus && Game1.options.gamepadControls)
		{
			int id = ((currentlySnappedComponent != null) ? currentlySnappedComponent.myID : 81114);
			populateClickableComponentList();
			currentlySnappedComponent = getComponentWithID(id);
			snapCursorToCurrentSnappedComponent();
		}
	}
}
