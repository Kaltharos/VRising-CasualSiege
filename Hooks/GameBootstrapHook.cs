using CasualSiege.Utils;
using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using Stunlock.Network;
using Unity.Collections;

namespace CasualSiege.Hooks
{
    [HarmonyPatch(typeof(GameBootstrap), nameof(GameBootstrap.Start))]
    public static class GameBootstrapStart_Patch
    {
        public static void Postfix()
        {
            Plugin.Initialize();
        }
    }

    [HarmonyPatch(typeof(SettingsManager), nameof(SettingsManager.VerifyServerGameSettings))]
    public class ServerGameSetting_Patch
    {
        private static bool isInitialized = false;
        public static void Postfix()
        {
            if (isInitialized == false)
            {
                Plugin.Initialize();
                isInitialized = true;
            }
        }
    }

    [HarmonyPatch(typeof(ServerBootstrapSystem), nameof(ServerBootstrapSystem.OnUserConnected))]
    public static class OnUserConnected_Patch
    {
        public static void Postfix(ServerBootstrapSystem __instance, NetConnectionId netConnectionId)
        {
            try
            {
                var userIndex = __instance._NetEndPointToApprovedUserIndex[netConnectionId];
                var serverClient = __instance._ApprovedUsersLookup[userIndex];

                Helper.UpdatePlayerCache(serverClient.UserEntity);
            }
            catch { }
        }
    }

    [HarmonyPatch(typeof(ServerBootstrapSystem), nameof(ServerBootstrapSystem.OnUserDisconnected))]
    public static class OnUserDisconnected_Patch
    {
        private static void Prefix(ServerBootstrapSystem __instance, NetConnectionId netConnectionId, ConnectionStatusChangeReason connectionStatusReason, string extraData)
        {
            try
            {
                var userIndex = __instance._NetEndPointToApprovedUserIndex[netConnectionId];
                var serverClient = __instance._ApprovedUsersLookup[userIndex];

                Helper.UpdatePlayerCache(serverClient.UserEntity, true);
            }
            catch { };
        }
    }

    [HarmonyPatch(typeof(Destroy_TravelBuffSystem), nameof(Destroy_TravelBuffSystem.OnUpdate))]
    public class Destroy_TravelBuffSystem_Patch
    {
        private static PrefabGUID birthCoffin = new PrefabGUID(722466953);
        private static void Postfix(Destroy_TravelBuffSystem __instance)
        {
            if (__instance.__OnUpdate_LambdaJob0_entityQuery != null)
            {
                var entities = __instance.__OnUpdate_LambdaJob0_entityQuery.ToEntityArray(Allocator.Temp);
                foreach (var entity in entities)
                {
                    PrefabGUID GUID = __instance.EntityManager.GetComponentData<PrefabGUID>(entity);
                    if (GUID.Equals(birthCoffin))
                    {
                        var Owner = __instance.EntityManager.GetComponentData<EntityOwner>(entity).Owner;
                        if (!__instance.EntityManager.HasComponent<PlayerCharacter>(Owner)) return;

                        var userEntity = __instance.EntityManager.GetComponentData<PlayerCharacter>(Owner).UserEntity._Entity;

                        Helper.UpdatePlayerCache(userEntity);
                    }
                }
            }
        }
    }
}
