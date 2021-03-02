using EntityStates;
using HenryMod.Modules.Components;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.Stinger
{
    public class StingerEntry : BaseSkillState
    {
        private HenryTracker tracker;

        public override void OnEnter()
        {
            base.OnEnter();
            this.tracker = base.GetComponent<HenryTracker>();

            if (!this.tracker.GetTrackingTarget())
            {
                this.activatorSkillSlot.AddOneStock();
                this.outer.SetNextStateToMain();
                return;
            }

            if (base.skillLocator.primary.skillDef.skillNameToken == HenryPlugin.developerPrefix + "_HENRY_BODY_PRIMARY_SLASH_NAME")
            {
                this.outer.SetNextState(new Stinger());
            }
            else
            {
                // i don't feel like coding the punch rn
                this.outer.SetNextState(new DashPunch());
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}