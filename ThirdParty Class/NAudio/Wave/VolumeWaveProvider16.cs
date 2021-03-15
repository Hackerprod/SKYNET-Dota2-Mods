using System;

namespace NAudio.Wave
{
	public class VolumeWaveProvider16 : IWaveProvider
	{
		private readonly IWaveProvider sourceProvider;

		private float volume;

		public float Volume
		{
			get
			{
				return volume;
			}
			set
			{
				volume = value;
			}
		}

		public WaveFormat WaveFormat => sourceProvider.WaveFormat;

		public VolumeWaveProvider16(IWaveProvider sourceProvider)
		{
			Volume = 1f;
			this.sourceProvider = sourceProvider;
			if (sourceProvider.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
			{
				throw new ArgumentException("Expecting PCM input");
			}
			if (sourceProvider.WaveFormat.BitsPerSample != 16)
			{
				throw new ArgumentException("Expecting 16 bit");
			}
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			int num = sourceProvider.Read(buffer, offset, count);
			if (volume == 0f)
			{
				for (int i = 0; i < num; i++)
				{
					buffer[offset++] = 0;
				}
			}
			else if (volume != 1f)
			{
				for (int j = 0; j < num; j += 2)
				{
					short num3 = (short)((buffer[offset + 1] << 8) | buffer[offset]);
					float num4 = (float)num3 * volume;
					num3 = (short)num4;
					if (Volume > 1f)
					{
						if (num4 > 32767f)
						{
							num3 = short.MaxValue;
						}
						else if (num4 < -32768f)
						{
							num3 = short.MinValue;
						}
					}
					buffer[offset++] = (byte)(num3 & 0xFF);
					buffer[offset++] = (byte)(num3 >> 8);
				}
			}
			return num;
		}
	}
}
