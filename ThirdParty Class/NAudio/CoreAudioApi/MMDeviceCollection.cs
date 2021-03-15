using NAudio.CoreAudioApi.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NAudio.CoreAudioApi
{
	public class MMDeviceCollection : IEnumerable<MMDevice>, IEnumerable
	{
		private IMMDeviceCollection _MMDeviceCollection;

		public int Count
		{
			get
			{
				Marshal.ThrowExceptionForHR(_MMDeviceCollection.GetCount(out int numDevices));
				return numDevices;
			}
		}

		public MMDevice this[int index]
		{
			get
			{
				_MMDeviceCollection.Item(index, out IMMDevice device);
				return new MMDevice(device);
			}
		}

		internal MMDeviceCollection(IMMDeviceCollection parent)
		{
			_MMDeviceCollection = parent;
		}

		public IEnumerator<MMDevice> GetEnumerator()
		{
			for (int index = 0; index < Count; index++)
			{
				yield return this[index];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
