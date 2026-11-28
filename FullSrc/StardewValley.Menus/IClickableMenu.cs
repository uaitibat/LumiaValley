using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Tools;

namespace StardewValley.Menus;

[InstanceStatics]
public abstract class IClickableMenu
{
	public delegate void onExit();

	protected IClickableMenu _childMenu;

	protected IClickableMenu _parentMenu;

	public const int currency_g = 0;

	public const int currency_starTokens = 1;

	public const int currency_qiCoins = 2;

	public const int currency_qiGems = 4;

	public const int greyedOutSpotIndex = 57;

	public const int outerBorderWithUpArrow = 61;

	public const int lvlMarkerRedIndex = 54;

	public const int lvlMarkerGreyIndex = 55;

	public const int borderWithDownArrowIndex = 46;

	public const int borderWithUpArrowIndex = 47;

	public const int littleHeartIndex = 49;

	public const int uncheckedBoxIndex = 50;

	public const int checkedBoxIndex = 51;

	public const int presentIconIndex = 58;

	public const int itemSpotIndex = 10;

	public static int borderWidth = 40;

	public static int tabYPositionRelativeToMenuY = -48;

	public static int spaceToClearTopBorder = 96;

	public static int spaceToClearSideBorder = 16;

	public const int spaceBetweenTabs = 4;

	public int width;

	public int height;

	public int xPositionOnScreen;

	public int yPositionOnScreen;

	public int currentRegion;

	public Action<IClickableMenu> behaviorBeforeCleanup;

	public onExit exitFunction;

	public ClickableTextureComponent upperRightCloseButton;

	public bool destroy;

	public bool gamePadControlsImplemented;

	protected int _dependencies;

	public List<ClickableComponent> allClickableComponents;

	public ClickableComponent currentlySnappedComponent;

	public IClickableMenu()
	{
	}

	public IClickableMenu(int x, int y, int width, int height, bool showUpperRightCloseButton = false)
	{
		Game1.mouseCursorTransparency = 1f;
		initialize(x, y, width, height, showUpperRightCloseButton);
		if (Game1.gameMode == 3 && Game1.player != null && !Game1.eventUp)
		{
			Game1.player.Halt();
		}
	}

	public void initialize(int x, int y, int width, int height, bool showUpperRightCloseButton = false)
	{
		if (Game1.player != null && !Game1.player.UsingTool && !Game1.eventUp)
		{
			Game1.player.forceCanMove();
		}
		xPositionOnScreen = x;
		yPositionOnScreen = y;
		this.width = width;
		this.height = height;
		if (showUpperRightCloseButton)
		{
			upperRightCloseButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 36, yPositionOnScreen - 8, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
		}
		for (int i = 0; i < 4; i++)
		{
			Game1.directionKeyPolling[i] = 250;
		}
	}

	public virtual bool HasFocus()
	{
		return Game1.activeClickableMenu == this;
	}

	public IClickableMenu GetChildMenu()
	{
		return _childMenu;
	}

	public IClickableMenu GetParentMenu()
	{
		return _parentMenu;
	}

	public void SetChildMenu(IClickableMenu menu)
	{
		_childMenu = menu;
		if (_childMenu != null)
		{
			_childMenu._parentMenu = this;
		}
	}

	public void AddDependency()
	{
		_dependencies++;
	}

	public void RemoveDependency()
	{
		_dependencies--;
		if (_dependencies <= 0 && Game1.activeClickableMenu != this && TitleMenu.subMenu != this && this is IDisposable)
		{
			(this as IDisposable).Dispose();
		}
	}

	public bool HasDependencies()
	{
		return _dependencies > 0;
	}

	public virtual bool areGamePadControlsImplemented()
	{
		return false;
	}

	public ClickableComponent getLastClickableComponentInThisListThatContainsThisXCoord(List<ClickableComponent> ccList, int xCoord)
	{
		for (int num = ccList.Count - 1; num >= 0; num--)
		{
			if (ccList[num].bounds.Contains(xCoord, ccList[num].bounds.Center.Y))
			{
				return ccList[num];
			}
		}
		return null;
	}

	public ClickableComponent getFirstClickableComponentInThisListThatContainsThisXCoord(List<ClickableComponent> ccList, int xCoord)
	{
		for (int i = 0; i < ccList.Count; i++)
		{
			if (ccList[i].bounds.Contains(xCoord, ccList[i].bounds.Center.Y))
			{
				return ccList[i];
			}
		}
		return null;
	}

	public ClickableComponent getLastClickableComponentInThisListThatContainsThisYCoord(List<ClickableComponent> ccList, int yCoord)
	{
		for (int num = ccList.Count - 1; num >= 0; num--)
		{
			if (ccList[num].bounds.Contains(ccList[num].bounds.Center.X, yCoord))
			{
				return ccList[num];
			}
		}
		return null;
	}

	public ClickableComponent getFirstClickableComponentInThisListThatContainsThisYCoord(List<ClickableComponent> ccList, int yCoord)
	{
		for (int i = 0; i < ccList.Count; i++)
		{
			if (ccList[i].bounds.Contains(ccList[i].bounds.Center.X, yCoord))
			{
				return ccList[i];
			}
		}
		return null;
	}

	public virtual void receiveGamePadButton(Buttons b)
	{
		if (Game1.options.snappyMenus && Game1.options.gamepadControls && b != Buttons.A)
		{
			_ = 16384;
		}
	}

