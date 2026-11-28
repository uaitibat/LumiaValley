using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.GameData;
using StardewValley.Network;
using StardewValley.SDKs;

namespace StardewValley.Menus;

public class CoopMenu : LoadGameMenu
{
	public enum Tab
	{
		JOIN_TAB,
		HOST_TAB
	}

	protected abstract class CoopMenuSlot : MenuSlot
	{
		protected new CoopMenu menu;

		public CoopMenuSlot(CoopMenu menu)
			: base(menu)
		{
			this.menu = menu;
		}
	}

	protected abstract class LabeledSlot : CoopMenuSlot
	{
		private string message;

		public LabeledSlot(CoopMenu menu, string message)
			: base(menu)
		{
			this.message = message;
		}

		public abstract override void Activate();

		public override void Draw(SpriteBatch b, int i)
		{
			int widthOfString = SpriteText.getWidthOfString(message);
			int heightOfString = SpriteText.getHeightOfString(message);
			Rectangle bounds = menu.slotButtons[i].bounds;
			int x = bounds.X + (bounds.Width - widthOfString) / 2;
			int y = bounds.Y + (bounds.Height - heightOfString) / 2;
			SpriteText.drawString(b, message, x, y);
		}
	}

	protected class LanSlot : LabeledSlot
	{
		public LanSlot(CoopMenu menu)
			: base(menu, Game1.content.LoadString("Strings\\UI:CoopMenu_JoinLANGame"))
		{
		}

		public override void Activate()
		{
			menu.enterIPPressed();
		}
	}

	protected class InviteCodeSlot : LabeledSlot
	{
		public InviteCodeSlot(CoopMenu menu)
			: base(menu, Game1.content.LoadString("Strings\\UI:CoopMenu_EnterInviteCode"))
		{
		}

		public override void Activate()
		{
			menu.enterInviteCodePressed();
		}
	}

	protected class HostNewFarmSlot : LabeledSlot
	{
		public HostNewFarmSlot(CoopMenu menu)
			: base(menu, Game1.content.LoadString("Strings\\UI:CoopMenu_HostNewFarm"))
		{
			ActivateDelay = 2150;
		}

		public override void Activate()
		{
			Game1.resetPlayer();
			TitleMenu.subMenu = new CharacterCustomization(CharacterCustomization.Source.HostNewFarm);
			Game1.changeMusicTrack("CloudCountry");
		}
	}

	protected class TooManyFarmsSlot : LabeledSlot
	{
		public TooManyFarmsSlot(CoopMenu menu)
			: base(menu, Game1.content.LoadString("Strings\\UI:TooManyFarmsMenu_TooManyFarms"))
		{
		}

		public override void Activate()
		{
		}
	}

	protected class HostFileSlot : SaveFileSlot
	{
		protected new CoopMenu menu;

		public HostFileSlot(CoopMenu menu, Farmer farmer)
			: base(menu, farmer)
		{
			this.menu = menu;
		}

		public override void Activate()
		{
			Game1.multiplayerMode = 2;
			base.Activate();
		}

		protected override void drawSlotSaveNumber(SpriteBatch b, int i)
		{
		}

		protected override string slotName()
		{
			return Game1.content.LoadString("Strings\\UI:CoopMenu_HostFile", Farmer.Name, Farmer.farmName.Value);
		}

		protected override string slotSubName()
		{
			return Farmer.Name;
		}

		protected override Vector2 portraitOffset()
		{
			return base.portraitOffset() - new Vector2(32f, 0f);
		}
	}

	protected class FriendFarmData
	{
		public object Lobby;

		public string OwnerName;

		public string FarmName;

		public int FarmType;

		public ModFarmType ModFarmType;

		public WorldDate Date;

		public bool PreviouslyJoined;

		public string ProtocolVersion;
	}

	protected class FriendFarmSlot : CoopMenuSlot
	{
		public FriendFarmData Farm;

		public FriendFarmSlot(CoopMenu menu, FriendFarmData farm)
			: base(menu)
		{
			Farm = farm;
		}

		public bool MatchAddress(object Lobby)
		{
			return object.Equals(Farm.Lobby, Lobby);
		}

		public void Update(FriendFarmData newData)
		{
			Farm = newData;
		}

