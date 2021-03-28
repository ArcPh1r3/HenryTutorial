using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace HenryMod.Modules.Components
{
    public class NemryEnergyComponent : MonoBehaviour
    {
        public float maxEnergy = 100f;
        public float currentEnergy;

        private float lastEnergy;
        private CharacterBody characterBody;
        private CharacterMaster characterMaster;
        private CharacterModel model;
        private ChildLocator childLocator;
        private HenryTracker tracker;
        private Animator modelAnimator;

        public enum WeaponMode
        {
            Sword,
            Gun
        };

        public WeaponMode weaponMode;

        private void Awake()
        {
            this.characterBody = this.gameObject.GetComponent<CharacterBody>();
            this.childLocator = this.gameObject.GetComponentInChildren<ChildLocator>();
            this.model = this.gameObject.GetComponentInChildren<CharacterModel>();
            this.tracker = this.gameObject.GetComponent<HenryTracker>();
            this.modelAnimator = this.gameObject.GetComponentInChildren<Animator>();
            this.weaponMode = WeaponMode.Sword;
            this.currentEnergy = this.maxEnergy;
        }

        private void Start()
        {
            this.characterMaster = this.characterBody.master;
        }

        public bool AddEnergy(float amount)
        {
            if (this.currentEnergy >= this.maxEnergy) return false;
            this.currentEnergy = Mathf.Clamp(this.currentEnergy + amount, 0f, this.maxEnergy);

            if (this.currentEnergy >= this.maxEnergy && this.lastEnergy < this.maxEnergy) Util.PlaySound("NemryMaxEnergy", this.gameObject);

            this.lastEnergy = this.currentEnergy;
            return true;
        }

        public bool SpendEnergy(float amount)
        {
            return SpendEnergy(amount, SkillSlot.None);
        }

        public bool SpendEnergy(float amount, SkillSlot skillSlot)
        {
            /*if (this.characterBody.HasBuff(BuffIndex.NoCooldowns)) return true;
            //
            int alienHeadCount = this.characterMaster.inventory.GetItemCount(ItemIndex.AlienHead);
            int purityCount = this.characterMaster.inventory.GetItemCount(ItemIndex.LunarBadLuck);
            int backupMagCount = this.characterMaster.inventory.GetItemCount(ItemIndex.SecondarySkillMagazine);
            int afterburnerCount = this.characterMaster.inventory.GetItemCount(ItemIndex.UtilitySkillMagazine);

            for (int i = 0; i < alienHeadCount; i++)
            {
                amount *= 0.75f;
            }

            if (skillSlot == SkillSlot.Secondary)
            {
                for (int i = 0; i < backupMagCount; i++)
                {
                    amount *= 0.95f;
                }
            }

            if (skillSlot == SkillSlot.Utility)
            {
                for (int i = 0; i < afterburnerCount; i++)
                {
                    amount *= 0.5f;
                }
            }

            amount = Mathf.Clamp(amount - (purityCount * 20f), 0, Mathf.Infinity);*/
            //

            if (this.currentEnergy < amount) return false;
            this.currentEnergy = Mathf.Clamp(this.currentEnergy - amount, 0f, this.maxEnergy);
            this.lastEnergy = this.currentEnergy;
            return true;
        }
    }
}