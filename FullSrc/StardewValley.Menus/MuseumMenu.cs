using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using xTile.Dimensions;

namespace StardewValley.Menus;

public class MuseumMenu : MenuWithInventory
{
	public const int startingState = 0;

	public const int placingInMuseumState = 1;

	public const int exitingState = 2;

	public int fadeTimer;

	public int state;

	public int menuPositionOffset;

	public bool fadeIntoBlack;

	public bool menuMovingDown;

	public float blackFadeAlpha;

	public SparklingText sparkleText;

	public Vector2 globalLocationOfSparklingArtifact;

	private bool holdingMuseumPiece;

	public bool reOrganizing;

	public MuseumMenu(InventoryMenu.highlightThisItem highlighterMethod)
		: base(highlighterMethod, okButton: true)
	{
		fadeTimer = 800;
		fadeIntoBlack = true;
		movePosition(0, Game1.uiViewport.Height - yPositionOnScreen - height);
		Game1.player.forceCanMove();
		if (Game1.options.SnappyMenus)
		{
			if (okButton != null)
			{
				okButton.myID = 106;
			}
			populateClickableComponentList();
			currentlySnappedComponent = getComponentWithID(0);
			snapCursorToCurrentSnappedComponent();
		}
		Game1.displayHUD = false;
	}

	public override bool shouldClampGamePadCursor()
	{
		return true;
	}

