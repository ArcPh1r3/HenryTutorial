using EntityStates;
using HenryMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.Nemry.Torrent
{
    public class TorrentEntry : BaseNemrySkillState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            this.SpendEnergy(10);

            if (this.isGrounded)
            {
                this.outer.SetNextState(new TorrentGround());
                return;
            }
            else
            {
                this.outer.SetNextState(new TorrentAir());
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}