		public override void Activate()
		{
			menu.setMenu(new FarmhandMenu(Program.sdk.Networking.CreateClient(Farm.Lobby)));
		}

		protected virtual string slotName()
		{
			string path = (Farm.PreviouslyJoined ? "Strings\\UI:CoopMenu_RevisitFriendFarm" : "Strings\\UI:CoopMenu_JoinFriendFarm");
			return Game1.content.LoadString(path, Farm.FarmName);
		}

		protected virtual void drawSlotName(SpriteBatch b, int i)
		{
			SpriteText.drawString(b, slotName(), menu.slotButtons[i].bounds.X + 128 + 36, menu.slotButtons[i].bounds.Y + 36);
		}

		protected virtual void drawSlotDate(SpriteBatch b, int i)
		{
			Utility.drawTextWithShadow(b, Farm.Date.Localize(), Game1.dialogueFont, new Vector2(menu.slotButtons[i].bounds.X + 128 + 32, menu.slotButtons[i].bounds.Y + 64 + 40), Game1.textColor);
		}

		protected virtual void drawSlotFarm(SpriteBatch b, int i)
		{
			int num = Farm.FarmType;
			if (num == 7)
			{
				num = 0;
			}
			Rectangle value = new Rectangle(22 * (num % 5), 324 + 21 * (num / 5), 22, 20);
			Texture2D mouseCursors = Game1.mouseCursors;
			Rectangle rectangle = new Rectangle(menu.slotButtons[i].bounds.X, menu.slotButtons[i].bounds.Y, 160, menu.slotButtons[i].bounds.Height);
			Rectangle destinationRectangle = new Rectangle(rectangle.X + (rectangle.Width - value.Width * 4) / 2, rectangle.Y + (rectangle.Height - value.Height * 4) / 2, value.Width * 4, value.Height * 4);
			if (Farm.ModFarmType != null && Farm.ModFarmType.IconTexture != null)
			{
				mouseCursors = Game1.content.Load<Texture2D>(Farm.ModFarmType.IconTexture);
				b.Draw(mouseCursors, destinationRectangle, null, Color.White);
			}
			else
			{
				b.Draw(mouseCursors, destinationRectangle, value, Color.White);
			}
		}

		protected virtual void drawSlotOwnerName(SpriteBatch b, int i)
		{
			float num = 1f;
			float num2 = 128f;
			float num3 = 44f;
			Utility.drawTextWithShadow(b, Farm.OwnerName, Game1.dialogueFont, new Vector2((float)(menu.slotButtons[i].bounds.X + menu.width) - num2 - Game1.dialogueFont.MeasureString(Farm.OwnerName).X * num, (float)menu.slotButtons[i].bounds.Y + num3), Game1.textColor, num);
		}

		public override void Draw(SpriteBatch b, int i)
		{
			drawSlotName(b, i);
			drawSlotDate(b, i);
			drawSlotFarm(b, i);
			drawSlotOwnerName(b, i);
		}
	}

	private class LobbyUpdateCallback : LobbyUpdateListener
	{
		private Action<object> callback;

		public LobbyUpdateCallback(Action<object> callback)
		{
			this.callback = callback;
		}

		public void OnLobbyUpdate(object lobby)
		{
			if (callback != null)
			{
				callback(lobby);
			}
		}
	}

	public const int region_refresh = 810;

	public const int region_joinTab = 811;

	public const int region_hostTab = 812;

	public const int region_tabs = 1000;

	protected List<MenuSlot> hostSlots = new List<MenuSlot>();

	public ClickableComponent refreshButton;

	public ClickableComponent joinTab;

	public ClickableComponent hostTab;

	private LobbyUpdateListener lobbyUpdateListener;

	private Tab currentTab;

	private bool smallScreenFormat;

	private bool isSetUp;

	private int updateCounter;

	private float _refreshDelay = -1f;

	private bool _tooManyFarms;

	public static string lastEnteredInviteCode;

	private StringBuilder _stringBuilder = new StringBuilder();

	protected override List<MenuSlot> MenuSlots
	{
		get
		{
			if (currentTab == Tab.JOIN_TAB)
			{
				return menuSlots;
			}
			if (currentTab == Tab.HOST_TAB)
			{
				return hostSlots;
			}
			return null;
		}
		set
		{
			if (currentTab == Tab.JOIN_TAB)
			{
				menuSlots = value;
			}
			else if (currentTab == Tab.HOST_TAB)
			{
				hostSlots = value;
			}
		}
	}