	public override void receiveKeyPress(Keys key)
	{
		if (fadeTimer > 0)
		{
			return;
		}
		if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && !Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.menuButton) && readyToClose())
		{
			state = 2;
			fadeTimer = 500;
			fadeIntoBlack = true;
		}
		else if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && !Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.menuButton) && !holdingMuseumPiece && menuMovingDown)
		{
			if (heldItem != null)
			{
				Game1.playSound("bigDeSelect");
				Utility.CollectOrDrop(heldItem);
				heldItem = null;
			}
			ReturnToDonatableItems();
		}
		else if (Game1.options.SnappyMenus && heldItem == null && !reOrganizing)
		{
			base.receiveKeyPress(key);
		}
		if (!Game1.options.SnappyMenus)
		{
			if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
			{
				Game1.panScreen(0, 4);
			}
			else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
			{
				Game1.panScreen(4, 0);
			}
			else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
			{
				Game1.panScreen(0, -4);
			}
			else if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
			{
				Game1.panScreen(-4, 0);
			}
		}
		else
		{
			if (heldItem == null && !reOrganizing)
			{
				return;
			}
			LibraryMuseum libraryMuseum = Game1.currentLocation as LibraryMuseum;
			Vector2 vector = new Vector2((int)((Utility.ModifyCoordinateFromUIScale(Game1.getMouseX()) + (float)Game1.viewport.X) / 64f), (int)((Utility.ModifyCoordinateFromUIScale(Game1.getMouseY()) + (float)Game1.viewport.Y) / 64f));
			if (!libraryMuseum.isTileSuitableForMuseumPiece((int)vector.X, (int)vector.Y) && (!reOrganizing || !libraryMuseum.museumPieces.ContainsKey(vector)))
			{
				vector = libraryMuseum.getFreeDonationSpot();
				Game1.setMousePosition((int)Utility.ModifyCoordinateForUIScale(vector.X * 64f - (float)Game1.viewport.X + 32f), (int)Utility.ModifyCoordinateForUIScale(vector.Y * 64f - (float)Game1.viewport.Y + 32f));
				return;
			}
			if (key == Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveUpButton))
			{
				vector = libraryMuseum.findMuseumPieceLocationInDirection(vector, 0, 21, !reOrganizing);
			}
			else if (key == Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveRightButton))
			{
				vector = libraryMuseum.findMuseumPieceLocationInDirection(vector, 1, 21, !reOrganizing);
			}
			else if (key == Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveDownButton))
			{
				vector = libraryMuseum.findMuseumPieceLocationInDirection(vector, 2, 21, !reOrganizing);
			}
			else if (key == Game1.options.getFirstKeyboardKeyFromInputButtonList(Game1.options.moveLeftButton))
			{
				vector = libraryMuseum.findMuseumPieceLocationInDirection(vector, 3, 21, !reOrganizing);
			}
			if (!Game1.viewport.Contains(new Location((int)(vector.X * 64f + 32f), Game1.viewport.Y + 1)))
			{
				Game1.panScreen((int)(vector.X * 64f - (float)Game1.viewport.X), 0);
			}
			else if (!Game1.viewport.Contains(new Location(Game1.viewport.X + 1, (int)(vector.Y * 64f + 32f))))
			{
				Game1.panScreen(0, (int)(vector.Y * 64f - (float)Game1.viewport.Y));
			}
			Game1.setMousePosition((int)Utility.ModifyCoordinateForUIScale((int)vector.X * 64 - Game1.viewport.X + 32), (int)Utility.ModifyCoordinateForUIScale((int)vector.Y * 64 - Game1.viewport.Y + 32));
		}
	}

	public override bool overrideSnappyMenuCursorMovementBan()
	{
		return false;
	}

	public override void receiveGamePadButton(Buttons b)
	{
		if (b == Buttons.B)
		{
			if (!holdingMuseumPiece)
			{
				_ = fadeTimer;
				_ = 0;
			}
		}
		else if (!menuMovingDown && (b == Buttons.DPadUp || b == Buttons.LeftThumbstickUp) && Game1.options.SnappyMenus && currentlySnappedComponent != null && currentlySnappedComponent.myID < 12)
		{
			reOrganizing = true;
			menuMovingDown = true;
			receiveKeyPress(Game1.options.moveUpButton[0].key);
		}
	}

	public override void receiveLeftClick(int x, int y, bool playSound = true)
	{
		if (fadeTimer > 0)
		{
			return;
		}
		Item item = heldItem;
		if (!holdingMuseumPiece)
		{
			int inventoryPositionOfClick = inventory.getInventoryPositionOfClick(x, y);
			if (heldItem == null)
			{
				if (inventoryPositionOfClick >= 0 && inventoryPositionOfClick < inventory.actualInventory.Count && inventory.highlightMethod(inventory.actualInventory[inventoryPositionOfClick]))
				{
					heldItem = inventory.actualInventory[inventoryPositionOfClick].getOne();
					inventory.actualInventory[inventoryPositionOfClick].Stack--;
					if (inventory.actualInventory[inventoryPositionOfClick].Stack <= 0)
					{
						inventory.actualInventory[inventoryPositionOfClick] = null;
					}
				}
			}
			else
			{
				heldItem = inventory.leftClick(x, y, heldItem);
			}
		}
		if (item == null && heldItem != null && Game1.isAnyGamePadButtonBeingPressed())
		{
			receiveGamePadButton(Buttons.DPadUp);
		}
		if (item != null && heldItem != null && (y < Game1.viewport.Height - (height - (IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 192)) || menuMovingDown))
		{
			int num = (int)(Utility.ModifyCoordinateFromUIScale(x) + (float)Game1.viewport.X) / 64;
			int num2 = (int)(Utility.ModifyCoordinateFromUIScale(y) + (float)Game1.viewport.Y) / 64;
			if ((Game1.currentLocation as LibraryMuseum).isTileSuitableForMuseumPiece(num, num2) && (Game1.currentLocation as LibraryMuseum).isItemSuitableForDonation(heldItem))
			{
				int num3 = heldItem.parentSheetIndex;
				int count = (Game1.currentLocation as LibraryMuseum).getRewardsForPlayer(Game1.player).Count;
				(Game1.currentLocation as LibraryMuseum).museumPieces.Add(new Vector2(num, num2), (heldItem as Object).parentSheetIndex);
				Game1.playSound("stoneStep");
				if ((Game1.currentLocation as LibraryMuseum).getRewardsForPlayer(Game1.player).Count > count && !holdingMuseumPiece)
				{
					sparkleText = new SparklingText(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:NewReward"), Color.MediumSpringGreen, Color.White);
					Game1.playSound("reward");
					globalLocationOfSparklingArtifact = new Vector2((float)(num * 64 + 32) - sparkleText.textWidth / 2f, num2 * 64 - 48);
				}
				else
				{
					Game1.playSound("newArtifact");
				}
				Game1.player.completeQuest(24);
				heldItem.Stack--;
				if (heldItem.Stack <= 0)
				{
					heldItem = null;
				}
				int num4 = (Game1.currentLocation as LibraryMuseum).museumPieces.Count();
				if (!holdingMuseumPiece)
				{
					Game1.stats.checkForArchaeologyAchievements();
					switch (num4)
					{
					case 95:
						Game1.multiplayer.globalChatInfoMessage("MuseumComplete", Game1.player.farmName);
						break;
					case 40:
						Game1.multiplayer.globalChatInfoMessage("Museum40", Game1.player.farmName);
						break;
					default:
						Game1.multiplayer.globalChatInfoMessage("donation", Game1.player.name, "object:" + num3);
						break;
					}
				}
				ReturnToDonatableItems();
			}
		}
		else if (heldItem == null && !inventory.isWithinBounds(x, y))
		{
			int num5 = (int)(Utility.ModifyCoordinateFromUIScale(x) + (float)Game1.viewport.X) / 64;
			int num6 = (int)(Utility.ModifyCoordinateFromUIScale(y) + (float)Game1.viewport.Y) / 64;
			Vector2 key = new Vector2(num5, num6);
			LibraryMuseum libraryMuseum = Game1.currentLocation as LibraryMuseum;
			if (libraryMuseum.museumPieces.ContainsKey(key))
			{
				heldItem = new Object(libraryMuseum.museumPieces[key], 1);
				libraryMuseum.museumPieces.Remove(key);
				holdingMuseumPiece = !libraryMuseum.museumAlreadyHasArtifact(heldItem.parentSheetIndex);
			}
		}
		if (heldItem != null && item == null)
		{
			menuMovingDown = true;
			reOrganizing = false;
		}
		if (okButton != null && okButton.containsPoint(x, y) && readyToClose())
		{
			if (fadeTimer <= 0)
			{
				Game1.playSound("bigDeSelect");
			}
			state = 2;
			fadeTimer = 800;
			fadeIntoBlack = true;
		}
	}

	public virtual void ReturnToDonatableItems()
	{
		menuMovingDown = false;
		holdingMuseumPiece = false;
		reOrganizing = false;
		if (Game1.options.SnappyMenus)
		{
			movePosition(0, -menuPositionOffset);
			menuPositionOffset = 0;
			base.snapCursorToCurrentSnappedComponent();
		}
	}

	public override bool readyToClose()
	{
		if (!holdingMuseumPiece && heldItem == null)
		{
			return !menuMovingDown;
		}
		return false;
	}

	protected override void cleanupBeforeExit()
	{
		if (heldItem != null)
		{
			heldItem = Game1.player.addItemToInventory(heldItem);
			if (heldItem != null)
			{
				Game1.createItemDebris(heldItem, Game1.player.Position, -1);
				heldItem = null;
			}
		}
		Game1.displayHUD = true;
	}

	public override void receiveRightClick(int x, int y, bool playSound = true)
	{
		Item item = heldItem;
		if (fadeTimer <= 0)
		{
			base.receiveRightClick(x, y, playSound: true);
		}
		if (heldItem != null && item == null)
		{
			menuMovingDown = true;
		}
	}

	public override void update(GameTime time)
	{
		base.update(time);
		if (sparkleText != null && sparkleText.update(time))
		{
			sparkleText = null;
		}
		if (fadeTimer > 0)
		{
			fadeTimer -= time.ElapsedGameTime.Milliseconds;
			if (fadeIntoBlack)
			{
				blackFadeAlpha = 0f + (1500f - (float)fadeTimer) / 1500f;
			}
			else
			{
				blackFadeAlpha = 1f - (1500f - (float)fadeTimer) / 1500f;
			}
			if (fadeTimer <= 0)
			{
				switch (state)
				{
				case 0:
					state = 1;
					Game1.viewportFreeze = true;
					Game1.viewport.Location = new Location(1152, 128);
					Game1.clampViewportToGameMap();
					fadeTimer = 800;
					fadeIntoBlack = false;
					break;
				case 2:
					Game1.viewportFreeze = false;
					fadeIntoBlack = false;
					fadeTimer = 800;
					state = 3;
					break;
				case 3:
					exitThisMenuNoSound();
					break;
				}
			}
		}
		if (menuMovingDown && menuPositionOffset < height / 3)
		{
			menuPositionOffset += 8;
			movePosition(0, 8);
		}
		else if (!menuMovingDown && menuPositionOffset > 0)
		{
			menuPositionOffset -= 8;
			movePosition(0, -8);
		}
		int num = Game1.getOldMouseX(ui_scale: false) + Game1.viewport.X;
		int num2 = Game1.getOldMouseY(ui_scale: false) + Game1.viewport.Y;
		if ((!Game1.options.SnappyMenus && Game1.lastCursorMotionWasMouse && num - Game1.viewport.X < 64) || Game1.input.GetGamePadState().ThumbSticks.Right.X < 0f)
		{
			Game1.panScreen(-4, 0);
			if (Game1.input.GetGamePadState().ThumbSticks.Right.X < 0f)
			{
				snapCursorToCurrentMuseumSpot();
			}
		}
		else if ((!Game1.options.SnappyMenus && Game1.lastCursorMotionWasMouse && num - (Game1.viewport.X + Game1.viewport.Width) >= -64) || Game1.input.GetGamePadState().ThumbSticks.Right.X > 0f)
		{
			Game1.panScreen(4, 0);
			if (Game1.input.GetGamePadState().ThumbSticks.Right.X > 0f)
			{
				snapCursorToCurrentMuseumSpot();
			}
		}
		if ((!Game1.options.SnappyMenus && Game1.lastCursorMotionWasMouse && num2 - Game1.viewport.Y < 64) || Game1.input.GetGamePadState().ThumbSticks.Right.Y > 0f)
		{
			Game1.panScreen(0, -4);
			if (Game1.input.GetGamePadState().ThumbSticks.Right.Y > 0f)
			{
				snapCursorToCurrentMuseumSpot();
			}
		}
		else if ((!Game1.options.SnappyMenus && Game1.lastCursorMotionWasMouse && num2 - (Game1.viewport.Y + Game1.viewport.Height) >= -64) || Game1.input.GetGamePadState().ThumbSticks.Right.Y < 0f)
		{
			Game1.panScreen(0, 4);
			if (Game1.input.GetGamePadState().ThumbSticks.Right.Y < 0f)
			{
				snapCursorToCurrentMuseumSpot();
			}
		}
		Keys[] pressedKeys = Game1.oldKBState.GetPressedKeys();
		foreach (Keys key in pressedKeys)
		{
			receiveKeyPress(key);
		}
	}

	private void snapCursorToCurrentMuseumSpot()
	{
		if (menuMovingDown)
		{
			Vector2 vector = new Vector2((Game1.getMouseX(ui_scale: false) + Game1.viewport.X) / 64, (Game1.getMouseY(ui_scale: false) + Game1.viewport.Y) / 64);
			Game1.setMousePosition((int)vector.X * 64 - Game1.viewport.X + 32, (int)vector.Y * 64 - Game1.viewport.Y + 32, ui_scale: false);
		}
	}

	public override void gameWindowSizeChanged(Microsoft.Xna.Framework.Rectangle oldBounds, Microsoft.Xna.Framework.Rectangle newBounds)
	{
		base.gameWindowSizeChanged(oldBounds, newBounds);
		movePosition(0, Game1.viewport.Height - yPositionOnScreen - height);
		Game1.player.forceCanMove();
	}

	public override void draw(SpriteBatch b)
	{
		if ((fadeTimer <= 0 || !fadeIntoBlack) && state != 3)
		{
			if (heldItem != null)
			{
				Game1.StartWorldDrawInUI(b);
				for (int i = Game1.viewport.Y / 64 - 1; i < (Game1.viewport.Y + Game1.viewport.Height) / 64 + 2; i++)
				{
					for (int j = Game1.viewport.X / 64 - 1; j < (Game1.viewport.X + Game1.viewport.Width) / 64 + 1; j++)
					{
						if ((Game1.currentLocation as LibraryMuseum).isTileSuitableForMuseumPiece(j, i))
						{
							b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(j, i) * 64f), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 29), Color.LightGreen);
						}
					}
				}
				Game1.EndWorldDrawInUI(b);
			}
			if (!holdingMuseumPiece)
			{
				base.draw(b, drawUpperPortion: false, drawDescriptionArea: false);
			}
			if (!hoverText.Equals(""))
			{
				IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont);
			}
			if (heldItem != null)
			{
				heldItem.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);
			}
			drawMouse(b);
			if (sparkleText != null)
			{
				sparkleText.draw(b, Utility.ModifyCoordinatesForUIScale(Game1.GlobalToLocal(Game1.viewport, globalLocationOfSparklingArtifact)));
			}
		}
		b.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * blackFadeAlpha);
	}
}
