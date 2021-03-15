using System;

namespace NAudio.CoreAudioApi
{
	[Flags]
	public enum EEndpointHardwareSupport
	{
		Volume = 0x1,
		Mute = 0x2,
		Meter = 0x4
	}
}
