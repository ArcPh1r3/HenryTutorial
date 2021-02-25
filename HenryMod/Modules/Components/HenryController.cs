using RoR2;
using UnityEngine;

namespace HenryMod.Modules.Components
{
    // just a class to run some custom code for things like weapon models
    public class HenryController : MonoBehaviour
    {
        private CharacterBody characterBody;
        private CharacterModel model;
        private ChildLocator childLocator;

        private void Awake()
        {
            this.characterBody = this.gameObject.GetComponent<CharacterBody>();
            this.childLocator = this.gameObject.GetComponentInChildren<ChildLocator>();
            this.model = this.GetComponentInChildren<CharacterModel>();

            Invoke("CheckWeapon", 0.2f);
        }

        private void CheckWeapon()
        {
            switch (this.characterBody.skillLocator.primary.skillDef.skillNameToken)
            {
                default:
                    this.childLocator.FindChild("SwordModel").gameObject.SetActive(true);
                    this.childLocator.FindChild("BoxingGloveL").gameObject.SetActive(false);
                    this.childLocator.FindChild("BoxingGloveR").gameObject.SetActive(false);
                    break;
                case HenryPlugin.developerPrefix + "_HENRY_BODY_PRIMARY_PUNCH_NAME":
                    this.childLocator.FindChild("SwordModel").gameObject.SetActive(false);
                    this.childLocator.FindChild("BoxingGloveL").gameObject.SetActive(true);
                    this.childLocator.FindChild("BoxingGloveR").gameObject.SetActive(true);
                    break;
            }

            if (this.characterBody.skillLocator.secondary.skillDef.skillNameToken == HenryPlugin.developerPrefix + "_HENRY_BODY_SECONDARY_STINGER_NAME")
            {
                this.childLocator.FindChild("GunModel").gameObject.SetActive(false);

                this.characterBody.gameObject.AddComponent<HenryTracker>();
                this.characterBody.crosshairPrefab = Resources.Load<GameObject>("Prefabs/Crosshair/SimpleDotCrosshair");
            }
        }
    }
}