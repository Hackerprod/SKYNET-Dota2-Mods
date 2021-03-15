using System;

namespace NAudio.Wave.Compression
{
	[Flags]
	public enum AcmDriverDetailsSupportFlags
	{
		Codec = 0x1,
		Converter = 0x2,
		Filter = 0x4,
		Hardware = 0x8,
		Async = 0x10,
		Local = 0x40000000,
		Disabled = int.MinValue
	}
}
