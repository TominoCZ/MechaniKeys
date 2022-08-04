using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;

namespace MechaniKeys
{
	class SoundPlayer : IDisposable
	{
		private readonly Dictionary<string, string> _sounds = new Dictionary<string, string>();
		public List<string> KeyTaps = new List<string>();

		public SoundPlayer()
		{
			Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

			Directory.CreateDirectory("assets/sounds/key");

			var keys = Directory.GetFiles("assets/sounds/key");
            foreach (var file in keys)
            {
				var id = $"key/{Path.GetFileNameWithoutExtension(file)}";

				Cache(id);

				KeyTaps.Add(id);
			}

			var specific = Directory.GetFiles("assets/sounds");
			foreach (var file in specific)
            {
				Cache(Path.GetFileNameWithoutExtension(file));
            }
		}

		public void Cache(string id, string ext = "wav")
		{
			_sounds.Add(id, $"assets/sounds/{id}.{ext}");
		}

		public void Play(string id, float volume = 1, float pitch = 1)
		{
			if (_sounds.TryGetValue(id, out var sound))
			{
				var s = Bass.BASS_StreamCreateFile(sound, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_PRESCAN | BASSFlag.BASS_FX_FREESOURCE);//sound, 0, 0, BASSFlag.BASS_STREAM_AUTOFREE);
				
				s = BassFx.BASS_FX_TempoCreate(s, BASSFlag.BASS_STREAM_PRESCAN | BASSFlag.BASS_STREAM_AUTOFREE);

				Bass.BASS_ChannelSetAttribute(s, BASSAttribute.BASS_ATTRIB_VOL, volume);
				Bass.BASS_ChannelSetAttribute(s, BASSAttribute.BASS_ATTRIB_TEMPO_PITCH, (pitch - 1) * 60);

				//Bass.BASS_ChannelPlay(sound, false);
				Bass.BASS_ChannelPlay(s, false);
			}
		}

		public void Dispose()
		{
			Bass.BASS_Free();
		}
	}
}