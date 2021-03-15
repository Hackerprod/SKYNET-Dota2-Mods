using System;

namespace NAudio.Wave.SampleProviders
{
	internal static class SampleProviderConverters
	{
		public static ISampleProvider ConvertWaveProviderIntoSampleProvider(IWaveProvider waveProvider)
		{
			if (waveProvider.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
			{
				if (waveProvider.WaveFormat.BitsPerSample == 8)
				{
					return new Pcm8BitToSampleProvider(waveProvider);
				}
				if (waveProvider.WaveFormat.BitsPerSample == 16)
				{
					return new Pcm16BitToSampleProvider(waveProvider);
				}
				if (waveProvider.WaveFormat.BitsPerSample == 24)
				{
					return new Pcm24BitToSampleProvider(waveProvider);
				}
				if (waveProvider.WaveFormat.BitsPerSample != 32)
				{
					throw new InvalidOperationException("Unsupported bit depth");
				}
				return new Pcm32BitToSampleProvider(waveProvider);
			}
			if (waveProvider.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
			{
				throw new ArgumentException("Unsupported source encoding");
			}
			if (waveProvider.WaveFormat.BitsPerSample == 64)
			{
				return new WaveToSampleProvider64(waveProvider);
			}
			return new WaveToSampleProvider(waveProvider);
		}
	}
}
