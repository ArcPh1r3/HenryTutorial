using System;
using UnityEngine;

public class FootstepHandler : MonoBehaviour
{
    public string baseFootstepString;
    public string sprintFootstepOverrideString;
    public bool enableFootstepDust;
    public GameObject footstepDustPrefab;

    private Animator animator;
    private Transform footstepDustInstanceTransform;
    private ParticleSystem footstepDustInstanceParticleSystem;

    private void Start()
    {
        this.animator = base.GetComponent<Animator>();
        if (this.enableFootstepDust)
        {
            this.footstepDustInstanceTransform = UnityEngine.Object.Instantiate<GameObject>(this.footstepDustPrefab, base.transform).transform;
            this.footstepDustInstanceParticleSystem = this.footstepDustInstanceTransform.GetComponent<ParticleSystem>();
        }
    }

    public void Footstep(AnimationEvent animationEvent)
    {
        if ((double)animationEvent.animatorClipInfo.weight > 0.5)
        {
            this.Footstep(animationEvent.stringParameter, (GameObject)animationEvent.objectReferenceParameter);
        }
    }
    public void Footstep(string childName, GameObject footstepEffect)
    {
        Debug.Log("hi xD");
    }
}