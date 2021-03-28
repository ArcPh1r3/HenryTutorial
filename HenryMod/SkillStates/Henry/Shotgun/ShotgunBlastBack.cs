using UnityEngine;

namespace HenryMod.SkillStates.Henry.Shotgun
{
    public class ShotgunBlastBack : BaseShotgunBlast
    {
        public override void OnEnter()
        {
            Ray aimRay = base.GetAimRay();
            this.aimDirection = aimRay.direction;
            this.pushForce = -aimRay.direction * BaseShotgunBlast.basePushForce;

            this.animString = "ShotgunBlastDown";

            base.OnEnter();
        }
    }
}