	public CoopMenu(bool tooManyFarms)
	{
		_tooManyFarms = tooManyFarms;
	}

	public override bool readyToClose()
	{
		if (isSetUp)
		{
			return base.readyToClose();
		}
		return true;
	}

	protected override bool hasDeleteButtons()
	{
		return false;
	}

	protected override void startListPopulation()
	{
	}

	protected virtual void connectionFinished()
	{
		string text = Game1.content.LoadString("Strings\\UI:CoopMenu_Refresh");
		int num = (int)Game1.dialogueFont.MeasureString(text).X + 64;
		Vector2 vector = new Vector2(backButton.bounds.Right - num, backButton.bounds.Y - 128);
		refreshButton = new ClickableComponent(new Rectangle((int)vector.X, (int)vector.Y, num, 96), "", text)
		{
			myID = 810,
			upNeighborID = -99998,
			leftNeighborID = -99998,
			rightNeighborID = -99998,
			downNeighborID = 81114
		};
		_refreshDelay = 8f;
		smallScreenFormat = Game1.graphics.GraphicsDevice.Viewport.Height < 1080;
		text = Game1.content.LoadString("Strings\\UI:CoopMenu_Join");
		num = (int)Game1.dialogueFont.MeasureString(text).X + 64;
		vector = (smallScreenFormat ? new Vector2(xPositionOnScreen, yPositionOnScreen) : new Vector2(xPositionOnScreen + IClickableMenu.borderWidth, yPositionOnScreen - 96));
		joinTab = new ClickableComponent(new Rectangle((int)vector.X, (int)vector.Y, num, smallScreenFormat ? 72 : 64), "", text)
		{
			myID = 811,
			downNeighborID = -99998,
			rightNeighborID = 812,
			region = 1000
		};
		text = Game1.content.LoadString("Strings\\UI:CoopMenu_Host");
		num = (int)Game1.dialogueFont.MeasureString(text).X + 64;
		vector = (smallScreenFormat ? new Vector2(joinTab.bounds.Right + ((!smallScreenFormat) ? 4 : 0), yPositionOnScreen) : new Vector2(joinTab.bounds.Right + 4, yPositionOnScreen - 64));
		hostTab = new ClickableComponent(new Rectangle((int)vector.X, (int)vector.Y, num, smallScreenFormat ? 72 : 64), "", text)
		{
			myID = 812,
			downNeighborID = -99998,
			leftNeighborID = 811,
			rightNeighborID = 800,
			region = 1000
		};
		backButton.upNeighborID = 810;
		if (_tooManyFarms)
		{
			hostSlots.Add(new TooManyFarmsSlot(this));
		}
		else
		{
			hostSlots.Add(new HostNewFarmSlot(this));
		}
		menuSlots.Add(new LanSlot(this));
		if (Program.sdk.Networking != null && Program.sdk.Networking.SupportsInviteCodes())
		{
			menuSlots.Add(new InviteCodeSlot(this));
		}
		SetTab(currentTab, play_sound: false);
		isSetUp = true;
		base.startListPopulation();
		populateClickableComponentList();
	}

	public override void receiveGamePadButton(Buttons b)
	{
		base.receiveGamePadButton(b);
		if (!IsDoingTask())
		{
			if (b == Buttons.LeftTrigger && joinTab.visible)
			{
				SetTab(Tab.JOIN_TAB);
				setCurrentlySnappedComponentTo(joinTab.myID);
				snapCursorToCurrentSnappedComponent();
			}
			else if (b == Buttons.RightTrigger && hostTab.visible)
			{
				SetTab(Tab.HOST_TAB);
				setCurrentlySnappedComponentTo(hostTab.myID);
				snapCursorToCurrentSnappedComponent();
			}
		}
	}

	public override void UpdateButtons()
	{
		base.UpdateButtons();
		foreach (ClickableComponent slotButton in slotButtons)
		{
			if (slotButton.myID == 0)
			{
				if (currentItemIndex == 0)
				{
					slotButton.upNeighborID = 811;
				}
				else
				{
					slotButton.upNeighborID = -7777;
				}
			}
		}
	}

