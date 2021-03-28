using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace HenryMod.SkillStates.MrGreen
{
    public class CloneDodge : BaseSkillState
    {
        public float storedDamage = 0f;
        public GameObject attacker = null;
        public Modules.Components.MrGreenCloneTracker tracker;

        private Vector3 dashVector = Vector3.zero;
        private float duration = 0.4f;
        private float speedCoefficient = 5f;

        public override void OnEnter()
        {
            base.OnEnter();
            base.characterBody.isSprinting = true;
            this.dashVector = -base.characterDirection.forward;

            Util.PlaySound("HenryStinger", base.gameObject);
            base.PlayAnimation("Gesture, Override", "BufferEmpty");

            base.gameObject.layer = LayerIndex.fakeActor.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();

            if (NetworkServer.active)
            {
                //base.characterBody.AddBuff(BuffIndex.HiddenInvincibility);

                CharacterMaster newCloneMaster = new MasterSummon
                {
                    masterPrefab = MasterCatalog.GetMasterPrefab(MasterCatalog.FindMasterIndex("MrGreenCloneMaster")),
                    position = base.transform.position,
                    rotation = base.modelLocator.modelBaseTransform.rotation,
                    summonerBodyObject = base.gameObject,
                    ignoreTeamMemberLimit = true
                }.Perform();

                Inventory inventory = newCloneMaster.inventory;
                inventory.CopyItemsFrom(base.characterBody.master.inventory);
                /*inventory.ResetItem(ItemIndex.WardOnLevel);
                inventory.ResetItem(ItemIndex.BeetleGland);
                inventory.ResetItem(ItemIndex.CrippleWardOnLevel);
                inventory.ResetItem(ItemIndex.TPHealingNova);
                inventory.ResetItem(ItemIndex.FocusConvergence);
                inventory.ResetItem(ItemIndex.TitanGoldDuringTP);*/

                var delayedDamage = newCloneMaster.gameObject.AddComponent<Modules.Components.DelayedDamageOnSpawn>();
                delayedDamage.attacker = this.attacker;
                delayedDamage.damage = this.storedDamage;
                delayedDamage.tracker = this.tracker;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.characterMotor.velocity = Vector3.zero;
            base.characterMotor.rootMotion = this.dashVector * (this.moveSpeedStat * this.speedCoefficient * Time.fixedDeltaTime) * Mathf.Cos(base.fixedAge / (this.duration * 1.3f) * 1.57079637f);

            if (base.isAuthority)
            {
                if (base.characterDirection)
                {
                    base.characterDirection.forward = -this.dashVector;
                }
            }

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.characterMotor.velocity *= 0.1f;
            base.OnExit();

            //if (NetworkServer.active) base.characterBody.RemoveBuff(BuffIndex.HiddenInvincibility);

            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}