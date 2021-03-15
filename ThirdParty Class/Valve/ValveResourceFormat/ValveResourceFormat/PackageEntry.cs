namespace ValveResourceFormat
{
	public class PackageEntry
	{
		public string FileName
		{
			get;
			set;
		}

		public string DirectoryName
		{
			get;
			set;
		}

		public string TypeName
		{
			get;
			set;
		}

		public uint CRC32
		{
			get;
			set;
		}

		public uint Length
		{
			get;
			set;
		}

		public uint Offset
		{
			get;
			set;
		}

		public ushort ArchiveIndex
		{
			get;
			set;
		}

		public uint TotalLength
		{
			get
			{
				uint num = Length;
				if (SmallData != null)
				{
					num = checked(num + (uint)SmallData.Length);
				}
				return num;
			}
		}

		public byte[] SmallData
		{
			get;
			set;
		}

		public string GetFileName()
		{
			string text = FileName;
			if (TypeName != string.Empty)
			{
				text = text + "." + TypeName;
			}
			return text;
		}

		public string GetFullPath()
		{
			if (DirectoryName == null)
			{
				return GetFileName();
			}
			return DirectoryName + "/" + GetFileName();
		}

		public override string ToString()
		{
			return $"{GetFullPath()} crc=0x{CRC32:x2} metadatasz={SmallData.Length} fnumber={ArchiveIndex} ofs=0x{Offset:x2} sz={Length}";
		}
	}
}
