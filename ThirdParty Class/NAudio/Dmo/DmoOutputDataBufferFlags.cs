using System;

namespace NAudio.Dmo
{
	[Flags]
	public enum DmoOutputDataBufferFlags
	{
		None = 0x0,
		SyncPoint = 0x1,
		Time = 0x2,
		TimeLength = 0x4,
		Incomplete = 0x1000000
	}
}
