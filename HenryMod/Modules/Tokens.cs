using R2API;
using System;

namespace HenryMod.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            string prefix = HenryPlugin.developerPrefix + "_HENRY_BODY_";

            string desc = "Henry<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Tip 1." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Tip 2." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Tip 3." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Tip 4." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so he left, bottom text.";

            LanguageAPI.Add(prefix + "NAME", "Henry");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "The Chosen One");
            LanguageAPI.Add(prefix + "LORE", "sample lore");
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);


            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            LanguageAPI.Add(prefix + "TYPHOON_SKIN_NAME", "Vergil");

            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Henry passive");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "Sample text.");

            LanguageAPI.Add(prefix + "PRIMARY_SLASH_NAME", "Sword");
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_DESCRIPTION", Helpers.agilePrefix + $"Swing forward for <style=cIsDamage>{100f * 3.5f}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_PUNCH_NAME", "Boxing Gloves");
            LanguageAPI.Add(prefix + "PRIMARY_PUNCH_DESCRIPTION", Helpers.agilePrefix + $"Punch rapidly for <style=cIsDamage>{100f * 2.4f}% damage</style>. <style=cIsUtility>Ignores armor.</style>");

            LanguageAPI.Add(prefix + "SECONDARY_GUN_NAME", "Pistol");
            LanguageAPI.Add(prefix + "SECONDARY_GUN_DESCRIPTION", Helpers.agilePrefix + $"Fire your handgun for <style=cIsDamage>{100f * SkillStates.Shoot.damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "SECONDARY_STINGER_NAME", "Stinger");
            LanguageAPI.Add(prefix + "SECONDARY_STINGER_DESCRIPTION", Helpers.agilePrefix + $"Lunge at en enemy for <style=cIsDamage>{100f * SkillStates.Stinger.Stinger.damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "UTILITY_ROLL_NAME", "Roll");
            LanguageAPI.Add(prefix + "UTILITY_ROLL_DESCRIPTION", "Roll a short distance, gaining <style=cIsUtility>300 armor</style>. <style=cIsUtility>You cannot be hit during the roll.</style>");

            LanguageAPI.Add(prefix + "SPECIAL_BOMB_NAME", "Bomb");
            LanguageAPI.Add(prefix + "SPECIAL_BOMB_DESCRIPTION", $"Throw a bomb for <style=cIsDamage>{100f * SkillStates.ThrowBomb.damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "UNLOCKABLE_ACHIEVEMENT_NAME", "Prelude");
            LanguageAPI.Add(prefix + "UNLOCKABLE_ACHIEVEMENT_DESC", "Enter Titanic Plains.");
            LanguageAPI.Add(prefix + "UNLOCKABLE_UNLOCKABLE_NAME", "Prelude");

            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Henry: Mastery");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As Henry, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Henry: Mastery");

            LanguageAPI.Add(prefix + "TYPHOONUNLOCKABLE_ACHIEVEMENT_NAME", "Henry: Grand Mastery");
            LanguageAPI.Add(prefix + "TYPHOONUNLOCKABLE_ACHIEVEMENT_DESC", "As Henry, beat the game or obliterate on Typhoon.");
            LanguageAPI.Add(prefix + "TYPHOONUNLOCKABLE_UNLOCKABLE_NAME", "Henry: Grand Mastery");
        }
    }
}