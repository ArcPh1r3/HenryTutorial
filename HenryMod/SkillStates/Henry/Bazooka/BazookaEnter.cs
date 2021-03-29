using EntityStates;
using HenryMod.Modules.Components;
using HenryMod.SkillStates.BaseStates;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace HenryMod.SkillStates.Bazooka
{
    public class BazookaEnter : BaseHenrySkillState
    {
        public static float baseDuration = 0.8f;
        public static SkillDef fireDef = Modules.Survivors.Henry.bazookaFireSkillDef;
        public static SkillDef cancelDef = Modules.Survivors.Henry.bazookaCancelSkillDef;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = BazookaEnter.baseDuration / this.attackSpeedStat;
            this.henryController.hasBazookaReady = true;

            if (NetworkServer.active) base.characterBody.AddBuff(RoR2Content.Buffs.Slow50);

            base.PlayAnimation("RightArm, Override", "BufferEmpty");
            base.PlayAnimation("LeftArm, Override", "BufferEmpty");
            base.PlayAnimation("Gesture, Override", "BufferEmpty");
            base.PlayAnimation("Bazooka, Override", "BazookaAim", "Bazooka.playbackRate", this.duration);
            Util.PlaySound("HenryBazookaEquip", base.gameObject);

            base.skillLocator.primary.SetSkillOverride(base.skillLocator.primary, BazookaEnter.fireDef, GenericSkill.SkillOverridePriority.Contextual);
            base.skillLocator.secondary.SetSkillOverride(base.skillLocator.secondary, BazookaEnter.cancelDef, GenericSkill.SkillOverridePriority.Contextual);
            base.skillLocator.special.SetSkillOverride(base.skillLocator.utility, BazookaEnter.cancelDef, GenericSkill.SkillOverridePriority.Contextual);
            base.skillLocator.utility.SetSkillOverride(base.skillLocator.special, BazookaEnter.cancelDef, GenericSkill.SkillOverridePriority.Contextual);

            if (base.cameraTargetParams) base.cameraTargetParams.aimMode = CameraTargetParams.AimType.OverTheShoulder;

            this.henryController.UpdateCrosshair();
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
            return InterruptPriority.Frozen;
        }
    }
}