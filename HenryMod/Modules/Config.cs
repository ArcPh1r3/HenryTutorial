using BepInEx.Configuration;
using UnityEngine;

namespace HenryMod.Modules
{
    public static class Config
    {
        public static void ReadConfig()
        {

        }

        // this helper automatically makes config entries for disabling survivors
        internal static ConfigEntry<bool> CharacterEnableConfig(string characterName)
        {
            return HenryPlugin.instance.Config.Bind<bool>(new ConfigDefinition(characterName, "Enabled"), true, new ConfigDescription("Set to false to disable this character"));
        }

        internal static ConfigEntry<bool> EnemyEnableConfig(string characterName)
        {
            return HenryPlugin.instance.Config.Bind<bool>(new ConfigDefinition(characterName, "Enabled"), true, new ConfigDescription("Set to false to disable this enemy"));
        }
    }
}