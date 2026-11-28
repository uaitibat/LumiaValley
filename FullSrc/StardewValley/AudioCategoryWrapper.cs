using System;
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
		// UWP/MonoGame: GetCategory devolve um AudioCategory "vazio" (sem XACT).
		// Ele nao e null, mas o campo interno e — SetVolume lanca NRE dentro.
		// Try/catch e a unica guarda confiavel aqui.
		try
		{
			if (audioCategory != null)
				audioCategory.SetVolume(volume);
		}
		catch (Exception)
		{
			// Audio indisponivel no UWP; segue sem volume.
		}
	}
}
