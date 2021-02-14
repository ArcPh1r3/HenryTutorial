using UnityEngine;
using EntityStates;

namespace HenryMod.SkillStates
{
    public class HenryMain : GenericCharacterMain
    {
        private Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            this.animator = base.GetModelAnimator();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.animator)
            {
                float i = 1;
                if (this.animator.GetBool("isGrounded")) i = 0;
                this.animator.SetFloat("inAir", i);
            }
        }
    }
}