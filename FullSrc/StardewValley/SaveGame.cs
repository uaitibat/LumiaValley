using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Ionic.Zlib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.GameData;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.Quests;
using StardewValley.TerrainFeatures;

namespace StardewValley;

public class SaveGame
{
	public enum SaveFixes
	{
		NONE,
		StoredBigCraftablesStackFix,
		PorchedCabinBushesFix,
		ChangeObeliskFootprintHeight,
		CreateStorageDressers,
		InferPreserves,
		TransferHatSkipHairFlag,
		RevealSecretNoteItemTastes,
		TransferHoneyTypeToPreserves,
		TransferNoteBlockScale,
		FixCropHarvestAmountsAndInferSeedIndex,
		Level9PuddingFishingRecipe,
		Level9PuddingFishingRecipe2,
		quarryMineBushes,
		MissingQisChallenge,
		BedsToFurniture,
		ChildBedsToFurniture,
		ModularizeFarmStructures,
		FixFlooringFlags,
		AddBugSteakRecipe,
		AddBirdie,
		AddTownBush,
		AddNewRingRecipes1_5,
		ResetForges,
		AddSquidInkRavioli,
		MakeDarkSwordVampiric,
		FixRingSheetIndex,
		FixBeachFarmBushes,
		AddCampfireKit,
		Level9PuddingFishingRecipe3,
		OstrichIncubatorFragility,
		FixBotchedBundleData,
		LeoChildrenFix,
		Leo6HeartGermanFix,
		BirdieQuestRemovedFix,
		SkippedSummit,
		MAX
	}

	public static XmlSerializer serializer = new XmlSerializer(typeof(SaveGame), new Type[25]
	{
		typeof(Tool),
		typeof(GameLocation),
		typeof(Duggy),
		typeof(Bug),
		typeof(BigSlime),
		typeof(Ghost),
		typeof(Child),
		typeof(Pet),
		typeof(Dog),
		typeof(Cat),
		typeof(Horse),
		typeof(GreenSlime),
		typeof(LavaCrab),
		typeof(RockCrab),
		typeof(ShadowGuy),
		typeof(SquidKid),
		typeof(Grub),
		typeof(Fly),
		typeof(DustSpirit),
		typeof(Quest),
		typeof(MetalHead),
		typeof(ShadowGirl),
		typeof(Monster),
		typeof(JunimoHarvester),
		typeof(TerrainFeature)
	});

	public static XmlSerializer farmerSerializer = new XmlSerializer(typeof(Farmer), new Type[1] { typeof(Tool) });

	public static XmlSerializer locationSerializer = new XmlSerializer(typeof(GameLocation), new Type[24]
	{
		typeof(Tool),
		typeof(Duggy),
		typeof(Ghost),
		typeof(GreenSlime),
		typeof(LavaCrab),
		typeof(RockCrab),
		typeof(ShadowGuy),
		typeof(Child),
		typeof(Pet),
		typeof(Dog),
		typeof(Cat),
		typeof(Horse),
		typeof(SquidKid),
		typeof(Grub),
		typeof(Fly),
		typeof(DustSpirit),
		typeof(Bug),
		typeof(BigSlime),
		typeof(BreakableContainer),
		typeof(MetalHead),
		typeof(ShadowGirl),
		typeof(Monster),
		typeof(JunimoHarvester),
		typeof(TerrainFeature)
	});

	[InstancedStatic]
	public static bool IsProcessing;

	[InstancedStatic]
	public static bool CancelToTitle;

	public Farmer player;

	public List<GameLocation> locations;

	public string currentSeason;

	public string samBandName;

	public string elliottBookName;

	public List<string> mailbox;

	public List<string> broadcastedMail;

	public List<string> worldStateIDs;

	public int lostBooksFound = -1;

	public int goldenWalnuts = -1;

	public int goldenWalnutsFound;

	public int miniShippingBinsObtained;

	public bool mineShrineActivated;

	public bool goldenCoconutCracked;

	public bool parrotPlatformsUnlocked;

	public bool farmPerfect;

	public List<string> foundBuriedNuts = new List<string>();

	public int visitsUntilY1Guarantee = -1;

	public Game1.MineChestType shuffleMineChests;

	public int dayOfMonth;

	public int year;

	public int farmerWallpaper;

	public int FarmerFloor;

	public int currentWallpaper;

	public int currentFloor;

	public int currentSongIndex;

	public int? countdownToWedding;

	public Point incubatingEgg;

	public double chanceToRainTomorrow;

	public double dailyLuck;

	public ulong uniqueIDForThisGame;

	public bool weddingToday;

	public bool isRaining;

	public bool isDebrisWeather;

	public bool shippingTax;

	public bool isLightning;

	public bool isSnowing;

	public bool shouldSpawnMonsters;

	public bool hasApplied1_3_UpdateChanges;

	public bool hasApplied1_4_UpdateChanges;

	public Stats stats;

	[InstancedStatic]
	public static SaveGame loaded;

	public float musicVolume;

	public float soundVolume;

	public int[] cropsOfTheWeek;

	public Object dishOfTheDay;

	public int highestPlayerLimit = -1;

	public int moveBuildingPermissionMode;

	public SerializableDictionary<GameLocation.LocationContext, LocationWeather> locationWeather;

	public SerializableDictionary<string, string> bannedUsers = new SerializableDictionary<string, string>();

	public SerializableDictionary<string, string> bundleData = new SerializableDictionary<string, string>();

	public SerializableDictionary<string, int> limitedNutDrops = new SerializableDictionary<string, int>();

	public long latestID;

	public Options options;

	public SerializableDictionary<long, Options> splitscreenOptions = new SerializableDictionary<long, Options>();

	public SerializableDictionary<string, string> CustomData = new SerializableDictionary<string, string>();

	public SerializableDictionary<int, MineInfo> mine_permanentMineChanges;

	[Obsolete]
	[XmlIgnore]
	public List<ResourceClump> mine_resourceClumps = new List<ResourceClump>();

	[Obsolete]
	[XmlIgnore]
	public int mine_mineLevel;

	[Obsolete]
	[XmlIgnore]
	public int mine_nextLevel;

	public int mine_lowestLevelReached;

	public int minecartHighScore;

	public int weatherForTomorrow;

	public string whichFarm;

	public int mine_lowestLevelReachedForOrder = -1;

	public int skullCavesDifficulty;

	public int minesDifficulty;

	public int currentGemBirdIndex;

	public NetLeaderboards junimoKartLeaderboards;

	public List<SpecialOrder> specialOrders;

	public List<SpecialOrder> availableSpecialOrders;

	public List<string> completedSpecialOrders;

	public List<string> acceptedSpecialOrderTypes = new List<string>();

	public List<Item> returnedDonations;

	public List<Item> junimoChest;

	public List<string> collectedNutTracker = new List<string>();

	public SerializableDictionary<FarmerPair, Friendship> farmerFriendships = new SerializableDictionary<FarmerPair, Friendship>();

	public SerializableDictionary<int, long> cellarAssignments = new SerializableDictionary<int, long>();

	public int lastAppliedSaveFix;

	public string gameVersion = Game1.version;

	public string gameVersionLabel;

	public static XmlSerializer GetSerializer(Type type)
	{
		return new XmlSerializer(type);
	}

