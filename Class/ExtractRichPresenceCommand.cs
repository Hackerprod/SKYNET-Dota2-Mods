using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Dota2
{
	public class ExtractRichPresenceCommand
	{
		private bool bool_0;

		public string Description
		{
			get;
		}

		public string Usage
		{
			get;
		}

		public string Name
		{
			get;
		}

		public string Module
		{
			get;
		}

		public void ProcessCommand()
		{
			if (bool_0)
			{
				modCommon.WriteLine("Already extracting rich presence tokens...");
			}
			else
			{
				string richPresencePath = Paths.GetRichPresencePath(570u);
				Paths.EnsureDirectory(richPresencePath);
				if (modCommon.package != null)
				{
					modCommon.WriteLine("Extracting rich presence tokens");
					bool_0 = true;
					try
					{
						try
						{
							List<PackageEntry> list = modCommon.package.Entries["txt"].Where(delegate(PackageEntry e)
							{
								if (!e.FileName.Contains("richpresence_spanish"))
								{
									return e.FileName.Contains("richpresence_english");
								}
								return true;
							}).ToList();
							if (list.Any())
							{
								modCommon.WriteLine($"{list.Count} rich presence languages found...");
								int num = 0;
								foreach (PackageEntry item in list)
								{
									string path = Path.Combine(richPresencePath, item.FileName.Replace("richpresence_", string.Empty) + ".vdf");
									num++;
									modCommon.Write($"{(float)num / ((float)list.Count * 1f) * 100f:0}% exported.\r\n");
									using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate))
									{
										byte[] array = default(byte[]);
                                        modCommon.package.ReadEntry(item, out array, true);
										fileStream.Write(array, 0, array.Length);
									}
								}
							}
						}
						finally
						{
							((IDisposable)modCommon.package)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						bool_0 = false;
						modCommon.WriteLine("Error extracting rich presence tokens.\r\n" + ex.Message + "\r\n" + ex.StackTrace);
						return;
					}
					modCommon.WriteLine("Rich presence tokens extracted successfully.");
				}
				else
				{
					//modCommon.WriteLine("Dota 2 was not found on content folders " + text + "...");
				}
				bool_0 = false;
			}
		}
	}
}
