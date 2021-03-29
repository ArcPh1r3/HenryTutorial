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
                float desiredDist = 10f;
                if (!this.isGrounded) desiredDist = 7f;

                float dist = Vector3.Distance(base.transform.position, this.tracker.GetTrackingTarget().transform.position);
                if (dist <= desiredDist)
                {
                    if (this.isGrounded)
                    {
                        this.outer.SetNextState(new Uppercut());
                        return;
                    }
                    else
                    {
                        this.outer.SetNextState(new AirSlam());
                        return;
                    }
                }
                else
                {
                    this.outer.SetNextState(new DashPunch());
                    return;
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}