using System;
using HenryMod.Modules.Misc;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace HenryMod.Modules.Components
{
    public class EnergyHUD : MonoBehaviour
    {
        public GameObject energyGauge;
        public Image energyFill;

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

                    if (this.energyGauge)
                    {
                        this.energyGauge.gameObject.SetActive(true);

                        float _fillAmount = energyComponent.currentEnergy / energyComponent.maxEnergy;
                        this.energyFill.fillAmount = _fillAmount;
                    }
                }
                else
                {
                    if (this.energyGauge)
                    {
                        this.energyGauge.gameObject.SetActive(false);
                    }
                }
            }
        }

        private bool ShouldShow(GenericSkill skill)
        {
            return skill && skill.skillDef && skill.skillDef.skillName != "Disabled";
        }
    }
}