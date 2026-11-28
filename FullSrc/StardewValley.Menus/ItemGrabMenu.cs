using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;

namespace StardewValley.Menus;

public class ItemGrabMenu : MenuWithInventory
{
	public delegate void behaviorOnItemSelect(Item item, Farmer who);

	public class TransferredItemSprite
	{
		public Item item;

		public Vector2 position;

		public float age;

		public float alpha = 1f;

		public TransferredItemSprite(Item transferred_item, int start_x, int start_y)
		{
			item = transferred_item;
			position.X = start_x;
			position.Y = start_y;
		}

		public bool Update(GameTime time)
		{
			float num = 0.15f;
			position.Y -= (float)time.ElapsedGameTime.TotalSeconds * 128f;
			age += (float)time.ElapsedGameTime.TotalSeconds;
			alpha = 1f - age / num;
			if (age >= num)
			{
				return true;
			}
			return false;
		}

		public void Draw(SpriteBatch b)
		{
			item.drawInMenu(b, position, 1f, alpha, 0.9f, StackDrawType.Hide, Color.White, drawShadow: false);
		}
	}

	public const int region_organizationButtons = 15923;

	public const int region_itemsToGrabMenuModifier = 53910;

	public const int region_fillStacksButton = 12952;

	public const int region_organizeButton = 106;

	public const int region_colorPickToggle = 27346;

	public const int region_specialButton = 12485;

	public const int region_lastShippedHolder = 12598;

	public const int source_none = 0;

	public const int source_chest = 1;

	public const int source_gift = 2;

	public const int source_fishingChest = 3;

	public const int specialButton_junimotoggle = 1;

	public InventoryMenu ItemsToGrabMenu;

	private TemporaryAnimatedSprite poof;

	public bool reverseGrab;

	public bool showReceivingMenu = true;

	public bool drawBG = true;

	public bool destroyItemOnClick;

	public bool canExitOnKey;

	public bool playRightClickSound;

	public bool allowRightClick;

	public bool shippingBin;

	private string message;

	private behaviorOnItemSelect behaviorFunction;

	public behaviorOnItemSelect behaviorOnItemGrab;

	private Item hoverItem;

	private Item sourceItem;

	public ClickableTextureComponent fillStacksButton;

	public ClickableTextureComponent organizeButton;

	public ClickableTextureComponent colorPickerToggleButton;

	public ClickableTextureComponent specialButton;

	public ClickableTextureComponent lastShippedHolder;

	public List<ClickableComponent> discreteColorPickerCC;

	public int source;

	public int whichSpecialButton;

	public object context;

	private bool snappedtoBottom;

	public DiscreteColorPicker chestColorPicker;

	private bool essential;

	protected List<TransferredItemSprite> _transferredItemSprites = new List<TransferredItemSprite>();

	protected bool _sourceItemInCurrentLocation;

	public ClickableTextureComponent junimoNoteIcon;

	private int junimoNotePulser;

	public ItemGrabMenu(IList<Item> inventory, object context = null)
		: base(null, okButton: true, trashCan: true)
	{
		this.context = context;
		ItemsToGrabMenu = new InventoryMenu(xPositionOnScreen + 32, yPositionOnScreen, playerInventory: false, inventory);
		trashCan.myID = 106;
		ItemsToGrabMenu.populateClickableComponentList();
		for (int i = 0; i < ItemsToGrabMenu.inventory.Count; i++)
		{
			if (ItemsToGrabMenu.inventory[i] != null)
			{
				ItemsToGrabMenu.inventory[i].myID += 53910;
				ItemsToGrabMenu.inventory[i].upNeighborID += 53910;
				ItemsToGrabMenu.inventory[i].rightNeighborID += 53910;
				ItemsToGrabMenu.inventory[i].downNeighborID = -7777;
				ItemsToGrabMenu.inventory[i].leftNeighborID += 53910;
				ItemsToGrabMenu.inventory[i].fullyImmutable = true;
				if (i % (ItemsToGrabMenu.capacity / ItemsToGrabMenu.rows) == 0)
				{
					ItemsToGrabMenu.inventory[i].leftNeighborID = dropItemInvisibleButton.myID;
				}
				if (i % (ItemsToGrabMenu.capacity / ItemsToGrabMenu.rows) == ItemsToGrabMenu.capacity / ItemsToGrabMenu.rows - 1)
				{
					ItemsToGrabMenu.inventory[i].rightNeighborID = trashCan.myID;
				}
			}
		}
		for (int j = 0; j < GetColumnCount(); j++)
		{
			if (base.inventory != null && base.inventory.inventory != null && base.inventory.inventory.Count >= GetColumnCount())
			{
				base.inventory.inventory[j].upNeighborID = (shippingBin ? 12598 : (-7777));
			}
		}
		if (!shippingBin)
		{
			for (int k = 0; k < GetColumnCount() * 3; k++)
			{
				if (base.inventory != null && base.inventory.inventory != null && base.inventory.inventory.Count > k)
				{
					base.inventory.inventory[k].upNeighborID = -7777;
					base.inventory.inventory[k].upNeighborImmutable = true;
				}
			}
		}
		if (trashCan != null)
		{
			trashCan.leftNeighborID = 11;
		}
		if (okButton != null)
		{
			okButton.leftNeighborID = 11;
		}
		populateClickableComponentList();
		if (Game1.options.SnappyMenus)
		{
			snapToDefaultClickableComponent();
		}
		base.inventory.showGrayedOutSlots = true;
		SetupBorderNeighbors();
	}

