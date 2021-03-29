using EntityStates;
using HenryMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.Nemry.ChargeSlash
{
    public class ChargeEntry : BaseNemrySkillState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            EntityStateMachine desiredStateMachine = this.outer;
            foreach (EntityStateMachine i in base.gameObject.GetComponents<EntityStateMachine>())
            {
                if (i)
                {
                    if (i.customName == "Body")
                    {
                        desiredStateMachine = i;
                    }
                }
            }

            desiredStateMachine.SetNextState(new StartCharge());
            this.outer.SetNextStateToMain();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}