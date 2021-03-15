using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ValveResourceFormat.ResourceTypes.NTROSerialization;

namespace ValveResourceFormat.ResourceTypes
{
	public class EntityLump : NTRO
	{
		public List<List<Tuple<uint, uint, object>>> Datas
		{
			get;
			private set;
		}

		public override void Read(BinaryReader reader, Resource resource)
		{
			base.Read(reader, resource);
			NTROArray nTROArray = (NTROArray)base.Output["m_entityKeyValues"];
			Datas = new List<List<Tuple<uint, uint, object>>>();
			foreach (NTROValue item2 in nTROArray)
			{
				NTROStruct value = ((NTROValue<NTROStruct>)item2).Value;
				NTROArray nTROArray2 = (NTROArray)value["m_keyValuesData"];
				List<byte> list = new List<byte>();
				foreach (NTROValue<byte> item3 in nTROArray2)
				{
					list.Add(item3.Value);
				}
				using (MemoryStream memoryStream = new MemoryStream(list.ToArray()))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						uint num = binaryReader.ReadUInt32();
						uint num2 = binaryReader.ReadUInt32();
						uint num3 = binaryReader.ReadUInt32();
						List<Tuple<uint, uint, object>> list2 = new List<Tuple<uint, uint, object>>();
						while (memoryStream.Position != memoryStream.Length)
						{
							uint item = binaryReader.ReadUInt32();
							uint num4 = binaryReader.ReadUInt32();
							switch (num4)
							{
							case 6u:
								list2.Add(new Tuple<uint, uint, object>(num4, item, binaryReader.ReadByte()));
								break;
							case 1u:
								list2.Add(new Tuple<uint, uint, object>(num4, item, binaryReader.ReadSingle()));
								break;
							case 5u:
							case 9u:
							case 37u:
								list2.Add(new Tuple<uint, uint, object>(num4, item, binaryReader.ReadBytes(4)));
								break;
							case 26u:
								list2.Add(new Tuple<uint, uint, object>(num4, item, binaryReader.ReadUInt64()));
								break;
							case 3u:
								list2.Add(new Tuple<uint, uint, object>(num4, item, $"{{{binaryReader.ReadSingle()}, {binaryReader.ReadSingle()}, {binaryReader.ReadSingle()}}}"));
								break;
							case 39u:
								list2.Add(new Tuple<uint, uint, object>(num4, item, binaryReader.ReadBytes(12)));
								break;
							case 30u:
								list2.Add(new Tuple<uint, uint, object>(num4, item, binaryReader.ReadNullTermString(Encoding.UTF8)));
								break;
							default:
								throw new NotImplementedException($"Unknown type {num4}");
							}
						}
						Datas.Add(list2);
					}
				}
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			checked
			{
				for (int i = 0; i < Datas.Count; i++)
				{
					stringBuilder.AppendLine($"===={i}====\r\n");
					List<Tuple<uint, uint, object>> list = Datas[i];
					for (int j = 0; j < list.Count; j++)
					{
						Tuple<uint, uint, object> tuple = list[j];
						object obj = tuple.Item3;
						if (obj.GetType() == typeof(byte[]))
						{
							byte[] source = obj as byte[];
							obj = string.Format("Array [{0}]", string.Join(", ", (from p in source
							select p.ToString()).ToArray()));
						}
						switch (tuple.Item2)
						{
						case 2433605045u:
							stringBuilder.AppendLine(string.Format("   {0,-20} | {1}\n", "Ambient Effect", obj));
							break;
						case 2777094460u:
							stringBuilder.AppendLine(string.Format("   {0,-20} | {1}\n", "Start Disabled", obj));
							break;
						case 3323665506u:
							stringBuilder.AppendLine(string.Format("   {0,-20} | {1}\n", "Class Name", obj));
							break;
						case 3827302934u:
							stringBuilder.AppendLine(string.Format("   {0,-20} | {1}\n", "Position", obj));
							break;
						case 3130579663u:
							stringBuilder.AppendLine(string.Format("   {0,-20} | {1}\n", "Angles", obj));
							break;
						case 432137260u:
							stringBuilder.AppendLine(string.Format("   {0,-20} | {1}\n", "Scale", obj));
							break;
						case 1226772763u:
							stringBuilder.AppendLine(string.Format("   {0,-20} | {1}\n", "Disable Shadows", obj));
							break;
						case 3368008710u:
							stringBuilder.AppendLine(string.Format("   {0,-20} | {1}\n", "World Model", obj));
							break;
						case 1677246174u:
							stringBuilder.AppendLine(string.Format("   {0,-20} | {1}\n", "FX Colour", obj));
							break;
						case 588463423u:
							stringBuilder.AppendLine(string.Format("   {0,-20} | {1}\n", "Colour", obj));
							break;
						case 1094168427u:
							stringBuilder.AppendLine(string.Format("   {0,-20} | {1}\n", "Name", obj));
							break;
						default:
							stringBuilder.AppendLine(string.Format("   {0,3}: {1} (type={2}, meta={3})\n", j, obj, tuple.Item1, tuple.Item2));
							break;
						}
					}
					stringBuilder.AppendLine($"----{i}----\r\n");
				}
				return stringBuilder.ToString();
			}
		}
	}
}
