using System.Threading;
using System.Threading.Tasks;

namespace TextCopy;

public class MockClipboard : IClipboard
{
	public virtual Task<string?> GetTextAsync(CancellationToken cancellation = default(CancellationToken))
	{
		return Task.FromResult<string>(null);
	}

	public virtual string? GetText()
	{
		return null;
	}

	public virtual Task SetTextAsync(string text, CancellationToken cancellation = default(CancellationToken))
	{
		return Task.CompletedTask;
	}

	public void SetText(string text)
	{
	}
}
