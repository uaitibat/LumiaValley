using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

internal static class LinuxClipboard
{
	private static bool isWsl;

	static LinuxClipboard()
	{
		isWsl = Environment.GetEnvironmentVariable("WSL_DISTRO_NAME") != null;
	}

	public static async Task SetTextAsync(string text, CancellationToken cancellation)
	{
		string tempFileName = Path.GetTempFileName();
		await File.WriteAllTextAsync(tempFileName, text, cancellation);
		if (!cancellation.IsCancellationRequested)
		{
			InnerSetText(tempFileName);
		}
	}

	public static void SetText(string text)
	{
		string tempFileName = Path.GetTempFileName();
		File.WriteAllText(tempFileName, text);
		InnerSetText(tempFileName);
	}

	private static void InnerSetText(string tempFileName)
	{
		try
		{
			if (isWsl)
			{
				BashRunner.Run("cat " + tempFileName + " | clip.exe ");
			}
			else
			{
				BashRunner.Run("cat " + tempFileName + " | xsel -i --clipboard ");
			}
		}
		finally
		{
			File.Delete(tempFileName);
		}
	}

	public static string? GetText()
	{
		string tempFileName = Path.GetTempFileName();
		try
		{
			InnerGetText(tempFileName);
			return File.ReadAllText(tempFileName);
		}
		finally
		{
			File.Delete(tempFileName);
		}
	}

	public static async Task<string?> GetTextAsync(CancellationToken cancellation)
	{
		string tempFileName = Path.GetTempFileName();
		try
		{
			InnerGetText(tempFileName);
			return await File.ReadAllTextAsync(tempFileName, cancellation);
		}
		finally
		{
			File.Delete(tempFileName);
		}
	}

	private static void InnerGetText(string tempFileName)
	{
		if (isWsl)
		{
			BashRunner.Run("powershell.exe Get-Clipboard  > " + tempFileName);
		}
		else
		{
			BashRunner.Run("xsel -o --clipboard  > " + tempFileName);
		}
	}
}
