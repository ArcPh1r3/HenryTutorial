using RoR2;
using System;
using UnityEngine;

namespace HenryMod.Modules.Components
{
    public class NemryUnlockComponent : MonoBehaviour
    {
        private HealthComponent healthComponent;

        public static event Action<Run> OnDeath = delegate { };

        private void Awake()
        {
            healthComponent = GetComponent<HealthComponent>();
        }

        private void FixedUpdate()
        {
            if (healthComponent && !healthComponent.alive)
            {
                if (healthComponent.body.teamComponent.teamIndex != TeamIndex.Player)
                {
                    OnDeath?.Invoke(Run.instance);
                }
            }
        }
    }
}