	public override void update(GameTime time)
	{
		float num = (float)time.ElapsedGameTime.TotalSeconds;
		updateCounter++;
		if (!isSetUp)
		{
			if (Program.sdk.ConnectionFinished)
			{
				connectionFinished();
			}
			return;
		}
		if (refreshButton != null && refreshButton.visible && _refreshDelay > 0f)
		{
			_refreshDelay -= num;
		}
		base.update(time);
	}

	public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
	{
		base.gameWindowSizeChanged(oldBounds, newBounds);
		smallScreenFormat = Game1.graphics.GraphicsDevice.Viewport.Height < 1080;
		string text = Game1.content.LoadString("Strings\\UI:CoopMenu_Join");
		int num = (int)Game1.dialogueFont.MeasureString(text).X + 64;
		Vector2 vector = (smallScreenFormat ? new Vector2(xPositionOnScreen, yPositionOnScreen) : new Vector2(xPositionOnScreen + IClickableMenu.borderWidth, yPositionOnScreen - 96));
		joinTab.bounds.X = (int)vector.X;
		joinTab.bounds.Y = (int)vector.Y;
		text = Game1.content.LoadString("Strings\\UI:CoopMenu_Host");
		num = (int)Game1.dialogueFont.MeasureString(text).X + 64;
		vector = (smallScreenFormat ? new Vector2(joinTab.bounds.Right + ((!smallScreenFormat) ? 4 : 0), yPositionOnScreen) : new Vector2(joinTab.bounds.Right + 4, yPositionOnScreen - 64));
		hostTab.bounds.X = (int)vector.X;
		hostTab.bounds.Y = (int)vector.Y;
		text = Game1.content.LoadString("Strings\\UI:CoopMenu_Refresh");
		num = (int)Game1.dialogueFont.MeasureString(text).X + 64;
		vector = new Vector2(backButton.bounds.Right - num, backButton.bounds.Y - 128);
		refreshButton.bounds.X = (int)vector.X;
		refreshButton.bounds.Y = (int)vector.Y;
	}

	protected override void saveFileScanComplete()
	{
		if (Program.sdk.Networking != null)
		{
			lobbyUpdateListener = new LobbyUpdateCallback(onLobbyUpdate);
			Program.sdk.Networking.AddLobbyUpdateListener(lobbyUpdateListener);
			Program.sdk.Networking.RequestFriendLobbyData();
		}
	}

	protected virtual FriendFarmData readLobbyFarmData(object lobby)
	{
		FriendFarmData friendFarmData = new FriendFarmData
		{
			Lobby = lobby,
			Date = new WorldDate()
		};
		friendFarmData.OwnerName = Program.sdk.Networking.GetLobbyOwnerName(lobby);
		friendFarmData.FarmName = Program.sdk.Networking.GetLobbyData(lobby, "farmName");
		string lobbyData = Program.sdk.Networking.GetLobbyData(lobby, "farmType");
		string lobbyData2 = Program.sdk.Networking.GetLobbyData(lobby, "modFarmType");
		string lobbyData3 = Program.sdk.Networking.GetLobbyData(lobby, "date");
		int num = -1;
		int num2 = 0;
		num = Convert.ToInt32(lobbyData);
		num2 = Convert.ToInt32(lobbyData3);
		friendFarmData.FarmType = num;
		friendFarmData.ModFarmType = null;
		if (!string.IsNullOrEmpty(lobbyData2))
		{
			List<ModFarmType> list = Game1.content.Load<List<ModFarmType>>("Data\\AdditionalFarms");
			if (list != null)
			{
				foreach (ModFarmType item in list)
				{
					if (item.ID == lobbyData2)
					{
						friendFarmData.ModFarmType = item;
						break;
					}
				}
			}
		}
		friendFarmData.Date.TotalDays = num2;
		friendFarmData.ProtocolVersion = Program.sdk.Networking.GetLobbyData(lobby, "protocolVersion");
		friendFarmData.FarmName = Program.sdk.FilterDirtyWords(friendFarmData.FarmName);
		friendFarmData.OwnerName = Program.sdk.FilterDirtyWords(friendFarmData.OwnerName);
		return friendFarmData;
	}

