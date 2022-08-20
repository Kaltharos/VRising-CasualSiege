using System.Collections.Generic;
using Unity.Entities;

namespace CasualSiege.Utils
{
    public static class Cache
    {
        public static Dictionary<Entity, Entity> PlyonOwnerCache = new();
        public static Dictionary<Entity, PlayerData> PlayerCache = new();
        public static Dictionary<Entity, PlayerGroup> AlliesCache = new();
    }
}
