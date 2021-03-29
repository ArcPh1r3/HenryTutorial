using UnityEngine;

namespace HenryMod.SkillStates.Henry.Shotgun
{
    public class ShotgunBlastDown : BaseShotgunBlast
    {
        public override void OnEnter()
        {
            this.animString = "ShotgunBlastDown";
            this.aimDirection = Vector3.up;
            this.pushForce = Vector3.down * BaseShotgunBlast.basePushForce;

            base.OnEnter();
        }
    }
}