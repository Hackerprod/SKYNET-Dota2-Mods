using System;
using System.Reflection;

namespace NAudio.Utils
{
	public static class FieldDescriptionHelper
	{
		public static string Describe(Type t, Guid guid)
		{
			FieldInfo[] fields = t.GetFields(BindingFlags.Static | BindingFlags.Public);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.IsPublic && fieldInfo.IsStatic && fieldInfo.FieldType == typeof(Guid) && (Guid)fieldInfo.GetValue(null) == guid)
				{
					object[] customAttributes = fieldInfo.GetCustomAttributes(inherit: false);
					foreach (object obj in customAttributes)
					{
						FieldDescriptionAttribute fieldDescriptionAttribute = obj as FieldDescriptionAttribute;
						if (fieldDescriptionAttribute != null)
						{
							return fieldDescriptionAttribute.Description;
						}
					}
					return fieldInfo.Name;
				}
			}
			return guid.ToString();
		}
	}
}
