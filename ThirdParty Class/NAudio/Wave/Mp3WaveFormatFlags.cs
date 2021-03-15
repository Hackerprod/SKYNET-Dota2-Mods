using System;

namespace NAudio.Wave
{
	[Flags]
	public enum Mp3WaveFormatFlags
	{
		PaddingIso = 0x0,
		PaddingOn = 0x1,
		PaddingOff = 0x2
	}
}
