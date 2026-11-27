using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

internal static class WindowsClipboard
{
	private const uint cfUnicodeText = 13u;

	public static async Task SetTextAsync(string text, CancellationToken cancellation)
	{
		await TryOpenClipboardAsync(cancellation);
		InnerSet(text);
	}

	public static void SetText(string text)
	{
		TryOpenClipboard();
		InnerSet(text);
	}

	private static void InnerSet(string text)
	{
		EmptyClipboard();
		IntPtr intPtr = default(IntPtr);
		try
		{
			int cb = (text.Length + 1) * 2;
			intPtr = Marshal.AllocHGlobal(cb);
			if (intPtr == (IntPtr)0)
			{
				ThrowWin32();
			}
			IntPtr intPtr2 = GlobalLock(intPtr);
			if (intPtr2 == (IntPtr)0)
			{
				ThrowWin32();
			}
			try
			{
				Marshal.Copy(text.ToCharArray(), 0, intPtr2, text.Length);
			}
			finally
			{
				GlobalUnlock(intPtr2);
			}
			if (SetClipboardData(13u, intPtr) == (IntPtr)0)
			{
				ThrowWin32();
			}
			intPtr = default(IntPtr);
		}
		finally
		{
			if (intPtr != (IntPtr)0)
			{
				Marshal.FreeHGlobal(intPtr);
			}
			CloseClipboard();
		}
	}

	private static async Task TryOpenClipboardAsync(CancellationToken cancellation)
	{
		int num = 10;
		while (!OpenClipboard((IntPtr)0))
		{
			int num2 = num - 1;
			num = num2;
			if (num2 == 0)
			{
				ThrowWin32();
			}
			await Task.Delay(100, cancellation);
		}
	}

	private static void TryOpenClipboard()
	{
		int num = 10;
		while (!OpenClipboard((IntPtr)0))
		{
			if (--num == 0)
			{
				ThrowWin32();
			}
			Thread.Sleep(100);
		}
	}

	public static async Task<string?> GetTextAsync(CancellationToken cancellation)
	{
		if (!IsClipboardFormatAvailable(13u))
		{
			return null;
		}
		await TryOpenClipboardAsync(cancellation);
		return InnerGet();
	}

	public static string? GetText()
	{
		if (!IsClipboardFormatAvailable(13u))
		{
			return null;
		}
		TryOpenClipboard();
		return InnerGet();
	}

	private static string? InnerGet()
	{
		IntPtr intPtr = default(IntPtr);
		IntPtr intPtr2 = default(IntPtr);
		try
		{
			intPtr = GetClipboardData(13u);
			if (intPtr == (IntPtr)0)
			{
				return null;
			}
			intPtr2 = GlobalLock(intPtr);
			if (intPtr2 == (IntPtr)0)
			{
				return null;
			}
			int num = GlobalSize(intPtr);
			byte[] array = new byte[num];
			Marshal.Copy(intPtr2, array, 0, num);
			return Encoding.Unicode.GetString(array).TrimEnd('\0');
		}
		finally
		{
			if (intPtr2 != (IntPtr)0)
			{
				GlobalUnlock(intPtr);
			}
			CloseClipboard();
		}
	}

	private static void ThrowWin32()
	{
		throw new Win32Exception(Marshal.GetLastWin32Error());
	}

	[DllImport("User32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool IsClipboardFormatAvailable(uint format);

	[DllImport("User32.dll", SetLastError = true)]
	private static extern IntPtr GetClipboardData(uint uFormat);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern IntPtr GlobalLock(IntPtr hMem);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool GlobalUnlock(IntPtr hMem);

	[DllImport("user32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool OpenClipboard(IntPtr hWndNewOwner);

	[DllImport("user32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool CloseClipboard();

	[DllImport("user32.dll", SetLastError = true)]
	private static extern IntPtr SetClipboardData(uint uFormat, IntPtr data);

	[DllImport("user32.dll")]
	private static extern bool EmptyClipboard();

	[DllImport("Kernel32.dll", SetLastError = true)]
	private static extern int GlobalSize(IntPtr hMem);
}
