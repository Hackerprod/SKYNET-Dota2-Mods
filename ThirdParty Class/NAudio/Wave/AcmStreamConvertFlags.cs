using System;

namespace NAudio.Wave
{
	[Flags]
	internal enum AcmStreamConvertFlags
	{
		BlockAlign = 0x4,
		Start = 0x10,
		End = 0x20
	}
}
