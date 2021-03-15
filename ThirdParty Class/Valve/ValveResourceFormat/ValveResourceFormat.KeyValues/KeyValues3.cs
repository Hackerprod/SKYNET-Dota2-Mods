using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ValveResourceFormat.KeyValues
{
	public static class KeyValues3
	{
		private enum State
		{
			HEADER,
			SEEK_VALUE,
			PROP_NAME,
			VALUE_STRUCT,
			VALUE_ARRAY,
			VALUE_STRING,
			VALUE_STRING_MULTI,
			VALUE_NUMBER,
			VALUE_FLAGGED,
			COMMENT,
			COMMENT_BLOCK
		}

		private class Parser
		{
			public StreamReader FileStream;

			public KVObject Root;

			public string CurrentName;

			public StringBuilder CurrentString;

			public char PreviousChar;

			public Queue<char> CharBuffer;

			public Stack<KVObject> ObjStack;

			public Stack<State> StateStack;

			public Parser()
			{
				ObjStack = new Stack<KVObject>();
				StateStack = new Stack<State>();
				StateStack.Push(State.HEADER);
				Root = new KVObject("root");
				ObjStack.Push(Root);
				PreviousChar = '\0';
				CharBuffer = new Queue<char>();
				CurrentString = new StringBuilder();
			}
		}

		public static KV3File ParseKVFile(string filename)
		{
			using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
			{
				return ParseKVFile(fileStream);
			}
		}

		public static KV3File ParseKVFile(Stream fileStream)
		{
			Parser parser = new Parser();
			parser.FileStream = new StreamReader(fileStream);
			while (!parser.FileStream.EndOfStream)
			{
				char c = NextChar(parser);
				switch (parser.StateStack.Peek())
				{
				case State.HEADER:
					ReadHeader(c, parser);
					break;
				case State.PROP_NAME:
					ReadPropName(c, parser);
					break;
				case State.SEEK_VALUE:
					SeekValue(c, parser);
					break;
				case State.VALUE_STRUCT:
					ReadValueStruct(c, parser);
					break;
				case State.VALUE_STRING:
					ReadValueString(c, parser);
					break;
				case State.VALUE_STRING_MULTI:
					ReadValueStringMulti(c, parser);
					break;
				case State.VALUE_NUMBER:
					ReadValueNumber(c, parser);
					break;
				case State.VALUE_ARRAY:
					ReadValueArray(c, parser);
					break;
				case State.VALUE_FLAGGED:
					ReadValueFlagged(c, parser);
					break;
				case State.COMMENT:
					ReadComment(c, parser);
					break;
				case State.COMMENT_BLOCK:
					ReadCommentBlock(c, parser);
					break;
				}
				parser.PreviousChar = c;
			}
			return new KV3File((KVObject)parser.Root.Properties.ElementAt(0).Value.Value);
		}

		private static void ReadHeader(char c, Parser parser)
		{
			parser.CurrentString.Append(c);
			if (c == '>' && parser.CurrentString.ToString().Substring(checked(parser.CurrentString.Length - 3)) == "-->")
			{
				parser.StateStack.Pop();
				parser.StateStack.Push(State.SEEK_VALUE);
			}
		}

		private static void SeekValue(char c, Parser parser)
		{
			checked
			{
				if (!char.IsWhiteSpace(c))
				{
					switch (c)
					{
					case '=':
						break;
					case '{':
						parser.StateStack.Pop();
						parser.StateStack.Push(State.VALUE_STRUCT);
						parser.ObjStack.Push(new KVObject(parser.CurrentString.ToString()));
						break;
					case '[':
						parser.StateStack.Pop();
						parser.StateStack.Push(State.VALUE_ARRAY);
						parser.StateStack.Push(State.SEEK_VALUE);
						parser.ObjStack.Push(new KVObject(parser.CurrentString.ToString(), isArray: true));
						break;
					case ']':
					{
						parser.StateStack.Pop();
						parser.StateStack.Pop();
						KVObject kVObject = parser.ObjStack.Pop();
						parser.ObjStack.Peek().AddProperty(kVObject.Key, new KVValue(KVType.ARRAY, kVObject));
						break;
					}
					case '"':
					{
						string text = PeekString(parser, 4);
						if (text.Contains("\"\"\n") || text == "\"\"\r\n")
						{
							SkipChars(parser, 2);
							parser.StateStack.Pop();
							parser.StateStack.Push(State.VALUE_STRING_MULTI);
							parser.CurrentString.Clear();
						}
						else
						{
							parser.StateStack.Pop();
							parser.StateStack.Push(State.VALUE_STRING);
							parser.CurrentString.Clear();
						}
						break;
					}
					default:
						if (ReadAheadMatches(parser, c, "false"))
						{
							parser.StateStack.Pop();
							parser.ObjStack.Peek().AddProperty(parser.CurrentName, new KVValue(KVType.BOOLEAN, false));
							SkipChars(parser, "false".Length - 1);
						}
						else if (ReadAheadMatches(parser, c, "true"))
						{
							parser.StateStack.Pop();
							parser.ObjStack.Peek().AddProperty(parser.CurrentName, new KVValue(KVType.BOOLEAN, true));
							SkipChars(parser, "true".Length - 1);
						}
						else if (ReadAheadMatches(parser, c, "null"))
						{
							parser.StateStack.Pop();
							parser.ObjStack.Peek().AddProperty(parser.CurrentName, new KVValue(KVType.NULL, null));
							SkipChars(parser, "null".Length - 1);
						}
						else if (char.IsDigit(c))
						{
							parser.StateStack.Pop();
							parser.StateStack.Push(State.VALUE_NUMBER);
							parser.CurrentString.Clear();
							parser.CurrentString.Append(c);
						}
						else
						{
							parser.StateStack.Pop();
							parser.StateStack.Push(State.VALUE_FLAGGED);
							parser.CurrentString.Clear();
							parser.CurrentString.Append(c);
						}
						break;
					}
				}
			}
		}

		private static void ReadPropName(char c, Parser parser)
		{
			if (char.IsWhiteSpace(c))
			{
				parser.StateStack.Pop();
				parser.StateStack.Push(State.SEEK_VALUE);
				parser.CurrentName = parser.CurrentString.ToString();
			}
			else
			{
				parser.CurrentString.Append(c);
			}
		}

		private static void ReadValueStruct(char c, Parser parser)
		{
			if (!char.IsWhiteSpace(c))
			{
				switch (c)
				{
				case '/':
					parser.StateStack.Push(State.COMMENT);
					parser.CurrentString.Clear();
					parser.CurrentString.Append(c);
					break;
				case '}':
				{
					KVObject kVObject = parser.ObjStack.Pop();
					parser.ObjStack.Peek().AddProperty(kVObject.Key, new KVValue(KVType.OBJECT, kVObject));
					parser.StateStack.Pop();
					break;
				}
				default:
					parser.StateStack.Push(State.PROP_NAME);
					parser.CurrentString.Clear();
					parser.CurrentString.Append(c);
					break;
				}
			}
		}

		private static void ReadValueString(char c, Parser parser)
		{
			if (c == '"' && parser.PreviousChar != '\\')
			{
				parser.StateStack.Pop();
				parser.ObjStack.Peek().AddProperty(parser.CurrentName, new KVValue(KVType.STRING, parser.CurrentString.ToString()));
			}
			else
			{
				parser.CurrentString.Append(c);
			}
		}

		private static void ReadValueStringMulti(char c, Parser parser)
		{
			string a = PeekString(parser, 2);
			checked
			{
				if (c == '"' && a == "\"\"" && parser.PreviousChar != '\\')
				{
					string text = parser.CurrentString.ToString();
					int num = 0;
					int num2 = text.Length;
					if (text.ElementAt(0) == '\n')
					{
						num = 1;
					}
					else if (text.ElementAt(0) == '\r' && text.ElementAt(1) == '\n')
					{
						num = 2;
					}
					if (text.ElementAt(text.Length - 1) == '\n')
					{
						num2 = ((text.ElementAt(text.Length - 2) != '\r') ? (text.Length - 1) : (text.Length - 2));
					}
					text = text.Substring(num, num2 - num);
					parser.StateStack.Pop();
					parser.ObjStack.Peek().AddProperty(parser.CurrentName, new KVValue(KVType.STRING_MULTI, text));
					SkipChars(parser, 2);
				}
				else
				{
					parser.CurrentString.Append(c);
				}
			}
		}

		private static void ReadValueNumber(char c, Parser parser)
		{
			if (c == ',')
			{
				parser.StateStack.Pop();
				parser.StateStack.Push(State.SEEK_VALUE);
				if (parser.CurrentString.ToString().Contains('.'))
				{
					parser.ObjStack.Peek().AddProperty(parser.CurrentName, new KVValue(KVType.DOUBLE, double.Parse(parser.CurrentString.ToString(), CultureInfo.InvariantCulture)));
				}
				else
				{
					parser.ObjStack.Peek().AddProperty(parser.CurrentName, new KVValue(KVType.INTEGER, long.Parse(parser.CurrentString.ToString(), CultureInfo.InvariantCulture)));
				}
			}
			else if (char.IsWhiteSpace(c))
			{
				parser.StateStack.Pop();
				if (parser.CurrentString.ToString().Contains('.'))
				{
					parser.ObjStack.Peek().AddProperty(parser.CurrentName, new KVValue(KVType.DOUBLE, double.Parse(parser.CurrentString.ToString(), CultureInfo.InvariantCulture)));
				}
				else
				{
					parser.ObjStack.Peek().AddProperty(parser.CurrentName, new KVValue(KVType.INTEGER, int.Parse(parser.CurrentString.ToString(), CultureInfo.InvariantCulture)));
				}
			}
			else
			{
				parser.CurrentString.Append(c);
			}
		}

		private static void ReadValueArray(char c, Parser parser)
		{
			if (!char.IsWhiteSpace(c) && c != ',')
			{
				throw new InvalidDataException("Error in array format.");
			}
			parser.StateStack.Push(State.SEEK_VALUE);
		}

		private static void ReadValueFlagged(char c, Parser parser)
		{
			if (char.IsWhiteSpace(c))
			{
				parser.StateStack.Pop();
				string[] array = parser.CurrentString.ToString().Split(new char[1]
				{
					':'
				}, 2);
				string a = array[0];
				KVFlag flag;
				if (!(a == "resource"))
				{
					if (!(a == "deferred_resource"))
					{
						throw new InvalidDataException("Unknown flag " + array[0]);
					}
					flag = KVFlag.DeferredResource;
				}
				else
				{
					flag = KVFlag.Resource;
				}
				parser.ObjStack.Peek().AddProperty(parser.CurrentName, new KVFlaggedValue(KVType.STRING, flag, array[1].Substring(1, checked(array[1].Length - 2))));
			}
			else
			{
				parser.CurrentString.Append(c);
			}
		}

		private static void ReadComment(char c, Parser parser)
		{
			if (parser.CurrentString.Length == 1 && c == '*')
			{
				parser.StateStack.Pop();
				parser.StateStack.Push(State.COMMENT_BLOCK);
			}
			switch (c)
			{
			case '\r':
				break;
			case '\n':
				parser.StateStack.Pop();
				break;
			default:
				parser.CurrentString.Append(c);
				break;
			}
		}

		private static void ReadCommentBlock(char c, Parser parser)
		{
			if (c == '/' && parser.CurrentString.ToString().Last() == '*')
			{
				parser.StateStack.Pop();
			}
			parser.CurrentString.Append(c);
		}

		private static char NextChar(Parser parser)
		{
			if (parser.CharBuffer.Count > 0)
			{
				return parser.CharBuffer.Dequeue();
			}
			return (char)checked((ushort)parser.FileStream.Read());
		}

		private static void SkipChars(Parser parser, int num)
		{
			for (int i = 0; i < num; i = checked(i + 1))
			{
				NextChar(parser);
			}
		}

		private static string PeekString(Parser parser, int length)
		{
			char[] array = new char[length];
			for (int i = 0; i < length; i = checked(i + 1))
			{
				if (i < parser.CharBuffer.Count)
				{
					array[i] = parser.CharBuffer.ElementAt(i);
				}
				else
				{
					array[i] = (char)checked((ushort)parser.FileStream.Read());
					parser.CharBuffer.Enqueue(array[i]);
				}
			}
			return string.Join(string.Empty, array);
		}

		private static bool ReadAheadMatches(Parser parser, char c, string pattern)
		{
			if (c.ToString() + PeekString(parser, checked(pattern.Length - 1)) == pattern)
			{
				return true;
			}
			return false;
		}
	}
}
