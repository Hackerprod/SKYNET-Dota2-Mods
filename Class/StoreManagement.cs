
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Dota2
{

    public class StoreManagement
    {
        /*
		private static readonly ILog ilog_0;

		private static readonly Random random_0;


		private long long_0;


		private List<uint> list_0;


		private readonly List<uint> list_1;

		public List<uint> ConsumableDefIndexes
		{
			get;
			private set;
		}

		public List<uint> PlusSeasonalTerrainsDefIndexes
		{
			get;
		}


		public void ClearEvents()
		{
			//mongoDbCollection_2.Collection.DeleteMany(FilterDefinition<ClaimedEventPoints>.get_Empty(), default(CancellationToken));
			//mongoDbCollection_3.Collection.DeleteMany(FilterDefinition<PlayerEventPoints>.get_Empty(), default(CancellationToken));
		}

		public void FillConsumables()
		{
			//ConsumableDefIndexes = IAsyncCursorSourceExtensions.ToList<uint>(IFindFluentExtensions.Project<DotaItem, DotaItem, uint>(IMongoCollectionExtensions.Find<DotaItem>(mongoDbCollection_1.Collection, (Expression<Func<DotaItem, bool>>)((DotaItem c) => c.ToolType == "consumable_ability"), null), (Expression<Func<DotaItem, uint>>)((DotaItem i) => i.DefIndex)), default(CancellationToken));
		}

		public void AddEarnedItemFromCrate(ulong steamId, uint defindex)
		{
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Expected O, but got Unknown
			/*IMongoCollection<PlayerItem> collection = mongoDbCollection_0.Collection;
			Expression<Func<PlayerItem, bool>> obj = (PlayerItem pi) => pi.SteamId == steamId && pi.DefIndex == defindex;
			UpdateDefinition<PlayerItem> obj2 = UpdateDefinitionExtensions.Set<PlayerItem, uint>(mongoDbCollection_0.Ub.Set<ulong>((Expression<Func<PlayerItem, ulong>>)((PlayerItem p) => p.SteamId), steamId), (Expression<Func<PlayerItem, uint>>)((PlayerItem p) => p.DefIndex), defindex);
			UpdateOptions val = new UpdateOptions();
			val.set_IsUpsert(true);
			IMongoCollectionExtensions.UpdateOne<PlayerItem>(collection, obj, obj2, val, default(CancellationToken));
		}

		public bool CrateItem(ulong steamId, uint defindex, out uint peekItemDef, out List<uint> itemsEarned)
		{
			itemsEarned = new List<uint>();
			peekItemDef = 0u;
			/*List<uint> itemsEarnedDefIndexes = IAsyncCursorSourceExtensions.ToList<uint>(IFindFluentExtensions.Project<PlayerItem, PlayerItem, uint>(IMongoCollectionExtensions.Find<PlayerItem>(mongoDbCollection_0.Collection, (Expression<Func<PlayerItem, bool>>)((PlayerItem ps) => ps.SteamId == steamId), null), (Expression<Func<PlayerItem, uint>>)((PlayerItem p) => p.DefIndex)), default(CancellationToken));
			DotaItem dotaItem = IFindFluentExtensions.FirstOrDefault<DotaItem, DotaItem>(IMongoCollectionExtensions.Find<DotaItem>(mongoDbCollection_1.Collection, (Expression<Func<DotaItem, bool>>)((DotaItem d) => d.DefIndex == defindex), null), default(CancellationToken));
			if (dotaItem == null)
			{
				return false;
			}
			itemsEarned.AddRange(dotaItem.CratedDefIndexes);
			itemsEarned.AddRange(dotaItem.CratedAdditionalDropDefIndexes);
			itemsEarned.RemoveAll((uint i) => !itemsEarnedDefIndexes.Contains(i));
			List<uint> list = dotaItem.CratedAdditionalDropDefIndexes.FindAll((uint i) => itemsEarnedDefIndexes.Contains(i));
			if (list.Count > 0)
			{
				peekItemDef = list[0];
			}
			return true;
		}

		public bool EquipItems(ulong steamId, IEnumerable<CMsgAdjustItemEquippedState> itemsToEquip, out List<CSOEconItem> itemsChanged, out ulong cacheVersion)
		{
			itemsChanged = new List<CSOEconItem>();
			cacheVersion = 0uL;
			/*foreach (CMsgAdjustItemEquippedState item2 in itemsToEquip)
			{
				itemsChanged.AddRange(CMsgTalentWinRates3_0.Cache.ClearItemEquippedState(steamId, item2.new_slot, item2.new_class));
				if (item2.item_id != 0L)
				{
					if (!CMsgTalentWinRates3_0.Cache.TryGetPlayerItemById(steamId, item2.item_id, out CSOEconItem item))
					{
						return false;
					}
					item.style = ((item2.style_index != 255) ? item2.style_index : 0);
					item.equipped_state.Clear();
					item.equipped_state.Add(new CSOEconItemEquipped
					{
						new_slot = item2.new_slot,
						new_class = item2.new_class
					});
					itemsChanged.Add(item);
					CMsgTalentWinRates3_0.Cache.UpdatePlayerItemEquipState(steamId, item2.item_id, item.style, item.equipped_state);
					CSODOTAGameAccountPlus gameAccountPlus;
					if (PlusSeasonalTerrainsDefIndexes.Contains(item.def_index) && CMsgTalentWinRates3_0.Cache.GetGameAccountPlus(steamId, out gameAccountPlus, out ulong _) && gameAccountPlus.plus_status != 0)
					{
						AddClaimedEvent(steamId, 19u, 0u, 0u, 40010u, 17u, 0uL);
						SendPlayerEventPoints(steamId, 19u);
					}
				}
			}
			cacheVersion = CMsgTalentWinRates3_0.Cache.UpdateServiceVersion(steamId, 1u);
			return true;
		}

		public ulong GetNewItemId()
		{
			return (ulong)Interlocked.Increment(ref long_0);
		}

		public bool GiveItem(ulong steamId, uint defindex, out CSOEconItem item, out ulong newCacheVersion)
		{
			newCacheVersion = 0uL;
			item = null;
			/*DotaItem dotaItem = IFindFluentExtensions.FirstOrDefault<DotaItem, DotaItem>(IMongoCollectionExtensions.Find<DotaItem>(mongoDbCollection_1.Collection, (Expression<Func<DotaItem, bool>>)((DotaItem i) => i.DefIndex == defindex), null), default(CancellationToken));
			if (dotaItem == null)
			{
				return false;
			}
			if (CMsgTalentWinRates3_0.Cache.TryGetPlayerItemByDefIndex(steamId, defindex, out CSOEconItem item2))
			{
				if (dotaItem.IsTreasure)
				{
					uint maxTreasureCount = dotaItem.MaxTreasureCount;
					item2.quantity++;
					item2.quantity = Math.Min(item2.quantity, maxTreasureCount);
				}
				else
				{
					item2.quantity = ((dotaItem.DefaultDropQuantity == 0) ? 1 : dotaItem.DefaultDropQuantity);
				}
				item = item2;
				CMsgTalentWinRates3_0.Cache.UpdatePlayerItemQuantity(steamId, item2.id, item2.quantity);
			}
			else
			{
				item = dotaItem.Item;
				if (item == null)
				{
					return false;
				}
				item.account_id = steamId.GetAccountId();
				item.id = GetNewItemId();
				CMsgTalentWinRates3_0.Cache.GivePlayerItem(steamId, item);
			}
			newCacheVersion = CMsgTalentWinRates3_0.Cache.UpdateServiceVersion(steamId, 1u);
			return true;
		}

		public void NotifyEconAccountChanged(ulong steamId, CSOEconGameAccountClient econAccount, ulong cacheVersion)
		{
			ClientGCMsgProtobuf<CMsgSOMultipleObjects> clientGCMsgProtobuf = new ClientGCMsgProtobuf<CMsgSOMultipleObjects>(26u)
			{
				Body = 
				{
					owner_soid = new CMsgSOIDOwner
					{
						id = steamId,
						type = 1
					},
					version = cacheVersion,
					service_id = 1
				}
			};
			clientGCMsgProtobuf.Body.objects_modified.Add(new CMsgSOMultipleObjects.SingleObject
			{
				type_id = 7,
				object_data = econAccount.Serialize()
			});
			////CMsgTalentWinRates3_0.Send(steamId, clientGCMsgProtobuf);
		}

		public void NotifyItemsCreated(ulong steamId, List<CSOEconItem> itemsAdded, ulong cacheVersion)
		{
	//		CMsgTalentWinRates3_0.Cache.InvalidatePlayerItemCache(steamId);
			foreach (CSOEconItem item in itemsAdded)
			{
				ClientGCMsgProtobuf<CMsgSOSingleObject> msg = new ClientGCMsgProtobuf<CMsgSOSingleObject>(21u)
				{
					Body = 
					{
						owner_soid = new CMsgSOIDOwner
						{
							id = steamId,
							type = 1
						},
						version = cacheVersion,
						service_id = 1,
						type_id = 1,
						object_data = item.Serialize()
					}
				};
	//			//CMsgTalentWinRates3_0.Send(steamId, msg);
			}
		}

		public void NotifyItemsModified(ulong steamId, List<CSOEconItem> itemsModified, ulong cacheVersion, bool notifyServer = false)
		{
	//		CMsgTalentWinRates3_0.Cache.InvalidatePlayerItemCache(steamId);
			foreach (CSOEconItem item in itemsModified)
			{
				ClientGCMsgProtobuf<CMsgSOSingleObject> msg = new ClientGCMsgProtobuf<CMsgSOSingleObject>(22u)
				{
					Body = 
					{
						owner_soid = new CMsgSOIDOwner
						{
							id = steamId,
							type = 1
						},
						version = cacheVersion,
						service_id = 1,
						type_id = 1,
						object_data = item.Serialize()
					}
				};
	//			//CMsgTalentWinRates3_0.Send(steamId, msg);
				Dota2Match match;
				if (notifyServer && CMsgTalentWinRates3_0.Matches.TryGetCurrentMatch(steamId, out match) && match.Server != null)
				{
	//				//CMsgTalentWinRates3_0.Send(match.Server.SteamId, msg);
				}
			}
		}

		public void NotifyItemsAdded(ulong steamId, List<byte[]> itemsAdded, ulong cacheVersion)
		{
	//		CMsgTalentWinRates3_0.Cache.InvalidatePlayerItemCache(steamId);
			ClientGCMsgProtobuf<CMsgSOMultipleObjects> clientGCMsgProtobuf = new ClientGCMsgProtobuf<CMsgSOMultipleObjects>(26u)
			{
				Body = 
				{
					owner_soid = new CMsgSOIDOwner
					{
						id = steamId,
						type = 1
					},
					version = cacheVersion,
					service_id = 1
				}
			};
			foreach (byte[] item in itemsAdded)
			{
				clientGCMsgProtobuf.Body.objects_added.Add(new CMsgSOMultipleObjects.SingleObject
				{
					object_data = item,
					type_id = 1
				});
			}
	//		//CMsgTalentWinRates3_0.Send(steamId, clientGCMsgProtobuf);
		}

		public void NotifyItemsAdded(ulong steamId, List<CSOEconItem> itemsAdded, ulong cacheVersion, bool notifyServer = false)
		{
	//		CMsgTalentWinRates3_0.Cache.InvalidatePlayerItemCache(steamId);
			ClientGCMsgProtobuf<CMsgSOMultipleObjects> clientGCMsgProtobuf = new ClientGCMsgProtobuf<CMsgSOMultipleObjects>(26u)
			{
				Body = 
				{
					owner_soid = new CMsgSOIDOwner
					{
						id = steamId,
						type = 1
					},
					version = cacheVersion,
					service_id = 1
				}
			};
			foreach (CSOEconItem item in itemsAdded)
			{
				clientGCMsgProtobuf.Body.objects_added.Add(new CMsgSOMultipleObjects.SingleObject
				{
					object_data = item.Serialize(),
					type_id = 1
				});
			}
	//		//CMsgTalentWinRates3_0.Send(steamId, clientGCMsgProtobuf);
			Dota2Match match;
			if (notifyServer && CMsgTalentWinRates3_0.Matches.TryGetCurrentMatch(steamId, out match) && match.Server != null)
			{
	//			//CMsgTalentWinRates3_0.Send(match.Server.SteamId, clientGCMsgProtobuf);
			}
		}

		public void NotifyItemsChanged(ulong steamId, List<CSOEconItem> itemsChanged, ulong cacheVersion, bool notifyServer = false)
		{
	//		CMsgTalentWinRates3_0.Cache.InvalidatePlayerItemCache(steamId);
			ClientGCMsgProtobuf<CMsgSOMultipleObjects> clientGCMsgProtobuf = new ClientGCMsgProtobuf<CMsgSOMultipleObjects>(26u)
			{
				Body = 
				{
					owner_soid = new CMsgSOIDOwner
					{
						id = steamId,
						type = 1
					},
					version = cacheVersion,
					service_id = 1
				}
			};
			List<CSOEconItem>.Enumerator enumerator = itemsChanged.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					CSOEconItem current = enumerator.Current;
					clientGCMsgProtobuf.Body.objects_modified.Add(new CMsgSOMultipleObjects.SingleObject
					{
						object_data = current.Serialize(),
						type_id = 1
					});
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
	//		//CMsgTalentWinRates3_0.Send(steamId, clientGCMsgProtobuf);
			Dota2Match match;
			if (notifyServer && CMsgTalentWinRates3_0.Matches.TryGetCurrentMatch(steamId, out match) && match.Server != null)
			{
				ClientGCMsgProtobuf<CMsgSOSingleObject> clientGCMsgProtobuf2 = new ClientGCMsgProtobuf<CMsgSOSingleObject>(21u)
				{
					Body = 
					{
						type_id = 1,
						owner_soid = new CMsgSOIDOwner
						{
							id = steamId,
							type = 1
						},
						version = cacheVersion,
						service_id = 1
					}
				};
				ClientGCMsgProtobuf<CMsgSOSingleObject> clientGCMsgProtobuf3 = new ClientGCMsgProtobuf<CMsgSOSingleObject>(23u)
				{
					Body = 
					{
						type_id = 1,
						owner_soid = new CMsgSOIDOwner
						{
							id = steamId,
							type = 1
						},
						version = cacheVersion,
						service_id = 1
					}
				};
				enumerator = itemsChanged.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						CSOEconItem current2 = enumerator.Current;
						if (ConsumableDefIndexes.IndexOf(current2.def_index) == -1)
						{
				/*			List<CSOEconItemEquipped> equipped_state = current2.equipped_state;
							if (equipped_state != null && equipped_state.Count == 0)
							{
								clientGCMsgProtobuf3.Body.object_data = new CSOEconItem
								{
									id = current2.id
								}.Serialize();
								//CMsgTalentWinRates3_0.Send(match.Server.SteamId, clientGCMsgProtobuf3);
							}
						}
						clientGCMsgProtobuf2.Body.object_data = current2.Serialize();
	//					//CMsgTalentWinRates3_0.Send(match.Server.SteamId, clientGCMsgProtobuf2);
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
	//			//CMsgTalentWinRates3_0.Send(match.Server.SteamId, clientGCMsgProtobuf);
			}
		}

		public void NotifyItemDestroyed(ulong steamId, CSOEconItem item, ulong cacheVersion, bool notifyServer = false)
		{
	//		CMsgTalentWinRates3_0.Cache.InvalidatePlayerItemCache(steamId);
			ClientGCMsgProtobuf<CMsgSOSingleObject> msg = new ClientGCMsgProtobuf<CMsgSOSingleObject>(23u)
			{
				Body = 
				{
					owner_soid = new CMsgSOIDOwner
					{
						id = steamId,
						type = 1
					},
					version = cacheVersion,
					service_id = 1,
					type_id = 1,
					object_data = item.Serialize()
				}
			};
	//		//CMsgTalentWinRates3_0.Send(steamId, msg);
			Dota2Match match;
			if (notifyServer && CMsgTalentWinRates3_0.Matches.TryGetCurrentMatch(steamId, out match) && match.Server != null)
			{
	//			//CMsgTalentWinRates3_0.Send(match.Server.SteamId, msg);
			}
		}

		public void NotifyItemsRemoved(ulong steamId, List<CSOEconItem> itemsRemoved, ulong cacheVersion, bool notifyServer = false)
		{
	//		CMsgTalentWinRates3_0.Cache.InvalidatePlayerItemCache(steamId);
			ClientGCMsgProtobuf<CMsgSOMultipleObjects> clientGCMsgProtobuf = new ClientGCMsgProtobuf<CMsgSOMultipleObjects>(26u)
			{
				Body = 
				{
					owner_soid = new CMsgSOIDOwner
					{
						id = steamId,
						type = 1
					},
					version = cacheVersion,
					service_id = 1
				}
			};
			foreach (CSOEconItem item in itemsRemoved)
			{
				clientGCMsgProtobuf.Body.objects_removed.Add(new CMsgSOMultipleObjects.SingleObject
				{
					object_data = item.Serialize(),
					type_id = 1
				});
			}
	//		//CMsgTalentWinRates3_0.Send(steamId, clientGCMsgProtobuf);
			Dota2Match match;
			if (notifyServer && CMsgTalentWinRates3_0.Matches.TryGetCurrentMatch(steamId, out match) && match.Server != null)
			{
	//			//CMsgTalentWinRates3_0.Send(match.Server.SteamId, clientGCMsgProtobuf);
			}
		}

		public bool PurchaseItem(ulong steamId, CGCStorePurchaseInit_LineItem lineItem, out CSOEconItem newItem, out ulong newCacheVersion)
		{
			newCacheVersion = 0uL;
			newItem = null;
	/*		DotaItem dotaItem = IFindFluentExtensions.FirstOrDefault<DotaItem, DotaItem>(IMongoCollectionExtensions.Find<DotaItem>(mongoDbCollection_1.Collection, (Expression<Func<DotaItem, bool>>)((DotaItem i) => i.DefIndex == lineItem.item_def_id), null), default(CancellationToken));
			if (dotaItem == null)
			{
				return false;
			}
			if (CMsgTalentWinRates3_0.Cache.TryGetPlayerItemByDefIndex(steamId, lineItem.item_def_id, out CSOEconItem item))
			{
				newItem = item;
				if (!dotaItem.IsTreasure)
				{
					newItem.quantity = 1u;
					newItem.attribute = dotaItem.Item.attribute;
					CMsgTalentWinRates3_0.Cache.UpdatePlayerItemAttributes(steamId, item.id, newItem.attribute);
				}
				else
				{
					uint maxTreasureCount = dotaItem.MaxTreasureCount;
					newItem.quantity += lineItem.quantity;
					newItem.quantity = Math.Min(item.quantity, maxTreasureCount);
				}
				CMsgTalentWinRates3_0.Cache.UpdatePlayerItemQuantity(steamId, item.id, newItem.quantity);
			}
			else
			{
				newItem = dotaItem.Item;
				newItem.account_id = steamId.GetAccountId();
				newItem.id = GetNewItemId();
				newItem.level = 1u;
				newItem.quantity = lineItem.quantity;
				CMsgTalentWinRates3_0.Cache.GivePlayerItem(steamId, newItem);
			}
	//		newCacheVersion = CMsgTalentWinRates3_0.Cache.UpdateServiceVersion(steamId, 1u);
			return true;
		}

		public ulong RemoveItemByDefIndex(ulong steamId, uint defindex)
		{
			/*CMsgTalentWinRates3_0.Cache.RemovePlayerItemByDefIndex(steamId, defindex);
			return 0;//CMsgTalentWinRates3_0.Cache.UpdateServiceVersion(steamId, 1u);
		}

		public ulong RemoveItemById(ulong steamId, ulong itemId)
		{
			//CMsgTalentWinRates3_0.Cache.RemovePlayerItemById(steamId, itemId);
            return 0;//CMsgTalentWinRates3_0.Cache.UpdateServiceVersion(steamId, 1u);
        }

		public ulong SetItemsPosition(ulong steamId, List<CMsgSetItemPositions.ItemPosition> itemPositions, out List<CSOEconItem> changedItems)
		{
			changedItems = new List<CSOEconItem>();
            /*foreach (CMsgSetItemPositions.ItemPosition itemPosition in itemPositions)
			{
				CSOEconItem playerItemById = CMsgTalentWinRates3_0.Cache.GetPlayerItemById(steamId, itemPosition.item_id);
				if (playerItemById != null)
				{
					playerItemById.inventory = itemPosition.position;
					changedItems.Add(playerItemById);
					CMsgTalentWinRates3_0.Cache.UpdatePlayerItemInventoryPosition(steamId, playerItemById.id, itemPosition.position);
				}
			}
            return 0;//CMsgTalentWinRates3_0.Cache.UpdateServiceVersion(steamId, 1u);
        }

		public bool UnlockCrate(ulong steamId, uint defindex, out uint peekItemDef)
		{
			peekItemDef = 0u;
			/*List<uint> itemsEarnedDefIndexes = IAsyncCursorSourceExtensions.ToList<uint>(IFindFluentExtensions.Project<PlayerItem, PlayerItem, uint>(IMongoCollectionExtensions.Find<PlayerItem>(mongoDbCollection_0.Collection, (Expression<Func<PlayerItem, bool>>)((PlayerItem ps) => ps.SteamId == steamId), null), (Expression<Func<PlayerItem, uint>>)((PlayerItem p) => p.DefIndex)), default(CancellationToken));
			DotaItem dotaItem = IFindFluentExtensions.FirstOrDefault<DotaItem, DotaItem>(IMongoCollectionExtensions.Find<DotaItem>(mongoDbCollection_1.Collection, (Expression<Func<DotaItem, bool>>)((DotaItem i) => i.DefIndex == defindex), null), default(CancellationToken));
			if (dotaItem == null)
			{
				return false;
			}
			List<uint> list = new List<uint>();
			list.AddRange(dotaItem.CratedDefIndexes);
			list.RemoveAll((uint i) => itemsEarnedDefIndexes.Contains(i));
			if (list.Count == 0)
			{
				list.AddRange(dotaItem.CratedAdditionalDropDefIndexes);
				list.RemoveAll((uint i) => itemsEarnedDefIndexes.Contains(i));
			}
			if (list.Count == 0)
			{
				List<uint> list2 = dotaItem.CratedAdditionalDropDefIndexes.FindAll((uint i) => itemsEarnedDefIndexes.Contains(i));
				if (list2.Count > 0)
				{
					peekItemDef = list2[0];
				}
				return true;
			}
			peekItemDef = ((1 == list.Count) ? list[0] : list[random_0.Next(0, list.Count - 1)]);
			return true;
		}

		private void method_0(ulong ulong_0, uint uint_0, CSOEconItem csoeconItem_0)
		{
			/*if (!CMsgTalentWinRates3_0.Cache.GetGameAccountPlus(ulong_0, out CSODOTAGameAccountPlus gameAccountPlus, out ulong version))
			{
				gameAccountPlus = CMsgTalentWinRates3_0.Cache.GetNewGameAccountPlus(ulong_0);
			}
			uint num3 = gameAccountPlus.prepaid_time_start = (gameAccountPlus.original_start_date = (uint)DateHelpers.DateTimeToUnixTime(DateTime.Now));
			gameAccountPlus.next_payment_date = 0u;
			gameAccountPlus.prepaid_time_balance += uint_0 * 86400;
			gameAccountPlus.plus_status = 1u;
			gameAccountPlus.steam_agreement_id = 0uL;
			gameAccountPlus.plus_flags = 8u;
			version = CMsgTalentWinRates3_0.Cache.SetGameAccountPlus(ulong_0, gameAccountPlus);
			ClientGCMsgProtobuf<CMsgSOMultipleObjects> msg = new ClientGCMsgProtobuf<CMsgSOMultipleObjects>(26u)
			{
				Body = 
				{
					owner_soid = new CMsgSOIDOwner
					{
						id = ulong_0,
						type = 1
					},
					version = version,
					service_id = 0,
					objects_modified = 
					{
						new CMsgSOMultipleObjects.SingleObject
						{
							object_data = gameAccountPlus.Serialize(),
							type_id = 2012
						}
					}
				}
			};
			//CMsgTalentWinRates3_0.Send(ulong_0, msg);
			ulong cacheVersion = RemoveItemById(ulong_0, csoeconItem_0.id);
			NotifyItemsRemoved(ulong_0, new List<CSOEconItem>
			{
				csoeconItem_0
			}, cacheVersion);
			AddClaimedEvent(ulong_0, 19u, 0u, 0u, 40000u, 17u, 0uL);
			AddClaimedEvent(ulong_0, 19u, 0u, 0u, 41000u, 17u, 0uL);
			List<CSOEconItem> list = new List<CSOEconItem>();
			if (GiveItem(ulong_0, 12317u, out CSOEconItem item, out version))
			{
				list.Add(item);
			}
			int index = random_0.Next(0, PlusSeasonalTerrainsDefIndexes.Count - 1);
			if (GiveItem(ulong_0, PlusSeasonalTerrainsDefIndexes[index], out CSOEconItem item2, out version))
			{
				list.Add(item2);
			}
			if (list.Count > 0)
			{
				NotifyItemsCreated(ulong_0, list, version);
			}
			SendPlayerEventPoints(ulong_0, 19u);
		}

		public void SendPlayerEventPoints(ulong steamId, uint eventId)
		{
			ClientGCMsgProtobuf<CMsgDOTAGetEventPointsResponse> clientGCMsgProtobuf = new ClientGCMsgProtobuf<CMsgDOTAGetEventPointsResponse>(7388u)
			{
				Body = 
				{
					total_points = 0,
					total_premium_points = 0,
					event_id = 19,
					points = 0,
					premium_points = 0,
					account_id = steamId.GetAccountId(),
					owned = true,
					audit_action = 17
				}
			};
			List<ClaimedEventPoints> playerClaimedEventPoints = GetPlayerClaimedEventPoints(steamId, eventId);
			PlayerEventPoints playerEventPoints = GetPlayerEventPoints(steamId, eventId);
			uint num = 0u;
			uint num2 = 0u;
			foreach (ClaimedEventPoints item in playerClaimedEventPoints)
			{
				num2 += item.PremiumPoints;
				num += item.Points;
				CMsgDOTAGetEventPointsResponse.Action action = new CMsgDOTAGetEventPointsResponse.Action
				{
					action_id = item.ActionId
				};
				if (item.ActionId % 2u != 0)
				{
					action.times_completed = (uint)item.TimesCompleted;
				}
				clientGCMsgProtobuf.Body.completed_actions.Add(action);
			}
			clientGCMsgProtobuf.Body.total_points = num;
			clientGCMsgProtobuf.Body.total_premium_points = num2;
			clientGCMsgProtobuf.Body.points = playerEventPoints.Points;
			clientGCMsgProtobuf.Body.premium_points = playerEventPoints.PremiumPoints;
	//		//CMsgTalentWinRates3_0.Send(steamId, clientGCMsgProtobuf);
		}

		public bool UnpackBundle(ulong steamId, ulong itemId, out CSOEconItem bundle, out List<CSOEconItem> items, out ulong newCacheVersion)
		{
			newCacheVersion = 0uL;
			items = new List<CSOEconItem>();
			bundle = CMsgTalentWinRates3_0.Cache.GetPlayerItemById(steamId, itemId);
			/*if (bundle == null)
			{
				return false;
			}
			uint bundleDefIndex = bundle.def_index;
			DotaItem dotaItem = IFindFluentExtensions.FirstOrDefault<DotaItem, DotaItem>(IMongoCollectionExtensions.Find<DotaItem>(mongoDbCollection_1.Collection, (Expression<Func<DotaItem, bool>>)((DotaItem i) => i.DefIndex == bundleDefIndex), null), default(CancellationToken));
			if (dotaItem == null)
			{
				return false;
			}
			foreach (CSOEconItem item2 in method_1(dotaItem.BundleDefIndexes))
			{
				if (CMsgTalentWinRates3_0.Cache.TryGetPlayerItemByDefIndex(steamId, item2.def_index, out CSOEconItem item))
				{
					CMsgTalentWinRates3_0.Cache.UpdatePlayerItemQuantity(steamId, item.id, item2.quantity);
					CMsgTalentWinRates3_0.Cache.UpdatePlayerItemAttributes(steamId, item.id, item2.attribute);
				}
				else
				{
					item2.account_id = steamId.GetAccountId();
					item2.id = GetNewItemId();
					items.Add(item2);
					CMsgTalentWinRates3_0.Cache.GivePlayerItem(steamId, item2);
				}
			}
			newCacheVersion = CMsgTalentWinRates3_0.Cache.UpdateServiceVersion(steamId, 1u);
			return true;
		}

		public ulong UpdateItem(ulong steamId, ulong itemId, Action<CSOEconItem> updateAction)
		{
            /*CSOEconItem playerItemById = CMsgTalentWinRates3_0.Cache.GetPlayerItemById(steamId, itemId);
			if (playerItemById == null)
			{
				return 0uL;
			}
			updateAction?.Invoke(playerItemById);
			CMsgTalentWinRates3_0.Cache.UpdatePlayerItemById(steamId, playerItemById.id, playerItemById);
            return 0; //CMsgTalentWinRates3_0.Cache.UpdateServiceVersion(steamId, 1u);
        }

		public bool UseItem(ulong steamId, ulong itemId)
		{
			/*CSOEconItem item = CMsgTalentWinRates3_0.Cache.GetPlayerItemById(steamId, itemId);
			if (item == null)
			{
				return false;
			}
			DotaItem dotaItem = IFindFluentExtensions.FirstOrDefault<DotaItem, DotaItem>(IMongoCollectionExtensions.Find<DotaItem>(mongoDbCollection_1.Collection, (Expression<Func<DotaItem, bool>>)((DotaItem i) => i.DefIndex == item.def_index), null), default(CancellationToken));
			if (dotaItem != null)
			{
				switch (item.def_index)
				{
				case 19990u:
					method_0(steamId, 3u, item);
					return true;
				case 19992u:
					method_0(steamId, 7u, item);
					return true;
				default:
					if (dotaItem.IsTool)
					{
						string toolType = dotaItem.ToolType;
						if (toolType == "style_unlock")
						{
							ulong cacheVersion = 0uL;
							List<CSOEconItem> modifiedItems = new List<CSOEconItem>();
							foreach (ItemStyleUnlock unlockStyle in dotaItem.UnlockStyles)
							{
								CSOEconItem playerItemByDefIndex = CMsgTalentWinRates3_0.Cache.GetPlayerItemByDefIndex(steamId, unlockStyle.DefIndex);
								if (playerItemByDefIndex != null)
								{
									cacheVersion = UpdateItem(steamId, playerItemByDefIndex.id, delegate(CSOEconItem sitem)
									{
										sitem.style = unlockStyle.Style;
										CSOEconItemAttribute gClass = sitem.attribute.Find((CSOEconItemAttribute a) => a.def_index == 400);
										uint num = (uint)Math.Pow(2.0, (double)unlockStyle.Style);
										if (gClass == null)
										{
											sitem.attribute.Add(new CSOEconItemAttribute
											{
												def_index = 400,
												value_bytes = BitConverter.GetBytes(num)
											});
										}
										else
										{
											uint num2 = BitConverter.ToUInt32(gClass.value_bytes, 0);
											if ((num2 & num) != num)
											{
												gClass.value_bytes = BitConverter.GetBytes(num2 + num);
											}
										}
										modifiedItems.Add(sitem);
									});
								}
							}
							List<CSOEconItem> list = new List<CSOEconItem>();
							CSOEconItem playerItemById = CMsgTalentWinRates3_0.Cache.GetPlayerItemById(steamId, itemId);
							if (playerItemById != null)
							{
								ulong cacheVersion2 = RemoveItemById(steamId, itemId);
								list.Add(playerItemById);
								NotifyItemsRemoved(steamId, list, cacheVersion2);
							}
							NotifyItemsModified(steamId, modifiedItems, cacheVersion);
							return true;
						}
					}
					return false;
				case 19994u:
				case 19995u:
					method_0(steamId, 365u, item);
					return true;
				case 19996u:
				case 19997u:
					method_0(steamId, 184u, item);
					return true;
				case 19998u:
				case 19999u:
					method_0(steamId, 31u, item);
					return true;
				}
			}
			return false;
		}

		private List<CSOEconItem> method_1(List<uint> list_2)
		{
			/*FilterDefinition<DotaItem> val = Builders<DotaItem>.get_Filter().In<uint>((Expression<Func<DotaItem, uint>>)((DotaItem i) => i.DefIndex), (IEnumerable<uint>)list_2);
			return (from i in IAsyncCursorSourceExtensions.ToList<DotaItem>(IMongoCollectionExtensions.Find<DotaItem>(mongoDbCollection_1.Collection, val, null), default(CancellationToken))
			select i.Item).ToList();
            return null; //
		}

		private LootItem method_2(Dictionary<string, uint> dictionary_0, KValue kvalue_0, KValue kvalue_1, KValue kvalue_2)
		{
			LootItem lootItem = new LootItem
			{
				LootName = kvalue_2.Name
			};
			foreach (KValue child in kvalue_2.Children)
			{
				uint value;
				switch (child.Name)
				{
				default:
					if (kvalue_1.HasKey(child.Name))
					{
						LootItem lootItem3 = method_2(dictionary_0, kvalue_0, kvalue_1, kvalue_1.GetKey(child.Name));
						lootItem.DefIndexes.AddRange(lootItem3.DefIndexes);
						lootItem.DefIndexes.AddRange(lootItem3.ChanceDefIndexes);
					}
					else if (dictionary_0.TryGetValue(child.Name, out value))
					{
						lootItem.DefIndexes.Add(value);
					}
					break;
				case "escalating_chance_drop":
				case "escalating_chance_drop_by_rarity":
				{
					KValue key2 = child.GetKey("item");
					if (key2 != KValue.Invalid && dictionary_0.TryGetValue(key2.AsString(), out value))
					{
						lootItem.ChanceDefIndexes.Add(value);
					}
					break;
				}
				case "additional_drop":
				{
					KValue key = child.GetKey("loot_list");
					KValue key2 = child.GetKey("item");
					if (key != KValue.Invalid)
					{
						KValue key3 = kvalue_0.GetKey("loot_lists/" + key.AsString());
						if (key3 != KValue.Invalid)
						{
							LootItem lootItem2 = method_2(dictionary_0, kvalue_0, kvalue_1, key3);
							lootItem.ChanceDefIndexes.AddRange(lootItem2.DefIndexes);
							lootItem.ChanceDefIndexes.AddRange(lootItem2.ChanceDefIndexes);
						}
					}
					if (key2 != KValue.Invalid && dictionary_0.TryGetValue(key2.AsString(), out value))
					{
						lootItem.ChanceDefIndexes.Add(value);
					}
					break;
				}
				}
			}
			return lootItem;
		}
        */
        public void CreateItemMatrix(string basePath)
        {
            /*
            Dictionary<string, uint> dictionary = new Dictionary<string, uint>();
            Dictionary<string, uint> dictionary2 = new Dictionary<string, uint>();
            Dictionary<string, uint> dictionary3 = new Dictionary<string, uint>();
            Dictionary<string, LootItem> dictionary4 = new Dictionary<string, LootItem>();
            string path = Path.Combine("data", "db", "items_game.txt");
            string[] files = Directory.GetFiles(Path.Combine(basePath, "db"), "dota_lang_*");
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
            using (KValue kValue2 = KValue.LoadAsText(path))
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
                            dictionary2.Add(current.Name, current["value"].AsUnsignedInt(0u));
                        }
                    }
                    finally
                    {
                        ((IDisposable)enumerator).Dispose();
                    }
                    Console.Write($"{dictionary2.Count} item qualities found...\r");
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
                    Console.Write($"{dictionary.Count} item attributes found...\r");
                }
                //mongoDbCollection_1.Collection.DeleteMany(FilterDefinition<DotaItem>.get_Empty(), default(CancellationToken));
                bool flag = false;
                Console.Write("Indexing item names...\r");
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
                Console.Write("Generating loot lists...\r");
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
                            //LootItem value2 = method_2(dictionary3, kValue2, key, current4);
                            //dictionary4[current4.Name] = value2;
                        }
                    }
                    finally
                    {
                        ((IDisposable)enumerator).Dispose();
                    }
                }
                Console.Write("Loot list generated...\r");
                int num = 0;
                int num2 = kValue2["items"].Children.Count - 1;
                Console.Write($"{num2} item found...\r");
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
                            dotaItem.QualityId = (dictionary2.ContainsKey(text3) ? dictionary2[text3] : 0);
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
                                    if (dictionary4.TryGetValue(key5.AsString(), out LootItem value5))
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
                                Console.Write($"{num} / {num2} item generated...\r");
                            }
                        }
                    }
                }
                finally
                {
                    ((IDisposable)enumerator).Dispose();
                }
                Console.WriteLine($"{num} items generated.");
            }*/
        }
    }
}
