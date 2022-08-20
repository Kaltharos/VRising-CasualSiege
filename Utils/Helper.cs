using ProjectM;
using ProjectM.Network;
using ProjectM.Scripting;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

namespace CasualSiege.Utils
{
    public static class Helper
    {
        public static ServerGameManager SGM = default;

        public static bool GetServerGameManager(out ServerGameManager sgm)
        {
            sgm = Plugin.Server.GetExistingSystem<ServerScriptMapper>()?._ServerGameManager;
            return true;
        }

        public static int GetAllies(Entity CharacterEntity, out Dictionary<Entity, Entity> Group)
        {
            Group = new();

            Team team = Helper.SGM._TeamChecker.GetTeam(CharacterEntity);
            int AlliedUsersCount = Helper.SGM._TeamChecker.GetAlliedUsersCount(team);
            if (AlliedUsersCount <= 1) return 0;

            NativeList<Entity> allyBuffer = Helper.SGM._TeamChecker.GetTeamsChecked();
            Helper.SGM._TeamChecker.GetAlliedUsers(team, allyBuffer);

            foreach (var userEntity in allyBuffer)
            {
                if (Plugin.Server.EntityManager.HasComponent<User>(userEntity))
                {
                    Entity characterEntity = Plugin.Server.EntityManager.GetComponentData<User>(userEntity).LocalCharacter._Entity;
                    if (characterEntity.Equals(CharacterEntity)) continue;
                    Group[userEntity] = characterEntity;
                }
            }

            return AlliedUsersCount - 1;
        }

        public static void CreatePlayerCache()
        {
            Cache.PlayerCache.Clear();
            var userEntities = Plugin.Server.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<User>()).ToEntityArray(Allocator.Temp);
            foreach (var userEntity in userEntities)
            {
                var userData = Plugin.Server.EntityManager.GetComponentData<User>(userEntity);
                PlayerData playerData = new PlayerData(userData.CharacterName, userData.PlatformId, userData.IsConnected, userEntity, userData.LocalCharacter._Entity);
                Cache.PlayerCache[userEntity] = playerData;
            }
        }

        public static void UpdatePlayerCache(Entity userEntity, bool forceOffline = false)
        {
            var userData = Plugin.Server.EntityManager.GetComponentData<User>(userEntity);
            if (userData.CharacterName.IsEmpty) return;
            if (forceOffline)
            {
                userData.IsConnected = false;
            }
            PlayerData playerData = new PlayerData(userData.CharacterName, userData.PlatformId, userData.IsConnected, userEntity, userData.LocalCharacter._Entity);
            Cache.PlayerCache[userEntity] = playerData;
        }

        public static PrefabGUID GetPrefabGUID(Entity entity)
        {
            var entityManager = Plugin.Server.EntityManager;
            PrefabGUID guid;
            try
            {
                guid = entityManager.GetComponentData<PrefabGUID>(entity);
            }
            catch
            {
                guid.GuidHash = 0;
            }
            return guid;
        }

        public static string GetPrefabName(PrefabGUID hashCode)
        {
            var s = Plugin.Server.GetExistingSystem<PrefabCollectionSystem>();
            string name = "Nonexistent";
            if (hashCode.GuidHash == 0)
            {
                return name;
            }
            try
            {
                name = s.PrefabNameLookupMap[hashCode].ToString();
            }
            catch
            {
                name = "NoPrefabName";
            }
            return name;
        }
    }
}