	public void drawMouse(SpriteBatch b, bool ignore_transparency = false, int cursor = -1)
	{
		if (!Game1.options.hardwareCursor)
		{
			float num = Game1.mouseCursorTransparency;
			if (ignore_transparency)
			{
				num = 1f;
			}
			if (cursor < 0)
			{
				cursor = ((Game1.options.snappyMenus && Game1.options.gamepadControls) ? 44 : 0);
			}
			b.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, cursor, 16, 16), Color.White * num, 0f, Vector2.Zero, 4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
		}
	}

	public void populateClickableComponentList()
	{
		allClickableComponents = new List<ClickableComponent>();
		FieldInfo[] fields = GetType().GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			if (fieldInfo.GetCustomAttributes(typeof(SkipForClickableAggregation), inherit: true).Count() != 0 || fieldInfo.DeclaringType == typeof(IClickableMenu))
			{
				continue;
			}
			if (fieldInfo.FieldType.IsSubclassOf(typeof(ClickableComponent)) || fieldInfo.FieldType == typeof(ClickableComponent))
			{
				if (fieldInfo.GetValue(this) != null)
				{
					allClickableComponents.Add((ClickableComponent)fieldInfo.GetValue(this));
				}
			}
			else if (fieldInfo.FieldType == typeof(List<ClickableComponent>))
			{
				List<ClickableComponent> list = (List<ClickableComponent>)fieldInfo.GetValue(this);
				if (list == null)
				{
					continue;
				}
				for (int num = list.Count - 1; num >= 0; num--)
				{
					if (list[num] != null)
					{
						allClickableComponents.Add(list[num]);
					}
				}
			}
			else if (fieldInfo.FieldType == typeof(List<ClickableTextureComponent>))
			{
				List<ClickableTextureComponent> list2 = (List<ClickableTextureComponent>)fieldInfo.GetValue(this);
				if (list2 == null)
				{
					continue;
				}
				for (int num2 = list2.Count - 1; num2 >= 0; num2--)
				{
					if (list2[num2] != null)
					{
						allClickableComponents.Add(list2[num2]);
					}
				}
			}
			else if (fieldInfo.FieldType == typeof(List<ClickableAnimatedComponent>))
			{
				List<ClickableAnimatedComponent> list3 = (List<ClickableAnimatedComponent>)fieldInfo.GetValue(this);
				for (int num3 = list3.Count - 1; num3 >= 0; num3--)
				{
					if (list3[num3] != null)
					{
						allClickableComponents.Add(list3[num3]);
					}
				}
			}
			else if (fieldInfo.FieldType == typeof(List<Bundle>))
			{
				List<Bundle> list4 = (List<Bundle>)fieldInfo.GetValue(this);
				for (int num4 = list4.Count - 1; num4 >= 0; num4--)
				{
					if (list4[num4] != null)
					{
						allClickableComponents.Add(list4[num4]);
					}
				}
			}
			else if (fieldInfo.FieldType == typeof(InventoryMenu))
			{
				allClickableComponents.AddRange(((InventoryMenu)fieldInfo.GetValue(this)).inventory);
				allClickableComponents.Add(((InventoryMenu)fieldInfo.GetValue(this)).dropItemInvisibleButton);
			}
			else if (fieldInfo.FieldType == typeof(List<Dictionary<ClickableTextureComponent, CraftingRecipe>>))
			{
				foreach (Dictionary<ClickableTextureComponent, CraftingRecipe> item in (List<Dictionary<ClickableTextureComponent, CraftingRecipe>>)fieldInfo.GetValue(this))
				{
					allClickableComponents.AddRange(item.Keys);
				}
			}
			else if (fieldInfo.FieldType == typeof(Dictionary<int, List<List<ClickableTextureComponent>>>))
			{
				foreach (List<List<ClickableTextureComponent>> value in ((Dictionary<int, List<List<ClickableTextureComponent>>>)fieldInfo.GetValue(this)).Values)
				{
					foreach (List<ClickableTextureComponent> item2 in value)
					{
						allClickableComponents.AddRange(item2);
					}
				}
			}
			else
			{
				if (!(fieldInfo.FieldType == typeof(Dictionary<int, ClickableTextureComponent>)))
				{
					continue;
				}
				foreach (ClickableTextureComponent value2 in ((Dictionary<int, ClickableTextureComponent>)fieldInfo.GetValue(this)).Values)
				{
					allClickableComponents.Add(value2);
				}
			}
		}
		if (Game1.activeClickableMenu is GameMenu gameMenu && this == gameMenu.GetCurrentPage())
		{
			gameMenu.AddTabsToClickableComponents(this);
		}
		if (upperRightCloseButton != null)
		{
			allClickableComponents.Add(upperRightCloseButton);
		}
	}

	public virtual void applyMovementKey(int direction)
	{
		if (allClickableComponents == null)
		{
			populateClickableComponentList();
		}
		moveCursorInDirection(direction);
	}

	/// <summary>
	/// return true if this method is overriden and a default clickablecomponent is snapped to.
	/// </summary>
	/// <returns></returns>
	public virtual void snapToDefaultClickableComponent()
	{
	}

	public void applyMovementKey(Keys key)
	{
		if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
		{
			applyMovementKey(0);
		}
		else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
		{
			applyMovementKey(1);
		}
		else if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
		{
			applyMovementKey(2);
		}
		else if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
		{
			applyMovementKey(3);
		}
	}

	/// <summary>
	/// Only use this if the child class overrides
	/// </summary>
	/// <param name="id"></param>
	public virtual void setCurrentlySnappedComponentTo(int id)
	{
		currentlySnappedComponent = getComponentWithID(id);
	}

	public void moveCursorInDirection(int direction)
	{
		if (currentlySnappedComponent == null && allClickableComponents != null && allClickableComponents.Count() > 0)
		{
			snapToDefaultClickableComponent();
			if (currentlySnappedComponent == null)
			{
				currentlySnappedComponent = allClickableComponents.First();
			}
		}
		if (currentlySnappedComponent == null)
		{
			return;
		}
		ClickableComponent clickableComponent = currentlySnappedComponent;
		switch (direction)
		{
		case 0:
			if (currentlySnappedComponent.upNeighborID == -99999)
			{
				snapToDefaultClickableComponent();
			}
			else if (currentlySnappedComponent.upNeighborID == -99998)
			{
				automaticSnapBehavior(0, currentlySnappedComponent.region, currentlySnappedComponent.myID);
			}
			else if (currentlySnappedComponent.upNeighborID == -7777)
			{
				customSnapBehavior(0, currentlySnappedComponent.region, currentlySnappedComponent.myID);
			}
			else
			{
				currentlySnappedComponent = getComponentWithID(currentlySnappedComponent.upNeighborID);
			}
			if (currentlySnappedComponent != null && (clickableComponent == null || (clickableComponent.upNeighborID != -7777 && clickableComponent.upNeighborID != -99998)) && !currentlySnappedComponent.downNeighborImmutable && !currentlySnappedComponent.fullyImmutable)
			{
				currentlySnappedComponent.downNeighborID = clickableComponent.myID;
			}
			if (currentlySnappedComponent == null)
			{
				noSnappedComponentFound(0, clickableComponent.region, clickableComponent.myID);
			}
			break;
		case 1:
			if (currentlySnappedComponent.rightNeighborID == -99999)
			{
				snapToDefaultClickableComponent();
			}
			else if (currentlySnappedComponent.rightNeighborID == -99998)
			{
				automaticSnapBehavior(1, currentlySnappedComponent.region, currentlySnappedComponent.myID);
			}
			else if (currentlySnappedComponent.rightNeighborID == -7777)
			{
				customSnapBehavior(1, currentlySnappedComponent.region, currentlySnappedComponent.myID);
			}
			else
			{
				currentlySnappedComponent = getComponentWithID(currentlySnappedComponent.rightNeighborID);
			}
			if (currentlySnappedComponent != null && (clickableComponent == null || (clickableComponent.rightNeighborID != -7777 && clickableComponent.rightNeighborID != -99998)) && !currentlySnappedComponent.leftNeighborImmutable && !currentlySnappedComponent.fullyImmutable)
			{
				currentlySnappedComponent.leftNeighborID = clickableComponent.myID;
			}
			if (currentlySnappedComponent == null && clickableComponent.tryDefaultIfNoRightNeighborExists)
			{
				snapToDefaultClickableComponent();
			}
			else if (currentlySnappedComponent == null)
			{
				noSnappedComponentFound(1, clickableComponent.region, clickableComponent.myID);
			}
			break;
		case 2:
			if (currentlySnappedComponent.downNeighborID == -99999)
			{
				snapToDefaultClickableComponent();
			}
			else if (currentlySnappedComponent.downNeighborID == -99998)
			{
				automaticSnapBehavior(2, currentlySnappedComponent.region, currentlySnappedComponent.myID);
			}
			else if (currentlySnappedComponent.downNeighborID == -7777)
			{
				customSnapBehavior(2, currentlySnappedComponent.region, currentlySnappedComponent.myID);
			}
			else
			{
				currentlySnappedComponent = getComponentWithID(currentlySnappedComponent.downNeighborID);
			}
			if (currentlySnappedComponent != null && (clickableComponent == null || (clickableComponent.downNeighborID != -7777 && clickableComponent.downNeighborID != -99998)) && !currentlySnappedComponent.upNeighborImmutable && !currentlySnappedComponent.fullyImmutable)
			{
				currentlySnappedComponent.upNeighborID = clickableComponent.myID;
			}
			if (currentlySnappedComponent == null && clickableComponent.tryDefaultIfNoDownNeighborExists)
			{
				snapToDefaultClickableComponent();
			}
			else if (currentlySnappedComponent == null)
			{
				noSnappedComponentFound(2, clickableComponent.region, clickableComponent.myID);
			}
			break;
		case 3:
			if (currentlySnappedComponent.leftNeighborID == -99999)
			{
				snapToDefaultClickableComponent();
			}
			else if (currentlySnappedComponent.leftNeighborID == -99998)
			{
				automaticSnapBehavior(3, currentlySnappedComponent.region, currentlySnappedComponent.myID);
			}
			else if (currentlySnappedComponent.leftNeighborID == -7777)
			{
				customSnapBehavior(3, currentlySnappedComponent.region, currentlySnappedComponent.myID);
			}
			else
			{
				currentlySnappedComponent = getComponentWithID(currentlySnappedComponent.leftNeighborID);
			}
			if (currentlySnappedComponent != null && (clickableComponent == null || (clickableComponent.leftNeighborID != -7777 && clickableComponent.leftNeighborID != -99998)) && !currentlySnappedComponent.rightNeighborImmutable && !currentlySnappedComponent.fullyImmutable)
			{
				currentlySnappedComponent.rightNeighborID = clickableComponent.myID;
			}
			if (currentlySnappedComponent == null)
			{
				noSnappedComponentFound(3, clickableComponent.region, clickableComponent.myID);
			}
			break;
		}
		if (currentlySnappedComponent != null && clickableComponent != null && currentlySnappedComponent.region != clickableComponent.region)
		{
			actionOnRegionChange(clickableComponent.region, currentlySnappedComponent.region);
		}
		if (currentlySnappedComponent == null)
		{
			currentlySnappedComponent = clickableComponent;
		}
		snapCursorToCurrentSnappedComponent();
		if (currentlySnappedComponent != clickableComponent)
		{
			Game1.playSound("shiny4");
		}
	}

	public virtual void snapCursorToCurrentSnappedComponent()
	{
		if (currentlySnappedComponent != null)
		{
			Game1.setMousePosition(currentlySnappedComponent.bounds.Right - currentlySnappedComponent.bounds.Width / 4, currentlySnappedComponent.bounds.Bottom - currentlySnappedComponent.bounds.Height / 4, ui_scale: true);
		}
	}

	protected virtual void noSnappedComponentFound(int direction, int oldRegion, int oldID)
	{
	}

	protected virtual void customSnapBehavior(int direction, int oldRegion, int oldID)
	{
	}

	public virtual bool IsActive()
	{
		if (_parentMenu == null)
		{
			return this == Game1.activeClickableMenu;
		}
		IClickableMenu parentMenu = _parentMenu;
		while (parentMenu != null && parentMenu._parentMenu != null)
		{
			parentMenu = parentMenu._parentMenu;
		}
		return parentMenu == Game1.activeClickableMenu;
	}

	public virtual void automaticSnapBehavior(int direction, int oldRegion, int oldID)
	{
		if (currentlySnappedComponent == null)
		{
			snapToDefaultClickableComponent();
			return;
		}
		Vector2 zero = Vector2.Zero;
		switch (direction)
		{
		case 3:
			zero.X = -1f;
			zero.Y = 0f;
			break;
		case 1:
			zero.X = 1f;
			zero.Y = 0f;
			break;
		case 0:
			zero.X = 0f;
			zero.Y = -1f;
			break;
		case 2:
			zero.X = 0f;
			zero.Y = 1f;
			break;
		}
		float num = -1f;
		ClickableComponent clickableComponent = null;
		for (int i = 0; i < allClickableComponents.Count; i++)
		{
			ClickableComponent clickableComponent2 = allClickableComponents[i];
			if ((clickableComponent2.leftNeighborID == -1 && clickableComponent2.rightNeighborID == -1 && clickableComponent2.upNeighborID == -1 && clickableComponent2.downNeighborID == -1) || clickableComponent2.myID == -500 || !IsAutomaticSnapValid(direction, currentlySnappedComponent, clickableComponent2) || !clickableComponent2.visible || clickableComponent2 == upperRightCloseButton || clickableComponent2 == currentlySnappedComponent)
			{
				continue;
			}
			Vector2 value = new Vector2(clickableComponent2.bounds.Center.X - currentlySnappedComponent.bounds.Center.X, clickableComponent2.bounds.Center.Y - currentlySnappedComponent.bounds.Center.Y);
			Vector2 value2 = new Vector2(value.X, value.Y);
			value2.Normalize();
			float num2 = Vector2.Dot(zero, value2);
			if (!(num2 > 0.01f))
			{
				continue;
			}
			float num3 = Vector2.DistanceSquared(Vector2.Zero, value);
			bool flag = false;
			switch (direction)
			{
			case 0:
			case 2:
				if (Math.Abs(value.X) < 32f)
				{
					flag = true;
				}
				break;
			case 1:
			case 3:
				if (Math.Abs(value.Y) < 32f)
				{
					flag = true;
				}
				break;
			}
			if (_ShouldAutoSnapPrioritizeAlignedElements() && (num2 > 0.99999f || flag))
			{
				num3 *= 0.01f;
			}
			if (num == -1f || num3 < num)
			{
				num = num3;
				clickableComponent = clickableComponent2;
			}
		}
		if (clickableComponent != null)
		{
			currentlySnappedComponent = clickableComponent;
		}
	}

	protected virtual bool _ShouldAutoSnapPrioritizeAlignedElements()
	{
		return true;
	}

	public virtual bool IsAutomaticSnapValid(int direction, ClickableComponent a, ClickableComponent b)
	{
		return true;
	}

	/// <summary>
	/// when the currentlySnappedComponent region changes 
	/// </summary>
	protected virtual void actionOnRegionChange(int oldRegion, int newRegion)
	{
	}

	public ClickableComponent getComponentWithID(int id)
	{
		if (id == -500)
		{
			return null;
		}
		if (allClickableComponents != null)
		{
			for (int i = 0; i < allClickableComponents.Count; i++)
			{
				if (allClickableComponents[i] != null && allClickableComponents[i].myID == id && allClickableComponents[i].visible)
				{
					return allClickableComponents[i];
				}
			}
			for (int j = 0; j < allClickableComponents.Count; j++)
			{
				if (allClickableComponents[j] != null && allClickableComponents[j].myAlternateID == id && allClickableComponents[j].visible)
				{
					return allClickableComponents[j];
				}
			}
		}
		return null;
	}

	public void initializeUpperRightCloseButton()
	{
		upperRightCloseButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 36, yPositionOnScreen - 8, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
	}

	public virtual void drawBackground(SpriteBatch b)
	{
		if (this is ShopMenu)
		{
			for (int i = 0; i < Game1.uiViewport.Width; i += 400)
			{
				for (int j = 0; j < Game1.uiViewport.Height; j += 384)
				{
					b.Draw(Game1.mouseCursors, new Vector2(i, j), new Rectangle(527, 0, 100, 96), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.08f);
				}
			}
			return;
		}
		if (Game1.isDarkOut())
		{
			b.Draw(Game1.mouseCursors, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), new Rectangle(639, 858, 1, 144), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.9f);
		}
		else if (Game1.IsRainingHere())
		{
			b.Draw(Game1.mouseCursors, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), new Rectangle(640, 858, 1, 184), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.9f);
		}
		else
		{
			b.Draw(Game1.mouseCursors, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), new Rectangle(639 + Utility.getSeasonNumber(Game1.currentSeason), 1051, 1, 400), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.9f);
		}
		b.Draw(Game1.mouseCursors, new Vector2(-120f, Game1.uiViewport.Height - 592), new Rectangle(0, Game1.currentSeason.Equals("winter") ? 1035 : ((Game1.isRaining || Game1.isDarkOut()) ? 886 : 737), 639, 148), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.08f);
		b.Draw(Game1.mouseCursors, new Vector2(2436f, Game1.uiViewport.Height - 592), new Rectangle(0, Game1.currentSeason.Equals("winter") ? 1035 : ((Game1.isRaining || Game1.isDarkOut()) ? 886 : 737), 639, 148), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.08f);
		if (Game1.isRaining)
		{
			b.Draw(Game1.staminaRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Blue * 0.2f);
		}
	}

	public virtual bool showWithoutTransparencyIfOptionIsSet()
	{
		if (this is GameMenu || this is ShopMenu || this is WheelSpinGame || this is ItemGrabMenu)
		{
			return true;
		}
		return false;
	}

	public virtual void clickAway()
	{
	}

	public virtual void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
	{
		xPositionOnScreen = (int)((float)(newBounds.Width - width) * ((float)xPositionOnScreen / (float)(oldBounds.Width - width)));
		yPositionOnScreen = (int)((float)(newBounds.Height - height) * ((float)yPositionOnScreen / (float)(oldBounds.Height - height)));
	}

	public virtual void setUpForGamePadMode()
	{
	}

	public virtual bool shouldClampGamePadCursor()
	{
		return false;
	}

	public virtual void releaseLeftClick(int x, int y)
	{
	}

	public virtual void leftClickHeld(int x, int y)
	{
	}

	public virtual void receiveLeftClick(int x, int y, bool playSound = true)
	{
		if (upperRightCloseButton != null && readyToClose() && upperRightCloseButton.containsPoint(x, y))
		{
			if (playSound)
			{
				Game1.playSound("bigDeSelect");
			}
			exitThisMenu();
		}
	}

	public virtual bool overrideSnappyMenuCursorMovementBan()
	{
		return false;
	}

	public virtual void receiveRightClick(int x, int y, bool playSound = true)
	{
	}

	public virtual void receiveKeyPress(Keys key)
	{
		if (key != 0)
		{
			if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && readyToClose())
			{
				exitThisMenu();
			}
			else if (Game1.options.snappyMenus && Game1.options.gamepadControls && !overrideSnappyMenuCursorMovementBan())
			{
				applyMovementKey(key);
			}
		}
	}

	public virtual void gamePadButtonHeld(Buttons b)
	{
	}

	public virtual ClickableComponent getCurrentlySnappedComponent()
	{
		return currentlySnappedComponent;
	}

	public virtual void receiveScrollWheelAction(int direction)
	{
	}

	public virtual void performHoverAction(int x, int y)
	{
		if (upperRightCloseButton != null)
		{
			upperRightCloseButton.tryHover(x, y, 0.5f);
		}
	}

	public virtual void draw(SpriteBatch b, int red = -1, int green = -1, int blue = -1)
	{
		if (upperRightCloseButton != null && shouldDrawCloseButton())
		{
			upperRightCloseButton.draw(b);
		}
	}

	public virtual void draw(SpriteBatch b)
	{
		if (upperRightCloseButton != null && shouldDrawCloseButton())
		{
			upperRightCloseButton.draw(b);
		}
	}

	public virtual bool isWithinBounds(int x, int y)
	{
		if (x - xPositionOnScreen < width && x - xPositionOnScreen >= 0 && y - yPositionOnScreen < height)
		{
			return y - yPositionOnScreen >= 0;
		}
		return false;
	}

	public virtual void update(GameTime time)
	{
	}

	protected virtual void cleanupBeforeExit()
	{
	}

	public virtual bool shouldDrawCloseButton()
	{
		return true;
	}

	public void exitThisMenuNoSound()
	{
		exitThisMenu(playSound: false);
	}

	public void exitThisMenu(bool playSound = true)
	{
		if (behaviorBeforeCleanup != null)
		{
			behaviorBeforeCleanup(this);
		}
		cleanupBeforeExit();
		if (playSound)
		{
			Game1.playSound("bigDeSelect");
		}
		if (this == Game1.activeClickableMenu)
		{
			Game1.exitActiveMenu();
		}
		else if (Game1.activeClickableMenu is GameMenu && (Game1.activeClickableMenu as GameMenu).GetCurrentPage() == this)
		{
			Game1.exitActiveMenu();
		}
		if (_parentMenu != null)
		{
			IClickableMenu parentMenu = _parentMenu;
			_parentMenu = null;
			parentMenu.SetChildMenu(null);
		}
		if (exitFunction != null)
		{
			onExit onExit = exitFunction;
			exitFunction = null;
			onExit();
		}
	}

	public virtual bool autoCenterMouseCursorForGamepad()
	{
		return true;
	}

	public virtual void emergencyShutDown()
	{
	}

	public virtual bool readyToClose()
	{
		return true;
	}

	protected void drawHorizontalPartition(SpriteBatch b, int yPosition, bool small = false, int red = -1, int green = -1, int blue = -1)
	{
		Color color = ((red == -1) ? Color.White : new Color(red, green, blue));
		Texture2D texture = ((red == -1) ? Game1.menuTexture : Game1.uncoloredMenuTexture);
		if (small)
		{
			b.Draw(texture, new Rectangle(xPositionOnScreen + 32, yPosition, width - 64, 64), Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 25), color);
			return;
		}
		b.Draw(texture, new Vector2(xPositionOnScreen, yPosition), Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 4), color);
		b.Draw(texture, new Rectangle(xPositionOnScreen + 64, yPosition, width - 128, 64), Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 6), color);
		b.Draw(texture, new Vector2(xPositionOnScreen + width - 64, yPosition), Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 7), color);
	}

	protected void drawVerticalPartition(SpriteBatch b, int xPosition, bool small = false, int red = -1, int green = -1, int blue = -1)
	{
		Color color = ((red == -1) ? Color.White : new Color(red, green, blue));
		Texture2D texture = ((red == -1) ? Game1.menuTexture : Game1.uncoloredMenuTexture);
		if (small)
		{
			b.Draw(texture, new Rectangle(xPosition, yPositionOnScreen + 64 + 32, 64, height - 128), Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 26), color);
			return;
		}
		b.Draw(texture, new Vector2(xPosition, yPositionOnScreen + 64), Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 1), color);
		b.Draw(texture, new Rectangle(xPosition, yPositionOnScreen + 128, 64, height - 192), Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 5), color);
		b.Draw(texture, new Vector2(xPosition, yPositionOnScreen + height - 64), Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 13), color);
	}

	protected void drawVerticalIntersectingPartition(SpriteBatch b, int xPosition, int yPosition, int red = -1, int green = -1, int blue = -1)
	{
		Color color = ((red == -1) ? Color.White : new Color(red, green, blue));
		Texture2D texture = ((red == -1) ? Game1.menuTexture : Game1.uncoloredMenuTexture);
		b.Draw(texture, new Vector2(xPosition, yPosition), Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 59), color);
		b.Draw(texture, new Rectangle(xPosition, yPosition + 64, 64, yPositionOnScreen + height - 64 - yPosition - 64), Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 63), color);
		b.Draw(texture, new Vector2(xPosition, yPositionOnScreen + height - 64), Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 62), color);
	}

	protected void drawVerticalUpperIntersectingPartition(SpriteBatch b, int xPosition, int partitionHeight, int red = -1, int green = -1, int blue = -1)
	{
		Color color = ((red == -1) ? Color.White : new Color(red, green, blue));
		Texture2D texture = ((red == -1) ? Game1.menuTexture : Game1.uncoloredMenuTexture);
		b.Draw(texture, new Vector2(xPosition, yPositionOnScreen + 64), Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 44), color);
		b.Draw(texture, new Rectangle(xPosition, yPositionOnScreen + 128, 64, partitionHeight - 32), Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 63), color);
		b.Draw(texture, new Vector2(xPosition, yPositionOnScreen + partitionHeight + 64), Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 39), color);
	}

	public static void drawTextureBox(SpriteBatch b, int x, int y, int width, int height, Color color)
	{
		drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y, width, height, color);
	}

	public static void drawTextureBox(SpriteBatch b, Texture2D texture, Rectangle sourceRect, int x, int y, int width, int height, Color color, float scale = 1f, bool drawShadow = true, float draw_layer = -1f)
	{
		int num = sourceRect.Width / 3;
		float layerDepth = draw_layer - 0.03f;
		if (draw_layer < 0f)
		{
			draw_layer = 0.8f - (float)y * 1E-06f;
			layerDepth = 0.77f;
		}
		if (drawShadow)
		{
			b.Draw(texture, new Vector2(x + width - (int)((float)num * scale) - 8, y + 8), new Rectangle(sourceRect.X + num * 2, sourceRect.Y, num, num), Color.Black * 0.4f, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
			b.Draw(texture, new Vector2(x - 8, y + height - (int)((float)num * scale) + 8), new Rectangle(sourceRect.X, num * 2 + sourceRect.Y, num, num), Color.Black * 0.4f, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
			b.Draw(texture, new Vector2(x + width - (int)((float)num * scale) - 8, y + height - (int)((float)num * scale) + 8), new Rectangle(sourceRect.X + num * 2, num * 2 + sourceRect.Y, num, num), Color.Black * 0.4f, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
			b.Draw(texture, new Rectangle(x + (int)((float)num * scale) - 8, y + 8, width - (int)((float)num * scale) * 2, (int)((float)num * scale)), new Rectangle(sourceRect.X + num, sourceRect.Y, num, num), Color.Black * 0.4f, 0f, Vector2.Zero, SpriteEffects.None, layerDepth);
			b.Draw(texture, new Rectangle(x + (int)((float)num * scale) - 8, y + height - (int)((float)num * scale) + 8, width - (int)((float)num * scale) * 2, (int)((float)num * scale)), new Rectangle(sourceRect.X + num, num * 2 + sourceRect.Y, num, num), Color.Black * 0.4f, 0f, Vector2.Zero, SpriteEffects.None, layerDepth);
			b.Draw(texture, new Rectangle(x - 8, y + (int)((float)num * scale) + 8, (int)((float)num * scale), height - (int)((float)num * scale) * 2), new Rectangle(sourceRect.X, num + sourceRect.Y, num, num), Color.Black * 0.4f, 0f, Vector2.Zero, SpriteEffects.None, layerDepth);
			b.Draw(texture, new Rectangle(x + width - (int)((float)num * scale) - 8, y + (int)((float)num * scale) + 8, (int)((float)num * scale), height - (int)((float)num * scale) * 2), new Rectangle(sourceRect.X + num * 2, num + sourceRect.Y, num, num), Color.Black * 0.4f, 0f, Vector2.Zero, SpriteEffects.None, layerDepth);
			b.Draw(texture, new Rectangle((int)((float)num * scale / 2f) + x - 8, (int)((float)num * scale / 2f) + y + 8, width - (int)((float)num * scale), height - (int)((float)num * scale)), new Rectangle(num + sourceRect.X, num + sourceRect.Y, num, num), Color.Black * 0.4f, 0f, Vector2.Zero, SpriteEffects.None, layerDepth);
		}
		b.Draw(texture, new Rectangle((int)((float)num * scale) + x, (int)((float)num * scale) + y, width - (int)((float)num * scale * 2f), height - (int)((float)num * scale * 2f)), new Rectangle(num + sourceRect.X, num + sourceRect.Y, num, num), color, 0f, Vector2.Zero, SpriteEffects.None, draw_layer);
		b.Draw(texture, new Vector2(x, y), new Rectangle(sourceRect.X, sourceRect.Y, num, num), color, 0f, Vector2.Zero, scale, SpriteEffects.None, draw_layer);
		b.Draw(texture, new Vector2(x + width - (int)((float)num * scale), y), new Rectangle(sourceRect.X + num * 2, sourceRect.Y, num, num), color, 0f, Vector2.Zero, scale, SpriteEffects.None, draw_layer);
		b.Draw(texture, new Vector2(x, y + height - (int)((float)num * scale)), new Rectangle(sourceRect.X, num * 2 + sourceRect.Y, num, num), color, 0f, Vector2.Zero, scale, SpriteEffects.None, draw_layer);
		b.Draw(texture, new Vector2(x + width - (int)((float)num * scale), y + height - (int)((float)num * scale)), new Rectangle(sourceRect.X + num * 2, num * 2 + sourceRect.Y, num, num), color, 0f, Vector2.Zero, scale, SpriteEffects.None, draw_layer);
		b.Draw(texture, new Rectangle(x + (int)((float)num * scale), y, width - (int)((float)num * scale) * 2, (int)((float)num * scale)), new Rectangle(sourceRect.X + num, sourceRect.Y, num, num), color, 0f, Vector2.Zero, SpriteEffects.None, draw_layer);
		b.Draw(texture, new Rectangle(x + (int)((float)num * scale), y + height - (int)((float)num * scale), width - (int)((float)num * scale) * 2, (int)((float)num * scale)), new Rectangle(sourceRect.X + num, num * 2 + sourceRect.Y, num, num), color, 0f, Vector2.Zero, SpriteEffects.None, draw_layer);
		b.Draw(texture, new Rectangle(x, y + (int)((float)num * scale), (int)((float)num * scale), height - (int)((float)num * scale) * 2), new Rectangle(sourceRect.X, num + sourceRect.Y, num, num), color, 0f, Vector2.Zero, SpriteEffects.None, draw_layer);
		b.Draw(texture, new Rectangle(x + width - (int)((float)num * scale), y + (int)((float)num * scale), (int)((float)num * scale), height - (int)((float)num * scale) * 2), new Rectangle(sourceRect.X + num * 2, num + sourceRect.Y, num, num), color, 0f, Vector2.Zero, SpriteEffects.None, draw_layer);
	}

	public void drawBorderLabel(SpriteBatch b, string text, SpriteFont font, int x, int y)
	{
		int num = (int)font.MeasureString(text).X;
		y += 52;
		b.Draw(Game1.mouseCursors, new Vector2(x, y), new Rectangle(256, 267, 6, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
		b.Draw(Game1.mouseCursors, new Vector2(x + 24, y), new Rectangle(262, 267, 1, 16), Color.White, 0f, Vector2.Zero, new Vector2(num, 4f), SpriteEffects.None, 0.87f);
		b.Draw(Game1.mouseCursors, new Vector2(x + 24 + num, y), new Rectangle(263, 267, 6, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
		Utility.drawTextWithShadow(b, text, font, new Vector2(x + 24, y + 20), Game1.textColor);
	}

	public static void drawToolTip(SpriteBatch b, string hoverText, string hoverTitle, Item hoveredItem, bool heldItem = false, int healAmountToDisplay = -1, int currencySymbol = 0, int extraItemToShowIndex = -1, int extraItemToShowAmount = -1, CraftingRecipe craftingIngredients = null, int moneyAmountToShowAtBottom = -1)
	{
		bool flag = hoveredItem != null && hoveredItem is Object && (int)(hoveredItem as Object).edibility != -300;
		int num = Math.Max((healAmountToDisplay != -1) ? ((int)Game1.smallFont.MeasureString(healAmountToDisplay + "+ Energy").X + 32) : 0, Math.Max((int)Game1.smallFont.MeasureString(hoverText).X, (hoverTitle != null) ? ((int)Game1.dialogueFont.MeasureString(hoverTitle).X) : 0)) + 32;
		if (flag)
		{
			int num2 = 9999;
			int num3 = 92;
			num = (int)Math.Max(num, Math.Max(Game1.smallFont.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Energy", num2)).X + (float)num3, Game1.smallFont.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Health", num2)).X + (float)num3));
		}
		drawHoverText(b, hoverText, Game1.smallFont, heldItem ? 40 : 0, heldItem ? 40 : 0, moneyAmountToShowAtBottom, hoverTitle, flag ? ((int)(hoveredItem as Object).edibility) : (-1), (flag && Game1.objectInformation[(hoveredItem as Object).parentSheetIndex].Split('/').Length > 7) ? hoveredItem.ModifyItemBuffs(Game1.objectInformation[(hoveredItem as Object).parentSheetIndex].Split('/')[7].Split(' ')) : null, hoveredItem, currencySymbol, extraItemToShowIndex, extraItemToShowAmount, -1, -1, 1f, craftingIngredients);
	}

	public static void drawHoverText(SpriteBatch b, string text, SpriteFont font, int xOffset = 0, int yOffset = 0, int moneyAmountToDisplayAtBottom = -1, string boldTitleText = null, int healAmountToDisplay = -1, string[] buffIconsToDisplay = null, Item hoveredItem = null, int currencySymbol = 0, int extraItemToShowIndex = -1, int extraItemToShowAmount = -1, int overrideX = -1, int overrideY = -1, float alpha = 1f, CraftingRecipe craftingIngredients = null, IList<Item> additional_craft_materials = null)
	{
		StringBuilder text2 = null;
		if (text != null)
		{
			text2 = new StringBuilder(text);
		}
		drawHoverText(b, text2, font, xOffset, yOffset, moneyAmountToDisplayAtBottom, boldTitleText, healAmountToDisplay, buffIconsToDisplay, hoveredItem, currencySymbol, extraItemToShowIndex, extraItemToShowAmount, overrideX, overrideY, alpha, craftingIngredients, additional_craft_materials);
	}

	public static void drawHoverText(SpriteBatch b, StringBuilder text, SpriteFont font, int xOffset = 0, int yOffset = 0, int moneyAmountToDisplayAtBottom = -1, string boldTitleText = null, int healAmountToDisplay = -1, string[] buffIconsToDisplay = null, Item hoveredItem = null, int currencySymbol = 0, int extraItemToShowIndex = -1, int extraItemToShowAmount = -1, int overrideX = -1, int overrideY = -1, float alpha = 1f, CraftingRecipe craftingIngredients = null, IList<Item> additional_craft_materials = null)
	{
		if (text == null || text.Length == 0)
		{
			return;
		}
		string text2 = null;
		if (boldTitleText != null && boldTitleText.Length == 0)
		{
			boldTitleText = null;
		}
		int num = Math.Max((healAmountToDisplay != -1) ? ((int)font.MeasureString(healAmountToDisplay + "+ Energy" + 32).X) : 0, Math.Max((int)font.MeasureString(text).X, (boldTitleText != null) ? ((int)Game1.dialogueFont.MeasureString(boldTitleText).X) : 0)) + 32;
		int num2 = Math.Max(20 * 3, (int)font.MeasureString(text).Y + 32 + (int)((moneyAmountToDisplayAtBottom > -1) ? (font.MeasureString(moneyAmountToDisplayAtBottom.ToString() ?? "").Y + 4f) : 8f) + (int)((boldTitleText != null) ? (Game1.dialogueFont.MeasureString(boldTitleText).Y + 16f) : 0f));
		if (extraItemToShowIndex != -1)
		{
			string[] array = Game1.objectInformation[extraItemToShowIndex].Split('/');
			string text3 = array[0];
			if (LocalizedContentManager.CurrentLanguageCode != 0)
			{
				text3 = array[4];
			}
			string text4 = Game1.content.LoadString("Strings\\UI:ItemHover_Requirements", extraItemToShowAmount, (extraItemToShowAmount > 1) ? Lexicon.makePlural(text3) : text3);
			int num3 = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, extraItemToShowIndex, 16, 16).Width * 2 * 4;
			num = Math.Max(num, num3 + (int)font.MeasureString(text4).X);
		}
		if (buffIconsToDisplay != null)
		{
			for (int i = 0; i < buffIconsToDisplay.Length; i++)
			{
				if (!buffIconsToDisplay[i].Equals("0"))
				{
					num2 += 34;
				}
			}
			num2 += 4;
		}
		if (craftingIngredients != null && Game1.options.showAdvancedCraftingInformation && craftingIngredients.getCraftCountText() != null)
		{
			num2 += (int)font.MeasureString("T").Y;
		}
		string text5 = null;
		if (hoveredItem != null)
		{
			num2 += 68 * hoveredItem.attachmentSlots();
			text5 = hoveredItem.getCategoryName();
			if (text5.Length > 0)
			{
				num = Math.Max(num, (int)font.MeasureString(text5).X + 32);
				num2 += (int)font.MeasureString("T").Y;
			}
			int num4 = 9999;
			int num5 = 92;
			Point extraSpaceNeededForTooltipSpecialIcons = hoveredItem.getExtraSpaceNeededForTooltipSpecialIcons(font, num, num5, num2, text, boldTitleText, moneyAmountToDisplayAtBottom);
			num = ((extraSpaceNeededForTooltipSpecialIcons.X != 0) ? extraSpaceNeededForTooltipSpecialIcons.X : num);
			num2 = ((extraSpaceNeededForTooltipSpecialIcons.Y != 0) ? extraSpaceNeededForTooltipSpecialIcons.Y : num2);
			if (hoveredItem is MeleeWeapon && (hoveredItem as MeleeWeapon).GetTotalForgeLevels() > 0)
			{
				num2 += (int)font.MeasureString("T").Y;
			}
			if (hoveredItem is MeleeWeapon && (hoveredItem as MeleeWeapon).GetEnchantmentLevel<GalaxySoulEnchantment>() > 0)
			{
				num2 += (int)font.MeasureString("T").Y;
			}
			if (hoveredItem is Object && (int)(hoveredItem as Object).edibility != -300)
			{
				num2 = ((healAmountToDisplay == -1) ? (num2 + 40) : (num2 + 40 * ((healAmountToDisplay <= 0) ? 1 : 2)));
				healAmountToDisplay = (hoveredItem as Object).staminaRecoveredOnConsumption();
				num = (int)Math.Max(num, Math.Max(font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Energy", num4)).X + (float)num5, font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Health", num4)).X + (float)num5));
			}
			if (buffIconsToDisplay != null)
			{
				for (int j = 0; j < buffIconsToDisplay.Length; j++)
				{
					if (!buffIconsToDisplay[j].Equals("0") && j <= 11)
					{
						num = (int)Math.Max(num, font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Buff" + j, num4)).X + (float)num5);
					}
				}
			}
		}
		Vector2 vector = Vector2.Zero;
		if (craftingIngredients != null)
		{
			if (Game1.options.showAdvancedCraftingInformation)
			{
				int craftableCount = craftingIngredients.getCraftableCount(additional_craft_materials);
				if (craftableCount > 1)
				{
					text2 = " (" + craftableCount + ")";
					vector = Game1.smallFont.MeasureString(text2);
				}
			}
			num = (int)Math.Max(Game1.dialogueFont.MeasureString(boldTitleText).X + vector.X + 12f, 384f);
			num2 += craftingIngredients.getDescriptionHeight(num - 8) + ((healAmountToDisplay == -1) ? (-32) : 0);
		}
		else if (text2 != null && boldTitleText != null)
		{
			vector = Game1.smallFont.MeasureString(text2);
			num = (int)Math.Max(num, Game1.dialogueFont.MeasureString(boldTitleText).X + vector.X + 12f);
		}
		int x = Game1.getOldMouseX() + 32 + xOffset;
		int num6 = Game1.getOldMouseY() + 32 + yOffset;
		if (overrideX != -1)
		{
			x = overrideX;
		}
		if (overrideY != -1)
		{
			num6 = overrideY;
		}
		if (x + num > Utility.getSafeArea().Right)
		{
			x = Utility.getSafeArea().Right - num;
			num6 += 16;
		}
		if (num6 + num2 > Utility.getSafeArea().Bottom)
		{
			x += 16;
			if (x + num > Utility.getSafeArea().Right)
			{
				x = Utility.getSafeArea().Right - num;
			}
			num6 = Utility.getSafeArea().Bottom - num2;
		}
		drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, num6, num + ((craftingIngredients != null) ? 21 : 0), num2, Color.White * alpha);
		if (boldTitleText != null)
		{
			Vector2 vector2 = Game1.dialogueFont.MeasureString(boldTitleText);
			drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, num6, num + ((craftingIngredients != null) ? 21 : 0), (int)Game1.dialogueFont.MeasureString(boldTitleText).Y + 32 + (int)((hoveredItem != null && text5.Length > 0) ? font.MeasureString("asd").Y : 0f) - 4, Color.White * alpha, 1f, drawShadow: false);
			b.Draw(Game1.menuTexture, new Rectangle(x + 12, num6 + (int)Game1.dialogueFont.MeasureString(boldTitleText).Y + 32 + (int)((hoveredItem != null && text5.Length > 0) ? font.MeasureString("asd").Y : 0f) - 4, num - 4 * ((craftingIngredients != null) ? 1 : 6), 4), new Rectangle(44, 300, 4, 4), Color.White);
			b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2(x + 16, num6 + 16 + 4) + new Vector2(2f, 2f), Game1.textShadowColor);
			b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2(x + 16, num6 + 16 + 4) + new Vector2(0f, 2f), Game1.textShadowColor);
			b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2(x + 16, num6 + 16 + 4), Game1.textColor);
			if (text2 != null)
			{
				Utility.drawTextWithShadow(b, text2, Game1.smallFont, new Vector2((float)(x + 16) + vector2.X, (int)((float)(num6 + 16 + 4) + vector2.Y / 2f - vector.Y / 2f)), Game1.textColor);
			}
			num6 += (int)Game1.dialogueFont.MeasureString(boldTitleText).Y;
		}
		if (hoveredItem != null && text5.Length > 0)
		{
			num6 -= 4;
			Utility.drawTextWithShadow(b, text5, font, new Vector2(x + 16, num6 + 16 + 4), hoveredItem.getCategoryColor(), 1f, -1f, 2, 2);
			num6 += (int)font.MeasureString("T").Y + ((boldTitleText != null) ? 16 : 0) + 4;
			if (hoveredItem is Tool && (hoveredItem as Tool).GetTotalForgeLevels() > 0)
			{
				string text6 = Game1.content.LoadString("Strings\\UI:Item_Tooltip_Forged");
				Utility.drawTextWithShadow(b, text6, font, new Vector2(x + 16, num6 + 16 + 4), Color.DarkRed, 1f, -1f, 2, 2);
				int totalForgeLevels = (hoveredItem as Tool).GetTotalForgeLevels();
				if (totalForgeLevels < (hoveredItem as Tool).GetMaxForges() && !(hoveredItem as Tool).hasEnchantmentOfType<DiamondEnchantment>())
				{
					Utility.drawTextWithShadow(b, " (" + totalForgeLevels + "/" + (hoveredItem as Tool).GetMaxForges() + ")", font, new Vector2((float)(x + 16) + font.MeasureString(text6).X, num6 + 16 + 4), Color.DimGray, 1f, -1f, 2, 2);
				}
				num6 += (int)font.MeasureString("T").Y;
			}
			if (hoveredItem is MeleeWeapon && (hoveredItem as MeleeWeapon).GetEnchantmentLevel<GalaxySoulEnchantment>() > 0)
			{
				GalaxySoulEnchantment enchantmentOfType = (hoveredItem as MeleeWeapon).GetEnchantmentOfType<GalaxySoulEnchantment>();
				string text7 = Game1.content.LoadString("Strings\\UI:Item_Tooltip_GalaxyForged");
				Utility.drawTextWithShadow(b, text7, font, new Vector2(x + 16, num6 + 16 + 4), Color.DarkRed, 1f, -1f, 2, 2);
				int level = enchantmentOfType.GetLevel();
				if (level < enchantmentOfType.GetMaximumLevel())
				{
					Utility.drawTextWithShadow(b, " (" + level + "/" + enchantmentOfType.GetMaximumLevel() + ")", font, new Vector2((float)(x + 16) + font.MeasureString(text7).X, num6 + 16 + 4), Color.DimGray, 1f, -1f, 2, 2);
				}
				num6 += (int)font.MeasureString("T").Y;
			}
		}
		else
		{
			num6 += ((boldTitleText != null) ? 16 : 0);
		}
		if (hoveredItem != null && craftingIngredients == null)
		{
			hoveredItem.drawTooltip(b, ref x, ref num6, font, alpha, text);
		}
		else if (text != null && text.Length != 0 && (text.Length != 1 || text[0] != ' '))
		{
			b.DrawString(font, text, new Vector2(x + 16, num6 + 16 + 4) + new Vector2(2f, 2f), Game1.textShadowColor * alpha);
			b.DrawString(font, text, new Vector2(x + 16, num6 + 16 + 4) + new Vector2(0f, 2f), Game1.textShadowColor * alpha);
			b.DrawString(font, text, new Vector2(x + 16, num6 + 16 + 4) + new Vector2(2f, 0f), Game1.textShadowColor * alpha);
			b.DrawString(font, text, new Vector2(x + 16, num6 + 16 + 4), Game1.textColor * 0.9f * alpha);
			num6 += (int)font.MeasureString(text).Y + 4;
		}
		if (craftingIngredients != null)
		{
			craftingIngredients.drawRecipeDescription(b, new Vector2(x + 16, num6 - 8), num, additional_craft_materials);
			num6 += craftingIngredients.getDescriptionHeight(num - 8);
		}
		if (healAmountToDisplay != -1)
		{
			int num7 = (hoveredItem as Object).staminaRecoveredOnConsumption();
			if (num7 >= 0)
			{
				int num8 = (hoveredItem as Object).healthRecoveredOnConsumption();
				Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(x + 16 + 4, num6 + 16), new Rectangle((num7 < 0) ? 140 : 0, 428, 10, 10), Color.White, 0f, Vector2.Zero, 3f, flipped: false, 0.95f);
				Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:ItemHover_Energy", ((num7 > 0) ? "+" : "") + num7), font, new Vector2(x + 16 + 34 + 4, num6 + 16), Game1.textColor);
				num6 += 34;
				if (num8 > 0)
				{
					Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(x + 16 + 4, num6 + 16), new Rectangle(0, 438, 10, 10), Color.White, 0f, Vector2.Zero, 3f, flipped: false, 0.95f);
					Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:ItemHover_Health", ((num8 > 0) ? "+" : "") + num8), font, new Vector2(x + 16 + 34 + 4, num6 + 16), Game1.textColor);
					num6 += 34;
				}
			}
			else if (num7 != -300)
			{
				Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(x + 16 + 4, num6 + 16), new Rectangle(140, 428, 10, 10), Color.White, 0f, Vector2.Zero, 3f, flipped: false, 0.95f);
				Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:ItemHover_Energy", num7.ToString() ?? ""), font, new Vector2(x + 16 + 34 + 4, num6 + 16), Game1.textColor);
				num6 += 34;
			}
		}
		if (buffIconsToDisplay != null)
		{
			for (int k = 0; k < buffIconsToDisplay.Length; k++)
			{
				if (!buffIconsToDisplay[k].Equals("0"))
				{
					Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(x + 16 + 4, num6 + 16), new Rectangle(10 + k * 10, 428, 10, 10), Color.White, 0f, Vector2.Zero, 3f, flipped: false, 0.95f);
					string text8 = ((Convert.ToInt32(buffIconsToDisplay[k]) > 0) ? "+" : "") + buffIconsToDisplay[k] + " ";
					if (k <= 11)
					{
						text8 = Game1.content.LoadString("Strings\\UI:ItemHover_Buff" + k, text8);
					}
					Utility.drawTextWithShadow(b, text8, font, new Vector2(x + 16 + 34 + 4, num6 + 16), Game1.textColor);
					num6 += 34;
				}
			}
		}
		if (hoveredItem != null && hoveredItem.attachmentSlots() > 0)
		{
			hoveredItem.drawAttachments(b, x + 16, num6 + 16);
			if (moneyAmountToDisplayAtBottom > -1)
			{
				num6 += 68 * hoveredItem.attachmentSlots();
			}
		}
		if (moneyAmountToDisplayAtBottom > -1)
		{
			b.DrawString(font, moneyAmountToDisplayAtBottom.ToString() ?? "", new Vector2(x + 16, num6 + 16 + 4) + new Vector2(2f, 2f), Game1.textShadowColor);
			b.DrawString(font, moneyAmountToDisplayAtBottom.ToString() ?? "", new Vector2(x + 16, num6 + 16 + 4) + new Vector2(0f, 2f), Game1.textShadowColor);
			b.DrawString(font, moneyAmountToDisplayAtBottom.ToString() ?? "", new Vector2(x + 16, num6 + 16 + 4) + new Vector2(2f, 0f), Game1.textShadowColor);
			b.DrawString(font, moneyAmountToDisplayAtBottom.ToString() ?? "", new Vector2(x + 16, num6 + 16 + 4), Game1.textColor);
			switch (currencySymbol)
			{
			case 0:
				b.Draw(Game1.debrisSpriteSheet, new Vector2((float)(x + 16) + font.MeasureString(moneyAmountToDisplayAtBottom.ToString() ?? "").X + 20f, num6 + 16 + 16), Game1.getSourceRectForStandardTileSheet(Game1.debrisSpriteSheet, 8, 16, 16), Color.White, 0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, 0.95f);
				break;
			case 1:
				b.Draw(Game1.mouseCursors, new Vector2((float)(x + 8) + font.MeasureString(moneyAmountToDisplayAtBottom.ToString() ?? "").X + 20f, num6 + 16 - 5), new Rectangle(338, 400, 8, 8), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
				break;
			case 2:
				b.Draw(Game1.mouseCursors, new Vector2((float)(x + 8) + font.MeasureString(moneyAmountToDisplayAtBottom.ToString() ?? "").X + 20f, num6 + 16 - 7), new Rectangle(211, 373, 9, 10), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
				break;
			case 4:
				b.Draw(Game1.objectSpriteSheet, new Vector2((float)(x + 8) + font.MeasureString(moneyAmountToDisplayAtBottom.ToString() ?? "").X + 20f, num6 + 16 - 7), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 858, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
				break;
			}
			num6 += 48;
		}
		if (extraItemToShowIndex != -1)
		{
			if (moneyAmountToDisplayAtBottom == -1)
			{
				num6 += 8;
			}
			string sub = Game1.objectInformation[extraItemToShowIndex].Split('/')[4];
			string text9 = Game1.content.LoadString("Strings\\UI:ItemHover_Requirements", extraItemToShowAmount, sub);
			float num9 = Math.Max(font.MeasureString(text9).Y + 21f, 96f);
			drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, num6 + 4, num + ((craftingIngredients != null) ? 21 : 0), (int)num9, Color.White);
			num6 += 20;
			b.DrawString(font, text9, new Vector2(x + 16, num6 + 4) + new Vector2(2f, 2f), Game1.textShadowColor);
			b.DrawString(font, text9, new Vector2(x + 16, num6 + 4) + new Vector2(0f, 2f), Game1.textShadowColor);
			b.DrawString(font, text9, new Vector2(x + 16, num6 + 4) + new Vector2(2f, 0f), Game1.textShadowColor);
			b.DrawString(Game1.smallFont, text9, new Vector2(x + 16, num6 + 4), Game1.textColor);
			b.Draw(Game1.objectSpriteSheet, new Vector2(x + 16 + (int)font.MeasureString(text9).X + 21, num6), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, extraItemToShowIndex, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
		}
		if (craftingIngredients != null && Game1.options.showAdvancedCraftingInformation)
		{
			Utility.drawTextWithShadow(b, craftingIngredients.getCraftCountText(), font, new Vector2(x + 16, num6 + 16 + 4), Game1.textColor, 1f, -1f, 2, 2);
			num6 += (int)font.MeasureString("T").Y + 4;
		}
	}
}
