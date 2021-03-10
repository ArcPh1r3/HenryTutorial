using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace HenryMod.Modules.Components
{
    public class MrGreenCloneTracker : MonoBehaviour
    {
        public MrGreenCloneTracker rootTracker;
        public int cloneCap = 10;
        public int currentCloneCount;
        public List<GameObject> clones;
        public float timeLastSpawnedClone;
        public bool isRoot;

        private float timeBetweenCloneSpawns = 0.5f;
        private CharacterBody characterBody;
        private CharacterModel model;
        private ChildLocator childLocator;

        private void Awake()
        {
            this.characterBody = this.gameObject.GetComponent<CharacterBody>();
            this.childLocator = this.gameObject.GetComponentInChildren<ChildLocator>();
            this.model = this.GetComponentInChildren<CharacterModel>();
            this.currentCloneCount = 0;
            this.clones = new List<GameObject>();
        }

        private void OnDestroy()
        {
            if (this.clones != null && this.isRoot)
            {
                foreach (GameObject i in this.clones)
                {
                    if (i)
                    {
                        CharacterDeathBehavior j = i.GetComponent<CharacterDeathBehavior>();
                        if (j) j.deathState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Commando.DeathState));

                        HealthComponent h = i.GetComponent<HealthComponent>();
                        if (h)
                        {
                            if (!h.alive)
                            {
                                Destroy(h.gameObject);
                            }
                            else h.Suicide();
                        }
                    }
                }
            }
        }

        public void AddClone(GameObject newClone)
        {
            this.currentCloneCount++;
            this.clones.Add(newClone);
            this.timeLastSpawnedClone = Time.time;
        }

        public bool canSpawnClone
        {
            get
            {
                return (this.currentCloneCount < this.cloneCap && Time.time > (this.timeBetweenCloneSpawns + this.timeLastSpawnedClone));
            }
        }
    }
}