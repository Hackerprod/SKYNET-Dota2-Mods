using System;

namespace NAudio.MediaFoundation
{
	[Flags]
	public enum _MFT_PROCESS_OUTPUT_FLAGS
	{
		None = 0x0,
		MFT_PROCESS_OUTPUT_DISCARD_WHEN_NO_BUFFER = 0x1,
		MFT_PROCESS_OUTPUT_REGENERATE_LAST_OUTPUT = 0x2
	}
}