	protected virtual bool checkFriendFarmCompatibility(FriendFarmData farm)
	{
		if (farm.FarmType < 0 || farm.FarmType > 7)
		{
			return false;
		}
		if (farm.ProtocolVersion != "1.5.5")
		{
			return false;
		}
		return true;
	}

	protected virtual void onLobbyUpdate(object lobby)
	{
		try
		{
			string lobbyData = Program.sdk.Networking.GetLobbyData(lobby, "protocolVersion");
			if (lobbyData != "1.5.5")
			{
				return;
			}
			Console.WriteLine("Receiving friend lobby data...");
			Console.WriteLine("Owner: " + Program.sdk.Networking.GetLobbyOwnerName(lobby));
			Console.WriteLine("farmName = " + Program.sdk.Networking.GetLobbyData(lobby, "farmName"));
			Console.WriteLine("farmType = " + Program.sdk.Networking.GetLobbyData(lobby, "farmType"));
			Console.WriteLine("date = " + Program.sdk.Networking.GetLobbyData(lobby, "date"));
			Console.WriteLine("protocolVersion = " + lobbyData);
			Console.WriteLine("farmhands = " + Program.sdk.Networking.GetLobbyData(lobby, "farmhands"));
			Console.WriteLine("newFarmhands = " + Program.sdk.Networking.GetLobbyData(lobby, "newFarmhands"));
			FriendFarmData friendFarmData = readLobbyFarmData(lobby);
			if (!checkFriendFarmCompatibility(friendFarmData) || (friendFarmData.FarmType == 7 && friendFarmData.ModFarmType == null))
			{
				return;
			}
			string userID = Program.sdk.Networking.GetUserID();
			string lobbyData2 = Program.sdk.Networking.GetLobbyData(lobby, "farmhands");
			string lobbyData3 = Program.sdk.Networking.GetLobbyData(lobby, "newFarmhands");
			bool flag = false;
			flag = Convert.ToBoolean(lobbyData3);
			if (lobbyData2 == "" && !flag)
			{
				return;
			}
			string[] source = lobbyData2.Split(',');
			if (!source.Contains(userID) && !flag)
			{
				return;
			}
			friendFarmData.PreviouslyJoined = source.Contains(userID);
			if (menuSlots == null)
			{
				return;
			}
			foreach (MenuSlot menuSlot in menuSlots)
			{
				if (menuSlot is FriendFarmSlot friendFarmSlot && friendFarmSlot.MatchAddress(lobby))
				{
					friendFarmSlot.Update(friendFarmData);
					return;
				}
			}
			menuSlots.Add(new FriendFarmSlot(this, friendFarmData));
			UpdateButtons();
			populateClickableComponentList();
		}
		catch (FormatException)
		{
		}
		catch (OverflowException)
		{
		}
	}

	public override bool IsAutomaticSnapValid(int direction, ClickableComponent a, ClickableComponent b)
	{
		if (a.region == 1000 && (direction == 2 || direction == 0) && b.region == 1000)
		{
			return false;
		}
		if (a.myID == 810 && direction == 0 && b.region != 900)
		{
			return false;
		}
		if (a.myID == 810 && direction == 1 && b.myID == 81114)
		{
			return false;
		}
		return base.IsAutomaticSnapValid(direction, a, b);
	}

	protected override void addSaveFiles(List<Farmer> files)
	{
		hostSlots.AddRange(files.Where((Farmer file) => file.slotCanHost).Select((Func<Farmer, MenuSlot>)((Farmer file) => new HostFileSlot(this, file))));
		UpdateButtons();
	}

	protected virtual void setMenu(IClickableMenu menu)
	{
		if (Game1.activeClickableMenu is TitleMenu)
		{
			TitleMenu.subMenu = menu;
		}
		else
		{
			Game1.activeClickableMenu = menu;
		}
	}

