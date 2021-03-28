using EntityStates;
using HenryMod.Modules.Components;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates.MrGreen
{
    public class Resurrect : BaseSkillState
    {
        public static float baseDuration = 3f;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = Resurrect.baseDuration / this.attackSpeedStat;

            base.PlayAnimation("FullBody, Override", "CastResurrect", "CastResurrect.playbackRate", this.duration);
        }

        private void CompleteResurrection()
        {
            MrGreenCloneTracker cloneTracker = base.GetComponent<MrGreenCloneTracker>();
            if (cloneTracker)
            {
                if (cloneTracker.clones != null)
                {
                    foreach (GameObject i in cloneTracker.clones)
                    {
                        if (i)
                        {
                            CharacterBody cloneBody = i.GetComponent<CharacterBody>();
                            if (cloneBody)
                            {
                                CharacterMaster cloneMaster = cloneBody.master;
                                if (cloneMaster)
                                {
                                    if (!cloneBody.healthComponent.alive)
                                    {
                                        cloneMaster.Respawn(cloneBody.footPosition, cloneBody.transform.rotation);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.CompleteResurrection();
                this.outer.SetNextStateToMain();
                return;
            }
        }
    }
}
