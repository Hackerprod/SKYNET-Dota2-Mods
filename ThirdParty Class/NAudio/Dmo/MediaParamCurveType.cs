using System;

namespace NAudio.Dmo
{
	[Flags]
	internal enum MediaParamCurveType
	{
		MP_CURVE_JUMP = 0x1,
		MP_CURVE_LINEAR = 0x2,
		MP_CURVE_SQUARE = 0x4,
		MP_CURVE_INVSQUARE = 0x8,
		MP_CURVE_SINE = 0x10
	}
}
