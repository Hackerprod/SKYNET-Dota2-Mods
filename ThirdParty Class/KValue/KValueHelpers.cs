using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
public static class KValueHelpers
{
    private static readonly Dictionary<char, char> EscapeMap = new Dictionary<char, char>
    {
        {
            'n',
            '\n'
        },
        {
            't',
            '\t'
        },
        {
            'v',
            '\v'
        },
        {
            'b',
            '\b'
        },
        {
            'r',
            '\r'
        },
        {
            'f',
            '\f'
        },
        {
            'a',
            '\a'
        },
        {
            '\\',
            '\\'
        }
    };

    public static string AsString(this KValue k)
    {
        object value = k.Value;
        object obj;
        if (value == null)
        {
            obj = null;
        }
        else
        {
            obj = value.ToString();
            if (obj != null)
            {
                goto IL_001b;
            }
        }
        obj = string.Empty;
        goto IL_001b;
    IL_001b:
        return (string)obj;
    }

    public static int AsInteger(this KValue k, int defaultValue = 0)
    {
        if (int.TryParse(k.Value.ToString(), out int result))
        {
            return result;
        }
        return defaultValue;
    }

    public static long AsLong(this KValue k, long defaultValue = 0L)
    {
        if (long.TryParse(k.Value.ToString(), out long result))
        {
            return result;
        }
        return defaultValue;
    }

    public static uint AsUnsignedInt(this KValue k, uint defaultValue = 0u)
    {
        if (uint.TryParse(k.Value.ToString(), out uint result))
        {
            return result;
        }
        return defaultValue;
    }

    public static ulong AsUnsignedLong(this KValue k, ulong defaultValue = 0uL)
    {
        if (ulong.TryParse(k.Value.ToString(), out ulong result))
        {
            return result;
        }
        return defaultValue;
    }

    public static float AsFloat(this KValue k, float defaultValue = 0f)
    {
        if (float.TryParse(k.Value.ToString(), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out float result))
        {
            return result;
        }
        return defaultValue;
    }

    public static bool AsBoolean(this KValue k, bool defaultValue = false)
    {
        if (int.TryParse(k.Value.ToString(), out int result))
        {
            return result != 0;
        }
        return defaultValue;
    }

    public static Color AsColor(this KValue k, Color defaultValue = default(Color))
    {
        return (k.Value as Color?) ?? defaultValue;
    }

    public static T AsEnum<T>(this KValue k, T defaultValue = default(T)) where T : struct
    {
        if (Enum.TryParse(k.Value.ToString(), out T result))
        {
            return result;
        }
        return defaultValue;
    }

    public static bool ReadAsText(this KValue k, Stream input)
    {
        new KValueTextReader(k, input);
        return true;
    }

    public static bool ReadFileAsText(this KValue k, string filename)
    {
        using (FileStream input = new FileStream(filename, FileMode.Open))
        {
            return k.ReadAsText(input);
        }
    }

    public static void SaveToFile(this KValue k, string path, bool asBinary)
    {
        using (FileStream stream = File.Create(path))
        {
            k.SaveToStream(stream, asBinary);
        }
    }

    public static void SaveToStream(this KValue k, Stream stream, bool asBinary)
    {
        if (asBinary)
        {
            k.RecursiveSaveBinaryToStream(stream);
        }
        else
        {
            k.RecursiveSaveTextToFile(stream);
        }
    }

