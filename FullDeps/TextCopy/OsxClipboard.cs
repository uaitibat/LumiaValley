using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

internal static class OsxClipboard
{
	private static IntPtr nsString;

	private static IntPtr nsPasteboard;

	private static IntPtr nsStringPboardType;

	private static IntPtr utfTextType;

	private static IntPtr generalPasteboard;

	private static IntPtr initWithUtf8Register;

	private static IntPtr allocRegister;

	private static IntPtr setStringRegister;

	private static IntPtr stringForTypeRegister;

	private static IntPtr utf8Register;

	private static IntPtr generalPasteboardRegister;

	private static IntPtr clearContentsRegister;

	static OsxClipboard()
	{
		nsString = objc_getClass("NSString");
		nsPasteboard = objc_getClass("NSPasteboard");
		initWithUtf8Register = sel_registerName("initWithUTF8String:");
		allocRegister = sel_registerName("alloc");
		setStringRegister = sel_registerName("setString:forType:");
		stringForTypeRegister = sel_registerName("stringForType:");
		utf8Register = sel_registerName("UTF8String");
		generalPasteboardRegister = sel_registerName("generalPasteboard");
		clearContentsRegister = sel_registerName("clearContents");
		utfTextType = objc_msgSend(objc_msgSend(nsString, allocRegister), initWithUtf8Register, "public.utf8-plain-text");
		nsStringPboardType = objc_msgSend(objc_msgSend(nsString, allocRegister), initWithUtf8Register, "NSStringPboardType");
		generalPasteboard = objc_msgSend(nsPasteboard, generalPasteboardRegister);
	}

	public static string? GetText()
	{
		IntPtr receiver = objc_msgSend(generalPasteboard, stringForTypeRegister, nsStringPboardType);
		IntPtr ptr = objc_msgSend(receiver, utf8Register);
		return Marshal.PtrToStringAnsi(ptr);
	}

	public static Task<string?> GetTextAsync(CancellationToken cancellation)
	{
		return Task.FromResult(GetText());
	}

	public static void SetText(string text)
	{
		IntPtr intPtr = default(IntPtr);
		try
		{
			intPtr = objc_msgSend(objc_msgSend(nsString, allocRegister), initWithUtf8Register, text);
			objc_msgSend(generalPasteboard, clearContentsRegister);
			objc_msgSend(generalPasteboard, setStringRegister, intPtr, utfTextType);
		}
		finally
		{
			if (intPtr != (IntPtr)0)
			{
				objc_msgSend(intPtr, sel_registerName("release"));
			}
		}
	}

	public static Task SetTextAsync(string text, CancellationToken cancellation)
	{
		SetText(text);
		return Task.CompletedTask;
	}

	[DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
	private static extern IntPtr objc_getClass(string className);

	[DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
	private static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector);

	[DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
	private static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, string arg1);

	[DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
	private static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, IntPtr arg1);

	[DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
	private static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);

	[DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
	private static extern IntPtr sel_registerName(string selectorName);
}
