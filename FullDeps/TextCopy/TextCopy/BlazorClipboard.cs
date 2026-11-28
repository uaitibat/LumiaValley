using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace TextCopy;

public class BlazorClipboard : IClipboard
{
	private static Type[] types = new Type[3]
	{
		typeof(string),
		typeof(CancellationToken),
		typeof(object[])
	};

	private object jsRuntime;

	private MethodInfo invokeAsync;

	public BlazorClipboard(object jsRuntime)
	{
		this.jsRuntime = jsRuntime;
		invokeAsync = jsRuntime.GetType().GetMethod("InvokeAsync", types);
		invokeAsync = invokeAsync.MakeGenericMethod(typeof(string));
	}

	public virtual async Task<string?> GetTextAsync(CancellationToken cancellation = default(CancellationToken))
	{
		object[] parameters = new object[3]
		{
			"navigator.clipboard.readText",
			cancellation,
			Array.Empty<object>()
		};
		return await (ValueTask<string>)invokeAsync.Invoke(jsRuntime, parameters);
	}

	public virtual string? GetText()
	{
		return GetTextAsync().GetAwaiter().GetResult();
	}

	public virtual async Task SetTextAsync(string text, CancellationToken cancellation = default(CancellationToken))
	{
		object[] parameters = new object[3]
		{
			"navigator.clipboard.writeText",
			cancellation,
			new object[1] { text }
		};
		await (ValueTask<string>)invokeAsync.Invoke(jsRuntime, parameters);
	}

	public virtual void SetText(string text)
	{
		SetTextAsync(text).GetAwaiter().GetResult();
	}
}
