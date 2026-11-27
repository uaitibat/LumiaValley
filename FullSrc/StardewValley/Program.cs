using System;
using System.IO;
using System.Text;
using StardewValley.SDKs;

namespace StardewValley;

public static class Program
{
	public enum LogType
	{
		Error,
		Disconnect
	}

	public const int build_steam = 0;

	public const int build_gog = 1;

	public const int build_rail = 2;

	public const int build_gdk = 3;

	public static bool GameTesterMode = false;

	public static bool releaseBuild = true;

	public const int buildType = 0;

	private static SDKHelper _sdk;

	public static Game1 gamePtr;

	public static bool handlingException;

	public static bool hasTriedToPrintLog;

	public static bool successfullyPrintedLog;

	internal static SDKHelper sdk
	{
		get
		{
			if (_sdk == null)
			{
				_sdk = new SteamHelper();
			}
			return _sdk;
		}
	}

	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	public static void Main(string[] args)
	{
		GameTesterMode = true;
		AppDomain.CurrentDomain.UnhandledException += handleException;
		using GameRunner gameRunner = new GameRunner();
		GameRunner.instance = gameRunner;
		gameRunner.Run();
	}

	public static string WriteLog(LogType logType, string message, bool append = false)
	{
		string text = "ErrorLogs";
		string path;
		if (logType != 0 && logType == LogType.Disconnect)
		{
			text = "DisconnectLogs";
			path = ((Game1.player != null) ? Game1.player.Name : "NullPlayer") + "_" + DateTime.Now.Month + "-" + DateTime.Now.Day + ".txt";
		}
		else
		{
			text = "ErrorLogs";
			path = ((Game1.player != null) ? Game1.player.Name : "NullPlayer") + "_" + Game1.uniqueIDForThisGame + "_" + ((Game1.player != null) ? ((int)Game1.player.millisecondsPlayed) : Game1.random.Next(999999)) + ".txt";
		}
		int folder = ((Environment.OSVersion.Platform != PlatformID.Unix) ? 26 : 28);
		string text2 = Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath((Environment.SpecialFolder)folder), "StardewValley"), text), path);
		FileInfo fileInfo = new FileInfo(Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath((Environment.SpecialFolder)folder), "StardewValley"), text), "asdfasdf"));
		if (!fileInfo.Directory.Exists)
		{
			fileInfo.Directory.Create();
		}
		fileInfo = null;
		if (append)
		{
			if (!File.Exists(text2))
			{
				File.CreateText(text2);
			}
			try
			{
				File.AppendAllText(text2, message + Environment.NewLine);
				return text2;
			}
			catch (Exception)
			{
				return null;
			}
		}
		if (File.Exists(text2))
		{
			File.Delete(text2);
		}
		try
		{
			File.WriteAllText(text2, message);
			return text2;
		}
		catch (Exception)
		{
			return null;
		}
	}

	public static void AppendDiagnostics(StringBuilder sb)
	{
		sb.AppendLine("Game Version: " + Game1.GetVersionString());
		try
		{
			if (sdk != null)
			{
				sb.AppendLine("SDK Helper: " + sdk.GetType().Name);
			}
			sb.AppendLine("Game Language: " + LocalizedContentManager.CurrentLanguageCode);
			try
			{
				sb.AppendLine("GPU: " + Game1.graphics.GraphicsDevice.Adapter.Description);
			}
			catch (Exception)
			{
				sb.AppendLine("GPU: Could not detect.");
			}
			sb.AppendLine("OS: " + Environment.OSVersion.Platform.ToString() + " " + Environment.OSVersion.VersionString);
			if (GameRunner.instance != null && GameRunner.instance.GetType().FullName.StartsWith("StardewModdingAPI."))
			{
				sb.AppendLine("Running SMAPI");
			}
			if (Game1.IsMultiplayer)
			{
				if (LocalMultiplayer.IsLocalMultiplayer())
				{
					sb.AppendLine("Multiplayer (Split Screen)");
				}
				else if (Game1.IsMasterGame)
				{
					sb.AppendLine("Multiplayer (Host)");
				}
				else
				{
					sb.AppendLine("Multiplayer (Client)");
				}
			}
			if (Game1.options.gamepadControls)
			{
				sb.AppendLine("Playing on Controller");
			}
			sb.AppendLine("In-game Date: " + Game1.currentSeason + " " + Game1.dayOfMonth + " Y" + Game1.year + " Time of Day: " + Game1.timeOfDay);
			sb.AppendLine("Game Location: " + ((Game1.currentLocation == null) ? "null" : Game1.currentLocation.NameOrUniqueName));
		}
		catch (Exception)
		{
		}
	}

	public static void handleException(object sender, UnhandledExceptionEventArgs args)
	{
		if (handlingException || !GameTesterMode)
		{
			return;
		}
		Game1.gameMode = 11;
		handlingException = true;
		StringBuilder stringBuilder = new StringBuilder();
		Exception ex = null;
		if (args != null)
		{
			ex = (Exception)args.ExceptionObject;
			stringBuilder.AppendLine("Message: " + ex.Message);
			stringBuilder.AppendLine("InnerException: " + ex.InnerException);
			stringBuilder.AppendLine("Stack Trace: " + ex.StackTrace);
			stringBuilder.AppendLine("");
		}
		AppendDiagnostics(stringBuilder);
		Game1.errorMessage = stringBuilder.ToString();
		long num = DateTime.Now.Ticks / 10000 + 25000;
		if (!hasTriedToPrintLog)
		{
			hasTriedToPrintLog = true;
			string text = WriteLog(LogType.Error, Game1.errorMessage);
			if (text != null)
			{
				successfullyPrintedLog = true;
				Game1.errorMessage = "(Error Report created at " + text + ")" + Environment.NewLine + Game1.errorMessage;
			}
		}
		if (args != null)
		{
			Game1.gameMode = 3;
		}
	}
}