	public virtual void DropRemainingItems()
	{
		if (ItemsToGrabMenu == null || ItemsToGrabMenu.actualInventory == null)
		{
			return;
		}
		foreach (Item item in ItemsToGrabMenu.actualInventory)
		{
			if (item != null)
			{
				Game1.createItemDebris(item, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
			}
		}
		ItemsToGrabMenu.actualInventory.Clear();
	}

	public override bool readyToClose()
	{
		return base.readyToClose();
	}

	public ItemGrabMenu(IList<Item> inventory, bool reverseGrab, bool showReceivingMenu, InventoryMenu.highlightThisItem highlightFunction, behaviorOnItemSelect behaviorOnItemSelectFunction, string message, behaviorOnItemSelect behaviorOnItemGrab = null, bool snapToBottom = false, bool canBeExitedWithKey = false, bool playRightClickSound = true, bool allowRightClick = true, bool showOrganizeButton = false, int source = 0, Item sourceItem = null, int whichSpecialButton = -1, object context = null)
		: base(highlightFunction, okButton: true, trashCan: true, 0, 0, 64)
	{
		this.source = source;
		this.message = message;
		this.reverseGrab = reverseGrab;
		this.showReceivingMenu = showReceivingMenu;
		this.playRightClickSound = playRightClickSound;
		this.allowRightClick = allowRightClick;
		base.inventory.showGrayedOutSlots = true;
		this.sourceItem = sourceItem;
		if (sourceItem != null && Game1.currentLocation.objects.Values.Contains(sourceItem))
		{
			_sourceItemInCurrentLocation = true;
		}
		else
		{
			_sourceItemInCurrentLocation = false;
		}
		if (source == 1 && sourceItem != null && sourceItem is Chest && (sourceItem as Chest).SpecialChestType == Chest.SpecialChestTypes.None)
		{
			chestColorPicker = new DiscreteColorPicker(xPositionOnScreen, yPositionOnScreen - 64 - IClickableMenu.borderWidth * 2, 0, new Chest(playerChest: true, sourceItem.ParentSheetIndex));
			chestColorPicker.colorSelection = chestColorPicker.getSelectionFromColor((sourceItem as Chest).playerChoiceColor);
			(chestColorPicker.itemToDrawColored as Chest).playerChoiceColor.Value = chestColorPicker.getColorFromSelection(chestColorPicker.colorSelection);
			colorPickerToggleButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width, yPositionOnScreen + height / 3 - 64 + -160, 64, 64), Game1.mouseCursors, new Rectangle(119, 469, 16, 16), 4f)
			{
				hoverText = Game1.content.LoadString("Strings\\UI:Toggle_ColorPicker"),
				myID = 27346,
				downNeighborID = -99998,
				leftNeighborID = 53921,
				region = 15923
			};
			if (InventoryPage.ShouldShowJunimoNoteIcon())
			{
				junimoNoteIcon = new ClickableTextureComponent("", new Rectangle(xPositionOnScreen + width, yPositionOnScreen + height / 3 - 64 + -216, 64, 64), "", Game1.content.LoadString("Strings\\UI:GameMenu_JunimoNote_Hover"), Game1.mouseCursors, new Rectangle(331, 374, 15, 14), 4f)
				{
					myID = 898,
					leftNeighborID = 11,
					downNeighborID = 106
				};
			}
		}
		this.whichSpecialButton = whichSpecialButton;
		this.context = context;
		if (whichSpecialButton == 1)
		{
			specialButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width, yPositionOnScreen + height / 3 - 64 + -160, 64, 64), Game1.mouseCursors, new Rectangle(108, 491, 16, 16), 4f)
			{
				myID = 12485,
				downNeighborID = (showOrganizeButton ? 12952 : 5948),
				region = 15923,
				leftNeighborID = 53921
			};
			if (context != null && context is JunimoHut)
			{
				specialButton.sourceRect.X = ((context as JunimoHut).noHarvest ? 124 : 108);
			}
		}
		if (snapToBottom)
		{
			movePosition(0, Game1.uiViewport.Height - (yPositionOnScreen + height - IClickableMenu.spaceToClearTopBorder));
			snappedtoBottom = true;
		}
		if (source == 1 && sourceItem != null && sourceItem is Chest && (sourceItem as Chest).GetActualCapacity() != 36)
		{
			int actualCapacity = (sourceItem as Chest).GetActualCapacity();
			int num = 3;
			if (actualCapacity < 9)
			{
				num = 1;
			}
			int num2 = 64 * (actualCapacity / num);
			ItemsToGrabMenu = new InventoryMenu(Game1.uiViewport.Width / 2 - num2 / 2, yPositionOnScreen + 64, playerInventory: false, inventory, highlightFunction, actualCapacity, num);
			if ((sourceItem as Chest).SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin)
			{
				base.inventory.moveItemSound = "Ship";
			}
		}
		else
		{
			ItemsToGrabMenu = new InventoryMenu(xPositionOnScreen + 32, yPositionOnScreen, playerInventory: false, inventory, highlightFunction);
		}
		ItemsToGrabMenu.populateClickableComponentList();
		for (int i = 0; i < ItemsToGrabMenu.inventory.Count; i++)
		{
			if (ItemsToGrabMenu.inventory[i] != null)
			{
				ItemsToGrabMenu.inventory[i].myID += 53910;
				ItemsToGrabMenu.inventory[i].upNeighborID += 53910;
				ItemsToGrabMenu.inventory[i].rightNeighborID += 53910;
				ItemsToGrabMenu.inventory[i].downNeighborID = -7777;
				ItemsToGrabMenu.inventory[i].leftNeighborID += 53910;
				ItemsToGrabMenu.inventory[i].fullyImmutable = true;
			}
		}
		behaviorFunction = behaviorOnItemSelectFunction;
		this.behaviorOnItemGrab = behaviorOnItemGrab;
		canExitOnKey = canBeExitedWithKey;
		if (showOrganizeButton)
		{
			fillStacksButton = new ClickableTextureComponent("", new Rectangle(xPositionOnScreen + width, yPositionOnScreen + height / 3 - 64 - 64 - 16, 64, 64), "", Game1.content.LoadString("Strings\\UI:ItemGrab_FillStacks"), Game1.mouseCursors, new Rectangle(103, 469, 16, 16), 4f)
			{
				myID = 12952,
				upNeighborID = ((colorPickerToggleButton != null) ? 27346 : ((specialButton != null) ? 12485 : (-500))),
				downNeighborID = 106,
				leftNeighborID = 53921,
				region = 15923
			};
			organizeButton = new ClickableTextureComponent("", new Rectangle(xPositionOnScreen + width, yPositionOnScreen + height / 3 - 64, 64, 64), "", Game1.content.LoadString("Strings\\UI:ItemGrab_Organize"), Game1.mouseCursors, new Rectangle(162, 440, 16, 16), 4f)
			{
				myID = 106,
				upNeighborID = 12952,
				downNeighborID = 5948,
				leftNeighborID = 53921,
				region = 15923
			};
		}
		RepositionSideButtons();
		if (chestColorPicker != null)
		{
			discreteColorPickerCC = new List<ClickableComponent>();
			for (int j = 0; j < chestColorPicker.totalColors; j++)
			{
				discreteColorPickerCC.Add(new ClickableComponent(new Rectangle(chestColorPicker.xPositionOnScreen + IClickableMenu.borderWidth / 2 + j * 9 * 4, chestColorPicker.yPositionOnScreen + IClickableMenu.borderWidth / 2, 36, 28), "")
				{
					myID = j + 4343,
					rightNeighborID = ((j < chestColorPicker.totalColors - 1) ? (j + 4343 + 1) : (-1)),
					leftNeighborID = ((j > 0) ? (j + 4343 - 1) : (-1)),
					downNeighborID = ((ItemsToGrabMenu != null && ItemsToGrabMenu.inventory.Count > 0) ? 53910 : 0)
				});
			}
		}
		if (organizeButton != null)
		{
			foreach (ClickableComponent item in ItemsToGrabMenu.GetBorder(InventoryMenu.BorderSide.Right))
			{
				item.rightNeighborID = organizeButton.myID;
			}
		}
		if (trashCan != null && base.inventory.inventory.Count >= 12 && base.inventory.inventory[11] != null)
		{
			base.inventory.inventory[11].rightNeighborID = 5948;
		}
		if (trashCan != null)
		{
			trashCan.leftNeighborID = 11;
		}
		if (okButton != null)
		{
			okButton.leftNeighborID = 11;
		}
		ClickableComponent clickableComponent = ItemsToGrabMenu.GetBorder(InventoryMenu.BorderSide.Right).FirstOrDefault();
		if (clickableComponent != null)
		{
			if (organizeButton != null)
			{
				organizeButton.leftNeighborID = clickableComponent.myID;
			}
			if (specialButton != null)
			{
				specialButton.leftNeighborID = clickableComponent.myID;
			}
			if (fillStacksButton != null)
			{
				fillStacksButton.leftNeighborID = clickableComponent.myID;
			}
			if (junimoNoteIcon != null)
			{
				junimoNoteIcon.leftNeighborID = clickableComponent.myID;
			}
		}
		populateClickableComponentList();
		if (Game1.options.SnappyMenus)
		{
			snapToDefaultClickableComponent();
		}
		SetupBorderNeighbors();
	}

	public virtual void RepositionSideButtons()
	{
		List<ClickableComponent> list = new List<ClickableComponent>();
		if (organizeButton != null)
		{
			list.Add(organizeButton);
		}
		if (fillStacksButton != null)
		{
			list.Add(fillStacksButton);
		}
		if (colorPickerToggleButton != null)
		{
			list.Add(colorPickerToggleButton);
		}
		if (specialButton != null)
		{
			list.Add(specialButton);
		}
		if (junimoNoteIcon != null)
		{
			list.Add(junimoNoteIcon);
		}
		int num = 80;
		if (list.Count >= 4)
		{
			num = 72;
		}
		for (int i = 0; i < list.Count; i++)
		{
			ClickableComponent clickableComponent = list[i];
			if (i > 0 && list.Count > 1)
			{
				clickableComponent.downNeighborID = list[i - 1].myID;
			}
			if (i < list.Count - 1 && list.Count > 1)
			{
				clickableComponent.upNeighborID = list[i + 1].myID;
			}
			clickableComponent.bounds.X = xPositionOnScreen + width;
			clickableComponent.bounds.Y = yPositionOnScreen + height / 3 - 64 - num * i;
		}
	}

	public void SetupBorderNeighbors()
	{
		List<ClickableComponent> border = inventory.GetBorder(InventoryMenu.BorderSide.Right);
		foreach (ClickableComponent item in border)
		{
			item.rightNeighborID = -99998;
			item.rightNeighborImmutable = true;
		}
		border = ItemsToGrabMenu.GetBorder(InventoryMenu.BorderSide.Right);
		bool flag = false;
		foreach (ClickableComponent allClickableComponent in allClickableComponents)
		{
			if (allClickableComponent.region == 15923)
			{
				flag = true;
				break;
			}
		}
		foreach (ClickableComponent item2 in border)
		{
			if (flag)
			{
				item2.rightNeighborID = -99998;
				item2.rightNeighborImmutable = true;
			}
			else
			{
				item2.rightNeighborID = -1;
			}
		}
		for (int i = 0; i < GetColumnCount(); i++)
		{
			if (inventory != null && inventory.inventory != null && inventory.inventory.Count >= 12)
			{
				inventory.inventory[i].upNeighborID = (shippingBin ? 12598 : ((discreteColorPickerCC != null && ItemsToGrabMenu != null && ItemsToGrabMenu.inventory.Count <= i && Game1.player.showChestColorPicker) ? 4343 : ((ItemsToGrabMenu.inventory.Count > i) ? (53910 + i) : 53910)));
			}
			if (discreteColorPickerCC != null && ItemsToGrabMenu != null && ItemsToGrabMenu.inventory.Count > i && Game1.player.showChestColorPicker)
			{
				ItemsToGrabMenu.inventory[i].upNeighborID = 4343;
			}
			else
			{
				ItemsToGrabMenu.inventory[i].upNeighborID = -1;
			}
		}
		if (shippingBin)
		{
			return;
		}
		for (int j = 0; j < 36; j++)
		{
			if (inventory != null && inventory.inventory != null && inventory.inventory.Count > j)
			{
				inventory.inventory[j].upNeighborID = -7777;
				inventory.inventory[j].upNeighborImmutable = true;
			}
		}
	}

	public virtual int GetColumnCount()
	{
		return ItemsToGrabMenu.capacity / ItemsToGrabMenu.rows;
	}

	public ItemGrabMenu setEssential(bool essential)
	{
		this.essential = essential;
		return this;
	}

	public void initializeShippingBin()
	{
		shippingBin = true;
		lastShippedHolder = new ClickableTextureComponent("", new Rectangle(xPositionOnScreen + width / 2 - 48, yPositionOnScreen + height / 2 - 80 - 64, 96, 96), "", Game1.content.LoadString("Strings\\UI:ShippingBin_LastItem"), Game1.mouseCursors, new Rectangle(293, 360, 24, 24), 4f)
		{
			myID = 12598,
			region = 12598
		};
		for (int i = 0; i < GetColumnCount(); i++)
		{
			if (inventory != null && inventory.inventory != null && inventory.inventory.Count >= GetColumnCount())
			{
				inventory.inventory[i].upNeighborID = -7777;
				if (i == 11)
				{
					inventory.inventory[i].rightNeighborID = 5948;
				}
			}
		}
		populateClickableComponentList();
		if (Game1.options.SnappyMenus)
		{
			snapToDefaultClickableComponent();
		}
	}

	protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
	{
		switch (direction)
		{
		case 2:
		{
			for (int j = 0; j < 12; j++)
			{
				if (inventory != null && inventory.inventory != null && inventory.inventory.Count >= GetColumnCount() && shippingBin)
				{
					inventory.inventory[j].upNeighborID = (shippingBin ? 12598 : (Math.Min(j, ItemsToGrabMenu.inventory.Count - 1) + 53910));
				}
			}
			if (!shippingBin && oldID >= 53910)
			{
				int num2 = oldID - 53910;
				if (num2 + GetColumnCount() <= ItemsToGrabMenu.inventory.Count - 1)
				{
					currentlySnappedComponent = getComponentWithID(num2 + GetColumnCount() + 53910);
					snapCursorToCurrentSnappedComponent();
					break;
				}
			}
			currentlySnappedComponent = getComponentWithID((oldRegion != 12598) ? ((oldID - 53910) % GetColumnCount()) : 0);
			snapCursorToCurrentSnappedComponent();
			break;
		}
		case 0:
		{
			if (shippingBin && Game1.getFarm().lastItemShipped != null && oldID < 12)
			{
				currentlySnappedComponent = getComponentWithID(12598);
				currentlySnappedComponent.downNeighborID = oldID;
				snapCursorToCurrentSnappedComponent();
				break;
			}
			if (oldID < 53910 && oldID >= 12)
			{
				currentlySnappedComponent = getComponentWithID(oldID - 12);
				break;
			}
			int num = oldID + GetColumnCount() * 2;
			for (int i = 0; i < 3; i++)
			{
				if (ItemsToGrabMenu.inventory.Count > num)
				{
					break;
				}
				num -= GetColumnCount();
			}
			if (showReceivingMenu)
			{
				if (num < 0)
				{
					if (ItemsToGrabMenu.inventory.Count > 0)
					{
						currentlySnappedComponent = getComponentWithID(53910 + ItemsToGrabMenu.inventory.Count - 1);
					}
					else if (discreteColorPickerCC != null)
					{
						currentlySnappedComponent = getComponentWithID(4343);
					}
				}
				else
				{
					currentlySnappedComponent = getComponentWithID(num + 53910);
					if (currentlySnappedComponent == null)
					{
						currentlySnappedComponent = getComponentWithID(53910);
					}
				}
			}
			snapCursorToCurrentSnappedComponent();
			break;
		}
		}
	}

	public override void snapToDefaultClickableComponent()
	{
		if (shippingBin)
		{
			currentlySnappedComponent = getComponentWithID(0);
		}
		else if (source == 1 && sourceItem != null && sourceItem is Chest && (sourceItem as Chest).SpecialChestType == Chest.SpecialChestTypes.MiniShippingBin)
		{
			currentlySnappedComponent = getComponentWithID(0);
		}
		else
		{
			currentlySnappedComponent = getComponentWithID((ItemsToGrabMenu.inventory.Count > 0 && showReceivingMenu) ? 53910 : 0);
		}
		snapCursorToCurrentSnappedComponent();
	}

	public void setSourceItem(Item item)
	{
		sourceItem = item;
		chestColorPicker = null;
		colorPickerToggleButton = null;
		if (source == 1 && sourceItem != null && sourceItem is Chest && (sourceItem as Chest).SpecialChestType == Chest.SpecialChestTypes.None)
		{
			chestColorPicker = new DiscreteColorPicker(xPositionOnScreen, yPositionOnScreen - 64 - IClickableMenu.borderWidth * 2, 0, new Chest(playerChest: true, sourceItem.ParentSheetIndex));
			chestColorPicker.colorSelection = chestColorPicker.getSelectionFromColor((sourceItem as Chest).playerChoiceColor);
			(chestColorPicker.itemToDrawColored as Chest).playerChoiceColor.Value = chestColorPicker.getColorFromSelection(chestColorPicker.colorSelection);
			colorPickerToggleButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width, yPositionOnScreen + height / 3 - 64 + -160, 64, 64), Game1.mouseCursors, new Rectangle(119, 469, 16, 16), 4f)
			{
				hoverText = Game1.content.LoadString("Strings\\UI:Toggle_ColorPicker")
			};
		}
		RepositionSideButtons();
	}

	public override bool IsAutomaticSnapValid(int direction, ClickableComponent a, ClickableComponent b)
	{
		if (direction == 1 && ItemsToGrabMenu.inventory.Contains(a) && inventory.inventory.Contains(b))
		{
			return false;
		}
		return base.IsAutomaticSnapValid(direction, a, b);
	}

	public void setBackgroundTransparency(bool b)
	{
		drawBG = b;
	}

	public void setDestroyItemOnClick(bool b)
	{
		destroyItemOnClick = b;
	}

	public override void receiveRightClick(int x, int y, bool playSound = true)
	{
		if (!allowRightClick)
		{
			receiveRightClickOnlyToolAttachments(x, y);
			return;
		}
		base.receiveRightClick(x, y, playSound && playRightClickSound);
		if (heldItem == null && showReceivingMenu)
		{
			heldItem = ItemsToGrabMenu.rightClick(x, y, heldItem, playSound: false);
			if (heldItem != null && behaviorOnItemGrab != null)
			{
				behaviorOnItemGrab(heldItem, Game1.player);
				if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemGrabMenu)
				{
					(Game1.activeClickableMenu as ItemGrabMenu).setSourceItem(sourceItem);
				}
				if (Game1.options.SnappyMenus)
				{
					(Game1.activeClickableMenu as ItemGrabMenu).currentlySnappedComponent = currentlySnappedComponent;
					(Game1.activeClickableMenu as ItemGrabMenu).snapCursorToCurrentSnappedComponent();
				}
			}
			if (Utility.IsNormalObjectAtParentSheetIndex(heldItem, 326))
			{
				heldItem = null;
				Game1.player.canUnderstandDwarves = true;
				poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2(x - x % 64 + 16, y - y % 64 + 16), flicker: false, flipped: false);
				Game1.playSound("fireball");
			}
			else if (heldItem is Object && Utility.IsNormalObjectAtParentSheetIndex(heldItem, 434))
			{
				Object o = heldItem as Object;
				heldItem = null;
				exitThisMenu(playSound: false);
				Game1.player.eatObject(o, overrideFullness: true);
			}
			else if (heldItem is Object && (bool)(heldItem as Object).isRecipe)
			{
				string key = heldItem.Name.Substring(0, heldItem.Name.IndexOf("Recipe") - 1);
				try
				{
					if ((heldItem as Object).Category == -7)
					{
						Game1.player.cookingRecipes.Add(key, 0);
					}
					else
					{
						Game1.player.craftingRecipes.Add(key, 0);
					}
					poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2(x - x % 64 + 16, y - y % 64 + 16), flicker: false, flipped: false);
					Game1.playSound("newRecipe");
				}
				catch (Exception)
				{
				}
				heldItem = null;
			}
			else if (Game1.player.addItemToInventoryBool(heldItem))
			{
				heldItem = null;
				Game1.playSound("coin");
			}
		}
		else if (reverseGrab || behaviorFunction != null)
		{
			behaviorFunction(heldItem, Game1.player);
			if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemGrabMenu)
			{
				(Game1.activeClickableMenu as ItemGrabMenu).setSourceItem(sourceItem);
			}
			if (destroyItemOnClick)
			{
				heldItem = null;
			}
		}
	}

	public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
	{
		if (snappedtoBottom)
		{
			movePosition((newBounds.Width - oldBounds.Width) / 2, Game1.uiViewport.Height - (yPositionOnScreen + height - IClickableMenu.spaceToClearTopBorder));
		}
		else
		{
			movePosition((newBounds.Width - oldBounds.Width) / 2, (newBounds.Height - oldBounds.Height) / 2);
		}
		if (ItemsToGrabMenu != null)
		{
			ItemsToGrabMenu.gameWindowSizeChanged(oldBounds, newBounds);
		}
		RepositionSideButtons();
		if (source == 1 && sourceItem != null && sourceItem is Chest && (sourceItem as Chest).SpecialChestType == Chest.SpecialChestTypes.None)
		{
			chestColorPicker = new DiscreteColorPicker(xPositionOnScreen, yPositionOnScreen - 64 - IClickableMenu.borderWidth * 2, 0, new Chest(playerChest: true, sourceItem.ParentSheetIndex));
			chestColorPicker.colorSelection = chestColorPicker.getSelectionFromColor((sourceItem as Chest).playerChoiceColor);
		}
	}

	public override void receiveLeftClick(int x, int y, bool playSound = true)
	{
		base.receiveLeftClick(x, y, !destroyItemOnClick);
		if (shippingBin && lastShippedHolder.containsPoint(x, y))
		{
			if (Game1.getFarm().lastItemShipped == null)
			{
				return;
			}
			Game1.getFarm().getShippingBin(Game1.player).Remove(Game1.getFarm().lastItemShipped);
			if (Game1.player.addItemToInventoryBool(Game1.getFarm().lastItemShipped))
			{
				Game1.playSound("coin");
				Game1.getFarm().lastItemShipped = null;
				if (Game1.player.ActiveObject != null)
				{
					Game1.player.showCarrying();
					Game1.player.Halt();
				}
			}
			else
			{
				Game1.getFarm().getShippingBin(Game1.player).Add(Game1.getFarm().lastItemShipped);
			}
			return;
		}
		if (chestColorPicker != null)
		{
			chestColorPicker.receiveLeftClick(x, y);
			if (sourceItem != null && sourceItem is Chest)
			{
				(sourceItem as Chest).playerChoiceColor.Value = chestColorPicker.getColorFromSelection(chestColorPicker.colorSelection);
			}
		}
		if (colorPickerToggleButton != null && colorPickerToggleButton.containsPoint(x, y))
		{
			Game1.player.showChestColorPicker = !Game1.player.showChestColorPicker;
			chestColorPicker.visible = Game1.player.showChestColorPicker;
			try
			{
				Game1.playSound("drumkit6");
			}
			catch (Exception)
			{
			}
			SetupBorderNeighbors();
			return;
		}
		if (whichSpecialButton != -1 && specialButton != null && specialButton.containsPoint(x, y))
		{
			Game1.playSound("drumkit6");
			if (whichSpecialButton == 1 && context != null && context is JunimoHut)
			{
				(context as JunimoHut).noHarvest.Value = !(context as JunimoHut).noHarvest;
				specialButton.sourceRect.X = ((context as JunimoHut).noHarvest ? 124 : 108);
			}
			return;
		}
		if (heldItem == null && showReceivingMenu)
		{
			heldItem = ItemsToGrabMenu.leftClick(x, y, heldItem, playSound: false);
			if (heldItem != null && behaviorOnItemGrab != null)
			{
				behaviorOnItemGrab(heldItem, Game1.player);
				if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemGrabMenu)
				{
					(Game1.activeClickableMenu as ItemGrabMenu).setSourceItem(sourceItem);
					if (Game1.options.SnappyMenus)
					{
						(Game1.activeClickableMenu as ItemGrabMenu).currentlySnappedComponent = currentlySnappedComponent;
						(Game1.activeClickableMenu as ItemGrabMenu).snapCursorToCurrentSnappedComponent();
					}
				}
			}
			if (heldItem is Object && Utility.IsNormalObjectAtParentSheetIndex(heldItem as Object, 326))
			{
				heldItem = null;
				Game1.player.canUnderstandDwarves = true;
				poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2(x - x % 64 + 16, y - y % 64 + 16), flicker: false, flipped: false);
				Game1.playSound("fireball");
			}
			else if (heldItem is Object && Utility.IsNormalObjectAtParentSheetIndex(heldItem as Object, 102))
			{
				heldItem = null;
				Game1.player.foundArtifact(102, 1);
				poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2(x - x % 64 + 16, y - y % 64 + 16), flicker: false, flipped: false);
				Game1.playSound("fireball");
			}
			if (heldItem is Object && Utility.IsNormalObjectAtParentSheetIndex(heldItem, 434))
			{
				Object o = heldItem as Object;
				heldItem = null;
				exitThisMenu(playSound: false);
				Game1.player.eatObject(o, overrideFullness: true);
			}
			else if (heldItem is Object && (bool)(heldItem as Object).isRecipe)
			{
				string key = heldItem.Name.Substring(0, heldItem.Name.IndexOf("Recipe") - 1);
				try
				{
					if ((heldItem as Object).Category == -7)
					{
						Game1.player.cookingRecipes.Add(key, 0);
					}
					else
					{
						Game1.player.craftingRecipes.Add(key, 0);
					}
					poof = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64, 64), 50f, 8, 0, new Vector2(x - x % 64 + 16, y - y % 64 + 16), flicker: false, flipped: false);
					Game1.playSound("newRecipe");
				}
				catch (Exception)
				{
				}
				heldItem = null;
			}
			else if (Game1.player.addItemToInventoryBool(heldItem))
			{
				heldItem = null;
				Game1.playSound("coin");
			}
		}
		else if ((reverseGrab || behaviorFunction != null) && isWithinBounds(x, y))
		{
			behaviorFunction(heldItem, Game1.player);
			if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is ItemGrabMenu)
			{
				(Game1.activeClickableMenu as ItemGrabMenu).setSourceItem(sourceItem);
				if (Game1.options.SnappyMenus)
				{
					(Game1.activeClickableMenu as ItemGrabMenu).currentlySnappedComponent = currentlySnappedComponent;
					(Game1.activeClickableMenu as ItemGrabMenu).snapCursorToCurrentSnappedComponent();
				}
			}
			if (destroyItemOnClick)
			{
				heldItem = null;
				return;
			}
		}
		if (organizeButton != null && organizeButton.containsPoint(x, y))
		{
			ClickableComponent clickableComponent = currentlySnappedComponent;
			organizeItemsInList(ItemsToGrabMenu.actualInventory);
			Item item = heldItem;
			heldItem = null;
			Game1.activeClickableMenu = new ItemGrabMenu(ItemsToGrabMenu.actualInventory, reverseGrab: false, showReceivingMenu: true, InventoryMenu.highlightAllItems, behaviorFunction, null, behaviorOnItemGrab, snapToBottom: false, canBeExitedWithKey: true, playRightClickSound: true, allowRightClick: true, showOrganizeButton: true, source, sourceItem, whichSpecialButton, context).setEssential(essential);
			if (clickableComponent != null)
			{
				Game1.activeClickableMenu.setCurrentlySnappedComponentTo(clickableComponent.myID);
				if (Game1.options.SnappyMenus)
				{
					snapCursorToCurrentSnappedComponent();
				}
			}
			(Game1.activeClickableMenu as ItemGrabMenu).heldItem = item;
			Game1.playSound("Ship");
		}
		else if (fillStacksButton != null && fillStacksButton.containsPoint(x, y))
		{
			FillOutStacks();
			Game1.playSound("Ship");
		}
		else if (junimoNoteIcon != null && junimoNoteIcon.containsPoint(x, y))
		{
			if (readyToClose())
			{
				Game1.activeClickableMenu = new JunimoNoteMenu(fromGameMenu: true);
			}
		}
		else if (heldItem != null && !isWithinBounds(x, y) && heldItem.canBeTrashed())
		{
			DropHeldItem();
		}
	}

	public virtual void DropHeldItem()
	{
		if (heldItem != null)
		{
			Game1.playSound("throwDownITem");
			Console.WriteLine("Dropping " + heldItem.Name);
			int direction = Game1.player.facingDirection;
			if (context is LibraryMuseum)
			{
				direction = 2;
			}
			Game1.createItemDebris(heldItem, Game1.player.getStandingPosition(), direction);
			if (inventory.onAddItem != null)
			{
				inventory.onAddItem(heldItem, Game1.player);
			}
			heldItem = null;
		}
	}

	public void FillOutStacks()
	{
		for (int i = 0; i < ItemsToGrabMenu.actualInventory.Count; i++)
		{
			Item item = ItemsToGrabMenu.actualInventory[i];
			if (item == null || item.maximumStackSize() <= 1)
			{
				continue;
			}
			for (int j = 0; j < inventory.actualInventory.Count; j++)
			{
				Item item2 = inventory.actualInventory[j];
				if (item2 == null || !item.canStackWith(item2))
				{
					continue;
				}
				TransferredItemSprite item3 = new TransferredItemSprite(item2.getOne(), inventory.inventory[j].bounds.X, inventory.inventory[j].bounds.Y);
				_transferredItemSprites.Add(item3);
				int stack = item2.Stack;
				if (item.getRemainingStackSpace() > 0)
				{
					stack = item.addToStack(item2);
					ItemsToGrabMenu.ShakeItem(item);
				}
				item2.Stack = stack;
				while (item2.Stack > 0)
				{
					Item item4 = null;
					if (!Utility.canItemBeAddedToThisInventoryList(item.getOne(), ItemsToGrabMenu.actualInventory, ItemsToGrabMenu.capacity))
					{
						break;
					}
					if (item4 == null)
					{
						for (int k = 0; k < ItemsToGrabMenu.actualInventory.Count; k++)
						{
							if (ItemsToGrabMenu.actualInventory[k] != null && ItemsToGrabMenu.actualInventory[k].canStackWith(item) && ItemsToGrabMenu.actualInventory[k].getRemainingStackSpace() > 0)
							{
								item4 = ItemsToGrabMenu.actualInventory[k];
								break;
							}
						}
					}
					if (item4 == null)
					{
						for (int l = 0; l < ItemsToGrabMenu.actualInventory.Count; l++)
						{
							if (ItemsToGrabMenu.actualInventory[l] == null)
							{
								Item item5 = (ItemsToGrabMenu.actualInventory[l] = item.getOne());
								item4 = item5;
								item4.Stack = 0;
								break;
							}
						}
					}
					if (item4 == null && ItemsToGrabMenu.actualInventory.Count < ItemsToGrabMenu.capacity)
					{
						item4 = item.getOne();
						item4.Stack = 0;
						ItemsToGrabMenu.actualInventory.Add(item4);
					}
					if (item4 == null)
					{
						break;
					}
					stack = item4.addToStack(item2);
					ItemsToGrabMenu.ShakeItem(item4);
					item2.Stack = stack;
				}
				if (item2.Stack == 0)
				{
					inventory.actualInventory[j] = null;
				}
			}
		}
	}

	public static void organizeItemsInList(IList<Item> items)
	{
		List<Item> list = new List<Item>(items);
		List<Item> list2 = new List<Item>();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == null)
			{
				list.RemoveAt(i);
				i--;
			}
			else if (list[i] is Tool)
			{
				list2.Add(list[i]);
				list.RemoveAt(i);
				i--;
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			Item item = list[j];
			if (item.getRemainingStackSpace() <= 0)
			{
				continue;
			}
			for (int k = j + 1; k < list.Count; k++)
			{
				Item item2 = list[k];
				if (item.canStackWith(item2))
				{
					item2.Stack = item.addToStack(item2);
					if (item2.Stack == 0)
					{
						list.RemoveAt(k);
						k--;
					}
				}
			}
		}
		list.Sort();
		list.InsertRange(0, list2);
		for (int l = 0; l < items.Count; l++)
		{
			items[l] = null;
		}
		for (int m = 0; m < list.Count; m++)
		{
			items[m] = list[m];
		}
	}

	public bool areAllItemsTaken()
	{
		for (int i = 0; i < ItemsToGrabMenu.actualInventory.Count; i++)
		{
			if (ItemsToGrabMenu.actualInventory[i] != null)
			{
				return false;
			}
		}
		return true;
	}

	public override void receiveGamePadButton(Buttons b)
	{
		base.receiveGamePadButton(b);
		if (b == Buttons.Back && organizeButton != null)
		{
			organizeItemsInList(Game1.player.items);
			Game1.playSound("Ship");
		}
		if (b == Buttons.RightShoulder)
		{
			ClickableComponent componentWithID = getComponentWithID(12952);
			if (componentWithID != null)
			{
				setCurrentlySnappedComponentTo(componentWithID.myID);
				snapCursorToCurrentSnappedComponent();
			}
			else
			{
				int num = -1;
				ClickableComponent clickableComponent = null;
				foreach (ClickableComponent allClickableComponent in allClickableComponents)
				{
					if (allClickableComponent.region == 15923 && (num == -1 || allClickableComponent.bounds.Y < num))
					{
						num = allClickableComponent.bounds.Y;
						clickableComponent = allClickableComponent;
					}
				}
				if (clickableComponent != null)
				{
					setCurrentlySnappedComponentTo(clickableComponent.myID);
					snapCursorToCurrentSnappedComponent();
				}
			}
		}
		if (shippingBin || b != Buttons.LeftShoulder)
		{
			return;
		}
		ClickableComponent componentWithID2 = getComponentWithID(53910);
		if (componentWithID2 != null)
		{
			setCurrentlySnappedComponentTo(componentWithID2.myID);
			snapCursorToCurrentSnappedComponent();
			return;
		}
		componentWithID2 = getComponentWithID(0);
		if (componentWithID2 != null)
		{
			setCurrentlySnappedComponentTo(0);
			snapCursorToCurrentSnappedComponent();
		}
	}

	public override void receiveKeyPress(Keys key)
	{
		if (Game1.options.snappyMenus && Game1.options.gamepadControls)
		{
			applyMovementKey(key);
		}
		if ((canExitOnKey || areAllItemsTaken()) && Game1.options.doesInputListContain(Game1.options.menuButton, key) && readyToClose())
		{
			exitThisMenu();
			if (Game1.currentLocation.currentEvent != null && Game1.currentLocation.currentEvent.CurrentCommand > 0)
			{
				Game1.currentLocation.currentEvent.CurrentCommand++;
			}
		}
		else if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && heldItem != null)
		{
			Game1.setMousePosition(trashCan.bounds.Center);
		}
		if (key == Keys.Delete && heldItem != null && heldItem.canBeTrashed())
		{
			Utility.trashItem(heldItem);
			heldItem = null;
		}
	}

	public override void update(GameTime time)
	{
		base.update(time);
		if (poof != null && poof.update(time))
		{
			poof = null;
		}
		if (chestColorPicker != null)
		{
			chestColorPicker.update(time);
		}
		if (sourceItem != null && sourceItem is Chest && _sourceItemInCurrentLocation)
		{
			Vector2 vector = (sourceItem as Object).tileLocation;
			if (vector != Vector2.Zero && !Game1.currentLocation.objects.ContainsKey(vector))
			{
				if (Game1.activeClickableMenu != null)
				{
					Game1.activeClickableMenu.emergencyShutDown();
				}
				Game1.exitActiveMenu();
			}
		}
		for (int i = 0; i < _transferredItemSprites.Count; i++)
		{
			if (_transferredItemSprites[i].Update(time))
			{
				_transferredItemSprites.RemoveAt(i);
				i--;
			}
		}
	}

	public override void performHoverAction(int x, int y)
	{
		hoveredItem = null;
		hoverText = "";
		base.performHoverAction(x, y);
		if (colorPickerToggleButton != null)
		{
			colorPickerToggleButton.tryHover(x, y, 0.25f);
			if (colorPickerToggleButton.containsPoint(x, y))
			{
				hoverText = colorPickerToggleButton.hoverText;
			}
		}
		if (organizeButton != null)
		{
			organizeButton.tryHover(x, y, 0.25f);
			if (organizeButton.containsPoint(x, y))
			{
				hoverText = organizeButton.hoverText;
			}
		}
		if (fillStacksButton != null)
		{
			fillStacksButton.tryHover(x, y, 0.25f);
			if (fillStacksButton.containsPoint(x, y))
			{
				hoverText = fillStacksButton.hoverText;
			}
		}
		if (specialButton != null)
		{
			specialButton.tryHover(x, y, 0.25f);
		}
		if (showReceivingMenu)
		{
			Item item = ItemsToGrabMenu.hover(x, y, heldItem);
			if (item != null)
			{
				hoveredItem = item;
			}
		}
		if (junimoNoteIcon != null)
		{
			junimoNoteIcon.tryHover(x, y);
			if (junimoNoteIcon.containsPoint(x, y))
			{
				hoverText = junimoNoteIcon.hoverText;
			}
			if (GameMenu.bundleItemHovered)
			{
				junimoNoteIcon.scale = junimoNoteIcon.baseScale + (float)Math.Sin((float)junimoNotePulser / 100f) / 4f;
				junimoNotePulser += (int)Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
			}
			else
			{
				junimoNotePulser = 0;
				junimoNoteIcon.scale = junimoNoteIcon.baseScale;
			}
		}
		if (hoverText != null)
		{
			return;
		}
		if (organizeButton != null)
		{
			hoverText = null;
			organizeButton.tryHover(x, y);
			if (organizeButton.containsPoint(x, y))
			{
				hoverText = organizeButton.hoverText;
			}
		}
		if (shippingBin)
		{
			hoverText = null;
			if (lastShippedHolder.containsPoint(x, y) && Game1.getFarm().lastItemShipped != null)
			{
				hoverText = lastShippedHolder.hoverText;
			}
		}
		if (chestColorPicker != null)
		{
			chestColorPicker.performHoverAction(x, y);
		}
	}

	public override void draw(SpriteBatch b)
	{
		if (drawBG)
		{
			b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);
		}
		base.draw(b, drawUpperPortion: false, drawDescriptionArea: false);
		if (showReceivingMenu)
		{
			b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen - 64, yPositionOnScreen + height / 2 + 64 + 16), new Rectangle(16, 368, 12, 16), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
			b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen - 64, yPositionOnScreen + height / 2 + 64 - 16), new Rectangle(21, 368, 11, 16), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
			b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen - 40, yPositionOnScreen + height / 2 + 64 - 44), new Rectangle(4, 372, 8, 11), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
			if ((source != 1 || sourceItem == null || !(sourceItem is Chest) || ((sourceItem as Chest).SpecialChestType != Chest.SpecialChestTypes.MiniShippingBin && (sourceItem as Chest).SpecialChestType != Chest.SpecialChestTypes.JunimoChest && (sourceItem as Chest).SpecialChestType != Chest.SpecialChestTypes.Enricher)) && source != 0)
			{
				b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen - 72, yPositionOnScreen + 64 + 16), new Rectangle(16, 368, 12, 16), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
				b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen - 72, yPositionOnScreen + 64 - 16), new Rectangle(21, 368, 11, 16), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
				Rectangle value = new Rectangle(127, 412, 10, 11);
				switch (source)
				{
				case 3:
					value.X += 10;
					break;
				case 2:
					value.X += 20;
					break;
				}
				b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen - 52, yPositionOnScreen + 64 - 44), value, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
			}
			Game1.drawDialogueBox(ItemsToGrabMenu.xPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder, ItemsToGrabMenu.yPositionOnScreen - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder, ItemsToGrabMenu.width + IClickableMenu.borderWidth * 2 + IClickableMenu.spaceToClearSideBorder * 2, ItemsToGrabMenu.height + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth * 2, speaker: false, drawOnlyBox: true);
			ItemsToGrabMenu.draw(b);
		}
		else if (message != null)
		{
			Game1.drawDialogueBox(Game1.uiViewport.Width / 2, ItemsToGrabMenu.yPositionOnScreen + ItemsToGrabMenu.height / 2, speaker: false, drawOnlyBox: false, message);
		}
		if (poof != null)
		{
			poof.draw(b, localPosition: true);
		}
		foreach (TransferredItemSprite transferredItemSprite in _transferredItemSprites)
		{
			transferredItemSprite.Draw(b);
		}
		if (shippingBin && Game1.getFarm().lastItemShipped != null)
		{
			lastShippedHolder.draw(b);
			Game1.getFarm().lastItemShipped.drawInMenu(b, new Vector2(lastShippedHolder.bounds.X + 16, lastShippedHolder.bounds.Y + 16), 1f);
			b.Draw(Game1.mouseCursors, new Vector2(lastShippedHolder.bounds.X + -8, lastShippedHolder.bounds.Bottom - 100), new Rectangle(325, 448, 5, 14), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
			b.Draw(Game1.mouseCursors, new Vector2(lastShippedHolder.bounds.X + 84, lastShippedHolder.bounds.Bottom - 100), new Rectangle(325, 448, 5, 14), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
			b.Draw(Game1.mouseCursors, new Vector2(lastShippedHolder.bounds.X + -8, lastShippedHolder.bounds.Bottom - 44), new Rectangle(325, 452, 5, 13), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
			b.Draw(Game1.mouseCursors, new Vector2(lastShippedHolder.bounds.X + 84, lastShippedHolder.bounds.Bottom - 44), new Rectangle(325, 452, 5, 13), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
		}
		if (colorPickerToggleButton != null)
		{
			colorPickerToggleButton.draw(b);
		}
		else if (specialButton != null)
		{
			specialButton.draw(b);
		}
		if (chestColorPicker != null)
		{
			chestColorPicker.draw(b);
		}
		if (organizeButton != null)
		{
			organizeButton.draw(b);
		}
		if (fillStacksButton != null)
		{
			fillStacksButton.draw(b);
		}
		if (junimoNoteIcon != null)
		{
			junimoNoteIcon.draw(b);
		}
		if (hoverText != null && (hoveredItem == null || hoveredItem == null || ItemsToGrabMenu == null))
		{
			if (hoverAmount > 0)
			{
				IClickableMenu.drawToolTip(b, hoverText, "", null, heldItem: true, -1, 0, -1, -1, null, hoverAmount);
			}
			else
			{
				IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont);
			}
		}
		if (hoveredItem != null)
		{
			IClickableMenu.drawToolTip(b, hoveredItem.getDescription(), hoveredItem.DisplayName, hoveredItem, heldItem != null);
		}
		else if (hoveredItem != null && ItemsToGrabMenu != null)
		{
			IClickableMenu.drawToolTip(b, ItemsToGrabMenu.descriptionText, ItemsToGrabMenu.descriptionTitle, hoveredItem, heldItem != null);
		}
		if (heldItem != null)
		{
			heldItem.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);
		}
		Game1.mouseCursorTransparency = 1f;
		drawMouse(b);
	}

	public override void emergencyShutDown()
	{
		base.emergencyShutDown();
		Console.WriteLine("ItemGrabMenu.emergencyShutDown");
		if (heldItem != null)
		{
			Console.WriteLine("Taking " + heldItem.Name);
			heldItem = Game1.player.addItemToInventory(heldItem);
		}
		if (heldItem != null)
		{
			DropHeldItem();
		}
		if (essential)
		{
			Console.WriteLine("essential");
			{
				foreach (Item item2 in ItemsToGrabMenu.actualInventory)
				{
					if (item2 != null)
					{
						Console.WriteLine("Taking " + item2.Name);
						Item item = Game1.player.addItemToInventory(item2);
						if (item != null)
						{
							Console.WriteLine("Dropping " + item.Name);
							Game1.createItemDebris(item, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
						}
					}
				}
				return;
			}
		}
		Console.WriteLine("essential");
	}
}
