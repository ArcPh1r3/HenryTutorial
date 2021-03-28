using HenryMod.Modules.Misc;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace HenryMod.Modules.Components
{
    internal class CustomPlayerCharacterMasterController : NetworkBehaviour
    {
        private PlayerCharacterMasterController playerCharacterMasterController;
        private CustomInputBank customInputBank;
        private bool hasChecked;

        public void Awake()
        {
            this.playerCharacterMasterController = this.GetComponent<PlayerCharacterMasterController>();
            this.hasChecked = false;
        }

        public void FixedUpdate()
        {
            if (!this.hasChecked && this.playerCharacterMasterController.body)
            {
                this.hasChecked = true;
                if (this.playerCharacterMasterController.body.baseNameToken != Modules.Enemies.Nemry.characterPrefab.GetComponent<CharacterBody>().baseNameToken)
                {
                    Destroy(this);
                    return;
                }
            }

            if (!this.customInputBank || !this.playerCharacterMasterController.hasEffectiveAuthority)
            {
                return;
            }

            bool swapSkillState = false;

            if (PlayerCharacterMasterController.CanSendBodyInput(playerCharacterMasterController.networkUser, out _, out var inputPlayer, out _))
            {
                swapSkillState = inputPlayer.GetButton(RewiredActions.WeaponSwapSkill);
            }

            this.customInputBank.weaponSwapSkill.PushState(swapSkillState);
        }

        internal static void SetBodyOverrideHook(On.RoR2.PlayerCharacterMasterController.orig_SetBody orig, PlayerCharacterMasterController self, GameObject newBody)
        {
            orig(self, newBody);

            var extraMaster = self.GetComponent<CustomPlayerCharacterMasterController>();
            if (!extraMaster)
            {
                return;
            }

            extraMaster.customInputBank = self.body ? self.body.GetComponent<CustomInputBank>() : null;
        }
    }
}
