using System.Collections.Generic;
using UnityEngine;

public class BaseUnattachedAnimator : MonoBehaviour {

    [System.Serializable]
    public class UnattachedAnimation {
        public string animationState;
        public int layer;
    }

    [System.Serializable]
    public class UnattachedAnimations {
        public List<UnattachedAnimation> animations;
    }

    [System.Serializable]
    public class UnattachedAnimationCombo {
        public string name;
        public KeyCode keyCode;
        public List<UnattachedAnimations> comboAnimations;
        [HideInInspector]
        public float comboTimer;
        [HideInInspector]
        public int comboStep;
    }

    [SerializeField]
    protected Animator animator;

    [SerializeField, Space]
    List<UnattachedAnimationCombo> animationCombos;

    [Header("whyt he fuck aren't these in the animator")]
    [SerializeField, Range(0, 0.999f)]
    protected float aimPitch = 0.5f;
    [SerializeField, Range(0, 0.999f)]
    protected float aimYaw = 0.5f;

    protected float _combatTim;
    protected float _jumpTim;

    private void Moob() {
        //man it's been so long since I've written a moob function

        float hori = Input.GetAxis("Horizontal");
        float veri = Input.GetAxis("Vertical");

        animator.SetBool("isMoving", Mathf.Abs(hori) + Mathf.Abs(veri) > 0.01f);
        animator.SetFloat("forwardSpeed", veri);
        animator.SetFloat("rightSpeed", hori);

        animator.SetBool("isSprinting", Input.GetKey(KeyCode.LeftControl));
    }

    private void Jumb() {

        if (Input.GetKeyDown(KeyCode.Space)) {
            animator.Play("Jump");
            animator.SetBool("isGrounded", false);
            _jumpTim = 1.5f;
        }

        _jumpTim -= Time.deltaTime;

        animator.SetFloat("upSpeed", Mathf.Lerp(-48, 16, _jumpTim / 2f));

        if (_jumpTim <= 0) {
            if (!animator.GetBool("isGrounded")) {
                animator.Play("LightImpact", 1);
            }
            animator.SetBool("isGrounded", true);
        }
    }

    private void Timers() {
        _combatTim -= Time.deltaTime;
        animator.SetBool("inCombat", _combatTim > 0);
    }

    protected virtual void Update() {
        if (!animator)
            return;

        Moob();
        Jumb();
        Shooting();
        Aiming();
        Timers();
    }
    private void Aiming() {

        if (Input.GetKeyDown(KeyCode.Q))
            aimYaw += 0.2f;

        if (Input.GetKeyDown(KeyCode.E))
            aimYaw -= 0.2f;

        aimYaw = Mathf.Clamp(aimYaw, 0, 0.999f);

        animator.SetFloat("aimYawCycle", aimYaw);
        animator.SetFloat("aimPitchCycle", aimPitch);
    }

    protected virtual void Shooting() {
        for (int i = 0; i < animationCombos.Count; i++) {
            RunCombo(animationCombos[i]);
        }
    }

    protected virtual void RunCombo(UnattachedAnimationCombo combo) {
        if (Input.GetKeyDown(combo.keyCode)) {

            List<UnattachedAnimation> animations = combo.comboAnimations[combo.comboStep].animations;
            for (int i = 0; i < animations.Count; i++) {
                animator.Play(animations[i].animationState, animations[i].layer);
            }

            combo.comboTimer = 2;

            combo.comboStep++;
            if(combo.comboStep >= combo.comboAnimations.Count) {
                combo.comboStep = 0;
            }

            _combatTim = 2;
        }

        combo.comboTimer -= Time.deltaTime;

        if (combo.comboTimer <= 0) {
            combo.comboStep = 0;
        }
    }
}
