using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

internal class KValueTextReader : StreamReader
{
    internal static Dictionary<char, char> EscapedMapping = new Dictionary<char, char>
    {
        {
            '\\',
            '\\'
        },
        {
            'n',
            '\n'
        },
        {
            'r',
            '\r'
        },
        {
            't',
            '\t'
        }
    };

    public KValueTextReader(KValue kv, Stream input)
        : base(input)
    {
        KValue kValue = kv;
        while (true)
        {
            string text = ReadToken(out bool wasQuoted, out bool wasConditional);
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            if (kValue == null)
            {
                kValue = new KValue(text);
            }
            else
            {
                kValue.Name = text;
            }
            text = ReadToken(out wasQuoted, out wasConditional);
            if (wasConditional)
            {
                text = ReadToken(out wasQuoted, out wasConditional);
            }
            if (!text.StartsWith("{") || wasQuoted)
            {
                break;
            }
            RecursiveLoadFromBuffer(kValue, this);
            kValue = null;
            if (base.EndOfStream)
            {
                return;
            }
        }
        //throw new Exception("LoadFromBuffer: missing {");
    }

    private void RecursiveLoadFromBuffer(KValue k, KValueTextReader kvr)
    {
        while (true)
        {
            bool wasQuoted;
            bool wasConditional;
            string text = kvr.ReadToken(out wasQuoted, out wasConditional);
            if (string.IsNullOrEmpty(text))
            {
                throw new Exception("RecursiveLoadFromBuffer: got EOF or empty keyname");
            }
            if (text.StartsWith("}") && !wasQuoted)
            {
                return;
            }
            KValue kValue = new KValue(text)
            {
                Children = new List<KValue>()
            };
            k.Children.Add(kValue);
            string text2 = kvr.ReadToken(out wasQuoted, out wasConditional);
            if (wasConditional && text2 != null)
            {
                text2 = kvr.ReadToken(out wasQuoted, out wasConditional);
            }
            if (text2 == null)
            {
                throw new Exception("RecursiveLoadFromBuffer:  got NULL key");
            }
            if (text2.StartsWith("}") && !wasQuoted)
            {
                throw new Exception("RecursiveLoadFromBuffer:  got } in key");
            }
            if (text2.StartsWith("{") && !wasQuoted)
            {
                RecursiveLoadFromBuffer(kValue, kvr);
            }
            else
            {
                if (wasConditional)
                {
                    break;
                }
                kValue.Value = text2;
                kValue.KType = KValueType.String;
            }
        }
        throw new Exception("RecursiveLoadFromBuffer:  got conditional between key and value");
    }

    private void EatWhiteSpace()
    {
        while (!base.EndOfStream && char.IsWhiteSpace((char)Peek()))
        {
            Read();
        }
    }

    private bool EatCppComment()
    {
        if (!base.EndOfStream)
        {
            if ((ushort)Peek() == 47)
            {
                ReadLine();
                return true;
            }
            return false;
        }
        return false;
    }

    public string ReadToken(out bool wasQuoted, out bool wasConditional)
    {
        wasQuoted = false;
        wasConditional = false;
        do
        {
            EatWhiteSpace();
            if (base.EndOfStream)
            {
                return null;
            }
        }
        while (EatCppComment());
        if (!base.EndOfStream)
        {
            char c = (char)Peek();
            switch (c)
            {
                case '"':
                    {
                        wasQuoted = true;
                        Read();
                        StringBuilder stringBuilder2 = new StringBuilder();
                        while (!base.EndOfStream)
                        {
                            if (Peek() == 92)
                            {
                                Read();
                                char c2 = (char)Read();
                                if (EscapedMapping.TryGetValue(c2, out char value))
                                {
                                    stringBuilder2.Append(value);
                                }
                                else
                                {
                                    stringBuilder2.Append(c2);
                                }
                            }
                            else
                            {
                                if (Peek() == 34)
                                {
                                    break;
                                }
                                stringBuilder2.Append((char)Read());
                            }
                        }
                        Read();
                        return stringBuilder2.ToString();
                    }
                default:
                    {
                        bool flag = false;
                        int num = 0;
                        StringBuilder stringBuilder = new StringBuilder();
                        while (true)
                        {
                            if (!base.EndOfStream)
                            {
                                c = (char)Peek();
                                switch (c)
                                {
                                    case '[':
                                        flag = true;
                                        goto default;
                                    default:
                                        if (c == ']' && flag)
                                        {
                                            wasConditional = true;
                                        }
                                        if (char.IsWhiteSpace(c))
                                        {
                                            break;
                                        }
                                        goto IL_0109;
                                    case '"':
                                    case '{':
                                    case '}':
                                        break;
                                }
                            }
                            break;
                        IL_0109:
                            if (num >= 1023)
                            {
                                throw new Exception("ReadToken overflow");
                            }
                            stringBuilder.Append(c);
                            Read();
                        }
                        return stringBuilder.ToString();
                    }
                case '{':
                case '}':
                    Read();
                    return c.ToString();
            }
        }
        return null;
    }
}