	public static IEnumerator<int> Save()
	{
		IsProcessing = true;
		if (LocalMultiplayer.IsLocalMultiplayer())
		{
			IEnumerator<int> save = getSaveEnumerator();
			while (save.MoveNext())
			{
				yield return save.Current;
			}
			yield return 100;
			yield break;
		}
		Console.WriteLine("SaveGame.Save() called.");
		yield return 1;
		IEnumerator<int> loader = getSaveEnumerator();
		Task saveTask = new Task(delegate
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			if (loader != null)
			{
				while (loader.MoveNext() && loader.Current < 100)
				{
				}
			}
		});
		Game1.hooks.StartTask(saveTask, "Save");
		while (!saveTask.IsCanceled && !saveTask.IsCompleted && !saveTask.IsFaulted)
		{
			yield return 1;
		}
		IsProcessing = false;
		if (saveTask.IsFaulted)
		{
			Exception baseException = saveTask.Exception.GetBaseException();
			Console.WriteLine("saveTask failed with an exception");
			Console.WriteLine(baseException);
			if (!(baseException is TaskCanceledException))
			{
				throw baseException;
			}
			Game1.ExitToTitle();
		}
		else
		{
			Console.WriteLine("SaveGame.Save() completed without exceptions.");
			yield return 100;
		}
	}

	public static string FilterFileName(string fileName)
	{
		string text = fileName;
		for (int i = 0; i < text.Length; i++)
		{
			char c = text[i];
			if (!char.IsLetterOrDigit(c))
			{
				fileName = fileName.Replace(c.ToString() ?? "", "");
			}
		}
		return fileName;
	}

	public static IEnumerator<int> getSaveEnumerator()
	{
		if (CancelToTitle)
		{
			throw new TaskCanceledException();
		}
		yield return 1;
		SaveGame saveData = new SaveGame();
		saveData.player = Game1.player;
		saveData.player.gameVersion = Game1.version;
		saveData.player.gameVersionLabel = Game1.versionLabel;
		saveData.locations = new List<GameLocation>();
		saveData.locations.AddRange(Game1.locations);
		foreach (GameLocation location in Game1.locations)
		{
			location.cleanupBeforeSave();
		}
		saveData.currentSeason = Game1.currentSeason;
		saveData.samBandName = Game1.samBandName;
		saveData.broadcastedMail = new List<string>();
		saveData.bannedUsers = Game1.bannedUsers;
		foreach (string item in Game1.player.team.broadcastedMail)
		{
			saveData.broadcastedMail.Add(item);
		}
		saveData.skullCavesDifficulty = Game1.netWorldState.Value.SkullCavesDifficulty;
		saveData.minesDifficulty = Game1.netWorldState.Value.MinesDifficulty;
		saveData.visitsUntilY1Guarantee = Game1.netWorldState.Value.VisitsUntilY1Guarantee;
		saveData.shuffleMineChests = Game1.netWorldState.Value.ShuffleMineChests;
		saveData.elliottBookName = Game1.elliottBookName;
		saveData.dayOfMonth = Game1.dayOfMonth;
		saveData.year = Game1.year;
		saveData.farmerWallpaper = Game1.farmerWallpaper;
		saveData.FarmerFloor = Game1.FarmerFloor;
		saveData.chanceToRainTomorrow = Game1.chanceToRainTomorrow;
		saveData.dailyLuck = Game1.player.team.sharedDailyLuck.Value;
		saveData.isRaining = Game1.isRaining;
		saveData.isLightning = Game1.isLightning;
		saveData.isSnowing = Game1.isSnowing;
		saveData.isDebrisWeather = Game1.isDebrisWeather;
		saveData.shouldSpawnMonsters = Game1.spawnMonstersAtNight;
		saveData.specialOrders = Game1.player.team.specialOrders.ToList();
		saveData.availableSpecialOrders = Game1.player.team.availableSpecialOrders.ToList();
		saveData.completedSpecialOrders = Game1.player.team.completedSpecialOrders.Keys.ToList();
		saveData.collectedNutTracker = new List<string>(Game1.player.team.collectedNutTracker.Keys);
		saveData.acceptedSpecialOrderTypes = Game1.player.team.acceptedSpecialOrderTypes.ToList();
		saveData.returnedDonations = Game1.player.team.returnedDonations.ToList();
		saveData.junimoChest = Game1.player.team.junimoChest.ToList();
		saveData.weddingToday = Game1.weddingToday;
		if (Game1.whichFarm == 7)
		{
			saveData.whichFarm = Game1.whichModFarm.ID;
		}
		else
		{
			saveData.whichFarm = Game1.whichFarm.ToString();
		}
		saveData.minecartHighScore = Game1.minecartHighScore;
		saveData.junimoKartLeaderboards = Game1.player.team.junimoKartScores;
		saveData.lastAppliedSaveFix = (int)Game1.lastAppliedSaveFix;
		saveData.locationWeather = new SerializableDictionary<GameLocation.LocationContext, LocationWeather>();
		foreach (int key in Game1.netWorldState.Value.LocationWeather.Keys)
		{
			LocationWeather value = Game1.netWorldState.Value.LocationWeather[key];
			saveData.locationWeather[(GameLocation.LocationContext)key] = value;
		}
		saveData.cellarAssignments = new SerializableDictionary<int, long>();
		foreach (int key2 in Game1.player.team.cellarAssignments.Keys)
		{
			saveData.cellarAssignments[key2] = Game1.player.team.cellarAssignments[key2];
		}
		saveData.uniqueIDForThisGame = Game1.uniqueIDForThisGame;
		saveData.musicVolume = Game1.options.musicVolumeLevel;
		saveData.soundVolume = Game1.options.soundVolumeLevel;
		saveData.shippingTax = Game1.shippingTax;
		saveData.cropsOfTheWeek = Game1.cropsOfTheWeek;
		saveData.mine_lowestLevelReached = Game1.netWorldState.Value.LowestMineLevel;
		saveData.mine_lowestLevelReachedForOrder = Game1.netWorldState.Value.LowestMineLevelForOrder;
		saveData.currentGemBirdIndex = Game1.currentGemBirdIndex;
		saveData.mine_permanentMineChanges = MineShaft.permanentMineChanges;
		saveData.currentFloor = Game1.currentFloor;
		saveData.currentWallpaper = Game1.currentWallpaper;
		saveData.dishOfTheDay = Game1.dishOfTheDay;
		saveData.latestID = (long)Game1.multiplayer.latestID;
		saveData.highestPlayerLimit = Game1.netWorldState.Value.HighestPlayerLimit;
		saveData.options = Game1.options;
		saveData.splitscreenOptions = Game1.splitscreenOptions;
		saveData.CustomData = Game1.CustomData;
		saveData.worldStateIDs = Game1.worldStateIDs;
		saveData.currentSongIndex = Game1.currentSongIndex;
		saveData.weatherForTomorrow = Game1.weatherForTomorrow;
		saveData.goldenWalnuts = Game1.netWorldState.Value.GoldenWalnuts;
		saveData.goldenWalnutsFound = Game1.netWorldState.Value.GoldenWalnutsFound;
		saveData.miniShippingBinsObtained = Game1.netWorldState.Value.MiniShippingBinsObtained;
		saveData.goldenCoconutCracked = Game1.netWorldState.Value.GoldenCoconutCracked.Value;
		saveData.parrotPlatformsUnlocked = Game1.netWorldState.Value.ParrotPlatformsUnlocked.Value;
		saveData.farmPerfect = Game1.player.team.farmPerfect.Value;
		saveData.lostBooksFound = Game1.netWorldState.Value.LostBooksFound;
		saveData.foundBuriedNuts = new List<string>(Game1.netWorldState.Value.FoundBuriedNuts.Keys);
		saveData.mineShrineActivated = Game1.player.team.mineShrineActivated;
		saveData.gameVersion = Game1.version;
		saveData.gameVersionLabel = Game1.versionLabel;
		saveData.limitedNutDrops = new SerializableDictionary<string, int>();
		foreach (string key3 in Game1.player.team.limitedNutDrops.Keys)
		{
			saveData.limitedNutDrops[key3] = Game1.player.team.limitedNutDrops[key3];
		}
		saveData.bundleData = new SerializableDictionary<string, string>();
		Dictionary<string, string> unlocalizedBundleData = Game1.netWorldState.Value.GetUnlocalizedBundleData();
		foreach (string key4 in unlocalizedBundleData.Keys)
		{
			saveData.bundleData[key4] = unlocalizedBundleData[key4];
		}
		saveData.moveBuildingPermissionMode = (int)Game1.player.team.farmhandsCanMoveBuildings.Value;
		saveData.hasApplied1_3_UpdateChanges = Game1.hasApplied1_3_UpdateChanges;
		saveData.hasApplied1_4_UpdateChanges = Game1.hasApplied1_4_UpdateChanges;
		foreach (FarmerPair key5 in Game1.player.team.friendshipData.Keys)
		{
			saveData.farmerFriendships[key5] = Game1.player.team.friendshipData[key5];
		}
		string tmpString = "_STARDEWVALLEYSAVETMP";
		bool save_backups_and_metadata = true;
		string text = FilterFileName(Game1.GetSaveGameName());
		string filenameNoTmpString = text + "_" + Game1.uniqueIDForThisGame;
		string filenameWithTmpString = text + "_" + Game1.uniqueIDForThisGame + tmpString;
		string save_directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley", "Saves", filenameNoTmpString + Path.DirectorySeparatorChar);
		if (Game1.savePathOverride != "")
		{
			save_directory = Game1.savePathOverride;
			if (Game1.savePathOverride != "")
			{
				save_backups_and_metadata = false;
			}
		}
		string path = Path.Combine(save_directory, filenameWithTmpString);
		ensureFolderStructureExists();
		string justFarmerFilePath = Path.Combine(save_directory, "SaveGameInfo" + tmpString);
		if (File.Exists(path))
		{
			File.Delete(path);
		}
		if (save_backups_and_metadata && File.Exists(justFarmerFilePath))
		{
			File.Delete(justFarmerFilePath);
		}
		Stream fstream = null;
		try
		{
			fstream = File.Create(path);
		}
		catch (IOException ex)
		{
			if (fstream != null)
			{
				fstream.Close();
				fstream.Dispose();
			}
			Game1.gameMode = 9;
			Game1.debugOutput = Game1.parseText(ex.Message);
			yield break;
		}
		MemoryStream mstream1 = new MemoryStream(1024);
		MemoryStream mstream2 = new MemoryStream(1024);
		if (CancelToTitle)
		{
			throw new TaskCanceledException();
		}
		yield return 2;
		XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
		xmlWriterSettings.CloseOutput = false;
		Console.WriteLine("Saving without compression...");
		_ = mstream2;
		XmlWriter xmlWriter = XmlWriter.Create(mstream1, xmlWriterSettings);
		xmlWriter.WriteStartDocument();
		serializer.Serialize(xmlWriter, saveData);
		xmlWriter.WriteEndDocument();
		xmlWriter.Flush();
		xmlWriter.Close();
		mstream1.Close();
		byte[] buffer1 = mstream1.ToArray();
		if (CancelToTitle)
		{
			throw new TaskCanceledException();
		}
		yield return 2;
		fstream.Write(buffer1, 0, buffer1.Length);
		fstream.Close();
		if (save_backups_and_metadata)
		{
			Game1.player.saveTime = (int)(DateTime.UtcNow - new DateTime(2012, 6, 22)).TotalMinutes;
			try
			{
				fstream = File.Create(justFarmerFilePath);
			}
			catch (IOException ex2)
			{
				fstream?.Close();
				Game1.gameMode = 9;
				Game1.debugOutput = Game1.parseText(ex2.Message);
				yield break;
			}
			xmlWriter = XmlWriter.Create(fstream, new XmlWriterSettings
			{
				CloseOutput = false
			});
			xmlWriter.WriteStartDocument();
			farmerSerializer.Serialize(xmlWriter, Game1.player);
			xmlWriter.WriteEndDocument();
			xmlWriter.Flush();
			fstream.Close();
		}
		if (CancelToTitle)
		{
			throw new TaskCanceledException();
		}
		yield return 2;
		path = Path.Combine(save_directory, filenameNoTmpString);
		justFarmerFilePath = Path.Combine(save_directory, "SaveGameInfo");
		if (save_backups_and_metadata)
		{
			string text2 = Path.Combine(save_directory, filenameNoTmpString + "_old");
			string text3 = Path.Combine(save_directory, "SaveGameInfo_old");
			if (File.Exists(text2))
			{
				File.Delete(text2);
			}
			if (File.Exists(text3))
			{
				File.Delete(text3);
			}
			try
			{
				File.Move(path, text2);
				File.Move(justFarmerFilePath, text3);
			}
			catch (Exception)
			{
			}
		}
		if (File.Exists(path))
		{
			File.Delete(path);
		}
		if (save_backups_and_metadata && File.Exists(justFarmerFilePath))
		{
			File.Delete(justFarmerFilePath);
		}
		path = Path.Combine(save_directory, filenameWithTmpString);
		if (File.Exists(path))
		{
			File.Move(path, path.Replace(tmpString, ""));
		}
		if (save_backups_and_metadata)
		{
			justFarmerFilePath = Path.Combine(save_directory, "SaveGameInfo" + tmpString);
			if (File.Exists(justFarmerFilePath))
			{
				File.Move(justFarmerFilePath, justFarmerFilePath.Replace(tmpString, ""));
			}
		}
		if (CancelToTitle)
		{
			throw new TaskCanceledException();
		}
		yield return 100;
	}

	public static bool IsNewGameSaveNameCollision(string save_name)
	{
		string text = save_name;
		string text2 = text;
		for (int i = 0; i < text2.Length; i++)
		{
			char c = text2[i];
			if (!char.IsLetterOrDigit(c))
			{
				text = text.Replace(c.ToString() ?? "", "");
			}
		}
		if (!text.EndsWith(Path.DirectorySeparatorChar.ToString()))
		{
			text += Path.DirectorySeparatorChar;
		}
		string path = text + "_" + Game1.uniqueIDForThisGame;
		return new FileInfo(Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley"), "Saves"), path)).Directory.Exists;
	}

	public static void ensureFolderStructureExists(string tmpString = "")
	{
		string text = Game1.GetSaveGameName();
		string text2 = text;
		for (int i = 0; i < text2.Length; i++)
		{
			char c = text2[i];
			if (!char.IsLetterOrDigit(c))
			{
				text = text.Replace(c.ToString() ?? "", "");
			}
		}
		string path = text + "_" + Game1.uniqueIDForThisGame + tmpString;
		FileInfo fileInfo = new FileInfo(Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley"), "Saves"), path));
		if (!fileInfo.Directory.Exists)
		{
			fileInfo.Directory.Create();
		}
		fileInfo = new FileInfo(Path.Combine(Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley"), "Saves"), path), "dummy"));
		if (!fileInfo.Directory.Exists)
		{
			fileInfo.Directory.Create();
		}
		fileInfo = null;
	}

	public static void Load(string filename)
	{
		Game1.gameMode = 6;
		Game1.loadingMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGame.cs.4690");
		Game1.currentLoader = getLoadEnumerator(filename);
	}

	public static void LoadFarmType()
	{
		List<ModFarmType> list = Game1.content.Load<List<ModFarmType>>("Data\\AdditionalFarms");
		Game1.whichFarm = -1;
		if (list != null)
		{
			foreach (ModFarmType item in list)
			{
				if (item.ID == loaded.whichFarm)
				{
					Game1.whichModFarm = item;
					Game1.whichFarm = 7;
					break;
				}
			}
		}
		if (loaded.whichFarm == null)
		{
			Game1.whichFarm = 0;
		}
		if (Game1.whichFarm < 0)
		{
			int result = 0;
			if (!int.TryParse(loaded.whichFarm, out result))
			{
				Game1.whichFarm = -1;
				throw new Exception(loaded.whichFarm + " is not a valid farm type.");
			}
			Game1.whichFarm = result;
		}
	}

	public static IEnumerator<int> getLoadEnumerator(string file)
	{
		Game1.SetSaveName(Path.GetFileNameWithoutExtension(file).Split('_').FirstOrDefault());
		Console.WriteLine("getLoadEnumerator('{0}')", file);
		Stopwatch stopwatch = Stopwatch.StartNew();
		Game1.loadingMessage = "Accessing save...";
		SaveGame saveData = new SaveGame();
		IsProcessing = true;
		if (CancelToTitle)
		{
			Game1.ExitToTitle();
		}
		yield return 1;
		Stream stream = null;
		string fullFilePath = file;
		Game1.savePathOverride = Path.GetDirectoryName(file);
		if (Game1.savePathOverride == "")
		{
			fullFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley", "Saves", file, file);
		}
		if (!File.Exists(fullFilePath))
		{
			fullFilePath += ".xml";
			if (!File.Exists(fullFilePath))
			{
				Game1.gameMode = 9;
				Game1.debugOutput = "File does not exist (-_-)";
				yield break;
			}
		}
		yield return 5;
		try
		{
			byte[] buffer = File.ReadAllBytes(fullFilePath);
			stream = new MemoryStream(buffer, writable: false);
		}
		catch (IOException ex)
		{
			Game1.gameMode = 9;
			Game1.debugOutput = Game1.parseText(ex.Message);
			if (stream != null)
			{
				stream.Close();
			}
			yield break;
		}
		Game1.loadingMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGame.cs.4696");
		yield return 7;
		byte num = (byte)stream.ReadByte();
		stream.Position--;
		if (num == 120)
		{
			Console.WriteLine("zlib stream detected...");
			stream = new ZlibStream(stream, CompressionMode.Decompress);
		}
		else
		{
			Console.WriteLine("regular stream detected...");
		}
		if (LocalMultiplayer.IsLocalMultiplayer())
		{
			loaded = (SaveGame)serializer.Deserialize(stream);
		}
		else
		{
			SaveGame pendingSaveGame = null;
			Task deserializeTask = new Task(delegate
			{
				Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
				pendingSaveGame = (SaveGame)serializer.Deserialize(stream);
			});
			Game1.hooks.StartTask(deserializeTask, "Load_Deserialize");
			while (!deserializeTask.IsCanceled && !deserializeTask.IsCompleted && !deserializeTask.IsFaulted)
			{
				yield return 20;
			}
			if (deserializeTask.IsFaulted)
			{
				Exception baseException = deserializeTask.Exception.GetBaseException();
				Console.WriteLine("deserializeTask failed with an exception");
				Console.WriteLine(baseException);
				throw baseException;
			}
			loaded = pendingSaveGame;
		}
		stream.Dispose();
		Game1.loadingMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGame.cs.4697");
		if (CancelToTitle)
		{
			Game1.ExitToTitle();
		}
		yield return 20;
		LoadFarmType();
		Game1.year = loaded.year;
		Game1.netWorldState.Value.CurrentPlayerLimit.Set(Game1.multiplayer.playerLimit);
		if (loaded.highestPlayerLimit >= 0)
		{
			Game1.netWorldState.Value.HighestPlayerLimit.Set(loaded.highestPlayerLimit);
		}
		else
		{
			Game1.netWorldState.Value.HighestPlayerLimit.Set(Math.Max(Game1.netWorldState.Value.HighestPlayerLimit.Value, Game1.multiplayer.MaxPlayers));
		}
		Game1.uniqueIDForThisGame = loaded.uniqueIDForThisGame;
		if (LocalMultiplayer.IsLocalMultiplayer())
		{
			Game1.loadForNewGame(loadedGame: true);
		}
		else
		{
			Task deserializeTask = new Task(delegate
			{
				Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
				Game1.loadForNewGame(loadedGame: true);
			});
			Game1.hooks.StartTask(deserializeTask, "Load_LoadForNewGame");
			while (!deserializeTask.IsCanceled && !deserializeTask.IsCompleted && !deserializeTask.IsFaulted)
			{
				yield return 24;
			}
			if (deserializeTask.IsFaulted)
			{
				Exception baseException2 = deserializeTask.Exception.GetBaseException();
				Console.WriteLine("loadNewGameTask failed with an exception");
				Console.WriteLine(baseException2);
				throw baseException2;
			}
			if (CancelToTitle)
			{
				Game1.ExitToTitle();
			}
			yield return 25;
		}
		Game1.weatherForTomorrow = loaded.weatherForTomorrow;
		Game1.dayOfMonth = loaded.dayOfMonth;
		Game1.year = loaded.year;
		Game1.currentSeason = loaded.currentSeason;
		Game1.worldStateIDs = loaded.worldStateIDs;
		Game1.loadingMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGame.cs.4698");
		if (loaded.mine_permanentMineChanges != null)
		{
			MineShaft.permanentMineChanges = loaded.mine_permanentMineChanges;
			Game1.netWorldState.Value.LowestMineLevel = loaded.mine_lowestLevelReached;
			Game1.netWorldState.Value.LowestMineLevelForOrder = loaded.mine_lowestLevelReachedForOrder;
		}
		Game1.currentGemBirdIndex = loaded.currentGemBirdIndex;
		if (loaded.bundleData.Count > 0)
		{
			Game1.netWorldState.Value.SetBundleData(loaded.bundleData);
			foreach (string key in Game1.netWorldState.Value.BundleData.Keys)
			{
				saveData.bundleData[key] = Game1.netWorldState.Value.BundleData[key];
			}
		}
		if (CancelToTitle)
		{
			Game1.ExitToTitle();
		}
		yield return 26;
		Game1.isRaining = loaded.isRaining;
		Game1.isLightning = loaded.isLightning;
		Game1.isSnowing = loaded.isSnowing;
		Game1.lastAppliedSaveFix = (SaveFixes)loaded.lastAppliedSaveFix;
		if (Game1.IsMasterGame)
		{
			Game1.netWorldState.Value.UpdateFromGame1();
		}
		if (loaded.locationWeather != null)
		{
			foreach (GameLocation.LocationContext key2 in loaded.locationWeather.Keys)
			{
				Game1.netWorldState.Value.GetWeatherForLocation(key2).CopyFrom(loaded.locationWeather[key2]);
			}
		}
		if (LocalMultiplayer.IsLocalMultiplayer())
		{
			loadDataToFarmer(loaded.player);
		}
		else
		{
			Task deserializeTask = new Task(delegate
			{
				Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
				loadDataToFarmer(loaded.player);
			});
			Game1.hooks.StartTask(deserializeTask, "Load_Farmer");
			while (!deserializeTask.IsCanceled && !deserializeTask.IsCompleted && !deserializeTask.IsFaulted)
			{
				yield return 1;
			}
			if (deserializeTask.IsFaulted)
			{
				Exception baseException3 = deserializeTask.Exception.GetBaseException();
				Console.WriteLine("loadFarmerTask failed with an exception");
				Console.WriteLine(baseException3);
				throw baseException3;
			}
		}
		Game1.player = loaded.player;
		if (Game1.MasterPlayer.hasOrWillReceiveMail("leoMoved") && Game1.getLocationFromName("Mountain") is Mountain mountain)
		{
			mountain.reloadMap();
			mountain.ApplyTreehouseIfNecessary();
			if (mountain.treehouseDoorDirty)
			{
				mountain.treehouseDoorDirty = false;
				NPC.populateRoutesFromLocationToLocationList();
			}
		}
		Game1.addParrotBoyIfNecessary();
		foreach (FarmerPair key3 in loaded.farmerFriendships.Keys)
		{
			Game1.player.team.friendshipData[key3] = loaded.farmerFriendships[key3];
		}
		Game1.spawnMonstersAtNight = loaded.shouldSpawnMonsters;
		Game1.player.team.limitedNutDrops.Clear();
		if (Game1.netWorldState != null && Game1.netWorldState.Value != null)
		{
			Game1.netWorldState.Value.RegisterSpecialCurrencies();
		}
		if (loaded.limitedNutDrops != null)
		{
			foreach (string key4 in loaded.limitedNutDrops.Keys)
			{
				if (loaded.limitedNutDrops[key4] > 0)
				{
					Game1.player.team.limitedNutDrops[key4] = loaded.limitedNutDrops[key4];
				}
			}
		}
		Game1.player.team.completedSpecialOrders.Clear();
		foreach (string completedSpecialOrder in loaded.completedSpecialOrders)
		{
			Game1.player.team.completedSpecialOrders[completedSpecialOrder] = true;
		}
		Game1.player.team.specialOrders.Clear();
		foreach (SpecialOrder specialOrder in loaded.specialOrders)
		{
			Game1.player.team.specialOrders.Add(specialOrder);
		}
		Game1.player.team.availableSpecialOrders.Clear();
		foreach (SpecialOrder availableSpecialOrder in loaded.availableSpecialOrders)
		{
			Game1.player.team.availableSpecialOrders.Add(availableSpecialOrder);
		}
		Game1.player.team.acceptedSpecialOrderTypes.Clear();
		Game1.player.team.acceptedSpecialOrderTypes.AddRange(loaded.acceptedSpecialOrderTypes);
		Game1.player.team.collectedNutTracker.Clear();
		foreach (string item in loaded.collectedNutTracker)
		{
			Game1.player.team.collectedNutTracker[item] = true;
		}
		Game1.player.team.junimoChest.Clear();
		foreach (Item item2 in loaded.junimoChest)
		{
			Game1.player.team.junimoChest.Add(item2);
		}
		Game1.player.team.returnedDonations.Clear();
		foreach (Item returnedDonation in loaded.returnedDonations)
		{
			Game1.player.team.returnedDonations.Add(returnedDonation);
		}
		if (loaded.stats != null)
		{
			Game1.player.stats = loaded.stats;
		}
		if (loaded.mailbox != null && !Game1.player.mailbox.Any())
		{
			Game1.player.mailbox.Clear();
			Game1.player.mailbox.AddRange(loaded.mailbox);
		}
		Game1.random = new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + 1);
		Game1.loadingMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGame.cs.4699");
		if (CancelToTitle)
		{
			Game1.ExitToTitle();
		}
		yield return 36;
		if (loaded.cellarAssignments != null)
		{
			foreach (int key5 in loaded.cellarAssignments.Keys)
			{
				Game1.player.team.cellarAssignments[key5] = loaded.cellarAssignments[key5];
			}
		}
		if (LocalMultiplayer.IsLocalMultiplayer())
		{
			loadDataToLocations(loaded.locations);
		}
		else
		{
			Task deserializeTask = new Task(delegate
			{
				Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
				loadDataToLocations(loaded.locations);
			});
			Game1.hooks.StartTask(deserializeTask, "Load_Locations");
			while (!deserializeTask.IsCanceled && !deserializeTask.IsCompleted && !deserializeTask.IsFaulted)
			{
				yield return 1;
			}
			if (deserializeTask.IsFaulted)
			{
				Exception baseException4 = deserializeTask.Exception.GetBaseException();
				Console.WriteLine("loadLocationsTask failed with an exception");
				Console.WriteLine(baseException4);
				throw deserializeTask.Exception.GetBaseException();
			}
		}
		foreach (Farmer allFarmer in Game1.getAllFarmers())
		{
			int money = allFarmer.Money;
			if (!Game1.player.team.individualMoney.ContainsKey(allFarmer.uniqueMultiplayerID))
			{
				Game1.player.team.individualMoney.Add(allFarmer.uniqueMultiplayerID, new NetIntDelta(money));
			}
			Game1.player.team.individualMoney[allFarmer.uniqueMultiplayerID].Value = money;
		}
		Game1.updateCellarAssignments();
		foreach (GameLocation location in Game1.locations)
		{
			if (location is BuildableGameLocation)
			{
				foreach (Building building in (location as BuildableGameLocation).buildings)
				{
					if (building.indoors.Value is FarmHouse)
					{
						(building.indoors.Value as FarmHouse).updateCellarWarps();
					}
				}
			}
			if (location is FarmHouse)
			{
				(location as FarmHouse).updateCellarWarps();
			}
		}
		if (CancelToTitle)
		{
			Game1.ExitToTitle();
		}
		yield return 50;
		yield return 51;
		Game1.isDebrisWeather = loaded.isDebrisWeather;
		if (Game1.isDebrisWeather)
		{
			Game1.populateDebrisWeatherArray();
		}
		else
		{
			Game1.debrisWeather.Clear();
		}
		yield return 53;
		Game1.player.team.sharedDailyLuck.Value = loaded.dailyLuck;
		yield return 54;
		yield return 55;
		Game1.setGraphicsForSeason();
		yield return 56;
		Game1.samBandName = loaded.samBandName;
		Game1.elliottBookName = loaded.elliottBookName;
		Game1.shippingTax = loaded.shippingTax;
		Game1.cropsOfTheWeek = loaded.cropsOfTheWeek;
		yield return 60;
		FurniturePlacer.addAllFurnitureOwnedByFarmer();
		yield return 63;
		Game1.weddingToday = loaded.weddingToday;
		Game1.loadingMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGame.cs.4700");
		yield return 64;
		Game1.loadingMessage = Game1.content.LoadString("Strings\\StringsFromCSFiles:SaveGame.cs.4701");
		yield return 73;
		Game1.farmerWallpaper = loaded.farmerWallpaper;
		yield return 75;
		Game1.updateWallpaperInFarmHouse(Game1.farmerWallpaper);
		yield return 77;
		Game1.FarmerFloor = loaded.FarmerFloor;
		if (CancelToTitle)
		{
			Game1.ExitToTitle();
		}
		yield return 79;
		Game1.updateFloorInFarmHouse(Game1.FarmerFloor);
		Game1.options.musicVolumeLevel = loaded.musicVolume;
		Game1.options.soundVolumeLevel = loaded.soundVolume;
		yield return 83;
		if (loaded.countdownToWedding.HasValue && loaded.countdownToWedding.Value != 0 && loaded.player.spouse != null && loaded.player.spouse != "")
		{
			WorldDate worldDate = new WorldDate(Game1.year, Game1.currentSeason, Game1.dayOfMonth);
			worldDate.TotalDays += loaded.countdownToWedding.Value;
			Friendship friendship = loaded.player.friendshipData[loaded.player.spouse];
			friendship.Status = FriendshipStatus.Engaged;
			friendship.WeddingDate = worldDate;
		}
		yield return 85;
		yield return 87;
		Game1.chanceToRainTomorrow = loaded.chanceToRainTomorrow;
		yield return 88;
		yield return 95;
		Game1.currentSongIndex = loaded.currentSongIndex;
		Game1.fadeToBlack = true;
		Game1.fadeIn = false;
		Game1.fadeToBlackAlpha = 0.99f;
		_ = Game1.player.mostRecentBed;
		if (Game1.player.mostRecentBed.X <= 0f)
		{
			Game1.player.Position = new Vector2(192f, 384f);
		}
		Game1.removeFrontLayerForFarmBuildings();
		Game1.addNewFarmBuildingMaps();
		GameLocation gameLocation = null;
		if (Game1.player.lastSleepLocation.Value != null && Game1.isLocationAccessible(Game1.player.lastSleepLocation.Value))
		{
			gameLocation = Game1.getLocationFromName(Game1.player.lastSleepLocation);
		}
		bool flag = true;
		if (gameLocation != null && (Game1.player.sleptInTemporaryBed.Value || gameLocation.GetFurnitureAt(Utility.PointToVector2(Game1.player.lastSleepPoint)) is BedFurniture))
		{
			Game1.currentLocation = gameLocation;
			Game1.player.currentLocation = Game1.currentLocation;
			Game1.player.Position = Utility.PointToVector2(Game1.player.lastSleepPoint) * 64f;
			flag = false;
		}
		if (flag)
		{
			Game1.currentLocation = Game1.getLocationFromName("FarmHouse");
		}
		Game1.currentLocation.map.LoadTileSheets(Game1.mapDisplayDevice);
		Game1.player.CanMove = true;
		Game1.player.ReequipEnchantments();
		if (loaded.junimoKartLeaderboards != null)
		{
			Game1.player.team.junimoKartScores.LoadScores(loaded.junimoKartLeaderboards.GetScores());
		}
		Game1.minecartHighScore = loaded.minecartHighScore;
		Game1.currentWallpaper = loaded.currentWallpaper;
		Game1.currentFloor = loaded.currentFloor;
		Game1.options = loaded.options;
		Game1.splitscreenOptions = loaded.splitscreenOptions;
		Game1.CustomData = loaded.CustomData;
		Game1.hasApplied1_3_UpdateChanges = loaded.hasApplied1_3_UpdateChanges;
		Game1.hasApplied1_4_UpdateChanges = loaded.hasApplied1_4_UpdateChanges;
		Game1.RefreshQuestOfTheDay();
		Game1.player.team.broadcastedMail.Clear();
		if (loaded.broadcastedMail != null)
		{
			foreach (string item3 in loaded.broadcastedMail)
			{
				Game1.player.team.broadcastedMail.Add(item3);
			}
		}
		if (Game1.options == null)
		{
			Game1.options = new Options();
			Game1.options.LoadDefaultOptions();
		}
		else
		{
			Game1.options.platformClampValues();
			Game1.options.SaveDefaultOptions();
		}
		try
		{
			StartupPreferences startupPreferences = new StartupPreferences();
			startupPreferences.loadPreferences(async: false, applyLanguage: false);
			Game1.options.gamepadMode = startupPreferences.gamepadMode;
		}
		catch (Exception)
		{
		}
		if (Game1.soundBank != null)
		{
			Game1.initializeVolumeLevels();
		}
		Game1.multiplayer.latestID = (ulong)loaded.latestID;
		Game1.netWorldState.Value.SkullCavesDifficulty = loaded.skullCavesDifficulty;
		Game1.netWorldState.Value.MinesDifficulty = loaded.minesDifficulty;
		Game1.netWorldState.Value.VisitsUntilY1Guarantee = loaded.visitsUntilY1Guarantee;
		Game1.netWorldState.Value.ShuffleMineChests = loaded.shuffleMineChests;
		Game1.netWorldState.Value.DishOfTheDay.Value = loaded.dishOfTheDay;
		if (Game1.IsRainingHere())
		{
			Game1.changeMusicTrack("rain", track_interruptable: true);
		}
		Game1.updateWeatherIcon();
		Game1.netWorldState.Value.MiniShippingBinsObtained.Set(loaded.miniShippingBinsObtained);
		Game1.netWorldState.Value.LostBooksFound.Set(loaded.lostBooksFound);
		Game1.netWorldState.Value.GoldenWalnuts.Set(loaded.goldenWalnuts);
		Game1.netWorldState.Value.GoldenWalnutsFound.Set(loaded.goldenWalnutsFound);
		Game1.netWorldState.Value.GoldenCoconutCracked.Value = loaded.goldenCoconutCracked;
		Game1.netWorldState.Value.FoundBuriedNuts.Clear();
		foreach (string foundBuriedNut in loaded.foundBuriedNuts)
		{
			Game1.netWorldState.Value.FoundBuriedNuts[foundBuriedNut] = true;
		}
		IslandSouth.SetupIslandSchedules();
		Game1.player.team.farmhandsCanMoveBuildings.Value = (FarmerTeam.RemoteBuildingPermissions)loaded.moveBuildingPermissionMode;
		Game1.player.team.mineShrineActivated.Value = loaded.mineShrineActivated;
		if (Game1.multiplayerMode == 2)
		{
			if (Program.sdk.Networking != null && Game1.options.serverPrivacy == ServerPrivacy.InviteOnly)
			{
				Game1.options.setServerMode("invite");
			}
			else if (Program.sdk.Networking != null && Game1.options.serverPrivacy == ServerPrivacy.FriendsOnly)
			{
				Game1.options.setServerMode("friends");
			}
			else
			{
				Game1.options.setServerMode("friends");
			}
		}
		Game1.bannedUsers = loaded.bannedUsers;
		bool flag2 = false;
		if (loaded.lostBooksFound < 0)
		{
			flag2 = true;
		}
		loaded = null;
		Game1.currentLocation.lastTouchActionLocation = Game1.player.getTileLocation();
		if (Game1.player.horseName.Value == null)
		{
			Horse horse = Utility.findHorse(Guid.Empty);
			if (horse != null && horse.displayName != "")
			{
				Game1.player.horseName.Value = horse.displayName;
				horse.ownerId.Value = Game1.player.UniqueMultiplayerID;
			}
		}
		Game1.UpdateHorseOwnership();
		foreach (Item item4 in Game1.player.items)
		{
			if (item4 != null && item4 is Object)
			{
				(item4 as Object).reloadSprite();
			}
		}
		Game1.gameMode = 3;
		Game1.AddModNPCs();
		try
		{
			Game1.fixProblems();
		}
		catch (Exception)
		{
		}
		foreach (Farmer allFarmer2 in Game1.getAllFarmers())
		{
			LevelUpMenu.AddMissedProfessionChoices(allFarmer2);
			LevelUpMenu.AddMissedLevelRecipes(allFarmer2);
			LevelUpMenu.RevalidateHealth(allFarmer2);
		}
		updateWedding();
		foreach (Building building2 in Game1.getFarm().buildings)
		{
			if ((int)building2.daysOfConstructionLeft <= 0 && building2.indoors.Value is Cabin)
			{
				(building2.indoors.Value as Cabin).updateFarmLayout();
			}
			if ((int)building2.daysOfConstructionLeft <= 0 && building2.indoors.Value is Shed)
			{
				Shed shed = building2.indoors.Value as Shed;
				shed.updateLayout();
				building2.updateInteriorWarps(shed);
			}
		}
		if (!Game1.hasApplied1_3_UpdateChanges)
		{
			Game1.apply1_3_UpdateChanges();
		}
		if (!Game1.hasApplied1_4_UpdateChanges)
		{
			Game1.apply1_4_UpdateChanges();
		}
		else
		{
			if (flag2)
			{
				Game1.recalculateLostBookCount();
			}
			Game1.UpdateFarmPerfection();
			Game1.doMorningStuff();
		}
		int num2 = (int)Game1.lastAppliedSaveFix;
		while (num2 < 35)
		{
			if (Enum.IsDefined(typeof(SaveFixes), num2))
			{
				num2++;
				SaveFixes saveFixes = (SaveFixes)num2;
				Console.WriteLine("Applying save fix: " + saveFixes);
				Game1.applySaveFix((SaveFixes)num2);
				Game1.lastAppliedSaveFix = (SaveFixes)num2;
			}
		}
		if (flag && Game1.player.currentLocation is FarmHouse)
		{
			Game1.player.Position = Utility.PointToVector2((Game1.player.currentLocation as FarmHouse).GetPlayerBedSpot()) * 64f;
		}
		BedFurniture.ShiftPositionForBed(Game1.player);
		Game1.stats.checkForAchievements();
		if (Game1.stats.stat_dictionary.ContainsKey("walnutsFound"))
		{
			Game1.netWorldState.Value.GoldenWalnutsFound.Value += (int)Game1.stats.stat_dictionary["walnutsFound"];
			Game1.stats.stat_dictionary.Remove("walnutsFound");
		}
		if (Game1.IsMasterGame)
		{
			Game1.netWorldState.Value.UpdateFromGame1();
		}
		Console.WriteLine("getLoadEnumerator() exited, elapsed = '{0}'", stopwatch.Elapsed);
		if (CancelToTitle)
		{
			Game1.ExitToTitle();
		}
		IsProcessing = false;
		Game1.player.currentLocation.lastTouchActionLocation = Game1.player.getTileLocation();
		Game1.player.currentLocation.resetForPlayerEntry();
		Game1.player.showToolUpgradeAvailability();
		Game1.dayTimeMoneyBox.questsDirty = true;
		yield return 100;
	}

	private static void updateWedding()
	{
	}

	public static void loadDataToFarmer(Farmer target)
	{
		target.gameVersion = target.gameVersion;
		target.items.CopyFrom(target.items);
		target.canMove = true;
		target.Sprite = new FarmerSprite(null);
		target.FarmerSprite.setOwner(target);
		if (target.cookingRecipes == null || target.cookingRecipes.Count() == 0)
		{
			target.cookingRecipes.Add("Fried Egg", 0);
		}
		if (target.craftingRecipes == null || target.craftingRecipes.Count() == 0)
		{
			target.craftingRecipes.Add("Lumber", 0);
		}
		if (!target.songsHeard.Contains("title_day"))
		{
			target.songsHeard.Add("title_day");
		}
		if (!target.songsHeard.Contains("title_night"))
		{
			target.songsHeard.Add("title_night");
		}
		if (target.addedSpeed > 0)
		{
			target.addedSpeed = 0;
		}
		target.maxItems.Value = target.maxItems;
		for (int i = 0; i < (int)target.maxItems; i++)
		{
			if (target.items.Count <= i)
			{
				target.items.Add(null);
			}
		}
		if (target.FarmerRenderer == null)
		{
			target.FarmerRenderer = new FarmerRenderer(target.getTexture(), target);
		}
		target.changeGender(target.IsMale);
		target.changeAccessory(target.accessory);
		target.changeShirt(target.shirt);
		target.changePants(target.pantsColor);
		target.changeSkinColor(target.skin);
		target.changeHairColor(target.hairstyleColor);
		target.changeHairStyle(target.hair);
		target.changeShoeColor(target.shoes);
		target.changeEyeColor(target.newEyeColor);
		target.Stamina = target.Stamina;
		target.health = target.health;
		target.MaxStamina = target.MaxStamina;
		target.mostRecentBed = target.mostRecentBed;
		target.Position = target.mostRecentBed;
		target.position.X -= 64f;
		if (!target.craftingRecipes.ContainsKey("Wood Path"))
		{
			target.craftingRecipes.Add("Wood Path", 1);
		}
		if (!target.craftingRecipes.ContainsKey("Gravel Path"))
		{
			target.craftingRecipes.Add("Gravel Path", 1);
		}
		if (!target.craftingRecipes.ContainsKey("Cobblestone Path"))
		{
			target.craftingRecipes.Add("Cobblestone Path", 1);
		}
		if (target.friendships != null && target.friendshipData.Count() == 0)
		{
			foreach (KeyValuePair<string, int[]> friendship3 in target.friendships)
			{
				Friendship friendship = new Friendship(friendship3.Value[0]);
				friendship.GiftsThisWeek = friendship3.Value[1];
				friendship.TalkedToToday = friendship3.Value[2] != 0;
				friendship.GiftsToday = friendship3.Value[3];
				friendship.ProposalRejected = friendship3.Value[4] != 0;
				target.friendshipData[friendship3.Key] = friendship;
			}
			target.friendships = null;
		}
		if (target.spouse != null && target.spouse != "")
		{
			bool flag = target.spouse.Contains("engaged");
			string text = target.spouse.Replace("engaged", "");
			Friendship friendship2 = target.friendshipData[text];
			if (friendship2.Status == FriendshipStatus.Friendly || friendship2.Status == FriendshipStatus.Dating || flag)
			{
				if (flag)
				{
					friendship2.Status = FriendshipStatus.Engaged;
				}
				else
				{
					friendship2.Status = FriendshipStatus.Married;
				}
				target.spouse = text;
				if (!flag)
				{
					friendship2.WeddingDate = new WorldDate(Game1.year, Game1.currentSeason, Game1.dayOfMonth);
					friendship2.WeddingDate.TotalDays -= target.daysMarried;
					target.daysMarried = 0;
				}
			}
		}
		target.questLog.Filter((Quest x) => x != null);
		target.songsHeard = target.songsHeard.Distinct().ToList();
		target.ConvertClothingOverrideToClothesItems();
		target.UpdateClothing();
	}

	public static void loadDataToLocations(List<GameLocation> gamelocations)
	{
		foreach (GameLocation gamelocation in gamelocations)
		{
			if (gamelocation is Cellar && Game1.getLocationFromName(gamelocation.name) == null)
			{
				Game1.locations.Add(new Cellar("Maps\\Cellar", gamelocation.name));
			}
			if (gamelocation is FarmHouse)
			{
				GameLocation locationFromName = Game1.getLocationFromName(gamelocation.name);
				(locationFromName as FarmHouse).setMapForUpgradeLevel((locationFromName as FarmHouse).upgradeLevel);
				(locationFromName as FarmHouse).fireplaceOn.Value = (gamelocation as FarmHouse).fireplaceOn;
				(locationFromName as FarmHouse).fridge.Value = (gamelocation as FarmHouse).fridge;
				(locationFromName as FarmHouse).farmerNumberOfOwner = (gamelocation as FarmHouse).farmerNumberOfOwner;
				(locationFromName as FarmHouse).ReadWallpaperAndFloorTileData();
			}
			if (gamelocation.name.Equals("Farm"))
			{
				GameLocation locationFromName2 = Game1.getLocationFromName(gamelocation.name);
				foreach (Building building in ((Farm)gamelocation).buildings)
				{
					building.load();
				}
				((Farm)locationFromName2).buildings.Set(((Farm)gamelocation).buildings);
				foreach (FarmAnimal value2 in ((Farm)gamelocation).animals.Values)
				{
					value2.reload(null);
				}
				((Farm)locationFromName2).greenhouseUnlocked.Value = ((Farm)gamelocation).greenhouseUnlocked.Value;
				((Farm)locationFromName2).greenhouseMoved.Value = ((Farm)gamelocation).greenhouseMoved.Value;
				((Farm)locationFromName2).UpdatePatio();
			}
			if (gamelocation.name.Equals("MovieTheater"))
			{
				(Game1.getLocationFromName(gamelocation.name) as MovieTheater).dayFirstEntered.Set((gamelocation as MovieTheater).dayFirstEntered);
			}
		}
		Game1.netWorldState.Value.ParrotPlatformsUnlocked.Set(loaded.parrotPlatformsUnlocked);
		Game1.player.team.farmPerfect.Value = loaded.farmPerfect;
		foreach (GameLocation gamelocation2 in gamelocations)
		{
			GameLocation locationFromName3 = Game1.getLocationFromName(gamelocation2.name);
			if (locationFromName3 == null)
			{
				continue;
			}
			locationFromName3.miniJukeboxCount.Value = gamelocation2.miniJukeboxCount.Value;
			locationFromName3.miniJukeboxTrack.Value = gamelocation2.miniJukeboxTrack.Value;
			locationFromName3.furniture.Set(gamelocation2.furniture);
			foreach (Furniture item in locationFromName3.furniture)
			{
				item.updateDrawPosition();
			}
			for (int num = gamelocation2.characters.Count - 1; num >= 0; num--)
			{
				initializeCharacter(gamelocation2.characters[num], locationFromName3);
			}
			foreach (LargeTerrainFeature largeTerrainFeature in gamelocation2.largeTerrainFeatures)
			{
				largeTerrainFeature.currentLocation = locationFromName3;
				largeTerrainFeature.loadSprite();
			}
			foreach (TerrainFeature value3 in gamelocation2.terrainFeatures.Values)
			{
				value3.currentLocation = locationFromName3;
				value3.loadSprite();
			}
			foreach (KeyValuePair<Vector2, Object> pair in gamelocation2.objects.Pairs)
			{
				pair.Value.initializeLightSource(pair.Key);
				pair.Value.reloadSprite();
			}
			if (gamelocation2.name.Equals("Farm"))
			{
				((Farm)locationFromName3).buildings.Set(((Farm)gamelocation2).buildings);
				foreach (FarmAnimal value4 in ((Farm)gamelocation2).animals.Values)
				{
					value4.reload(null);
				}
				foreach (Building building2 in ((Farm)locationFromName3).buildings)
				{
					building2.load();
				}
			}
			if (locationFromName3 == null)
			{
				continue;
			}
			locationFromName3.characters.Set(gamelocation2.characters);
			locationFromName3.netObjects.Set(gamelocation2.netObjects.Pairs);
			locationFromName3.numberOfSpawnedObjectsOnMap = gamelocation2.numberOfSpawnedObjectsOnMap;
			locationFromName3.terrainFeatures.Set(gamelocation2.terrainFeatures.Pairs);
			locationFromName3.largeTerrainFeatures.Set(gamelocation2.largeTerrainFeatures);
			if (locationFromName3.name.Equals("Farm"))
			{
				((Farm)locationFromName3).animals.MoveFrom(((Farm)gamelocation2).animals);
				(locationFromName3 as Farm).piecesOfHay.Value = (gamelocation2 as Farm).piecesOfHay;
				List<ResourceClump> other = new List<ResourceClump>((gamelocation2 as Farm).resourceClumps);
				(gamelocation2 as Farm).resourceClumps.Clear();
				(locationFromName3 as Farm).resourceClumps.Set(other);
				(locationFromName3 as Farm).hasSeenGrandpaNote = (gamelocation2 as Farm).hasSeenGrandpaNote;
				(locationFromName3 as Farm).grandpaScore = (gamelocation2 as Farm).grandpaScore;
				(locationFromName3 as Farm).petBowlWatered.Set((gamelocation2 as Farm).petBowlWatered.Value);
			}
			if (locationFromName3.name.Equals("Town"))
			{
				(locationFromName3 as Town).daysUntilCommunityUpgrade.Value = (gamelocation2 as Town).daysUntilCommunityUpgrade;
			}
			if (locationFromName3 is Beach)
			{
				(locationFromName3 as Beach).bridgeFixed.Value = (gamelocation2 as Beach).bridgeFixed;
			}
			if (locationFromName3 is Woods)
			{
				(locationFromName3 as Woods).stumps.MoveFrom((gamelocation2 as Woods).stumps);
				(locationFromName3 as Woods).hasUnlockedStatue.Value = (gamelocation2 as Woods).hasUnlockedStatue.Value;
			}
			if (locationFromName3 is CommunityCenter)
			{
				(locationFromName3 as CommunityCenter).areasComplete.Set((gamelocation2 as CommunityCenter).areasComplete);
			}
			if (locationFromName3 is ShopLocation && gamelocation2 is ShopLocation)
			{
				(locationFromName3 as ShopLocation).itemsFromPlayerToSell.MoveFrom((gamelocation2 as ShopLocation).itemsFromPlayerToSell);
				(locationFromName3 as ShopLocation).itemsToStartSellingTomorrow.MoveFrom((gamelocation2 as ShopLocation).itemsToStartSellingTomorrow);
			}
			if (locationFromName3 is Forest)
			{
				if (Game1.dayOfMonth % 7 % 5 == 0)
				{
					(locationFromName3 as Forest).travelingMerchantDay = true;
					(locationFromName3 as Forest).travelingMerchantBounds.Clear();
					(locationFromName3 as Forest).travelingMerchantBounds.Add(new Rectangle(1472, 640, 492, 112));
					(locationFromName3 as Forest).travelingMerchantBounds.Add(new Rectangle(1652, 744, 76, 48));
					(locationFromName3 as Forest).travelingMerchantBounds.Add(new Rectangle(1812, 744, 104, 48));
					foreach (Rectangle travelingMerchantBound in (locationFromName3 as Forest).travelingMerchantBounds)
					{
						Utility.clearObjectsInArea(travelingMerchantBound, locationFromName3);
					}
				}
				(locationFromName3 as Forest).log = (gamelocation2 as Forest).log;
			}
			locationFromName3.TransferDataFromSavedLocation(gamelocation2);
			if (locationFromName3 is IslandLocation)
			{
				(locationFromName3 as IslandLocation).AddAdditionalWalnutBushes();
			}
		}
		List<NPC> list = new List<NPC>();
		foreach (GameLocation location in Game1.locations)
		{
			for (int i = 0; i < location.characters.Count; i++)
			{
				list.Add(location.characters[i]);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			list[j].reloadSprite();
		}
		foreach (GameLocation location2 in Game1.locations)
		{
			if (!(location2 is BuildableGameLocation))
			{
				continue;
			}
			foreach (Building building3 in (location2 as BuildableGameLocation).buildings)
			{
				GameLocation value = building3.indoors.Value;
				if (value == null)
				{
					continue;
				}
				for (int num2 = value.characters.Count - 1; num2 >= 0; num2--)
				{
					if (num2 < value.characters.Count)
					{
						value.characters[num2].reloadSprite();
					}
				}
			}
		}
		foreach (GameLocation location3 in Game1.locations)
		{
			if (!location3.name.Equals("Farm"))
			{
				continue;
			}
			Game1.getLocationFromName(location3.name);
			foreach (Building building4 in ((Farm)location3).buildings)
			{
				if (building4 is Stable && (int)building4.daysOfConstructionLeft <= 0)
				{
					(building4 as Stable).grabHorse();
				}
			}
		}
		if (Game1.getLocationFromName("FarmCave") is FarmCave farmCave)
		{
			farmCave.UpdateReadyFlag();
		}
		Game1.player.currentLocation = Utility.getHomeOfFarmer(Game1.player);
	}

	public static void initializeCharacter(NPC c, GameLocation location)
	{
		string defaultMap = c.DefaultMap;
		Vector2 defaultPosition = c.DefaultPosition;
		c.reloadData();
		if (c.DefaultMap != defaultMap)
		{
			c.DefaultMap = defaultMap;
			c.DefaultPosition = defaultPosition;
		}
		if (!c.DefaultPosition.Equals(Vector2.Zero))
		{
			c.Position = c.DefaultPosition;
		}
		c.currentLocation = location;
		if (c.datingFarmer)
		{
			Friendship friendship = Game1.player.friendshipData[c.Name];
			if (!friendship.IsDating())
			{
				friendship.Status = FriendshipStatus.Dating;
			}
			c.datingFarmer = false;
		}
		else if (c.divorcedFromFarmer)
		{
			Friendship friendship2 = Game1.player.friendshipData[c.Name];
			if (!friendship2.IsDivorced())
			{
				friendship2.RoommateMarriage = false;
				friendship2.Status = FriendshipStatus.Divorced;
			}
			c.divorcedFromFarmer = false;
		}
	}
}
