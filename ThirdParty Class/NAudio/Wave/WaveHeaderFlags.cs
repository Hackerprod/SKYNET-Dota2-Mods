using System;

namespace NAudio.Wave
{
	[Flags]
	public enum WaveHeaderFlags
	{
		BeginLoop = 0x4,
		Done = 0x1,
		EndLoop = 0x8,
		InQueue = 0x10,
		Prepared = 0x2
	}
}
