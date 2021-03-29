using UnityEngine;

namespace HenryMod.SkillStates.Henry.Shotgun
{
    public class ShotgunBlastUp : BaseShotgunBlast
    {
        public override void OnEnter()
        {
            this.animString = "ShotgunBlastUp";
            this.aimDirection = Vector3.down;
            this.pushForce = Vector3.up * BaseShotgunBlast.basePushForce;

            base.OnEnter();
        }
    }
}