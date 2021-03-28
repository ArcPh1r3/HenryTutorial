using EntityStates;
using UnityEngine.Networking;

namespace HenryMod.SkillStates.MrGreen
{
    public class CloneRespawnState : BaseState
    {
        public static float duration = 1f;

        public override void OnEnter()
        {
            base.OnEnter();
            base.PlayAnimation("Body", "Spawn");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority && base.fixedAge >= CloneRespawnState.duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }
    }
}