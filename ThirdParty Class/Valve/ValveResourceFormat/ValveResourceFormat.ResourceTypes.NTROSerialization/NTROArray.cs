using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;

namespace ValveResourceFormat.ResourceTypes.NTROSerialization
{
	public class NTROArray : NTROValue, IList<NTROValue>, ICollection<NTROValue>, IEnumerable<NTROValue>, IEnumerable
	{
		private readonly NTROValue[] contents;

		public bool IsIndirection
		{
			get;
			private set;
		}

		public NTROValue this[int index]
		{
			get
			{
				return ((IList<NTROValue>)contents)[index];
			}
			set
			{
				((IList<NTROValue>)contents)[index] = value;
			}
		}

		public int Count => ((ICollection<NTROValue>)contents).Count;

		public bool IsReadOnly => ((ICollection<NTROValue>)contents).IsReadOnly;

		public NTROArray(DataType type, int count, bool pointer = false, bool isIndirection = false)
		{
			base.Type = type;
			base.Pointer = pointer;
			IsIndirection = isIndirection;
			contents = new NTROValue[count];
		}

		public override void WriteText(IndentedTextWriter writer)
		{
			if (Count > 1)
			{
				throw new NotImplementedException("NTROArray.Count > 1");
			}
			using (IEnumerator<NTROValue> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					NTROValue current = enumerator.Current;
					current.WriteText(writer);
				}
			}
		}

		public void Add(NTROValue item)
		{
			((ICollection<NTROValue>)contents).Add(item);
		}

		public void Clear()
		{
			((ICollection<NTROValue>)contents).Clear();
		}

		public bool Contains(NTROValue item)
		{
			return ((ICollection<NTROValue>)contents).Contains(item);
		}

		public void CopyTo(NTROValue[] array, int arrayIndex)
		{
			((ICollection<NTROValue>)contents).CopyTo(array, arrayIndex);
		}

		public IEnumerator<NTROValue> GetEnumerator()
		{
			return ((IEnumerable<NTROValue>)contents).GetEnumerator();
		}

		public int IndexOf(NTROValue item)
		{
			return ((IList<NTROValue>)contents).IndexOf(item);
		}

		public void Insert(int index, NTROValue item)
		{
			((IList<NTROValue>)contents).Insert(index, item);
		}

		public bool Remove(NTROValue item)
		{
			return ((ICollection<NTROValue>)contents).Remove(item);
		}

		public void RemoveAt(int index)
		{
			((IList<NTROValue>)contents).RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<NTROValue>)contents).GetEnumerator();
		}

		public T Get<T>(int index)
		{
			return ((NTROValue<T>)this[index]).Value;
		}

		public T[] ToArray<T>()
		{
			T[] array = new T[Count];
			for (int i = 0; i < Count; i = checked(i + 1))
			{
				array[i] = Get<T>(i);
			}
			return array;
		}
	}
}
