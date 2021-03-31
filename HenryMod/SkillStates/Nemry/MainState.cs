using UnityEngine;
using EntityStates;
using RoR2;
using HenryMod.Modules.Components;

namespace HenryMod.SkillStates.Nemry
{
    public class MainState : HenryMain
    {
        private CustomInputBank customInputBank;

        public override void OnEnter()
        {
            base.OnEnter();
            this.customInputBank = base.gameObject.GetComponent<CustomInputBank>();
        }

        public override void Update()
        {
            base.Update();

            // weapon swap
            if (base.isAuthority && !this.localUser.isUIFocused && this.customInputBank)
            {
                if (this.customInputBank.weaponSwapSkill.down)
                {
                    this.weaponStateMachine.SetInterruptState(new WeaponSwap(), InterruptPriority.Any);
                    return;
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }
    }
}