namespace ValveResourceFormat.ResourceTypes.NTROSerialization
{
	public class Vector3
	{
		public float X
		{
			get;
		}

		public float Y
		{
			get;
		}

		public float Z
		{
			get;
		}

		public Vector3(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public override string ToString()
		{
			return $"({X:F6}, {Y:F6}, {Z:F6})";
		}
	}
}
