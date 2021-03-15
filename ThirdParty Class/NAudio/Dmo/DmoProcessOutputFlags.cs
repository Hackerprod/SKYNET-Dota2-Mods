using System;

namespace NAudio.Dmo
{
	[Flags]
	public enum DmoProcessOutputFlags
	{
		None = 0x0,
		DiscardWhenNoBuffer = 0x1
	}
}
