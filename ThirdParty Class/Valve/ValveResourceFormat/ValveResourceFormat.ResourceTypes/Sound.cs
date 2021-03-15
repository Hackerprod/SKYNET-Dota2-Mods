using System;
using System.IO;
using ValveResourceFormat.Blocks;
using ValveResourceFormat.ResourceTypes.NTROSerialization;

namespace ValveResourceFormat.ResourceTypes
{
	public class Sound : NTRO
	{
		public enum AudioFileType
		{
			Unknown0,
			WAV,
			MP3,
			Unknown3,
            AAC,
        }

		public AudioFileType Type
		{
			get;
			private set;
		}

		public uint SampleRate
		{
			get;
			private set;
		}

		public uint Bits
		{
			get;
			private set;
		}

		public uint Channels
		{
			get;
			private set;
		}

		public uint AudioFormat
		{
			get;
			private set;
		}

		public uint SampleSize
		{
			get;
			private set;
		}

		public int LoopStart
		{
			get;
			private set;
		}

		public float Duration
		{
			get;
			private set;
		}

		public override void Read(BinaryReader reader, Resource resource)
		{
            if (resource.IntrospectionManifest == null)
            {
                ResourceIntrospectionManifest.ResourceDiskStruct resourceDiskStruct = new ResourceIntrospectionManifest.ResourceDiskStruct();
                ResourceIntrospectionManifest.ResourceDiskStruct.Field item = new ResourceIntrospectionManifest.ResourceDiskStruct.Field
                {
                    FieldName = "m_bitpackedsoundinfo",
                    Type = DataType.UInt32
                };
                resourceDiskStruct.FieldIntrospection.Add(item);
                item = new ResourceIntrospectionManifest.ResourceDiskStruct.Field
                {
                    FieldName = "m_loopStart",
                    Type = DataType.Int32,
                    OnDiskOffset = 4
                };
                resourceDiskStruct.FieldIntrospection.Add(item);
                item = new ResourceIntrospectionManifest.ResourceDiskStruct.Field
                {
                    FieldName = "m_flDuration",
                    Type = DataType.Float,
                    OnDiskOffset = 12
                };
                resourceDiskStruct.FieldIntrospection.Add(item);
                resource.Blocks[BlockType.NTRO] = new ResourceIntrospectionManifest();
                resource.IntrospectionManifest.ReferencedStructs.Add(resourceDiskStruct);
            }
            reader.BaseStream.Position = base.Offset;
            base.Read(reader, resource);
            LoopStart = ((NTROValue<int>)base.Output["m_loopStart"]).Value;
            Duration = ((NTROValue<float>)base.Output["m_flDuration"]).Value;
            uint value = ((NTROValue<uint>)base.Output["m_bitpackedsoundinfo"]).Value;
            if (ExtractSub(value, 27, 5) == 0)
            {
                SampleRate = ExtractSub(value, 0, 16);
                Type = GetTypeFromNewFormat(ExtractSub(value, 16, 2));
                Bits = ExtractSub(value, 20, 7);
                SampleSize = Bits / 8u;
                Channels = 1u;
                AudioFormat = 1u;
            }
            else
            {
                Type = (AudioFileType)ExtractSub(value, 0, 2);
                Bits = ExtractSub(value, 2, 5);
                Channels = ExtractSub(value, 7, 2);
                SampleSize = ExtractSub(value, 9, 3);
                AudioFormat = ExtractSub(value, 12, 2);
                SampleRate = ExtractSub(value, 14, 17);
            }
            if (Type > AudioFileType.MP3)
            {
                throw new NotImplementedException($"Unknown audio file format '{Type}', please report this on GitHub.");
            }
        }
        public static AudioFileType GetTypeFromNewFormat(uint type)
        {
            switch (type)
            {
                case 0u:
                    return AudioFileType.WAV;
                case 2u:
                    return AudioFileType.MP3;
                default:
                    return AudioFileType.Unknown3;
            }
        }
        public static uint ExtractSub(uint l, byte offset, byte nrBits)
		{
			uint num = l >> (int)offset;
			checked
			{
				int num2 = (1 << unchecked((int)nrBits)) - 1;
				return (uint)(num & num2);
			}
		}

		public byte[] GetSound()
		{
			using (MemoryStream memoryStream = GetSoundStream())
			{
				return memoryStream.ToArray();
			}
		}

		public MemoryStream GetSoundStream()
		{
			checked
			{
				base.Reader.BaseStream.Position = base.Offset + base.Size;
				uint num = (uint)(base.Reader.BaseStream.Length - base.Reader.BaseStream.Position);
				MemoryStream memoryStream = new MemoryStream();
				if (Type == AudioFileType.WAV)
				{
					byte[] array = new byte[4]
					{
						82,
						73,
						70,
						70
					};
					byte[] array2 = new byte[4]
					{
						87,
						65,
						86,
						69
					};
					byte[] array3 = new byte[4]
					{
						102,
						109,
						116,
						32
					};
					byte[] array4 = new byte[4]
					{
						100,
						97,
						116,
						97
					};
					uint source = SampleRate * Channels * unchecked(Bits / 8u);
					uint source2 = Channels * unchecked(Bits / 8u);
					memoryStream.Write(array, 0, array.Length);
					memoryStream.Write(PackageInt(num + 42u, 4), 0, 4);
					memoryStream.Write(array2, 0, array2.Length);
					memoryStream.Write(array3, 0, array3.Length);
					memoryStream.Write(PackageInt(16u, 4), 0, 4);
					memoryStream.Write(PackageInt(AudioFormat, 2), 0, 2);
					memoryStream.Write(PackageInt(Channels, 2), 0, 2);
					memoryStream.Write(PackageInt(SampleRate, 4), 0, 4);
					memoryStream.Write(PackageInt(source, 4), 0, 4);
					memoryStream.Write(PackageInt(source2, 2), 0, 2);
					memoryStream.Write(PackageInt(Bits, 2), 0, 2);
					memoryStream.Write(array4, 0, array4.Length);
					memoryStream.Write(PackageInt(num, 4), 0, 4);
				}
				base.Reader.BaseStream.CopyTo(memoryStream, (int)num);
				memoryStream.Flush();
				memoryStream.Seek(0L, SeekOrigin.Begin);
				return memoryStream;
			}
		}

		private static byte[] PackageInt(uint source, int length)
		{
			byte[] array = new byte[length];
			checked
			{
				array[0] = (byte)(source & 0xFF);
				array[1] = (byte)((source >> 8) & 0xFF);
				if (length == 4)
				{
					array[2] = (byte)((source >> 16) & 0xFF);
					array[3] = (byte)((source >> 24) & 0xFF);
				}
				return array;
			}
		}

		public override string ToString()
		{
			string arg = base.ToString();
			arg = arg + "\nSample Rate: " + SampleRate;
			arg = arg + "\nBits: " + Bits;
			arg = arg + "\nType: " + Type;
			arg = arg + "\nSampleSize: " + SampleSize;
			arg = arg + "\nFormat: " + AudioFormat;
			return arg + "\nChannels: " + Channels;
		}
	}
}