    public static string SerializeToText(this KValue k)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            k.SaveToStream(memoryStream, asBinary: false);
            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }
    }

    private static void RecursiveSaveBinaryToStream(this KValue k, Stream f)
    {
        k.RecursiveSaveBinaryToStreamCore(f);
        f.WriteByte(8);
    }

    private static void RecursiveSaveBinaryToStreamCore(this KValue k, Stream f)
    {
        if (k.Children.Any())
        {
            f.WriteByte(0);
            f.WriteNullTermString(k.Name, Encoding.UTF8);
            foreach (KValue child in k.Children)
            {
                child.RecursiveSaveBinaryToStreamCore(f);
            }
            f.WriteByte(8);
        }
        else if (k.Value == null)
        {
            f.WriteByte(0);
            f.WriteNullTermString(k.Name, Encoding.UTF8);
            f.WriteByte(8);
        }
        else
        {
            switch (k.KType)
            {
                case KValueType.Pointer:
                case KValueType.End:
                case (KValueType)9:
                    break;
                case KValueType.String:
                    f.WriteByte(1);
                    f.WriteNullTermString(k.Name, Encoding.UTF8);
                    f.WriteNullTermString(k.AsString() ?? string.Empty, Encoding.UTF8);
                    break;
                case KValueType.Int32:
                    f.WriteByte(2);
                    f.WriteNullTermString(k.Name, Encoding.UTF8);
                    f.WriteInt32L(k.AsInteger());
                    break;
                case KValueType.Float32:
                    f.WriteByte(3);
                    f.WriteNullTermString(k.Name, Encoding.UTF8);
                    f.WriteFloatL(k.AsFloat());
                    break;
                case KValueType.WideString:
                    f.WriteByte(5);
                    f.WriteNullTermString(k.Name, Encoding.UTF8);
                    f.WriteWideString(k.AsString() ?? string.Empty, Encoding.UTF8);
                    break;
                case KValueType.Color:
                    {
                        f.WriteByte(6);
                        f.WriteNullTermString(k.Name, Encoding.UTF8);
                        Color color = k.AsColor();
                        f.WriteByte(color.R);
                        f.WriteByte(color.G);
                        f.WriteByte(color.B);
                        break;
                    }
                case KValueType.UInt64:
                    f.WriteByte(7);
                    f.WriteNullTermString(k.Name, Encoding.UTF8);
                    f.WriteUInt64L(k.AsUnsignedLong(0uL));
                    break;
                case KValueType.Int64:
                    f.WriteByte(10);
                    f.WriteNullTermString(k.Name, Encoding.UTF8);
                    f.WriteInt64L(k.AsLong());
                    break;
            }
        }
    }

    private static void WriteIndents(Stream stream, int indentLevel)
    {
        WriteString(stream, new string('\t', indentLevel));
    }

    private static void WriteString(Stream stream, string str, bool quote = false)
    {
        byte[] bytes = Encoding.UTF8.GetBytes((quote ? "\"" : "") + str.Replace("\"", "\\\"") + (quote ? "\"" : ""));
        stream.Write(bytes, 0, bytes.Length);
    }

    private static string EscapeString(string value)
    {
        foreach (System.Collections.Generic.KeyValuePair<char, char> item in EscapeMap)
        {
            string oldValue = new string(item.Value, 1);
            string newValue = "\\" + item.Key.ToString();
            value = value.Replace(oldValue, newValue);
        }
        value = value.Replace("\\\\t", "\t");
        return value;
    }

    private static void RecursiveSaveTextToFile(this KValue k, Stream stream, int indentLevel = 0)
    {
        bool flag;
        if (flag = (k.Name != null))
        {
            WriteIndents(stream, indentLevel);
            WriteString(stream, EscapeString(k.Name), quote: true);
            WriteString(stream, "\n");
            WriteIndents(stream, indentLevel);
            WriteString(stream, "{\n");
        }
        else
        {
            indentLevel--;
        }
        if (k.Children != null)
        {
            foreach (KValue child in k.Children)
            {
                if (child != null)
                {
                    if (child.Value == null)
                    {
                        child.RecursiveSaveTextToFile(stream, indentLevel + 1);
                    }
                    else
                    {
                        WriteIndents(stream, indentLevel + 1);
                        WriteString(stream, EscapeString(child.Name), quote: true);
                        WriteString(stream, "\t\t");
                        switch (child.KType)
                        {
                            case KValueType.Color:
                                {
                                    Color color = (child.Value as Color?) ?? default(Color);
                                    WriteString(stream, "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2"), quote: true);
                                    break;
                                }
                            default:
                                WriteString(stream, child.AsString(), quote: true);
                                break;
                            case KValueType.String:
                            case KValueType.WideString:
                                WriteString(stream, EscapeString(child.AsString()), quote: true);
                                break;
                        }
                        WriteString(stream, "\n");
                    }
                }
            }
        }
        if (flag)
        {
            WriteIndents(stream, indentLevel);
            WriteString(stream, "}\n");
        }
    }

    public static bool ReadAsBinary(this KValue k, Stream input)
    {
        KValue kValue = new KValue();
        k.Children.Add(kValue);
        return kValue.TryReadAsBinary(input);
    }

    public static bool TryReadAsBinary(this KValue k, Stream input)
    {
        return TryReadAsBinaryCore(input, k, null);
    }

    private static bool TryReadAsBinaryCore(Stream input, KValue current, KValue parent)
    {
        current.Children = new List<KValue>();
        while (true)
        {
            KValueType kValueType = (KValueType)input.ReadByte();
            if (kValueType == KValueType.End)
            {
                break;
            }
            current.Name = input.ReadNullTermString(Encoding.UTF8).ToLower();
            current.KType = kValueType;
            switch (kValueType)
            {
                case KValueType.None:
                    {
                        KValue current2 = new KValue();
                        if (!TryReadAsBinaryCore(input, current2, current))
                        {
                            return false;
                        }
                        break;
                    }
                case KValueType.String:
                    current.Value = input.ReadNullTermString(Encoding.UTF8);
                    break;
                case KValueType.Int32:
                    current.Value = input.ReadInt32L();
                    break;
                case KValueType.Float32:
                    current.Value = input.ReadFloatL();
                    break;
                case KValueType.Pointer:
                    current.Value = input.ReadInt32L();
                    break;
                case KValueType.WideString:
                    current.Value = input.ReadWideString();
                    break;
                case KValueType.Color:
                    current.Value = Color.FromArgb(input.ReadByte(), input.ReadByte(), input.ReadByte());
                    break;
                case KValueType.UInt64:
                    current.Value = input.ReadUInt64L();
                    break;
                case KValueType.Int64:
                    current.Value = input.ReadInt64L();
                    break;
                default:
                    return false;
            }
            parent?.Children.Add(current);
            current = new KValue();
        }
        return true;
    }

    public static object ToDictionary(this KValue k)
    {
        return k.ToDictionary(0);
    }

    private static object ToDictionary(this KValue k, int depth)
    {
        bool num = k.Name != null;
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        List<KValue>.Enumerator enumerator;
        if (num)
        {
            if (k.Value != null)
            {
                Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
                if (k.Value is string)
                {
                    dictionary2[k.Name] = EscapeString(k.Value.ToString());
                    if (depth <= 0)
                    {
                        return dictionary2;
                    }
                    return EscapeString(k.Value.ToString());
                }
                dictionary2[k.Name] = k.Value;
                if (depth <= 0)
                {
                    return dictionary2;
                }
                return k.Value;
            }
            Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
            enumerator = k.Children.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    KValue current = enumerator.Current;
                    dictionary3[EscapeString(current.Name)] = current.ToDictionary(depth + 1);
                }
            }
            finally
            {
                ((IDisposable)enumerator).Dispose();
            }
            dictionary[k.Name] = dictionary3;
            if (depth != 0)
            {
                return dictionary3;
            }
            return dictionary;
        }
        if (k.Value == null)
        {
            enumerator = k.Children.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    KValue current2 = enumerator.Current;
                    dictionary[EscapeString(current2.Name)] = current2.ToDictionary(depth + 1);
                }
                return dictionary;
            }
            finally
            {
                ((IDisposable)enumerator).Dispose();
            }
        }
        if (k.Value is string)
        {
            return EscapeString(k.Value.ToString());
        }
        return k.Value;
    }

    public static KValue Clone(this KValue k)
    {
        KValue kValue = new KValue(k.Name, k.Value, k.KType);
        if (k.Children != null && k.Children.Count != 0)
        {
            foreach (KValue child in k.Children)
            {
                kValue.Children.Add(child.Clone());
            }
            return kValue;
        }
        return kValue;
    }

    public static bool ContainsKey(this KValue k, string key)
    {
        return k?.Children.FirstOrDefault((KValue c) => string.Equals(c.Name, key, StringComparison.OrdinalIgnoreCase)) != null;
    }

    public static bool RemoveKey(this KValue k, string path)
    {
        if (k == null)
        {
            return false;
        }
        string[] array = path.Split(new string[4]
        {
            "/",
            "|",
            ",",
            "."
        }, StringSplitOptions.RemoveEmptyEntries);
        if (array.Length == 0)
        {
            return false;
        }
        if (array.Length != 0 && array[0].Equals(k.Name, StringComparison.InvariantCultureIgnoreCase))
        {
            array = array.Skip(1).ToArray();
        }
        KValue kValue = null;
        int num = 0;
        while (true)
        {
            if (num >= array.Length)
            {
                return false;
            }
            string key = array[num];
            if (kValue == null && k.ContainsKey(key))
            {
                if (num == array.Length - 1 && k.Children.Remove(k[key]))
                {
                    return true;
                }
                kValue = k[key];
            }
            else
            {
                if (kValue == null || !kValue.ContainsKey(key))
                {
                    break;
                }
                if (num == array.Length - 1 && kValue.Children.Remove(kValue[key]))
                {
                    return true;
                }
                kValue = kValue[key];
            }
            num++;
        }
        return false;
    }

    public static KValue GetKey(this KValue k, string path)
    {
        string[] array = path.Split(new string[4]
        {
            "/",
            "|",
            ",",
            "."
        }, StringSplitOptions.RemoveEmptyEntries);
        if (array.Length != 0 && array[0].Equals(k.Name, StringComparison.InvariantCultureIgnoreCase))
        {
            array = array.Skip(1).ToArray();
        }
        KValue kValue = null;
        string[] array2 = array;
        int num = 0;
        while (true)
        {
            if (num >= array2.Length)
            {
                return kValue;
            }
            string key = array2[num];
            if (kValue == null && k.ContainsKey(key))
            {
                kValue = k[key];
            }
            else
            {
                if (kValue == null || !kValue.ContainsKey(key))
                {
                    break;
                }
                kValue = kValue[key];
            }
            num++;
        }
        return KValue.Invalid;
    }

    public static bool HasKey(this KValue k, string path)
    {
        string[] array = path.Split(new string[4]
        {
            "/",
            "|",
            ",",
            "."
        }, StringSplitOptions.RemoveEmptyEntries);
        if (array.Length != 0 && array[0].Equals(k.Name, StringComparison.InvariantCultureIgnoreCase))
        {
            array = array.Skip(1).ToArray();
        }
        KValue kValue = null;
        string[] array2 = array;
        int num = 0;
        while (true)
        {
            if (num >= array2.Length)
            {
                return true;
            }
            string key = array2[num];
            if (kValue == null && k.ContainsKey(key))
            {
                kValue = k[key];
            }
            else
            {
                if (kValue == null || !kValue.ContainsKey(key))
                {
                    break;
                }
                kValue = kValue[key];
            }
            num++;
        }
        return false;
    }
}

