using System;

namespace ValveResourceFormat
{
	[Flags]
	public enum VTexFlags
	{
		SUGGEST_CLAMPS = 0x1,
		SUGGEST_CLAMPT = 0x2,
		SUGGEST_CLAMPU = 0x4,
		NO_LOD = 0x8,
		CUBE_TEXTURE = 0x10,
		VOLUME_TEXTURE = 0x20,
		TEXTURE_ARRAY = 0x40
	}
}
