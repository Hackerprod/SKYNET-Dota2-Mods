using System;

namespace NAudio.Dmo
{
	[Flags]
	public enum DmoInputDataBufferFlags
	{
		None = 0x0,
		SyncPoint = 0x1,
		Time = 0x2,
		TimeLength = 0x4
	}
}
