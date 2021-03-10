using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace HenryMod.Modules.Components
{
    public class DelayedDamageOnSpawn : MonoBehaviour
    {
        public GameObject attacker;
        public float damage;
        public MrGreenCloneTracker tracker;

        private CharacterMaster master;

        private void Awake()
        {
            this.master = this.gameObject.GetComponent<CharacterMaster>();
        }

        private void FixedUpdate()
        {
            if (this.master)
            {
                if (this.master.GetBody())
                {
                    this.TakeDamage();
                    return;
                }
            }
        }

        private void TakeDamage()
        {
            Destroy(this);

            MrGreenCloneTracker newTracker = this.master.GetBody().GetComponent<MrGreenCloneTracker>();
            if (newTracker) newTracker.rootTracker = this.tracker;

            this.tracker.AddClone(this.master.GetBodyObject());

            if (!NetworkServer.active) return;

            this.master.GetBody().healthComponent.TakeDamage(new DamageInfo
            {
                attacker = this.attacker,
                crit = false,
                damage = this.damage,
                damageColorIndex = DamageColorIndex.Default,
                damageType = DamageType.NonLethal,
                dotIndex = DotController.DotIndex.None,
                force = Vector3.zero,
                inflictor = this.attacker,
                position = this.transform.position,
                procChainMask = default(ProcChainMask),
                procCoefficient = 0f
            });
        }
    }
}