using NAudio.CoreAudioApi.Interfaces;
using System.Runtime.InteropServices;

namespace NAudio.CoreAudioApi
{
	public class SessionCollection
	{
		private readonly IAudioSessionEnumerator audioSessionEnumerator;

		public AudioSessionControl this[int index]
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioSessionEnumerator.GetSession(index, out IAudioSessionControl session));
				return new AudioSessionControl(session);
			}
		}

		public int Count
		{
			get
			{
				Marshal.ThrowExceptionForHR(audioSessionEnumerator.GetCount(out int sessionCount));
				return sessionCount;
			}
		}

		internal SessionCollection(IAudioSessionEnumerator realEnumerator)
		{
			audioSessionEnumerator = realEnumerator;
		}
	}
}
