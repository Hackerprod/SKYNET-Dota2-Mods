using System;

namespace NAudio.Wave.Compression
{
	[Flags]
	internal enum AcmFormatChooseStyleFlags
	{
		None = 0x0,
		ShowHelp = 0x4,
		EnableHook = 0x8,
		EnableTemplate = 0x10,
		EnableTemplateHandle = 0x20,
		InitToWfxStruct = 0x40,
		ContextHelp = 0x80
	}
}
