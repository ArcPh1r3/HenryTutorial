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

            string outro = "..and so he left, searching for a new identity.";
            string outroFailure = "..and so he vanished, forever a blank slate.";

            LanguageAPI.Add(prefix + "NAME", "Henry");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "The Chosen One");
            LanguageAPI.Add(prefix + "LORE", "sample lore");
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            LanguageAPI.Add(prefix + "TYPHOON_SKIN_NAME", "V1");
            LanguageAPI.Add(prefix + "DANTE_SKIN_NAME", "Dante");
            LanguageAPI.Add(prefix + "VERGIL_SKIN_NAME", "Vergil");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Henry passive");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "Sample text.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_NAME", "Sword");
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_DESCRIPTION", Helpers.agilePrefix + $"Swing forward for <style=cIsDamage>{100f * StaticValues.swordDamageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_PUNCH_NAME", "Boxing Gloves");
            LanguageAPI.Add(prefix + "PRIMARY_PUNCH_DESCRIPTION", Helpers.agilePrefix + $"Punch rapidly for <style=cIsDamage>{100f * StaticValues.boxingGlovesDamageCoefficient}% damage</style>. <style=cIsUtility>Ignores armor.</style>");

            LanguageAPI.Add(prefix + "PRIMARY_GUN_NAME", "Pistol");
            LanguageAPI.Add(prefix + "PRIMARY_GUN_DESCRIPTION", Helpers.agilePrefix + $"Fire a small pistol for <style=cIsDamage>{100f * StaticValues.pistolDamageCoefficient}% damage</style>.");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_GUN_NAME", "Handgun");
            LanguageAPI.Add(prefix + "SECONDARY_GUN_DESCRIPTION", Helpers.agilePrefix + $"Fire a handgun for <style=cIsDamage>{100f * StaticValues.gunDamageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "SECONDARY_STINGER_NAME", "Stinger");
            LanguageAPI.Add(prefix + "SECONDARY_STINGER_DESCRIPTION", Helpers.agilePrefix + $"Lunge at an enemy for <style=cIsDamage>{100f * StaticValues.stingerDamageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "SECONDARY_UZI_NAME", "Uzi");
            LanguageAPI.Add(prefix + "SECONDARY_UZI_DESCRIPTION", $"Fire an uzi for <style=cIsDamage>{100f * StaticValues.uziDamageCoefficient}% damage</style>.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_ROLL_NAME", "Roll");
            LanguageAPI.Add(prefix + "UTILITY_ROLL_DESCRIPTION", "Roll a short distance, gaining <style=cIsUtility>300 armor</style>. <style=cIsUtility>You cannot be hit during the roll.</style>");

            LanguageAPI.Add(prefix + "UTILITY_SHOTGUN_NAME", "Shotgun");
            LanguageAPI.Add(prefix + "UTILITY_SHOTGUN_DESCRIPTION", $"Fire a <style=cIsUtility>shotgun</style> for <style=cIsDamage>{StaticValues.shotgunBulletCount}x{100f * StaticValues.shotgunDamageCoefficient}% damage</style>, launching yourself with the recoil.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_BOMB_NAME", "Bomb");
            LanguageAPI.Add(prefix + "SPECIAL_BOMB_DESCRIPTION", $"Throw a bomb for <style=cIsDamage>{100f * StaticValues.bombDamageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "SPECIAL_SCEPBOMB_NAME", "More Bombs!");
            LanguageAPI.Add(prefix + "SPECIAL_SCEPBOMB_DESCRIPTION", $"Throw a bomb for <style=cIsDamage>{100f * StaticValues.bombDamageCoefficient}% damage</style>." + Helpers.ScepterDescription("Hold up to 4 bombs. Cooldown is halved."));

            LanguageAPI.Add(prefix + "SPECIAL_BAZOOKA_NAME", "Bazooka");
            LanguageAPI.Add(prefix + "SPECIAL_BAZOOKA_DESCRIPTION", $"Charge and fire rockets for <style=cIsDamage>{100f * StaticValues.bazookaMinDamageCoefficient}%-{100f * StaticValues.bazookaMaxDamageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_BAZOOKA_NAME", "Fire");
            LanguageAPI.Add(prefix + "PRIMARY_BAZOOKA_DESCRIPTION", $"Charge and fire a rocket for <style=cIsDamage>{100f * StaticValues.bazookaMinDamageCoefficient}%-{100f * StaticValues.bazookaMaxDamageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_BAZOOKAOUT_NAME", "Cancel");
            LanguageAPI.Add(prefix + "PRIMARY_BAZOOKAOUT_DESCRIPTION", "Put your <style=cIsDamage>bazooka</style> away.");

            LanguageAPI.Add(prefix + "SPECIAL_SCEPBAZOOKA_NAME", "Armageddon");
            LanguageAPI.Add(prefix + "SPECIAL_SCEPBAZOOKA_DESCRIPTION", $"Charge and fire rockets for <style=cIsDamage>{100f * StaticValues.bazookaMinDamageCoefficient}%-{100f * StaticValues.bazookaMaxDamageCoefficient}% damage</style>." + Helpers.ScepterDescription("Hold two Bazookas at once."));

            LanguageAPI.Add(prefix + "PRIMARY_SCEPBAZOOKA_NAME", "Fire");
            LanguageAPI.Add(prefix + "PRIMARY_SCEPBAZOOKA_DESCRIPTION", $"Charge and fire a rocket for <style=cIsDamage>{100f * StaticValues.bazookaMinDamageCoefficient}%-{100f * StaticValues.bazookaMaxDamageCoefficient}% damage</style>." + Helpers.ScepterDescription("Hold two Bazookas at once."));
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

            LanguageAPI.Add(prefix + "VERGILUNLOCKABLE_ACHIEVEMENT_NAME", "Henry: 200% Motivated");
            LanguageAPI.Add(prefix + "VERGILUNLOCKABLE_ACHIEVEMENT_DESC", "As Henry, defeat the King of Nothing with no items.");
            LanguageAPI.Add(prefix + "VERGILUNLOCKABLE_UNLOCKABLE_NAME", "Henry: 200% Motivated");
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

            #region Nemry
            prefix = HenryPlugin.developerPrefix + "_NEMRY_BODY_";

            desc = "Nemesis Henry is a volatile swordsman who wields a special blade that stores energy to boost its attacks. He has no cooldowns, but must manage his energy well.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc += "< ! > Sword is a good all-rounder while Boxing Gloves are better for laying a beatdown on more powerful foes." + Environment.NewLine + Environment.NewLine;
            desc += "< ! > Pistol is a powerful anti air, with its low cooldown and high damage." + Environment.NewLine + Environment.NewLine;
            desc += "< ! > Roll has a lingering armor buff that helps to use it aggressively." + Environment.NewLine + Environment.NewLine;
            desc += "< ! > Bomb can be used to wipe crowds with ease." + Environment.NewLine + Environment.NewLine;

            outro = "..and so he left, bottom text.";
            outroFailure = "..and so he left, bottom text.";

            LanguageAPI.Add(prefix + "NAME", "Nemesis Henry");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "The Fallen One");
            LanguageAPI.Add(prefix + "LORE", "sample lore");
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Nemesis");
            LanguageAPI.Add(prefix + "MASTERY_SKIN_NAME", "Captive #001");
            LanguageAPI.Add(prefix + "TYPHOON_SKIN_NAME", "FLLFFL");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Acidic Gunblade");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "Nemesis Henry wields a special weapon that can swap between Sword Mode and Gun Mode. Also, his skills don't have regular cooldowns, but instead rely on his sword's <style=cIsHealing>Energy</style> system.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_SPEAR_NAME", "Skewer");
            LanguageAPI.Add(prefix + "PRIMARY_SPEAR_DESCRIPTION", Helpers.agilePrefix + $"Stab forward for <style=cIsDamage>{100f * 1.6f}% damage</style>. <style=cIsDamage>Deals double damage and heals for <style=cIsHealing>50%</style> of damage dealt at the tip</style>.");

            LanguageAPI.Add(HenryPlugin.developerPrefix + "_WEAPON_SWAP_SKILL", "Weapon Swap");

            desc = $"Swing your blade forward for <style=cIsDamage>{100f * 2.8f}% damage</style>. Generate <style=cIsHealing>20% Energy</style> on hit.";
            LanguageAPI.Add(prefix + "PRIMARY_SWORD_NAME", "Sword Mode");
            LanguageAPI.Add(prefix + "PRIMARY_SWORD_DESCRIPTION", desc);

            desc = $"<style=cIsHealing>5% Energy.</style> Fire a piercing <style=cIsHealing>Energy</style> shot for <style=cIsDamage>{100f * SkillStates.Nemry.ShootGun.boostedDamageCoefficient}% damage</style>.";
            LanguageAPI.Add(prefix + "PRIMARY_GUN_NAME", "Gun Mode");
            LanguageAPI.Add(prefix + "PRIMARY_GUN_DESCRIPTION", desc);

            desc = $"Swing your blade for <style=cIsDamage>{100f * 2.8f}% damage</style>. Generate <style=cIsHealing>20% Energy</style> on hit.";
            desc += $"\n<style=cIsHealing>5% Energy.</style> Fire a piercing <style=cIsHealing>Energy</style> shot for <style=cIsDamage>{100f * SkillStates.Nemry.ShootGun.boostedDamageCoefficient}% damage</style>.";
            LanguageAPI.Add(prefix + "PRIMARY_SWORDCSS_NAME", "Sword Mode / Gun Mode");
            LanguageAPI.Add(prefix + "PRIMARY_SWORDCSS_DESCRIPTION", desc);
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_BLAST_NAME", "Null Blast");
            LanguageAPI.Add(prefix + "SECONDARY_BLAST_DESCRIPTION", Helpers.agilePrefix + $"Throw a blob of <style=cIsHealth>mysterious energy</style> for <style=cIsDamage>{100f * SkillStates.Nemry.VoidBlast.damageCoefficient}% damage</style>.");

            desc = $"<style=cIsHealing>25% Energy.</style> Lunge at a nearby enemy for <style=cIsDamage>{100f * SkillStates.Nemry.ChargeSlash.Lunge.minDamageCoefficient}% damage</style>. Perform a combo attack instead if within melee range.";
            LanguageAPI.Add(prefix + "SECONDARY_CHARGE_NAME", "Swordmaster");
            LanguageAPI.Add(prefix + "SECONDARY_CHARGE_DESCRIPTION", desc);

            desc = $"<style=cIsHealing>10% Energy.</style> Fire a barrage of <style=cIsHealing>Energy</style> shots. Hold to continue firing, consuming more <style=cIsHealing>Energy</style> per shot.";
            LanguageAPI.Add(prefix + "SECONDARY_TORRENT_NAME", "Gunslinger");
            LanguageAPI.Add(prefix + "SECONDARY_TORRENT_DESCRIPTION", desc);

            desc = $"<style=cIsHealing>25% Energy.</style> Lunge at a nearby enemy for <style=cIsDamage>{100f * SkillStates.Nemry.ChargeSlash.Lunge.minDamageCoefficient}% damage</style>.";
            desc += $"\n<style=cIsHealing>10% Energy.</style> Fire a barrage of <style=cIsHealing>Energy</style> shots.";
            LanguageAPI.Add(prefix + "SECONDARY_CHARGECSS_NAME", "Swordmaster / Gunslinger");
            LanguageAPI.Add(prefix + "SECONDARY_CHARGECSS_DESCRIPTION", desc);
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_DODGESLASH_NAME", "Swift Slash");
            LanguageAPI.Add(prefix + "UTILITY_DODGESLASH_DESCRIPTION", "Backflip, slashing nearby enemies for <style=cIsDamage>300% damage</style>. <style=cIsUtility>You cannot be hit during the flip.</style>");

            desc = "<style=cIsHealing>10% Energy.</style> <style=cIsUtility>Instantly blink</style> to a nearby enemy.";
            LanguageAPI.Add(prefix + "UTILITY_BLINK_NAME", "Flash Step");
            LanguageAPI.Add(prefix + "UTILITY_BLINK_DESCRIPTION", desc);

            desc = "<style=cIsHealing>20% Energy.</style> Discharge a burst of <style=cIsHealing>Energy</style> for <style=cIsDamage>400% damage</style>, <style=cIsUtility>launching yourself with the recoil</style>.";
            LanguageAPI.Add(prefix + "UTILITY_BURST_NAME", "Energy Burst");
            LanguageAPI.Add(prefix + "UTILITY_BURST_DESCRIPTION", desc);

            desc = "<style=cIsHealing>10% Energy.</style> <style=cIsUtility>Instantly blink</style> to a nearby enemy.";
            desc += "\n<style=cIsHealing>20% Energy.</style> Discharge a burst of <style=cIsHealing>Energy</style> for <style=cIsDamage>400% damage</style>.";
            LanguageAPI.Add(prefix + "UTILITY_BLINKCSS_NAME", "Flash Step / Energy Burst");
            LanguageAPI.Add(prefix + "UTILITY_BLINKCSS_DESCRIPTION", desc);
            #endregion

            #region Special
            desc = $"<style=cIsHealing>100% Energy.</style> Plunge your blade into an enemy for <style=cIsDamage>2800% damage</style>, inflicting a potent debuff. <style=cIsHealing>Heal for 50% of damage dealt.</style>";
            LanguageAPI.Add(prefix + "SPECIAL_STAB_NAME", "Hydra Bite");
            LanguageAPI.Add(prefix + "SPECIAL_STAB_DESCRIPTION", desc);

            desc = $"<style=cIsHealing>100% Energy.</style> Charge up a powerful beam that deals <style=cIsDamage>8x400% damage</style>.";
            LanguageAPI.Add(prefix + "SPECIAL_BEAM_NAME", "Hyper Beam");
            LanguageAPI.Add(prefix + "SPECIAL_BEAM_DESCRIPTION", desc);

            desc = $"<style=cIsHealing>100% Energy.</style> Plunge your blade into an enemy for <style=cIsDamage>2800% damage</style>, inflicting a potent debuff. <style=cIsHealing>Heal for 50% of damage dealt.</style>";
            desc += $"\n<style=cIsHealing>100% Energy.</style> Charge up a powerful beam that deals <style=cIsDamage>a lot of damage</style>.";
            LanguageAPI.Add(prefix + "SPECIAL_STABCSS_NAME", "Hydra Bite / Hyper Beam");
            LanguageAPI.Add(prefix + "SPECIAL_STABCSS_DESCRIPTION", desc);
            #endregion

            #region Achievements
            LanguageAPI.Add(prefix + "UNLOCKABLE_ACHIEVEMENT_NAME", "???");
            LanguageAPI.Add(prefix + "UNLOCKABLE_ACHIEVEMENT_DESC", "Defeat Henry's Vestige.");
            LanguageAPI.Add(prefix + "UNLOCKABLE_UNLOCKABLE_NAME", "???");

            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Nemesis Henry: Mastery");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As Nemesis Henry, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Nemesis Henry: Mastery");

            LanguageAPI.Add(prefix + "TYPHOONUNLOCKABLE_ACHIEVEMENT_NAME", "Nemesis Henry: Grand Mastery");
            LanguageAPI.Add(prefix + "TYPHOONUNLOCKABLE_ACHIEVEMENT_DESC", "As Nemesis Henry, beat the game or obliterate on Typhoon.");
            LanguageAPI.Add(prefix + "TYPHOONUNLOCKABLE_UNLOCKABLE_NAME", "Nemesis Henry: Grand Mastery");
            #endregion
            #endregion
        }
    }
}