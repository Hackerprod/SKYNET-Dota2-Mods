namespace NAudio.Codecs
{
	public class G722Codec
	{
		private static readonly int[] wl = new int[8]
		{
			-60,
			-30,
			58,
			172,
			334,
			538,
			1198,
			3042
		};

		private static readonly int[] rl42 = new int[16]
		{
			0,
			7,
			6,
			5,
			4,
			3,
			2,
			1,
			7,
			6,
			5,
			4,
			3,
			2,
			1,
			0
		};

		private static readonly int[] ilb = new int[32]
		{
			2048,
			2093,
			2139,
			2186,
			2233,
			2282,
			2332,
			2383,
			2435,
			2489,
			2543,
			2599,
			2656,
			2714,
			2774,
			2834,
			2896,
			2960,
			3025,
			3091,
			3158,
			3228,
			3298,
			3371,
			3444,
			3520,
			3597,
			3676,
			3756,
			3838,
			3922,
			4008
		};

		private static readonly int[] wh = new int[3]
		{
			0,
			-214,
			798
		};

		private static readonly int[] rh2 = new int[4]
		{
			2,
			1,
			2,
			1
		};

		private static readonly int[] qm2 = new int[4]
		{
			-7408,
			-1616,
			7408,
			1616
		};

		private static readonly int[] qm4 = new int[16]
		{
			0,
			-20456,
			-12896,
			-8968,
			-6288,
			-4240,
			-2584,
			-1200,
			20456,
			12896,
			8968,
			6288,
			4240,
			2584,
			1200,
			0
		};

		private static readonly int[] qm5 = new int[32]
		{
			-280,
			-280,
			-23352,
			-17560,
			-14120,
			-11664,
			-9752,
			-8184,
			-6864,
			-5712,
			-4696,
			-3784,
			-2960,
			-2208,
			-1520,
			-880,
			23352,
			17560,
			14120,
			11664,
			9752,
			8184,
			6864,
			5712,
			4696,
			3784,
			2960,
			2208,
			1520,
			880,
			280,
			-280
		};

		private static readonly int[] qm6 = new int[64]
		{
			-136,
			-136,
			-136,
			-136,
			-24808,
			-21904,
			-19008,
			-16704,
			-14984,
			-13512,
			-12280,
			-11192,
			-10232,
			-9360,
			-8576,
			-7856,
			-7192,
			-6576,
			-6000,
			-5456,
			-4944,
			-4464,
			-4008,
			-3576,
			-3168,
			-2776,
			-2400,
			-2032,
			-1688,
			-1360,
			-1040,
			-728,
			24808,
			21904,
			19008,
			16704,
			14984,
			13512,
			12280,
			11192,
			10232,
			9360,
			8576,
			7856,
			7192,
			6576,
			6000,
			5456,
			4944,
			4464,
			4008,
			3576,
			3168,
			2776,
			2400,
			2032,
			1688,
			1360,
			1040,
			728,
			432,
			136,
			-432,
			-136
		};

		private static readonly int[] qmf_coeffs = new int[12]
		{
			3,
			-11,
			12,
			32,
			-210,
			951,
			3876,
			-805,
			362,
			-156,
			53,
			-11
		};

		private static readonly int[] q6 = new int[32]
		{
			0,
			35,
			72,
			110,
			150,
			190,
			233,
			276,
			323,
			370,
			422,
			473,
			530,
			587,
			650,
			714,
			786,
			858,
			940,
			1023,
			1121,
			1219,
			1339,
			1458,
			1612,
			1765,
			1980,
			2195,
			2557,
			2919,
			0,
			0
		};

		private static readonly int[] iln = new int[32]
		{
			0,
			63,
			62,
			31,
			30,
			29,
			28,
			27,
			26,
			25,
			24,
			23,
			22,
			21,
			20,
			19,
			18,
			17,
			16,
			15,
			14,
			13,
			12,
			11,
			10,
			9,
			8,
			7,
			6,
			5,
			4,
			0
		};

		private static readonly int[] ilp = new int[32]
		{
			0,
			61,
			60,
			59,
			58,
			57,
			56,
			55,
			54,
			53,
			52,
			51,
			50,
			49,
			48,
			47,
			46,
			45,
			44,
			43,
			42,
			41,
			40,
			39,
			38,
			37,
			36,
			35,
			34,
			33,
			32,
			0
		};

		private static readonly int[] ihn = new int[3]
		{
			0,
			1,
			0
		};

		private static readonly int[] ihp = new int[3]
		{
			0,
			3,
			2
		};

		private static short Saturate(int amp)
		{
			short num = (short)amp;
			if (amp == num)
			{
				return num;
			}
			if (amp > 32767)
			{
				return short.MaxValue;
			}
			return short.MinValue;
		}

		private static void Block4(G722CodecState s, int band, int d)
		{
			s.Band[band].d[0] = d;
			s.Band[band].r[0] = Saturate(s.Band[band].s + d);
			s.Band[band].p[0] = Saturate(s.Band[band].sz + d);
			for (int i = 0; i < 3; i++)
			{
				s.Band[band].sg[i] = s.Band[band].p[i] >> 15;
			}
			int num = Saturate(s.Band[band].a[1] << 2);
			int num2 = (s.Band[band].sg[0] == s.Band[band].sg[1]) ? (-num) : num;
			if (num2 > 32767)
			{
				num2 = 32767;
			}
			int num3 = (s.Band[band].sg[0] == s.Band[band].sg[2]) ? 128 : (-128);
			num3 += num2 >> 7;
			num3 += s.Band[band].a[2] * 32512 >> 15;
			if (num3 > 12288)
			{
				num3 = 12288;
			}
			else if (num3 < -12288)
			{
				num3 = -12288;
			}
			s.Band[band].ap[2] = num3;
			s.Band[band].sg[0] = s.Band[band].p[0] >> 15;
			s.Band[band].sg[1] = s.Band[band].p[1] >> 15;
			num = ((s.Band[band].sg[0] == s.Band[band].sg[1]) ? 192 : (-192));
			num2 = s.Band[band].a[1] * 32640 >> 15;
			s.Band[band].ap[1] = Saturate(num + num2);
			num3 = Saturate(15360 - s.Band[band].ap[2]);
			if (s.Band[band].ap[1] > num3)
			{
				s.Band[band].ap[1] = num3;
			}
			else if (s.Band[band].ap[1] < -num3)
			{
				s.Band[band].ap[1] = -num3;
			}
			num = ((d != 0) ? 128 : 0);
			s.Band[band].sg[0] = d >> 15;
			for (int i = 1; i < 7; i++)
			{
				s.Band[band].sg[i] = s.Band[band].d[i] >> 15;
				num2 = ((s.Band[band].sg[i] == s.Band[band].sg[0]) ? num : (-num));
				num3 = s.Band[band].b[i] * 32640 >> 15;
				s.Band[band].bp[i] = Saturate(num2 + num3);
			}
			for (int i = 6; i > 0; i--)
			{
				s.Band[band].d[i] = s.Band[band].d[i - 1];
				s.Band[band].b[i] = s.Band[band].bp[i];
			}
			for (int i = 2; i > 0; i--)
			{
				s.Band[band].r[i] = s.Band[band].r[i - 1];
				s.Band[band].p[i] = s.Band[band].p[i - 1];
				s.Band[band].a[i] = s.Band[band].ap[i];
			}
			num = Saturate(s.Band[band].r[1] + s.Band[band].r[1]);
			num = s.Band[band].a[1] * num >> 15;
			num2 = Saturate(s.Band[band].r[2] + s.Band[band].r[2]);
			num2 = s.Band[band].a[2] * num2 >> 15;
			s.Band[band].sp = Saturate(num + num2);
			s.Band[band].sz = 0;
			for (int i = 6; i > 0; i--)
			{
				num = Saturate(s.Band[band].d[i] + s.Band[band].d[i]);
				s.Band[band].sz += s.Band[band].b[i] * num >> 15;
			}
			s.Band[band].sz = Saturate(s.Band[band].sz);
			s.Band[band].s = Saturate(s.Band[band].sp + s.Band[band].sz);
		}

		public int Decode(G722CodecState state, short[] outputBuffer, byte[] inputG722Data, int inputLength)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			while (num3 < inputLength)
			{
				int num5;
				if (state.Packed)
				{
					if (state.InBits < state.BitsPerSample)
					{
						state.InBuffer |= (uint)(inputG722Data[num3++] << state.InBits);
						state.InBits += 8;
					}
					num5 = ((int)state.InBuffer & ((1 << state.BitsPerSample) - 1));
					state.InBuffer >>= state.BitsPerSample;
					state.InBits -= state.BitsPerSample;
				}
				else
				{
					num5 = inputG722Data[num3++];
				}
				int num8;
				int num7;
				int num9;
				switch (state.BitsPerSample)
				{
				default:
					num7 = (num5 & 0x3F);
					num8 = ((num5 >> 6) & 3);
					num9 = qm6[num7];
					num7 >>= 2;
					break;
				case 7:
					num7 = (num5 & 0x1F);
					num8 = ((num5 >> 5) & 3);
					num9 = qm5[num7];
					num7 >>= 1;
					break;
				case 6:
					num7 = (num5 & 0xF);
					num8 = ((num5 >> 4) & 3);
					num9 = qm4[num7];
					break;
				}
				num9 = state.Band[0].det * num9 >> 15;
				int num10 = state.Band[0].s + num9;
				if (num10 > 16383)
				{
					num10 = 16383;
				}
				else if (num10 < -16384)
				{
					num10 = -16384;
				}
				num9 = qm4[num7];
				int d = state.Band[0].det * num9 >> 15;
				num9 = rl42[num7];
				num7 = state.Band[0].nb * 127 >> 7;
				num7 += wl[num9];
				if (num7 < 0)
				{
					num7 = 0;
				}
				else if (num7 > 18432)
				{
					num7 = 18432;
				}
				state.Band[0].nb = num7;
				num7 = ((state.Band[0].nb >> 6) & 0x1F);
				num9 = 8 - (state.Band[0].nb >> 11);
				int num11 = (num9 < 0) ? (ilb[num7] << -num9) : (ilb[num7] >> num9);
				state.Band[0].det = num11 << 2;
				Block4(state, 0, d);
				if (!state.EncodeFrom8000Hz)
				{
					num9 = qm2[num8];
					int num12 = state.Band[1].det * num9 >> 15;
					num2 = num12 + state.Band[1].s;
					if (num2 > 16383)
					{
						num2 = 16383;
					}
					else if (num2 < -16384)
					{
						num2 = -16384;
					}
					num9 = rh2[num8];
					num7 = state.Band[1].nb * 127 >> 7;
					num7 += wh[num9];
					if (num7 < 0)
					{
						num7 = 0;
					}
					else if (num7 > 22528)
					{
						num7 = 22528;
					}
					state.Band[1].nb = num7;
					num7 = ((state.Band[1].nb >> 6) & 0x1F);
					num9 = 10 - (state.Band[1].nb >> 11);
					num11 = ((num9 < 0) ? (ilb[num7] << -num9) : (ilb[num7] >> num9));
					state.Band[1].det = num11 << 2;
					Block4(state, 1, num12);
				}
				if (state.ItuTestMode)
				{
					outputBuffer[num++] = (short)(num10 << 1);
					outputBuffer[num++] = (short)(num2 << 1);
				}
				else if (state.EncodeFrom8000Hz)
				{
					outputBuffer[num++] = (short)(num10 << 1);
				}
				else
				{
					for (int i = 0; i < 22; i++)
					{
						state.QmfSignalHistory[i] = state.QmfSignalHistory[i + 2];
					}
					state.QmfSignalHistory[22] = num10 + num2;
					state.QmfSignalHistory[23] = num10 - num2;
					int num16 = 0;
					int num17 = 0;
					for (int i = 0; i < 12; i++)
					{
						num17 += state.QmfSignalHistory[2 * i] * qmf_coeffs[i];
						num16 += state.QmfSignalHistory[2 * i + 1] * qmf_coeffs[11 - i];
					}
					outputBuffer[num++] = (short)(num16 >> 11);
					outputBuffer[num++] = (short)(num17 >> 11);
				}
			}
			return num;
		}

		public int Encode(G722CodecState state, byte[] outputBuffer, short[] inputBuffer, int inputBufferCount)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			while (num3 < inputBufferCount)
			{
				int num5;
				int i;
				if (state.ItuTestMode)
				{
					num5 = (num2 = inputBuffer[num3++] >> 1);
				}
				else if (state.EncodeFrom8000Hz)
				{
					num5 = inputBuffer[num3++] >> 1;
				}
				else
				{
					for (i = 0; i < 22; i++)
					{
						state.QmfSignalHistory[i] = state.QmfSignalHistory[i + 2];
					}
					state.QmfSignalHistory[22] = inputBuffer[num3++];
					state.QmfSignalHistory[23] = inputBuffer[num3++];
					int num9 = 0;
					int num10 = 0;
					for (i = 0; i < 12; i++)
					{
						num10 += state.QmfSignalHistory[2 * i] * qmf_coeffs[i];
						num9 += state.QmfSignalHistory[2 * i + 1] * qmf_coeffs[11 - i];
					}
					num5 = num9 + num10 >> 14;
					num2 = num9 - num10 >> 14;
				}
				int num11 = Saturate(num5 - state.Band[0].s);
				int num12 = (num11 >= 0) ? num11 : (-(num11 + 1));
				int num13;
				for (i = 1; i < 30; i++)
				{
					num13 = q6[i] * state.Band[0].det >> 12;
					if (num12 < num13)
					{
						break;
					}
				}
				int num14 = (num11 < 0) ? iln[i] : ilp[i];
				int num15 = num14 >> 2;
				int num16 = qm4[num15];
				int d = state.Band[0].det * num16 >> 15;
				int num17 = rl42[num15];
				num12 = state.Band[0].nb * 127 >> 7;
				state.Band[0].nb = num12 + wl[num17];
				if (state.Band[0].nb < 0)
				{
					state.Band[0].nb = 0;
				}
				else if (state.Band[0].nb > 18432)
				{
					state.Band[0].nb = 18432;
				}
				num13 = ((state.Band[0].nb >> 6) & 0x1F);
				num16 = 8 - (state.Band[0].nb >> 11);
				int num18 = (num16 < 0) ? (ilb[num13] << -num16) : (ilb[num13] >> num16);
				state.Band[0].det = num18 << 2;
				Block4(state, 0, d);
				int num19;
				if (state.EncodeFrom8000Hz)
				{
					num19 = (0xC0 | num14) >> 8 - state.BitsPerSample;
				}
				else
				{
					int num20 = Saturate(num2 - state.Band[1].s);
					num12 = ((num20 >= 0) ? num20 : (-(num20 + 1)));
					num13 = 564 * state.Band[1].det >> 12;
					int num21 = (num12 < num13) ? 1 : 2;
					int num22 = (num20 < 0) ? ihn[num21] : ihp[num21];
					num16 = qm2[num22];
					int d2 = state.Band[1].det * num16 >> 15;
					int num23 = rh2[num22];
					num12 = state.Band[1].nb * 127 >> 7;
					state.Band[1].nb = num12 + wh[num23];
					if (state.Band[1].nb < 0)
					{
						state.Band[1].nb = 0;
					}
					else if (state.Band[1].nb > 22528)
					{
						state.Band[1].nb = 22528;
					}
					num13 = ((state.Band[1].nb >> 6) & 0x1F);
					num16 = 10 - (state.Band[1].nb >> 11);
					num18 = ((num16 < 0) ? (ilb[num13] << -num16) : (ilb[num13] >> num16));
					state.Band[1].det = num18 << 2;
					Block4(state, 1, d2);
					num19 = ((num22 << 6) | num14) >> 8 - state.BitsPerSample;
				}
				if (state.Packed)
				{
					state.OutBuffer |= (uint)(num19 << state.OutBits);
					state.OutBits += state.BitsPerSample;
					if (state.OutBits >= 8)
					{
						outputBuffer[num++] = (byte)(state.OutBuffer & 0xFF);
						state.OutBits -= 8;
						state.OutBuffer >>= 8;
					}
				}
				else
				{
					outputBuffer[num++] = (byte)num19;
				}
			}
			return num;
		}
	}
}
