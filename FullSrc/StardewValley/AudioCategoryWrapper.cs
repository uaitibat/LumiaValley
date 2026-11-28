using Microsoft.Xna.Framework.Audio;

namespace StardewValley;

public class AudioCategoryWrapper : IAudioCategory
{
	private AudioCategory audioCategory;

	public AudioCategoryWrapper(AudioCategory category)
	{
		audioCategory = category;
	}

	public void SetVolume(float volume)
	{
		// UWP/MonoGame: GetCategory pode devolver null (audio XACT indisponivel).
		// Sem essa guarda, updateMusic() derruba o jogo no primeiro frame.
		if (audioCategory != null)
			audioCategory.SetVolume(volume);
	}
}
