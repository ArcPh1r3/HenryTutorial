using EntityStates;
using HenryMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.Henry
{
    public class UziIdle : BaseHenrySkillState
    {
        private float duration;
        private bool hasReloaded;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = base.skillLocator.secondary.CalculateFinalRechargeInterval();
            this.hasReloaded = false;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= 0.6f * this.duration)
            {
                this.StartReload();
            }

            if (base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        private void StartReload()
        {
            if (this.hasReloaded) return;
            this.hasReloaded = true;
            base.PlayCrossfade("Gesture, Override", "ReloadUzi", 0.05f);
            Util.PlaySound("HenryUziReload", base.gameObject); 
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }
    }
}