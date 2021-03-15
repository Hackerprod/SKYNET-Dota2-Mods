using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using ValveResourceFormat.ResourceTypes.NTROSerialization;

namespace ValveResourceFormat.ResourceTypes
{
	public class SoundEventScript : NTRO
	{
		public Dictionary<string, string> SoundEventScriptValue
		{
			get;
			private set;
		}

		public override void Read(BinaryReader reader, Resource resource)
		{
			base.Read(reader, resource);
			SoundEventScriptValue = new Dictionary<string, string>();
			NTROArray nTROArray = (NTROArray)base.Output["m_SoundEvents"];
			foreach (NTROValue item in nTROArray)
			{
				NTROStruct value = ((NTROValue<NTROStruct>)item).Value;
				string value2 = ((NTROValue<string>)value["m_SoundName"]).Value;
				string value3 = ((NTROValue<string>)value["m_OperatorsKV"]).Value.Replace("\n", Environment.NewLine);
				if (SoundEventScriptValue.ContainsKey(value2))
				{
					SoundEventScriptValue.Remove(value2);
				}
				SoundEventScriptValue.Add(value2, value3);
			}
		}

		public override string ToString()
		{
			checked
			{
				using (StringWriter stringWriter = new StringWriter())
				{
					using (IndentedTextWriter indentedTextWriter = new IndentedTextWriter(stringWriter, "\t"))
					{
						foreach (KeyValuePair<string, string> item in SoundEventScriptValue)
						{
							indentedTextWriter.WriteLine("\"" + item.Key + "\"");
							indentedTextWriter.WriteLine("{");
							indentedTextWriter.Indent++;
							indentedTextWriter.Write(item.Value.Replace(Environment.NewLine, Environment.NewLine + "\t").TrimEnd('\t'));
							indentedTextWriter.Indent--;
							indentedTextWriter.WriteLine("}");
							indentedTextWriter.WriteLine(string.Empty);
						}
						return stringWriter.ToString();
					}
				}
			}
		}
	}
}
