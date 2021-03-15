using NAudio.CoreAudioApi.Interfaces;
using System;
using System.Runtime.InteropServices;

namespace NAudio.CoreAudioApi
{
	public class AudioRenderClient : IDisposable
	{
		private IAudioRenderClient audioRenderClientInterface;

		internal AudioRenderClient(IAudioRenderClient audioRenderClientInterface)
		{
			this.audioRenderClientInterface = audioRenderClientInterface;
		}

		public IntPtr GetBuffer(int numFramesRequested)
		{
			Marshal.ThrowExceptionForHR(audioRenderClientInterface.GetBuffer(numFramesRequested, out IntPtr dataBufferPointer));
			return dataBufferPointer;
		}

		public void ReleaseBuffer(int numFramesWritten, AudioClientBufferFlags bufferFlags)
		{
			Marshal.ThrowExceptionForHR(audioRenderClientInterface.ReleaseBuffer(numFramesWritten, bufferFlags));
		}

		public void Dispose()
		{
			if (audioRenderClientInterface != null)
			{
				Marshal.ReleaseComObject(audioRenderClientInterface);
				audioRenderClientInterface = null;
				GC.SuppressFinalize(this);
			}
		}
	}
}
