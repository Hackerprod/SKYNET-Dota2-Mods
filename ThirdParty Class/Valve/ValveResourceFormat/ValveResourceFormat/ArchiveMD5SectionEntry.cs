namespace ValveResourceFormat
{
	public class ArchiveMD5SectionEntry
	{
		public uint ArchiveIndex
		{
			get;
			set;
		}

		public uint Offset
		{
			get;
			set;
		}

		public uint Length
		{
			get;
			set;
		}

		public byte[] Checksum
		{
			get;
			set;
		}
	}
}
