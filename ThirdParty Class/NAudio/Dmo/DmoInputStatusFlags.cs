using System;

namespace NAudio.Dmo
{
	[Flags]
	internal enum DmoInputStatusFlags
	{
		None = 0x0,
		DMO_INPUT_STATUSF_ACCEPT_DATA = 0x1
	}
}
