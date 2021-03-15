using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public class KValue : IDisposable
{
    public static readonly KValue Invalid = new KValue
    {
        KType = KValueType.Invalid
    };

    private bool _disposed;

    public string Name
    {
        get;
        set;
    }

    public object Value
    {
        get;
        set;
    }

    public KValueType KType
    {
        get;
        set;
    }

    public List<KValue> Children
    {
        get;
        internal set;
    }

    public KValue this[string key]
    {
        get
        {
            List<KValue> children = Children;
            object obj;
            if (children == null)
            {
                obj = null;
            }
            else
            {
                obj = children.FirstOrDefault((KValue c) => string.Equals(c.Name, key, StringComparison.OrdinalIgnoreCase));
                if (obj != null)
                {
                    goto IL_0034;
                }
            }
            obj = Invalid;
            goto IL_0034;
        IL_0034:
            return (KValue)obj;
        }
        set
        {
            KValue kValue = Children.FirstOrDefault((KValue c) => string.Equals(c.Name, key, StringComparison.OrdinalIgnoreCase));
            if (kValue != null)
            {
                Children.Remove(kValue);
                kValue.Dispose();
            }
            value.Name = key;
            Children.Add(value);
        }
    }

    public KValue()
    {
        Children = new List<KValue>();
    }

    public KValue(string name)
    {
        Name = name;
        Children = new List<KValue>();
    }

    public KValue(string name, object value)
    {
        Name = name;
        Value = value;
        Children = new List<KValue>();
    }

    public KValue(string name, object value, KValueType ktype)
    {
        Name = name;
        Value = value;
        KType = ktype;
        Children = new List<KValue>();
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public static KValue CreateWithChildren(string name = null, params KValue[] children)
    {
        KValue kValue = new KValue(name);
        if (children.Length != 0)
        {
            kValue.Children.AddRange(children);
        }
        return kValue;
    }

    public static KValue CreateWithValue(string name, object value)
    {
        return new KValue(name, value);
    }

    public static KValue CreateWithValue(string name, object value, KValueType valType)
    {
        return new KValue(name, value, valType);
    }

    public override string ToString()
    {
        return $"{Name} = {Value}";
    }

    ~KValue()
    {
        Dispose(disposing: false);
    }

    public void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                if (Children != null)
                {
                    for (int num = Children.Count - 1; num >= 0; num--)
                    {
                        Children[num].Dispose();
                    }
                    Children.Clear();
                    Children.TrimExcess();
                    Children = null;
                }
                Value = null;
                Name = null;
            }
            _disposed = true;
        }
    }

    public static KValue LoadAsText(string path)
    {
        return LoadFromFile(path, asBinary: false);
    }

    public static KValue LoadAsBinary(string path)
    {
        KValue kValue = LoadFromFile(path, asBinary: true);
        if (kValue == null)
        {
            return null;
        }
        return new KValue
        {
            Children =
            {
                kValue
            }
        };
    }

    public static bool TryLoadAsBinary(string path, out KValue keyValue)
    {
        keyValue = LoadFromFile(path, asBinary: true);
        return keyValue != null;
    }

    public static KValue LoadFromStream(Stream input, bool asBinary)
    {
        KValue kValue = new KValue();
        if (asBinary)
        {
            if (!kValue.TryReadAsBinary(input))
            {
                return null;
            }
        }
        else if (!kValue.ReadAsText(input))
        {
            return null;
        }
        return kValue;
    }

    private static KValue LoadFromFile(string path, bool asBinary)
    {
        if (!File.Exists(path))
        {
            return null;
        }
        using (FileStream input = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            KValue kValue = new KValue();
            if (asBinary)
            {
                if (!kValue.TryReadAsBinary(input))
                {
                    return null;
                }
            }
            else if (!kValue.ReadAsText(input))
            {
                return null;
            }
            return kValue;
        }
    }

    public static KValue LoadFromString(string input)
    {
        using (MemoryStream input2 = new MemoryStream(Encoding.UTF8.GetBytes(input)))
        {
            KValue kValue = new KValue();
            try
            {
                return (!kValue.ReadAsText(input2)) ? null : kValue;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    internal void SaveToFile(string path)
    {
        using (FileStream stream = File.Create(path))
        {
            SaveToStream(stream);
        }
    }

    private void SaveToStream(FileStream stream)
    {
        RecursiveSaveTextToFile(stream);
    }

    private void RecursiveSaveTextToFile(Stream stream, int indentLevel = 0)
    {
        bool flag = Name != null;
        if (flag)
        {
            WriteIndents(stream, indentLevel);
            WriteString(stream, EscapeString(Name), quote: true);
            WriteString(stream, "\n");
            WriteIndents(stream, indentLevel);
            WriteString(stream, "{\n");
        }
        else
        {
            indentLevel--;
        }
        foreach (KValue child in Children)
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
                    string name = child.Value.GetType().Name;
                    if (!(name == "Color"))
                    {
                        if (name == "String")
                        {
                            WriteString(stream, EscapeString(child.AsString()), quote: true);
                        }
                        else
                        {
                            WriteString(stream, child.AsString(), quote: true);
                        }
                    }
                    else
                    {
                        Color color = (child.Value as Color?) ?? default(Color);
                        WriteString(stream, "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2"), quote: true);
                    }
                    WriteString(stream, "\n");
                }
            }
        }
        if (flag)
        {
            WriteIndents(stream, indentLevel);
            WriteString(stream, "}\n");
        }
    }




    private static readonly Regex EscapeRegex = new Regex("[\\t\\r\\n\\a\\b\\f\\v\\\\]", RegexOptions.Compiled);

    private static readonly Dictionary<string, string> escapeMap = new Dictionary<string, string>
        {
            {
                "\\",
                "\\\\"
            },
            {
                "\a",
                "\\a"
            },
            {
                "\b",
                "\\b"
            },
            {
                "\f",
                "\\f"
            },
            {
                "\t",
                "\\t"
            },
            {
                "\n",
                "\\n"
            },
            {
                "\r",
                "\\r"
            },
            {
                "\v",
                "\\v"
            }
        }; private static string EscapeString(string value)
    {
        return EscapeRegex.Replace(value, (Match me) => escapeMap[me.Value]);
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

}
