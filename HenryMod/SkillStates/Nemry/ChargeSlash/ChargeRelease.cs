using EntityStates;
using HenryMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.Nemry.ChargeSlash
{
    public class ChargeRelease : BaseNemrySkillState
    {
        public float charge;

        public override void OnEnter()
        {
            base.OnEnter();
            base.characterBody.isSprinting = true;
            if (base.cameraTargetParams) base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
            base.characterBody.hideCrosshair = false;

            EntityStateMachine desiredStateMachine = this.outer;
            foreach (EntityStateMachine i in base.gameObject.GetComponents<EntityStateMachine>())
            {
                if (i)
                {
                    if (i.customName == "Weapon")
                    {
                        desiredStateMachine = i;
                    }
                }
            }

            if (!this.tracker.GetTrackingTarget())
            {
                if (this.isGrounded)
                {
                    Uppercut nextState = new Uppercut
                    {
                        charge = this.charge
                    };
                    desiredStateMachine.SetNextState(nextState);
                    this.outer.SetNextStateToMain();
                    return;
                }
                else
                {
                    Downslash nextState = new Downslash
                    {
                        charge = this.charge
                    };
                    desiredStateMachine.SetNextState(nextState);
                    this.outer.SetNextStateToMain();
                    return;
                }
            }

            float desiredDist = 10f;
            if (!this.isGrounded) desiredDist = 7f;

            float dist = Vector3.Distance(base.transform.position, this.tracker.GetTrackingTarget().transform.position);
            if (dist <= desiredDist)
            {
                if (this.isGrounded)
                {
                    Uppercut nextState = new Uppercut
                    {
                        charge = this.charge
                    };
                    desiredStateMachine.SetNextState(nextState);
                    this.outer.SetNextStateToMain();
                    return;
                }
                else
                {
                    Downslash nextState = new Downslash
                    {
                        charge = this.charge
                    };
                    desiredStateMachine.SetNextState(nextState);
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
            else
            {
                Lunge nextState = new Lunge
                {
                    charge = this.charge
                };
                desiredStateMachine.SetNextState(nextState);
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}