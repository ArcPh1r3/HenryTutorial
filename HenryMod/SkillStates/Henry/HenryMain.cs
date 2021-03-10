using UnityEngine;
using EntityStates;
using RoR2;

namespace HenryMod.SkillStates
{
    public class HenryMain : GenericCharacterMain
    {
        private Animator animator;

        public LocalUser localUser;

        public override void OnEnter()
        {
            base.OnEnter();
            this.animator = base.GetModelAnimator();
            this.localUser = LocalUserManager.readOnlyLocalUsersList[0];
        }

        public override void Update()
        {
            base.Update();

            // emotes
            if (base.isAuthority && base.characterMotor.isGrounded && !this.localUser.isUIFocused)
            {
                if (Input.GetKeyDown(Modules.Config.restKeybind.Value))
                {
                    this.outer.SetInterruptState(EntityState.Instantiate(new SerializableEntityStateType(typeof(Emotes.Rest))), InterruptPriority.Any);
                    return;
                }
                else if (Input.GetKeyDown(Modules.Config.danceKeybind.Value))
                {
                    this.outer.SetInterruptState(EntityState.Instantiate(new SerializableEntityStateType(typeof(Emotes.Dance))), InterruptPriority.Any);
                    return;
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.animator)
            {
                // this is solely for the punch animation
                float i = 1;
                if (this.animator.GetBool("isGrounded")) i = 0;
                this.animator.SetFloat("inAir", i);

                // rest idle
                this.animator.SetBool("inCombat", (!base.characterBody.outOfCombat || !base.characterBody.outOfDanger));
            }
        }
    }
}