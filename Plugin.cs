using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using Unity.Entities;
using UnityEngine;
using CasualSiege;
using CasualSiege.Utils;

#if WETSTONE
using Wetstone.API;
#endif

[assembly: AssemblyVersion(BuildConfig.Version)]
[assembly: AssemblyTitle(BuildConfig.Name)]

namespace CasualSiege
{
    [BepInPlugin(BuildConfig.PackageID, BuildConfig.Name, BuildConfig.Version)]

#if WETSTONE
    [BepInDependency("xyz.molenzwiebel.wetstone")]
    [Reloadable]
    public class Plugin : BasePlugin, IRunOnInitialized
#else
    public class Plugin : BasePlugin
#endif
    {
        private Harmony harmony;

        public static ConfigEntry<bool> EnableMod;
        public static ConfigEntry<bool> FactorAllies;
        public static ConfigEntry<int> MaxAllyCacheAge;

        public static bool isInitialized = false;

        public static ManualLogSource Logger;

        private static World _serverWorld;
        public static World Server
        {
            get
            {
                if (_serverWorld != null) return _serverWorld;

                _serverWorld = GetWorld("Server")
                    ?? throw new System.Exception("There is no Server world (yet). Did you install a server mod on the client?");
                return _serverWorld;
            }
        }

        public static bool IsServer => Application.productName == "VRisingServer";

        private static World GetWorld(string name)
        {
            foreach (var world in World.s_AllWorlds)
            {
                if (world.Name == name)
                {
                    return world;
                }
            }

            return null;
        }

        public void InitConfig()
        {
            EnableMod = Config.Bind("Config", "Enable Mod", true, "Enable/disable the mod.");
            FactorAllies = Config.Bind("Config", "Factor in Ally Status", true, "Include the player allies online status before blocking siege.");
            MaxAllyCacheAge = Config.Bind("Config", "Max Ally Cache Age", 300, "Max age of the player allies cache in seconds.\n" +
                "If the cache age is older than specified, the cahce will be renewed.\n" +
                "Don't set this too short as allies gathering process can slightly impact your server performance.\n" +
                "This cache is only for allies gathering, their online/offline status is updated instantly.");
        }

        public override void Load()
        {
            InitConfig();
            Logger = Log;
            harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            Log.LogInfo($"Plugin {BuildConfig.Name}-v{BuildConfig.Version} is loaded!");
        }

        public override bool Unload()
        {
            Config.Clear();
            harmony.UnpatchSelf();
            return true;
        }

        public static void Initialize()
        {
            //-- Always Re-Initialize
            Helper.GetServerGameManager(out Helper.SGM);
            Helper.CreatePlayerCache();

            //-- Initialize Only Once
            if (!isInitialized)
            {
                isInitialized = true;
            }
        }

        public void OnGameInitialized()
        {
            Initialize();
        }
    }
}
