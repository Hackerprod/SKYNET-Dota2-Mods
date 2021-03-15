using System;

namespace NAudio.Wave.SampleProviders
{
	public class SinPanStrategy : IPanStrategy
	{
		private const float HalfPi = 1.57079637f;

		public StereoSamplePair GetMultipliers(float pan)
		{
			float num = (0f - pan + 1f) / 2f;
			float left = (float)Math.Sin((double)(num * 1.57079637f));
			float right = (float)Math.Cos((double)(num * 1.57079637f));
			StereoSamplePair result = default(StereoSamplePair);
			result.Left = left;
			result.Right = right;
			return result;
		}
	}
}
