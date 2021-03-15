using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NAudio.MediaFoundation
{
	public static class MediaFoundationApi
	{
		private static bool initialized;

		public static void Startup()
		{
			if (!initialized)
			{
				int num = 2;
				OperatingSystem oSVersion = Environment.OSVersion;
				if (oSVersion.Version.Major == 6 && oSVersion.Version.Minor == 0)
				{
					num = 1;
				}
				MediaFoundationInterop.MFStartup((num << 16) | 0x70);
				initialized = true;
			}
		}

		public static IEnumerable<IMFActivate> EnumerateTransforms(Guid category)
		{
			MediaFoundationInterop.MFTEnumEx(category, _MFT_ENUM_FLAG.MFT_ENUM_FLAG_ALL, null, null, out IntPtr interfacesPointer, out int interfaceCount);
			IMFActivate[] interfaces = new IMFActivate[interfaceCount];
			for (int j = 0; j < interfaceCount; j++)
			{
				IntPtr pUnk = Marshal.ReadIntPtr(new IntPtr(interfacesPointer.ToInt64() + j * Marshal.SizeOf(interfacesPointer)));
				interfaces[j] = (IMFActivate)Marshal.GetObjectForIUnknown(pUnk);
			}
			try
			{
				IMFActivate[] array = interfaces;
				foreach (IMFActivate i in array)
				{
					yield return i;
				}
			}
			finally
			{
			}
			Marshal.FreeCoTaskMem(interfacesPointer);
		}

		public static void Shutdown()
		{
			if (initialized)
			{
				MediaFoundationInterop.MFShutdown();
				initialized = false;
			}
		}

		public static IMFMediaType CreateMediaType()
		{
			MediaFoundationInterop.MFCreateMediaType(out IMFMediaType ppMFType);
			return ppMFType;
		}

		public static IMFMediaType CreateMediaTypeFromWaveFormat(WaveFormat waveFormat)
		{
			IMFMediaType iMFMediaType = CreateMediaType();
			try
			{
				MediaFoundationInterop.MFInitMediaTypeFromWaveFormatEx(iMFMediaType, waveFormat, Marshal.SizeOf(waveFormat));
				return iMFMediaType;
			}
			catch (Exception)
			{
				Marshal.ReleaseComObject(iMFMediaType);
				throw;
			}
		}

		public static IMFMediaBuffer CreateMemoryBuffer(int bufferSize)
		{
			MediaFoundationInterop.MFCreateMemoryBuffer(bufferSize, out IMFMediaBuffer ppBuffer);
			return ppBuffer;
		}

		public static IMFSample CreateSample()
		{
			MediaFoundationInterop.MFCreateSample(out IMFSample ppIMFSample);
			return ppIMFSample;
		}

		public static IMFAttributes CreateAttributes(int initialSize)
		{
			MediaFoundationInterop.MFCreateAttributes(out IMFAttributes ppMFAttributes, initialSize);
			return ppMFAttributes;
		}

		public static IMFByteStream CreateByteStream(object stream)
		{
			MediaFoundationInterop.MFCreateMFByteStreamOnStreamEx(stream, out IMFByteStream ppByteStream);
			return ppByteStream;
		}

		public static IMFSourceReader CreateSourceReaderFromByteStream(IMFByteStream byteStream)
		{
			MediaFoundationInterop.MFCreateSourceReaderFromByteStream(byteStream, null, out IMFSourceReader ppSourceReader);
			return ppSourceReader;
		}
	}
}
