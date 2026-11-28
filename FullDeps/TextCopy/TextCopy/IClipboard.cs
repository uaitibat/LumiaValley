using System.Threading;
using System.Threading.Tasks;

namespace TextCopy;

public interface IClipboard
{
	Task<string?> GetTextAsync(CancellationToken cancellation = default(CancellationToken));

	string? GetText();

	Task SetTextAsync(string text, CancellationToken cancellation = default(CancellationToken));

	void SetText(string text);
}
