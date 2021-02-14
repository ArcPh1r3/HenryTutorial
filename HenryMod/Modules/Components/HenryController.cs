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
            if (this.characterBody.skillLocator.primary.skillDef.skillNameToken == HenryPlugin.developerPrefix + "_HENRY_BODY_PRIMARY_PUNCH_NAME")
            {
                this.childLocator.FindChild("SwordModel").gameObject.SetActive(false);
                this.childLocator.FindChild("BoxingGloveL").gameObject.SetActive(true);
                this.childLocator.FindChild("BoxingGloveR").gameObject.SetActive(true);
            }
        }
    }
}