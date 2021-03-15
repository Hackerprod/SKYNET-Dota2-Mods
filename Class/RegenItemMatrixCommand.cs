using SKYNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Dota2
{
	public class RegenItemMatrixCommand 
	{
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
		public void ProcessCommand(Package package)
		{
			string text3 = Path.Combine("data", "db");
			string path = Path.Combine(text3, "items_game.txt");
			string path2 = Path.Combine(text3, "lang_spanish.txt");
            string path3 = Path.Combine(text3, "lang_english.txt");
            string path4 = Path.Combine(text3, "portraits.txt");
            string path5 = Path.Combine(text3, "activelist.txt");
            string path6 = Path.Combine(text3, "items.txt");

            if (!Settings.GenerateOnStart && File.Exists(path))
                return;


            Paths.EnsureDirectory(text3);
			if (package != null)
			{
                SaveFile(package, "txt", "items_game", "", path);

                SaveFile(package, "txt", "items_spanish", "", path2);

                SaveFile(package, "txt", "items_english", "", path3);

                SaveFile(package, "txt", "portraits", "scripts/npc", path4);

                SaveFile(package, "txt", "activelist", "scripts/npc", path5);

                SaveFile(package, "txt", "items", "scripts/npc", path6);
            }
            else
			{
				modCommon.WriteLine("Dota 2 was not found on content folders...");
			}
			if (File.Exists(path))
			{
	//			modCommon.WriteLine("Generating item matrix on database...");
                CreateItemMatrix("data");
                //				CMsgTalentWinRates3_0.Store.FillConsumables();
    //            modCommon.WriteLine("Dota 2 item matrix generated successfully");
                return;

                try
                {
				}
				catch (Exception ex2)
				{
					modCommon.WriteLine("Error generating Store item matrix.\r" + ex2.Message + "\r" + ex2.StackTrace);
				}
			}
		}

        private void SaveFile(Package package, string type, string FileName, string DirectoryName, string path)
        {
            try
            {
                PackageEntry val6 = package.Entries[type].Find((PackageEntry p) => p.FileName == FileName && p.DirectoryName.Contains(DirectoryName));
                if (val6 != null)
                {
                    using (FileStream fileStream5 = new FileStream(path, FileMode.OpenOrCreate))
                    {
                        byte[] array3 = default(byte[]);
                        package.ReadEntry(val6, out array3, true);
                        fileStream5.Write(array3, 0, array3.Length);
                    }
                }

            }
            catch (Exception)
            {
            }
        }

        public void CreateItemMatrix(string basePath)
        {

            Dictionary<string, uint> dictionary = new Dictionary<string, uint>();
            Dictionary<string, uint> qualities = new Dictionary<string, uint>();
            Dictionary<string, uint> dictionary3 = new Dictionary<string, uint>();
            string path = Path.Combine("data", "db", "items_game.txt");
            string[] files = Directory.GetFiles(Path.Combine("data", "db"), "lang_*");

            List<KValue> list = new List<KValue>();
            string[] array = files;
            for (int i = 0; i < array.Length; i++)
            {
                KValue kValue = KValue.LoadAsText(array[i]);
                if (kValue != null)
                {
                    list.Add(kValue);
                }
            }
            return;
            //items_game.txt
            /*using (KValue kValue2 = KValue.LoadAsText(path))
            {
                if (kValue2 == null)
                {
                    throw new Exception("Error loading items database.");
                }
                List<KValue>.Enumerator enumerator;
                if (kValue2.ContainsKey("qualities"))
                {
                    enumerator = kValue2["qualities"].Children.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            KValue current = enumerator.Current;
                            qualities.Add(current.Name, current["value"].AsUnsignedInt(0u));
                        }
                    }
                    finally
                    {
                        ((IDisposable)enumerator).Dispose();
                    }
                    modCommon.Write($"{qualities.Count} item qualities found...\r");
                }
                if (kValue2.ContainsKey("attributes"))
                {
                    enumerator = kValue2["attributes"].Children.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            KValue current2 = enumerator.Current;
                            uint value = Convert.ToUInt32(current2.Name);
                            dictionary[current2["name"].AsString()] = value;
                        }
                    }
                    finally
                    {
                        ((IDisposable)enumerator).Dispose();
                    }
                    modCommon.Write($"{dictionary.Count} item attributes found...\r");
                }
                //mongoDbCollection_1.Collection.DeleteMany(FilterDefinition<DotaItem>.get_Empty(), default(CancellationToken));
				bool flag = false;
				modCommon.Write("Indexing item names...\r");
				enumerator = kValue2["items"].Children.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						KValue current3 = enumerator.Current;
						if (!(current3.Name == "default") && uint.TryParse(current3.Name, out uint result) && current3.HasKey("name"))
						{
							dictionary3[current3.GetKey("name").AsString()] = result;
						}
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				modCommon.Write("Generating loot lists...\r");
				if (kValue2.ContainsKey("loot_lists"))
				{
					flag = true;
					KValue key = kValue2.GetKey("loot_lists");
					enumerator = key.Children.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							KValue current4 = enumerator.Current;
//							LootItem value2 = method_2(dictionary3, kValue2, key, current4);
//							dictionary4[current4.Name] = value2;
						}
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
				}
				modCommon.Write("Loot list generated...\r");
				int num = 0;
				int num2 = kValue2["items"].Children.Count - 1;
				modCommon.Write($"{num2} item found...\r");
				enumerator = kValue2["items"].Children.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						KValue current5 = enumerator.Current;
						if (!(current5.Name == "default"))
						{
							DotaItem dotaItem = new DotaItem
							{
								DefIndex = Convert.ToUInt32(current5.Name),
								ImageInventory = (current5["image_inventory"].AsString() ?? ""),
								Name = (current5["name"].AsString() ?? "").Trim()
							};
							List<KValue>.Enumerator enumerator2;
							if (current5.ContainsKey("item_name") || current5.ContainsKey("item_description"))
							{
                                string text = current5.ContainsKey("item_name") ? current5["item_name"].AsString().Substring(1) : "";
								string text2 = current5.ContainsKey("item_description") ? current5["item_description"].AsString().Substring(1) : "";
								enumerator2 = list.GetEnumerator();
								try
								{
									while (enumerator2.MoveNext())
									{
										KValue current6 = enumerator2.Current;
										string key2 = current6["Language"].AsString().ToLower();
										KValue k = current6["Tokens"];
										string value3 = string.IsNullOrEmpty(text) ? "" : k.GetKey(text).AsString();
										string value4 = string.IsNullOrEmpty(text2) ? "" : k.GetKey(text2).AsString();
										dotaItem.Names.Add(key2, value3);
										dotaItem.Descriptions.Add(key2, value4);
									}
								}
								finally
								{
									((IDisposable)enumerator2).Dispose();
								}
							}
							string text3 = "";
							if (dotaItem.Name.ToLower().Contains("default"))
							{
								dotaItem.IsDefault = true;
							}
							if (current5.ContainsKey("item_quality"))
							{
								text3 = current5["item_quality"].AsString();
							}
							bool flag2 = false;
							if (current5.ContainsKey("prefab"))
							{
								string text4 = current5["prefab"].AsString();
								if (kValue2.ContainsKey("prefabs") && kValue2["prefabs"].ContainsKey(text4))
								{
									text3 = kValue2["prefabs"][text4]["item_quality"].AsString();
								}
								if (text4 == "default_item")
								{
									dotaItem.IsDefault = true;
								}
								flag2 = (text4 == "tool");
							}
							dotaItem.Quality = text3;
							dotaItem.QualityId = (qualities.ContainsKey(text3) ? qualities[text3] : 0);
							if (current5.ContainsKey("default_drop_quantity"))
							{
								dotaItem.DefaultDropQuantity = current5["default_drop_quantity"].AsUnsignedInt(0u);
							}
							if (current5.HasKey("price_info"))
							{
								KValue key3 = current5.GetKey("price_info/price");
								if (key3 != KValue.Invalid)
								{
									dotaItem.Price = key3.AsUnsignedInt(0u);
								}
							}
							KValue key4 = current5.GetKey("shards_purchase_price");
							if (key4 != KValue.Invalid)
							{
								dotaItem.Price = key4.AsUnsignedInt(0u);
							}
							bool flag3;
							if (flag3 = current5.ContainsKey("static_attributes"))
							{
								enumerator2 = current5["static_attributes"].Children.GetEnumerator();
								try
								{
									while (enumerator2.MoveNext())
									{
										KValue current7 = enumerator2.Current;
										if (dictionary.ContainsKey(current7.Name))
										{
											uint defIndex = dictionary[current7.Name];
											string s = current7.ContainsKey("value") ? current7["value"].AsString() : "";
											KValue kValue3 = kValue2["attributes"][defIndex.ToString()];
											ItemStaticAttribute itemStaticAttribute = new ItemStaticAttribute
											{
												DefIndex = defIndex
											};
											if (kValue3.ContainsKey("attribute_type"))
											{
												switch (kValue3["attribute_type"].AsString().ToLower())
												{
												case "socket":
												case "string":
													itemStaticAttribute.ValueBytes = Encoding.UTF8.GetBytes(s);
													break;
												case "uint64":
													if (ulong.TryParse(s, out ulong result3))
													{
														itemStaticAttribute.ValueBytes = BitConverter.GetBytes(result3);
													}
													break;
												case "uint32":
													if (uint.TryParse(s, out uint result2))
													{
														itemStaticAttribute.Value = result2;
													}
													break;
												}
											}
											if (kValue3.ContainsKey("stored_as_integer") && uint.TryParse(s, out uint result4))
											{
												if (kValue3["stored_as_integer"].AsString() == "1")
												{
													itemStaticAttribute.Value = result4;
												}
												else
												{
													itemStaticAttribute.ValueBytes = BitConverter.GetBytes(result4);
												}
											}
											dotaItem.StaticAttributes.Add(itemStaticAttribute);
										}
									}
								}
								finally
								{
									((IDisposable)enumerator2).Dispose();
								}
							}
							if (current5.ContainsKey("visuals") && current5["visuals"].ContainsKey("styles"))
							{
								uint num3 = 0u;
								enumerator2 = current5["visuals"]["styles"].Children.GetEnumerator();
								try
								{
									while (enumerator2.MoveNext())
									{
										KValue current8 = enumerator2.Current;
										if (uint.TryParse(current8.Name, out uint result5))
										{
											dotaItem.Styles.Add(result5);
											if (result5 != 0 && current8.HasKey("unlock/gem"))
											{
												num3 += (uint)Math.Pow(2.0, (double)result5);
											}
										}
									}
								}
								finally
								{
									((IDisposable)enumerator2).Dispose();
								}
								ItemStaticAttribute item = new ItemStaticAttribute
								{
									DefIndex = 400,
									ValueBytes = BitConverter.GetBytes(num3)
								};
								dotaItem.StaticAttributes.Add(item);
							}
							if (flag && flag3)
							{
								KValue key5 = current5.GetKey("static_attributes/treasure loot list/value");
								if (key5 != KValue.Invalid)
								{
									dotaItem.IsTreasure = true;
									if (Lootdictionary.TryGetValue(key5.AsString(), out LootItem value5))
									{
										dotaItem.CratedDefIndexes.AddRange(value5.DefIndexes);
										dotaItem.CratedAdditionalDropDefIndexes.AddRange(value5.ChanceDefIndexes);
										dotaItem.MaxTreasureCount = (uint)(value5.DefIndexes.Count + value5.ChanceDefIndexes.Count);
									}
								}
							}
							if (current5.ContainsKey("bundle"))
							{
								dotaItem.IsBundle = true;
								enumerator2 = current5.GetKey("bundle").Children.GetEnumerator();
								try
								{
									while (enumerator2.MoveNext())
									{
										KValue current9 = enumerator2.Current;
										if (dictionary3.TryGetValue(current9.Name, out uint value6))
										{
											dotaItem.BundleDefIndexes.Add(value6);
										}
									}
								}
								finally
								{
									((IDisposable)enumerator2).Dispose();
								}
							}
							if (flag2 && current5.HasKey("tool/type"))
							{
								string text5 = current5.GetKey("tool/type").AsString();
								dotaItem.IsTool = true;
								dotaItem.ToolType = text5;
								switch (text5)
								{
								case "backpack_expander":
								{
									KValue key13 = current5.GetKey("tool/usage/backpack_slots");
									if (key13 != KValue.Invalid)
									{
										dotaItem.ToolGrantBackpackSlots = key13.AsUnsignedInt(0u);
									}
									break;
								}
								case "style_unlock":
								{
									KValue key8 = current5.GetKey("tool/usage/style");
									KValue key9 = current5.GetKey("tool/usage/item_def");
									if (key8 != KValue.Invalid && key9 != KValue.Invalid)
									{
										dotaItem.UnlockStyles.Add(new ItemStyleUnlock
										{
											Style = key8.AsUnsignedInt(0u),
											DefIndex = key9.AsUnsignedInt(0u)
										});
									}
									KValue key10 = current5.GetKey("tool/usage/unlocks");
									if (key10 != KValue.Invalid)
									{
										enumerator2 = key10.Children.GetEnumerator();
										try
										{
											while (enumerator2.MoveNext())
											{
												KValue current10 = enumerator2.Current;
												KValue key11 = current10.GetKey("style");
												KValue key12 = current10.GetKey("item_def");
												if (key11 != KValue.Invalid && key12 != KValue.Invalid)
												{
													dotaItem.UnlockStyles.Add(new ItemStyleUnlock
													{
														Style = key11.AsUnsignedInt(0u),
														DefIndex = key12.AsUnsignedInt(0u)
													});
												}
											}
										}
										finally
										{
											((IDisposable)enumerator2).Dispose();
										}
									}
									break;
								}
								case "hero_statue_forge":
								{
									KValue key6 = current5.GetKey("tool/usage/effigy_itemdef_index");
									if (key6 != KValue.Invalid)
									{
										dotaItem.ToolStatueForgeDefIndex = key6.AsUnsignedInt(0u);
									}
									KValue key7 = current5.GetKey("tool/usage/status_effect");
									if (key7 != KValue.Invalid)
									{
										dotaItem.ToolStatueForgeStatusEffect = key7.AsUnsignedInt(0u);
									}
									break;
								}
								}
							}
							//mongoDbCollection_1.Collection.InsertOne(dotaItem, null, default(CancellationToken));
							num++;
							if (num % 1000 == 0)
							{
								modCommon.Write($"{num} / {num2} item generated...\r");
							}
						}
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				modCommon.WriteLine($"{num} items generated.");
            }*/
        }

    }
}
