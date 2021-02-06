using BepInEx.Configuration;

namespace HenryMod.Modules
{
    public static class Config
    {
        public static void ReadConfig()
        {
            // there actually isn't any config right now but if you wanted to add some it would go here.
        }

        // this helper automatically makes config entries for disabling survivors
        internal static ConfigEntry<bool> CharacterEnableConfig(string characterName)
        {
            return HenryPlugin.instance.Config.Bind<bool>(new ConfigDefinition(characterName, "Enabled"), true, new ConfigDescription("Set to false to disable this character"));
        }
    }
}