using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace TextCopy;

public static class ClipboardService
{
	private static Func<CancellationToken, Task<string?>> getAsyncFunc;

	private static Func<string?> getFunc;

	private static Func<string, CancellationToken, Task> setAsyncAction;

	private static Action<string> setAction;

	public static Task<string?> GetTextAsync(CancellationToken cancellation = default(CancellationToken))
	{
		return getAsyncFunc(cancellation);
	}

	public static string? GetText()
	{
		return getFunc();
	}

	static ClipboardService()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("Browser")))
		{
			throw new Exception("The static class ClipboardService is not supported on Blazor. Instead inject an IClipboard using ServiceExtensionsInjectClipboard.");
		}
		getAsyncFunc = CreateAsyncGet();
		getFunc = CreateGet();
		setAsyncAction = CreateAsyncSet();
		setAction = CreateSet();
	}

	public static Task SetTextAsync(string text, CancellationToken cancellation = default(CancellationToken))
	{
		Guard.AgainstNull(text, "text");
		return setAsyncAction(text, cancellation);
	}

	public static void SetText(string text)
	{
		Guard.AgainstNull(text, "text");
		setAction(text);
	}

	private static Func<CancellationToken, Task<string?>> CreateAsyncGet()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			return WindowsClipboard.GetTextAsync;
		}
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			return OsxClipboard.GetTextAsync;
		}
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			return LinuxClipboard.GetTextAsync;
		}
		throw new NotSupportedException();
	}

	private static Func<string?> CreateGet()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			return WindowsClipboard.GetText;
		}
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			return OsxClipboard.GetText;
		}
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			return LinuxClipboard.GetText;
		}
		throw new NotSupportedException();
	}

	private static Func<string, CancellationToken, Task> CreateAsyncSet()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			return WindowsClipboard.SetTextAsync;
		}
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			return OsxClipboard.SetTextAsync;
		}
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			return LinuxClipboard.SetTextAsync;
		}
		return delegate
		{
			throw new NotSupportedException();
		};
	}

	private static Action<string> CreateSet()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			return WindowsClipboard.SetText;
		}
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
		{
			return OsxClipboard.SetText;
		}
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			return LinuxClipboard.SetText;
		}
		return delegate
		{
			throw new NotSupportedException();
		};
	}
}
