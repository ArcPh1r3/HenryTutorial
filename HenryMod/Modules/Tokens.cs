using R2API;
using System;

namespace HenryMod.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            string prefix = HenryPlugin.developerPrefix;

            string desc = "Henry<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Tip 1." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Tip 2." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Tip 3." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Tip 4." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so he left, bottom text.";

            LanguageAPI.Add(prefix + "_HENRY_BODY_NAME", "Henry");
            LanguageAPI.Add(prefix + "_HENRY_BODY_DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "_HENRY_BODY_SUBTITLE", "The Chosen One");
            LanguageAPI.Add(prefix + "_HENRY_BODY_LORE", "sample lore");
            LanguageAPI.Add(prefix + "_HENRY_BODY_OUTRO_FLAVOR", outro);


            LanguageAPI.Add(prefix + "_HENRY_BODY_DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "_HENRY_BODY_MASTERY_SKIN_NAME", "Alternate");

            LanguageAPI.Add(prefix + "_HENRY_BODY_PASSIVE_NAME", "Henry passive");
            LanguageAPI.Add(prefix + "_HENRY_BODY_PASSIVE_DESCRIPTION", "Sample text.");

            LanguageAPI.Add(prefix + "_HENRY_BODY_PRIMARY_SLASH_NAME", "Sword");
            LanguageAPI.Add(prefix + "_HENRY_BODY_PRIMARY_SLASH_DESCRIPTION", $"Swing forward for <style=cIsDamage>{100f * 3.5f}% damage</style>.");

            LanguageAPI.Add(prefix + "_HENRY_BODY_SECONDARY_GUN_NAME", "Pistol");
            LanguageAPI.Add(prefix + "_HENRY_BODY_SECONDARY_GUN_DESCRIPTION", $"Fire your handgun for <style=cIsDamage>{100f * SkillStates.Shoot.damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "_HENRY_BODY_UTILITY_ROLL_NAME", "Roll");
            LanguageAPI.Add(prefix + "_HENRY_BODY_UTILITY_ROLL_DESCRIPTION", "Roll a short distance, gaining <style=cIsUtility>300 armor</style>. <style=cIsUtility>You cannot be hit during the roll.</style>");

            LanguageAPI.Add(prefix + "_HENRY_BODY_SPECIAL_BOMB_NAME", "Bomb");
            LanguageAPI.Add(prefix + "_HENRY_BODY_SPECIAL_BOMB_DESCRIPTION", $"Throw a bomb for <style=cIsDamage>{100f * SkillStates.ThrowBomb.damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "_HENRY_BODY_UNLOCKABLE_ACHIEVEMENT_NAME", "Prelude");
            LanguageAPI.Add(prefix + "_HENRY_BODY_UNLOCKABLE_ACHIEVEMENT_DESC", "Enter Titanic Plains.");
            LanguageAPI.Add(prefix + "_HENRY_BODY_UNLOCKABLE_UNLOCKABLE_NAME", "Prelude");

            LanguageAPI.Add(prefix + "_HENRY_BODY_MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Henry: Mastery");
            LanguageAPI.Add(prefix + "_HENRY_BODY_MASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As Henry, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "_HENRY_BODY_MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Henry: Mastery");
        }
    }
}