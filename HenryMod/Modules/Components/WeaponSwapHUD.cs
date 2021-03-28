using System;
using HenryMod.Modules.Misc;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace HenryMod.Modules.Components
{
    public class WeaponSwapHUD : MonoBehaviour
    {
        public GameObject swapSkillIcon;

        private HUD hud;

        private void Awake()
        {
            this.hud = this.GetComponent<HUD>();
        }

        public void Update()
        {
            if (this.hud.targetBodyObject)
            {
                NemryEnergyComponent energyComponent = this.hud.targetBodyObject.GetComponent<NemryEnergyComponent>();
                if (energyComponent)
                {
                    PlayerCharacterMasterController masterController = this.hud.targetMaster ? this.hud.targetMaster.playerCharacterMasterController : null;

                    if (this.swapSkillIcon)
                    {
                        this.swapSkillIcon.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (this.swapSkillIcon)
                    {
                        this.swapSkillIcon.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}