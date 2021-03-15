using System;

namespace NAudio.Dmo
{
	[Flags]
	internal enum InputStreamInfoFlags
	{
		None = 0x0,
		DMO_INPUT_STREAMF_WHOLE_SAMPLES = 0x1,
		DMO_INPUT_STREAMF_SINGLE_SAMPLE_PER_BUFFER = 0x2,
		DMO_INPUT_STREAMF_FIXED_SAMPLE_SIZE = 0x4,
		DMO_INPUT_STREAMF_HOLDS_BUFFERS = 0x8
	}
}
