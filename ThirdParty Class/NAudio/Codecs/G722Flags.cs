using System;

namespace NAudio.Codecs
{
	[Flags]
	public enum G722Flags
	{
		None = 0x0,
		SampleRate8000 = 0x1,
		Packed = 0x2
	}
}
