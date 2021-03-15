using NAudio.CoreAudioApi.Interfaces;
using System;
using System.Runtime.InteropServices;

namespace NAudio.CoreAudioApi
{
	public class AudioCaptureClient : IDisposable
	{
		private IAudioCaptureClient audioCaptureClientInterface;

		internal AudioCaptureClient(IAudioCaptureClient audioCaptureClientInterface)
		{
			this.audioCaptureClientInterface = audioCaptureClientInterface;
		}

		public IntPtr GetBuffer(out int numFramesToRead, out AudioClientBufferFlags bufferFlags, out long devicePosition, out long qpcPosition)
		{
			Marshal.ThrowExceptionForHR(audioCaptureClientInterface.GetBuffer(out IntPtr dataBuffer, out numFramesToRead, out bufferFlags, out devicePosition, out qpcPosition));
			return dataBuffer;
		}

		public IntPtr GetBuffer(out int numFramesToRead, out AudioClientBufferFlags bufferFlags)
		{
			Marshal.ThrowExceptionForHR(audioCaptureClientInterface.GetBuffer(out IntPtr dataBuffer, out numFramesToRead, out bufferFlags, out long _, out long _));
			return dataBuffer;
		}

		public int GetNextPacketSize()
		{
			Marshal.ThrowExceptionForHR(audioCaptureClientInterface.GetNextPacketSize(out int numFramesInNextPacket));
			return numFramesInNextPacket;
		}

		public void ReleaseBuffer(int numFramesWritten)
		{
			Marshal.ThrowExceptionForHR(audioCaptureClientInterface.ReleaseBuffer(numFramesWritten));
		}

		public void Dispose()
		{
			if (audioCaptureClientInterface != null)
			{
				Marshal.ReleaseComObject(audioCaptureClientInterface);
				audioCaptureClientInterface = null;
				GC.SuppressFinalize(this);
			}
		}
	}
}
