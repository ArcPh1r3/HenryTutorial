using UnityEngine;
using EntityStates;
using RoR2;
using HenryMod.Modules.Components;
using HenryMod.SkillStates.Emotes;

namespace HenryMod.SkillStates
{
    public class HenryMain : GenericCharacterMain
    {
        private Animator animator;
        private HenryController henryController;
        protected EntityStateMachine weaponStateMachine;

        public LocalUser localUser;

        public override void OnEnter()
        {
            base.OnEnter();
            this.animator = base.GetModelAnimator();
            this.henryController = base.GetComponent<HenryController>();
            this.localUser = LocalUserManager.readOnlyLocalUsersList[0];

            foreach (EntityStateMachine i in base.gameObject.GetComponents<EntityStateMachine>())
            {
                if (i)
                {
                    if (i.customName == "Weapon")
                    {
                        this.weaponStateMachine = i;
                    }
                }
            }
        }

        public override void Update()
        {
            base.Update();

            // emotes
            if (base.isAuthority && base.characterMotor.isGrounded && !this.localUser.isUIFocused)
            {
                if (Input.GetKeyDown(Modules.Config.restKeybind.Value))
                {
                    this.outer.SetInterruptState(new Rest(), InterruptPriority.Any);
                    return;
                }
                else if (Input.GetKeyDown(Modules.Config.danceKeybind.Value))
                {
                    this.outer.SetInterruptState(new Dance(), InterruptPriority.Any);
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

            if (this.henryController)
            {
                this.animator.SetBool("inBazooka", this.henryController.hasBazookaReady);

                // bazooka stuff
                if (this.henryController.hasBazookaReady)
                {
                    base.characterBody.isSprinting = false;
                    base.StartAimMode(0.5f, false);
                }
            }
        }
    }
}