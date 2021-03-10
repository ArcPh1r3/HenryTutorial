using EntityStates;
using UnityEngine.Networking;

namespace HenryMod.SkillStates.MrGreen
{
    public class CloneSpawnState : BaseState
    {
        public static float duration = 0.2f;

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority && base.fixedAge >= CloneSpawnState.duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }
    }
}