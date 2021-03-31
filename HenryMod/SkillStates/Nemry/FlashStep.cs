using EntityStates;
using HenryMod.Modules.Components;
using HenryMod.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.Nemry
{
    public class FlashStep : BaseNemrySkillState
    {
        public static float duration = 0.1f;

        private Transform modelTransform;
        private float stopwatch;
        private Vector3 blinkDestination = Vector3.zero;
        private Vector3 blinkStart = Vector3.zero;
        private Animator animator;
        private CharacterModel characterModel;
        private HurtBoxGroup hurtboxGroup;

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound("NemryBlink", base.gameObject);
            this.modelTransform = base.GetModelTransform();

            if (this.modelTransform)
            {
                this.animator = this.modelTransform.GetComponent<Animator>();
                this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
                this.hurtboxGroup = this.modelTransform.GetComponent<HurtBoxGroup>();
            }

            if (this.characterModel)
            {
                this.characterModel.invisibilityCount++;
            }

            if (this.hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }

            if (base.characterMotor)
            {
                base.characterMotor.enabled = false;
            }

            Vector3 desiredPosition = base.transform.position;

            if (this.tracker && this.tracker.GetTrackingTarget())
            {
                desiredPosition = this.tracker.GetTrackingTarget().transform.position - (1.25f * base.GetAimRay().direction);
                desiredPosition.y = this.tracker.GetTrackingTarget().transform.position.y;
            }

            this.blinkDestination = base.transform.position;
            this.blinkStart = base.transform.position;

            this.blinkDestination = desiredPosition;
            this.blinkDestination += base.transform.position - base.characterBody.footPosition;

            this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));

            this.SpendEnergy(10f, SkillSlot.Utility);
        }

        private void CreateBlinkEffect(Vector3 origin)
        {
            EffectData effectData = new EffectData();
            effectData.rotation = Util.QuaternionSafeLookRotation(this.blinkDestination - this.blinkStart);
            effectData.origin = origin;
            EffectManager.SpawnEffect(EntityStates.ImpMonster.BlinkState.blinkPrefab, effectData, false);
        }

        private void SetPosition(Vector3 newPosition)
        {
            if (base.characterMotor)
            {
                base.characterMotor.Motor.SetPositionAndRotation(newPosition, Quaternion.identity, true);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.stopwatch += Time.fixedDeltaTime;

            if (base.characterMotor && base.characterDirection)
            {
                base.characterMotor.velocity = Vector3.zero;
            }

            this.SetPosition(Vector3.Lerp(this.blinkStart, this.blinkDestination, this.stopwatch / FlashStep.duration));

            if (this.stopwatch >= FlashStep.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            Util.PlaySound(EntityStates.ImpMonster.BlinkState.endSoundString, base.gameObject);
            this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));

            this.modelTransform = base.GetModelTransform();
            if (this.modelTransform && EntityStates.ImpMonster.BlinkState.destealthMaterial)
            {
                TemporaryOverlay temporaryOverlay = this.animator.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = 1f;
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = EntityStates.ImpMonster.BlinkState.destealthMaterial;
                temporaryOverlay.inspectorCharacterModel = this.animator.gameObject.GetComponent<CharacterModel>();
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.animateShaderAlpha = true;
            }

            if (this.characterModel)
            {
                this.characterModel.invisibilityCount--;
            }

            if (this.hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }

            if (base.characterMotor)
            {
                base.characterMotor.enabled = true;
            }

            base.OnExit();
        }
    }
}