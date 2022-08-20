using System;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Entities;

namespace CasualSiege.Utils
{
    public struct PlayerGroup
    {
        public int AllyCount { get; set; }
        public Dictionary<Entity, Entity> Allies { get; set; }
        public DateTime TimeStamp { get; set; }

        public PlayerGroup(int allyCount = 0, Dictionary<Entity, Entity> allies = default, DateTime timeStamp = default)
        {
            AllyCount = allyCount;
            Allies = allies;
            TimeStamp = timeStamp;
        }
    }

    public struct PlayerData
    {
        public FixedString64 CharacterName { get; set; }
        public ulong SteamID { get; set; }
        public bool IsConnected { get; set; }
        public Entity UserEntity { get; set; }
        public Entity CharEntity { get; set; }
        public PlayerData(FixedString64 characterName = default, ulong steamID = 0, bool isConnected = false, Entity userEntity = default, Entity charEntity = default)
        {
            CharacterName = characterName;
            SteamID = steamID;
            IsConnected = isConnected;
            UserEntity = userEntity;
            CharEntity = charEntity;
        }
    }
}
