using EntityStates;
using HenryMod.Modules.Components;
using HenryMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.Nemry.Stab
{
    public class StabSuccess : BaseNemrySkillState
    {
        public static float duration = 1.2f;

        private bool hasJumped;

        public override void OnEnter()
        {
            base.OnEnter();
            base.PlayAnimation("FullBody, Override", "StabSuccess", "Stab.playbackRate", 1.25f * StabSuccess.duration);
            this.hasJumped = false;

            this.SpendEnergy(100f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= 0.5f * StabSuccess.duration)
            {
                this.Jump();
            }

            if (base.isAuthority && base.fixedAge >= StabSuccess.duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        private void Jump()
        {
            if (this.hasJumped) return;
            this.hasJumped = true;

            EffectManager.SimpleMuzzleFlash(Modules.Assets.energyBurstEffect, base.gameObject, "Muzzle", false);

            if (base.isAuthority)
            {
                base.characterMotor.velocity = -base.characterDirection.forward * 45f;
                base.characterMotor.velocity.y = 15f;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}