	private void enterIPPressed()
	{
		string default_text = "";
		try
		{
			StartupPreferences startupPreferences = new StartupPreferences();
			startupPreferences.loadPreferences(async: false, applyLanguage: false);
			default_text = startupPreferences.lastEnteredIP;
		}
		catch (Exception)
		{
		}
		string title = Game1.content.LoadString("Strings\\UI:CoopMenu_EnterIP");
		setMenu(new TitleTextInputMenu(title, delegate(string address)
		{
			try
			{
				StartupPreferences startupPreferences2 = new StartupPreferences();
				startupPreferences2.loadPreferences(async: false, applyLanguage: false);
				startupPreferences2.lastEnteredIP = address;
				startupPreferences2.savePreferences(async: false);
			}
			catch (Exception)
			{
			}
			if (address == "")
			{
				address = "localhost";
			}
			setMenu(new FarmhandMenu(Game1.multiplayer.InitClient(new LidgrenClient(address))));
		}, default_text, "join_menu"));
	}

	private void enterInviteCodePressed()
	{
		if (Program.sdk.Networking == null || !Program.sdk.Networking.SupportsInviteCodes())
		{
			return;
		}
		string title = Game1.content.LoadString("Strings\\UI:CoopMenu_EnterInviteCode");
		setMenu(new TitleTextInputMenu(title, delegate(string code)
		{
			lastEnteredInviteCode = code;
			object lobbyFromInviteCode = Program.sdk.Networking.GetLobbyFromInviteCode(code);
			if (lobbyFromInviteCode != null)
			{
				Client client = Program.sdk.Networking.CreateClient(lobbyFromInviteCode);
				setMenu(new FarmhandMenu(client));
			}
		}, lastEnteredInviteCode, "join_menu"));
	}

	private bool tabClick(int x, int y)
	{
		if (joinTab.visible && joinTab.containsPoint(x, y))
		{
			SetTab(Tab.JOIN_TAB);
			return true;
		}
		if (hostTab.visible && hostTab.containsPoint(x, y))
		{
			SetTab(Tab.HOST_TAB);
			return true;
		}
		return false;
	}

	public virtual void SetTab(Tab new_tab, bool play_sound = true)
	{
		if (currentTab == new_tab)
		{
			return;
		}
		currentTab = new_tab;
		if (!smallScreenFormat && isSetUp)
		{
			if (currentTab == Tab.HOST_TAB)
			{
				hostTab.bounds.Y = yPositionOnScreen - 96;
				joinTab.bounds.Y = yPositionOnScreen - 64;
			}
			else
			{
				hostTab.bounds.Y = yPositionOnScreen - 64;
				joinTab.bounds.Y = yPositionOnScreen - 96;
			}
		}
		if (play_sound)
		{
			Game1.playSound("smallSelect");
		}
		if (isSetUp)
		{
			UpdateButtons();
		}
	}

	public override void receiveLeftClick(int x, int y, bool playSound = true)
	{
		if (!isSetUp)
		{
			return;
		}
		if (refreshButton.visible && refreshButton.containsPoint(x, y))
		{
			if (_refreshDelay < 0f)
			{
				Game1.playSound("bigDeSelect");
				setMenu(new CoopMenu(_tooManyFarms));
			}
		}
		else if (!smallScreenFormat || !tabClick(x, y))
		{
			base.receiveLeftClick(x, y, playSound);
			if (!smallScreenFormat && !loading)
			{
				tabClick(x, y);
			}
		}
	}

	public override void performHoverAction(int x, int y)
	{
		if (isSetUp)
		{
			if (refreshButton.visible && refreshButton.containsPoint(x, y))
			{
				refreshButton.scale = 1f;
			}
			else
			{
				refreshButton.scale = 0f;
			}
			if (smallScreenFormat && (hostTab.containsPoint(x, y) || joinTab.containsPoint(x, y)))
			{
				base.performHoverAction(-100, -100);
			}
			else
			{
				base.performHoverAction(x, y);
			}
		}
	}

	protected override string getStatusText()
	{
		return null;
	}

