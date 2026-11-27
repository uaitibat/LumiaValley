using System.Threading;
using System.Threading.Tasks;

namespace TextCopy;

public class Clipboard : IClipboard
{
	public virtual Task<string?> GetTextAsync(CancellationToken cancellation = default(CancellationToken))
	{
		return ClipboardService.GetTextAsync(cancellation);
	}

	public virtual string? GetText()
	{
		return ClipboardService.GetText();
	}

	public virtual Task SetTextAsync(string text, CancellationToken cancellation = default(CancellationToken))
	{
		return ClipboardService.SetTextAsync(text, cancellation);
	}

	public virtual void SetText(string text)
	{
		ClipboardService.SetText(text);
	}
}
