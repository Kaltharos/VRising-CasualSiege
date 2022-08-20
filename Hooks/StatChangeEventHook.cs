using HarmonyLib;
using ProjectM;
using ProjectM.CastleBuilding;
using CasualSiege.Utils;
using Unity.Entities;
using System;

namespace CasualSiege.Hooks
{
    [HarmonyPatch(typeof(StatChangeSystem), nameof(StatChangeSystem.ApplyHealthChangeToEntity))]
    public class StatChangeSystem_Patch
    {
        private static void Prefix(StatChangeSystem __instance, ref StatChangeEvent statChange)
        {
            if (!Plugin.EnableMod.Value) return;

            if (!__instance.EntityManager.HasComponent<CastleHeartConnection>(statChange.Entity)) return;
            var HeartEntity = __instance.EntityManager.GetComponentData<CastleHeartConnection>(statChange.Entity).CastleHeartEntity._Entity;

            if (!__instance.EntityManager.HasComponent<Pylonstation>(HeartEntity)) return;
            var CastleHeart = __instance.EntityManager.GetComponentData<Pylonstation>(HeartEntity);

            if (CastleHeart.State != PylonstationState.Processing) return;

            if (!Cache.PlyonOwnerCache.TryGetValue(HeartEntity, out Entity userEntity))
            {
                userEntity = __instance.EntityManager.GetComponentData<UserOwner>(HeartEntity).Owner._Entity;
            }
            Cache.PlayerCache.TryGetValue(userEntity, out var playerData);

            if (playerData.IsConnected == false)
            {
                if (Plugin.FactorAllies.Value)
                {
                    var playerAllies = GetAllies(playerData.CharEntity);
                    if (playerAllies.AllyCount > 0)
                    {
                        foreach (var ally in playerAllies.Allies)
                        {
                            Cache.PlayerCache.TryGetValue(ally.Key, out var allyData);
                            if (allyData.IsConnected == true) return;
                        }
                    }
                }

                statChange.Change = 0;
                return;
            }
        }

        private static PlayerGroup GetAllies(Entity characterEntity)
        {
            if (Cache.AlliesCache.TryGetValue(characterEntity, out var playerGroup))
            {
                TimeSpan CacheAge = DateTime.Now - playerGroup.TimeStamp;
                if (CacheAge.TotalSeconds > Plugin.MaxAllyCacheAge.Value) goto UpdateCache;
                goto ReturnResult;
            }

            UpdateCache:
            int allyCount = Helper.GetAllies(characterEntity, out var Group);
            playerGroup = new PlayerGroup()
            {
                AllyCount = allyCount,
                Allies = Group,
                TimeStamp = DateTime.Now
            };
            Cache.AlliesCache[characterEntity] = playerGroup;

            ReturnResult:
            return playerGroup;
        }
    }
}
