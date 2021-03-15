using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace ValveResourceFormat.ThirdParty
{
	internal sealed class AsnKeyParser
	{
		private readonly AsnParser _parser;

		public AsnKeyParser(ICollection<byte> contents)
		{
			_parser = new AsnParser(contents);
		}

		public static byte[] TrimLeadingZero(byte[] values)
		{
			checked
			{
				byte[] array;
				if (values[0] == 0 && values.Length > 1)
				{
					array = new byte[values.Length - 1];
					Array.Copy(values, 1, array, 0, values.Length - 1);
				}
				else
				{
					array = new byte[values.Length];
					Array.Copy(values, array, values.Length);
				}
				return array;
			}
		}

		public static bool EqualOid(byte[] first, byte[] second)
		{
			if (first.Length != second.Length)
			{
				return false;
			}
			for (int i = 0; i < first.Length; i = checked(i + 1))
			{
				if (first[i] != second[i])
				{
					return false;
				}
			}
			return true;
		}

		public RSAParameters ParseRSAPublicKey()
		{
			RSAParameters result = default(RSAParameters);
			int position = _parser.CurrentPosition();
			int num = _parser.NextSequence();
			if (num != _parser.RemainingBytes())
			{
				StringBuilder stringBuilder = new StringBuilder("Incorrect Sequence Size. ");
				stringBuilder.AppendFormat("Specified: {0}, Remaining: {1}", num.ToString(CultureInfo.InvariantCulture), _parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
				throw new BerDecodeException(stringBuilder.ToString(), position);
			}
			position = _parser.CurrentPosition();
			num = _parser.NextSequence();
			if (num > _parser.RemainingBytes())
			{
				StringBuilder stringBuilder2 = new StringBuilder("Incorrect AlgorithmIdentifier Size. ");
				stringBuilder2.AppendFormat("Specified: {0}, Remaining: {1}", num.ToString(CultureInfo.InvariantCulture), _parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
				throw new BerDecodeException(stringBuilder2.ToString(), position);
			}
			position = _parser.CurrentPosition();
			byte[] first = _parser.NextOID();
			byte[] second = new byte[9]
			{
				42,
				134,
				72,
				134,
				247,
				13,
				1,
				1,
				1
			};
			if (!EqualOid(first, second))
			{
				throw new BerDecodeException("Expected OID 1.2.840.113549.1.1.1", position);
			}
			if (_parser.IsNextNull())
			{
				_parser.NextNull();
			}
			else
			{
				_parser.Next();
			}
			position = _parser.CurrentPosition();
			num = _parser.NextBitString();
			if (num > _parser.RemainingBytes())
			{
				StringBuilder stringBuilder3 = new StringBuilder("Incorrect PublicKey Size. ");
				stringBuilder3.AppendFormat("Specified: {0}, Remaining: {1}", num.ToString(CultureInfo.InvariantCulture), _parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
				throw new BerDecodeException(stringBuilder3.ToString(), position);
			}
			position = _parser.CurrentPosition();
			num = _parser.NextSequence();
			if (num < _parser.RemainingBytes())
			{
				StringBuilder stringBuilder4 = new StringBuilder("Incorrect RSAPublicKey Size. ");
				stringBuilder4.AppendFormat("Specified: {0}, Remaining: {1}", num.ToString(CultureInfo.InvariantCulture), _parser.RemainingBytes().ToString(CultureInfo.InvariantCulture));
				throw new BerDecodeException(stringBuilder4.ToString(), position);
			}
			result.Modulus = TrimLeadingZero(_parser.NextInteger());
			result.Exponent = TrimLeadingZero(_parser.NextInteger());
			return result;
		}
	}
}
