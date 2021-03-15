using System;

namespace NAudio.Dmo
{
	[Flags]
	internal enum DmoSetTypeFlags
	{
		None = 0x0,
		DMO_SET_TYPEF_TEST_ONLY = 0x1,
		DMO_SET_TYPEF_CLEAR = 0x2
	}
}
