using NAudio.CoreAudioApi.Interfaces;
using System.Runtime.InteropServices;

namespace NAudio.CoreAudioApi
{
	public class AudioEndpointVolumeChannels
	{
		private readonly IAudioEndpointVolume audioEndPointVolume;

		private readonly AudioEndpointVolumeChannel[] channels;

		public int Count
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioEndPointVolume.GetChannelCount(out int pnChannelCount));
				return pnChannelCount;
			}
		}

		public AudioEndpointVolumeChannel this[int index]
		{
			get
			{
				return channels[index];
			}
		}

		internal AudioEndpointVolumeChannels(IAudioEndpointVolume parent)
		{
			audioEndPointVolume = parent;
			int count = Count;
			channels = new AudioEndpointVolumeChannel[count];
			for (int i = 0; i < count; i++)
			{
				channels[i] = new AudioEndpointVolumeChannel(audioEndPointVolume, i);
			}
		}
	}
}
