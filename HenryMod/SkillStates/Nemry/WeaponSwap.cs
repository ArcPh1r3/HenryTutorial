using UnityEngine;
using EntityStates;
using RoR2;
using HenryMod.SkillStates.BaseStates;
using HenryMod.Modules.Components;
using RoR2.Skills;

namespace HenryMod.SkillStates.Nemry
{
    public class WeaponSwap : BaseNemrySkillState
    {
        public static float baseDuration = 0.4f;

        public static SkillDef gunPrimaryDef = Modules.Enemies.Nemry.gunPrimaryDef;
        public static SkillDef gunSecondaryDef = Modules.Enemies.Nemry.gunSecondaryDef;
        public static SkillDef gunUtilityDef = Modules.Enemies.Nemry.gunUtilityDef;
        public static SkillDef gunSpecialDef = Modules.Enemies.Nemry.gunSpecialDef;

        private float duration;
        private Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = WeaponSwap.baseDuration / this.attackSpeedStat;
            this.animator = base.GetModelAnimator();

            if (this.energyComponent.weaponMode == NemryEnergyComponent.WeaponMode.Sword)
            {
                base.PlayAnimation("Gesture, Override", "GunMode", "WeaponSwap.playbackRate", this.duration);
                Util.PlaySound("NemryEquipGun", base.gameObject);

                base.characterBody.crosshairPrefab = Modules.Assets.LoadCrosshair("Standard");

                base.skillLocator.primary.SetSkillOverride(base.skillLocator.primary, WeaponSwap.gunPrimaryDef, GenericSkill.SkillOverridePriority.Contextual);
                base.skillLocator.secondary.SetSkillOverride(base.skillLocator.secondary, WeaponSwap.gunSecondaryDef, GenericSkill.SkillOverridePriority.Contextual);
                base.skillLocator.utility.SetSkillOverride(base.skillLocator.utility, WeaponSwap.gunUtilityDef, GenericSkill.SkillOverridePriority.Contextual);
                base.skillLocator.special.SetSkillOverride(base.skillLocator.special, WeaponSwap.gunSpecialDef, GenericSkill.SkillOverridePriority.Contextual);

                this.energyComponent.weaponMode = NemryEnergyComponent.WeaponMode.Gun;
                this.animator.SetLayerWeight(this.animator.GetLayerIndex("Body, Gun"), 1f);
            }
            else
            {
                base.PlayAnimation("Gesture, Override", "SwordMode", "WeaponSwap.playbackRate", this.duration);
                Util.PlaySound("NemryEquipSword", base.gameObject);

                base.characterBody.crosshairPrefab = Modules.Assets.LoadCrosshair("SimpleDot");

                base.skillLocator.primary.UnsetSkillOverride(base.skillLocator.primary, WeaponSwap.gunPrimaryDef, GenericSkill.SkillOverridePriority.Contextual);
                base.skillLocator.secondary.UnsetSkillOverride(base.skillLocator.secondary, WeaponSwap.gunSecondaryDef, GenericSkill.SkillOverridePriority.Contextual);
                base.skillLocator.utility.UnsetSkillOverride(base.skillLocator.utility, WeaponSwap.gunUtilityDef, GenericSkill.SkillOverridePriority.Contextual);
                base.skillLocator.special.UnsetSkillOverride(base.skillLocator.special, WeaponSwap.gunSpecialDef, GenericSkill.SkillOverridePriority.Contextual);

                this.energyComponent.weaponMode = NemryEnergyComponent.WeaponMode.Sword;
                this.animator.SetLayerWeight(this.animator.GetLayerIndex("Body, Gun"), 0f);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}