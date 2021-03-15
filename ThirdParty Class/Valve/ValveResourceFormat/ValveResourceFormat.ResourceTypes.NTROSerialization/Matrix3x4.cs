using System.CodeDom.Compiler;
using System.IO;

namespace ValveResourceFormat.ResourceTypes.NTROSerialization
{
	public class Matrix3x4
	{
		public float[] Values
		{
			get;
		}

		public Matrix3x4(float field0, float field1, float field2, float field3, float field4, float field5, float field6, float field7, float field8, float field9, float field10, float field11)
		{
			Values = new float[12]
			{
				field0,
				field1,
				field2,
				field3,
				field4,
				field5,
				field6,
				field7,
				field8,
				field9,
				field10,
				field11
			};
		}

		public override string ToString()
		{
			using (StringWriter stringWriter = new StringWriter())
			{
				using (IndentedTextWriter writer = new IndentedTextWriter(stringWriter, "\t"))
				{
					WriteText(writer);
					return stringWriter.ToString();
				}
			}
		}

		public void WriteText(IndentedTextWriter writer)
		{
			writer.WriteLine();
			writer.WriteLine("{0:F4} {1:F4} {2:F4} {3:F4}", Values[0], Values[1], Values[2], Values[3]);
			writer.WriteLine("{0:F4} {1:F4} {2:F4} {3:F4}", Values[4], Values[5], Values[6], Values[7]);
			writer.WriteLine("{0:F4} {1:F4} {2:F4} {3:F4}", Values[8], Values[9], Values[10], Values[11]);
		}
	}
}
