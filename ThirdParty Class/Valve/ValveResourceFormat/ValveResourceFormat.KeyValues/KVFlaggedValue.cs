namespace ValveResourceFormat.KeyValues
{
	public class KVFlaggedValue : KVValue
	{
		public KVFlag Flag
		{
			get;
			private set;
		}

		public KVFlaggedValue(KVType type, object value)
			: base(type, value)
		{
			Flag = KVFlag.None;
		}

		public KVFlaggedValue(KVType type, KVFlag flag, object value)
			: base(type, value)
		{
			Flag = flag;
		}
	}
}
