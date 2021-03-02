using R2API;
using System;

namespace HenryMod.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            #region Henry
            string prefix = HenryPlugin.developerPrefix + "_HENRY_BODY_";

            string desc = "Henry is a skilled fighter who makes use of a wide arsenal of weaponry to take down his foes.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Sword is a good all-rounder while Boxing Gloves are better for laying a beatdown on more powerful foes." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Pistol is a powerful anti air, with its low cooldown and high damage." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Roll has a lingering armor buff that helps to use it aggressively." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Bomb can be used to wipe crowds with ease." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so he left, bottom text.";

            LanguageAPI.Add(prefix + "NAME", "Henry");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "The Chosen One");
            LanguageAPI.Add(prefix + "LORE", "sample lore");
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            LanguageAPI.Add(prefix + "DANTE_SKIN_NAME", "Dante");
            LanguageAPI.Add(prefix + "TYPHOON_SKIN_NAME", "Vergil");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Henry passive");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "Sample text.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_NAME", "Sword");
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_DESCRIPTION", Helpers.agilePrefix + $"Swing forward for <style=cIsDamage>{100f * 3.5f}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_PUNCH_NAME", "Boxing Gloves");
            LanguageAPI.Add(prefix + "PRIMARY_PUNCH_DESCRIPTION", Helpers.agilePrefix + $"Punch rapidly for <style=cIsDamage>{100f * 2.8f}% damage</style>. <style=cIsUtility>Ignores armor.</style>");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_GUN_NAME", "Pistol");
            LanguageAPI.Add(prefix + "SECONDARY_GUN_DESCRIPTION", Helpers.agilePrefix + $"Fire your handgun for <style=cIsDamage>{100f * SkillStates.Shoot.damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "SECONDARY_STINGER_NAME", "Stinger");
            LanguageAPI.Add(prefix + "SECONDARY_STINGER_DESCRIPTION", Helpers.agilePrefix + $"Lunge at en enemy for <style=cIsDamage>{100f * SkillStates.Stinger.Stinger.damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "SECONDARY_UZI_NAME", "Uzi");
            LanguageAPI.Add(prefix + "SECONDARY_UZI_DESCRIPTION", $"Fire an uzi for <style=cIsDamage>{100f * SkillStates.ShootUzi.damageCoefficient}% damage</style>.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_ROLL_NAME", "Roll");
            LanguageAPI.Add(prefix + "UTILITY_ROLL_DESCRIPTION", "Roll a short distance, gaining <style=cIsUtility>300 armor</style>. <style=cIsUtility>You cannot be hit during the roll.</style>");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_BOMB_NAME", "Bomb");
            LanguageAPI.Add(prefix + "SPECIAL_BOMB_DESCRIPTION", $"Throw a bomb for <style=cIsDamage>{100f * SkillStates.ThrowBomb.damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "SPECIAL_SCEPBOMB_NAME", "More Bombs!");
            LanguageAPI.Add(prefix + "SPECIAL_SCEPBOMB_DESCRIPTION", $"Throw a bomb for <style=cIsDamage>{100f * SkillStates.ThrowBomb.damageCoefficient}% damage</style>." + Helpers.ScepterDescription("Hold up to 4 bombs. Cooldown is halved."));
            #endregion

            #region Achievements
            LanguageAPI.Add(prefix + "UNLOCKABLE_ACHIEVEMENT_NAME", "Prelude");
            LanguageAPI.Add(prefix + "UNLOCKABLE_ACHIEVEMENT_DESC", "Enter Titanic Plains.");
            LanguageAPI.Add(prefix + "UNLOCKABLE_UNLOCKABLE_NAME", "Prelude");

            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Henry: Mastery");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As Henry, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Henry: Mastery");

            LanguageAPI.Add(prefix + "TYPHOONUNLOCKABLE_ACHIEVEMENT_NAME", "Henry: Grand Mastery");
            LanguageAPI.Add(prefix + "TYPHOONUNLOCKABLE_ACHIEVEMENT_DESC", "As Henry, beat the game or obliterate on Typhoon.");
            LanguageAPI.Add(prefix + "TYPHOONUNLOCKABLE_UNLOCKABLE_NAME", "Henry: Grand Mastery");

            LanguageAPI.Add(prefix + "DANTEUNLOCKABLE_ACHIEVEMENT_NAME", "Henry: Jackpot");
            LanguageAPI.Add(prefix + "DANTEUNLOCKABLE_ACHIEVEMENT_DESC", "As Henry, defeat a Twisted Scavenger on Monsoon.");
            LanguageAPI.Add(prefix + "DANTEUNLOCKABLE_UNLOCKABLE_NAME", "Henry: Jackpot");
            #endregion
            #endregion

            #region MrGreen
            prefix = HenryPlugin.developerPrefix + "_MRGREEN_BODY_";

            desc = "Mr. Green<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Sword is a good all-rounder while Boxing Gloves are better for laying a beatdown on more powerful foes." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Pistol is a powerful anti air, with its low cooldown and high damage." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Roll has a lingering armor buff that helps to use it aggressively." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Bomb can be used to wipe crowds with ease." + Environment.NewLine + Environment.NewLine;

            outro = "..and so he left, bottom text.";

            LanguageAPI.Add(prefix + "NAME", "Mr. Green");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "The");
            LanguageAPI.Add(prefix + "LORE", "sample lore part 2");
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Mr. Green passive");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "Sample text.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_PUNCH_NAME", "Martial Arts");
            LanguageAPI.Add(prefix + "PRIMARY_PUNCH_DESCRIPTION", $"Perform a swift combo of punches and kicks for <style=cIsDamage>{100f * 0.9f}% damage</style>.");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_CLONETHROW_NAME", "Clone Throw");
            LanguageAPI.Add(prefix + "SECONDARY_CLONETHROW_DESCRIPTION", $"Summon a clone and throw it forward, grabbing nearby enemies and slamming them into the ground for <style=cIsDamage>{100f * SkillStates.Shoot.damageCoefficient}% damage</style>.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_DASH_NAME", "People's Elbow");
            LanguageAPI.Add(prefix + "UTILITY_DASH_DESCRIPTION", "Dash a short distance. Upon contact with a <style=cIsUtility>clone</style>, leap into the air and descend with a powerful <style=cIsUtility>elbow drop</style> for <style=cIsDamage>800% damage</style>.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_RES_NAME", "Resurrection");
            LanguageAPI.Add(prefix + "SPECIAL_RES_DESCRIPTION", $"Resurrect all fallen clones.");
            #endregion
            #endregion
        }
    }
}