	private void drawTabs(SpriteBatch b)
	{
		if (isSetUp)
		{
			Color color = (smallScreenFormat ? Color.Orange : new Color(255, 255, 150));
			Color yellow = Color.Yellow;
			Color color2 = (smallScreenFormat ? Color.DarkOrange : new Color(221, 148, 84));
			Color darkGoldenrod = Color.DarkGoldenrod;
			if (joinTab.visible)
			{
				bool flag = currentTab == Tab.JOIN_TAB;
				bool flag2 = currentTab != 0 && joinTab.containsPoint(Game1.getMouseX(), Game1.getMouseY());
				IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), joinTab.bounds.X, joinTab.bounds.Y, joinTab.bounds.Width, joinTab.bounds.Height + ((!smallScreenFormat) ? 64 : 0), flag ? color : (flag2 ? yellow : Color.White), 1f, drawShadow: false);
				Utility.drawTextWithColoredShadow(b, joinTab.label, Game1.dialogueFont, new Vector2(joinTab.bounds.Center.X, joinTab.bounds.Y + 40) - Game1.dialogueFont.MeasureString(joinTab.label) / 2f, Game1.textColor, flag2 ? darkGoldenrod : (flag ? color2 : new Color(221, 148, 84)), 1.01f);
			}
			if (hostTab.visible)
			{
				bool flag3 = currentTab == Tab.HOST_TAB;
				bool flag4 = currentTab != Tab.HOST_TAB && hostTab.containsPoint(Game1.getMouseX(), Game1.getMouseY());
				IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), hostTab.bounds.X, hostTab.bounds.Y, hostTab.bounds.Width, hostTab.bounds.Height + ((!smallScreenFormat) ? 64 : 0), flag3 ? color : (flag4 ? yellow : Color.White), 1f, drawShadow: false);
				Utility.drawTextWithColoredShadow(b, hostTab.label, Game1.dialogueFont, new Vector2(hostTab.bounds.Center.X, hostTab.bounds.Y + 40) - Game1.dialogueFont.MeasureString(hostTab.label) / 2f, Game1.textColor, flag4 ? darkGoldenrod : (flag3 ? color2 : new Color(221, 148, 84)), 1.01f);
			}
		}
	}

	public override void snapToDefaultClickableComponent()
	{
		base.snapToDefaultClickableComponent();
		if (currentlySnappedComponent == null)
		{
			currentlySnappedComponent = getComponentWithID(811);
			snapCursorToCurrentSnappedComponent();
		}
	}

	protected override void drawBefore(SpriteBatch b)
	{
		base.drawBefore(b);
		if (isSetUp && !smallScreenFormat)
		{
			drawTabs(b);
		}
	}

	protected override void drawExtra(SpriteBatch b)
	{
		base.drawExtra(b);
		if (!isSetUp)
		{
			return;
		}
		if (refreshButton.visible)
		{
			Color color = ((refreshButton.scale > 0f) ? Color.Wheat : Color.White);
			if (_refreshDelay > 0f)
			{
				color = Color.Gray;
			}
			IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(432, 439, 9, 9), refreshButton.bounds.X, refreshButton.bounds.Y, refreshButton.bounds.Width, refreshButton.bounds.Height, color, 4f);
			Utility.drawTextWithShadow(b, refreshButton.label, Game1.dialogueFont, new Vector2(refreshButton.bounds.Center.X, refreshButton.bounds.Center.Y + 4) - Game1.dialogueFont.MeasureString(refreshButton.label) / 2f, Game1.textColor, 1f, -1f, -1, -1, 0f);
		}
		if (smallScreenFormat)
		{
			drawTabs(b);
		}
	}

	protected override void drawStatusText(SpriteBatch b)
	{
		if (getStatusText() != null)
		{
			base.drawStatusText(b);
		}
		else if (!isSetUp)
		{
			int num = 1 + Program.sdk.ConnectionProgress;
			int num2 = updateCounter / 5 % num;
			string value = Game1.content.LoadString("Strings\\UI:CoopMenu_ConnectingOnlineServices");
			_stringBuilder.Clear();
			_stringBuilder.Append(value);
			for (int i = 0; i < num2; i++)
			{
				_stringBuilder.Append(".");
			}
			string s = _stringBuilder.ToString();
			for (int j = num2; j < num; j++)
			{
				_stringBuilder.Append(".");
			}
			int widthOfString = SpriteText.getWidthOfString(_stringBuilder.ToString());
			SpriteText.drawString(b, s, Game1.graphics.GraphicsDevice.Viewport.Bounds.Center.X - widthOfString / 2, Game1.graphics.GraphicsDevice.Viewport.Bounds.Center.Y);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (lobbyUpdateListener != null && Program.sdk.Networking != null)
		{
			Program.sdk.Networking.RemoveLobbyUpdateListener(lobbyUpdateListener);
		}
		lobbyUpdateListener = null;
		base.Dispose(disposing);
	}
}
