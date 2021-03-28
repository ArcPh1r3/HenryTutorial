using HenryMod.SkillStates.BaseStates;
using UnityEngine;

namespace HenryMod.SkillStates.Henry.Shotgun
{
    public class ShotgunBlastEntry : BaseHenrySkillState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            if (this.isGrounded)
            {
                this.outer.SetNextState(new ShotgunBlastUp());
                return;
            }
            else
            {
                this.outer.SetNextState(new ShotgunBlastBack());
                return;
            }
        }
    }
}