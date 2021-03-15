using NAudio.CoreAudioApi.Interfaces;
using System.Runtime.InteropServices;

namespace NAudio.CoreAudioApi
{
	public class AudioEndpointVolumeVolumeRange
	{
		private readonly float volumeMinDecibels;

		private readonly float volumeMaxDecibels;

		private readonly float volumeIncrementDecibels;

		public float MinDecibels => volumeMinDecibels;

		public float MaxDecibels => volumeMaxDecibels;

		public float IncrementDecibels => volumeIncrementDecibels;

		internal AudioEndpointVolumeVolumeRange(IAudioEndpointVolume parent)
		{
			Marshal.ThrowExceptionForHR(parent.GetVolumeRange(out volumeMinDecibels, out volumeMaxDecibels, out volumeIncrementDecibels));
		}
	}
}
