using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace HenryMod.SkillStates.Nemry
{
    public class SpawnState : BaseState
    {
        public static float duration = 3.6f;

        private CameraRigController cameraController;
        private bool initCamera;
        private Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            this.animator = base.GetModelAnimator();

            base.PlayAnimation("Body", "Spawn");
            Util.PlaySound(EntityStates.NullifierMonster.SpawnState.spawnSoundString, base.gameObject);

            if (base.GetAimAnimator()) base.GetAimAnimator().enabled = false;
            this.animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 0);
            this.animator.SetLayerWeight(animator.GetLayerIndex("AimYaw"), 0);

            //if (NetworkServer.active) base.characterBody.AddBuff(BuffIndex.HiddenInvincibility);

            EffectManager.SimpleMuzzleFlash(EntityStates.NullifierMonster.SpawnState.spawnEffectPrefab, base.gameObject, "SpawnOrigin", false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!this.cameraController)
            {
                if (base.characterBody && base.characterBody.master)
                {
                    if (base.characterBody.master.playerCharacterMasterController)
                    {
                        if (base.characterBody.master.playerCharacterMasterController.networkUser)
                        {
                            this.cameraController = base.characterBody.master.playerCharacterMasterController.networkUser.cameraRigController;
                        }
                    }
                }
            }
            else
            {
                if (!this.initCamera)
                {
                    this.initCamera = true;
                    this.cameraController.SetPitchYawFromLookVector(-base.characterDirection.forward);
                }
            }

            if (base.fixedAge >= SpawnState.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (base.GetAimAnimator()) base.GetAimAnimator().enabled = true;
            this.animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 1);
            this.animator.SetLayerWeight(animator.GetLayerIndex("AimYaw"), 1);

            //if (NetworkServer.active) base.characterBody.RemoveBuff(BuffIndex.HiddenInvincibility);
        }
    }
}
