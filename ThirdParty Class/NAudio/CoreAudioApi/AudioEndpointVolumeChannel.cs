using NAudio.CoreAudioApi.Interfaces;
using System;
using System.Runtime.InteropServices;

namespace NAudio.CoreAudioApi
{
	public class AudioEndpointVolumeChannel
	{
		private readonly uint channel;

		private readonly IAudioEndpointVolume audioEndpointVolume;

		public float VolumeLevel
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioEndpointVolume.GetChannelVolumeLevel(channel, out float pfLevelDB));
				return pfLevelDB;
			}
			set
			{
				Marshal.ThrowExceptionForHR(audioEndpointVolume.SetChannelVolumeLevel(channel, value, Guid.Empty));
			}
		}

		public float VolumeLevelScalar
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioEndpointVolume.GetChannelVolumeLevelScalar(channel, out float pfLevel));
				return pfLevel;
			}
			set
			{
				Marshal.ThrowExceptionForHR(audioEndpointVolume.SetChannelVolumeLevelScalar(channel, value, Guid.Empty));
			}
		}

		internal AudioEndpointVolumeChannel(IAudioEndpointVolume parent, int channel)
		{
			this.channel = (uint)channel;
			audioEndpointVolume = parent;
		}
	}
}
