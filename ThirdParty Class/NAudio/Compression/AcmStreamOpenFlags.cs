using System;

namespace NAudio.Wave.Compression
{
	[Flags]
	internal enum AcmStreamOpenFlags
	{
		Query = 0x1,
		Async = 0x2,
		NonRealTime = 0x4,
		CallbackTypeMask = 0x70000,
		CallbackNull = 0x0,
		CallbackWindow = 0x10000,
		CallbackTask = 0x20000,
		CallbackFunction = 0x30000,
		CallbackThread = 0x20000,
		CallbackEvent = 0x50000
	}
}
