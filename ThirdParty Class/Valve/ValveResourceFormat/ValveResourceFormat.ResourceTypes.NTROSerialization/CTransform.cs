namespace ValveResourceFormat.ResourceTypes.NTROSerialization
{
	public class CTransform
	{
		public float[] Values
		{
			get;
		}

		public CTransform(float field0, float field1, float field2, float field3, float field4, float field5, float field6, float field7)
		{
			Values = new float[8]
			{
				field0,
				field1,
				field2,
				field3,
				field4,
				field5,
				field6,
				field7
			};
		}

		public override string ToString()
		{
			return string.Format("q={{{0:F}, {1:F}, {2:F}; w={3}}} p={{{4:F}, {5:F}, {6}}}", Values[4], Values[5], Values[6], Values[7].ToString("F"), Values[0], Values[1], Values[2].ToString("F"));
		}
	}
}
