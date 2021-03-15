namespace ValveResourceFormat.KeyValues
{
	public class KVValue
	{
		public KVType Type
		{
			get;
			private set;
		}

		public object Value
		{
			get;
			private set;
		}

		public KVValue(KVType type, object value)
		{
			Type = type;
			Value = value;
		}
	}
}
