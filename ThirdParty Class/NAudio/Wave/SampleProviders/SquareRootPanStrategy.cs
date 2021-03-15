using System;

namespace NAudio.Wave.SampleProviders
{
	public class SquareRootPanStrategy : IPanStrategy
	{
		public StereoSamplePair GetMultipliers(float pan)
		{
			float num = (0f - pan + 1f) / 2f;
			float left = (float)Math.Sqrt((double)num);
			float right = (float)Math.Sqrt((double)(1f - num));
			StereoSamplePair result = default(StereoSamplePair);
			result.Left = left;
			result.Right = right;
			return result;
		}
	}
}
