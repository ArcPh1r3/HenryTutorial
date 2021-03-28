using System.Collections.Generic;
using RoR2;
using UnityEngine;

namespace HenryMod.Modules
{
    internal class ContentPacks
    {
        internal static ContentPack contentPack;

        internal void CreateContentPack()
        {
            contentPack = new ContentPack()
            {
                artifactDefs = new ArtifactDef[0],
                bodyPrefabs = Modules.Prefabs.bodyPrefabs.ToArray(),
                buffDefs = Modules.Buffs.buffDefs.ToArray(),
                effectDefs = Modules.Assets.effectDefs.ToArray(),
                eliteDefs = new EliteDef[0],
                entityStateConfigurations = new EntityStateConfiguration[0],
                entityStateTypes = Modules.States.entityStates.ToArray(),
                equipmentDefs = new EquipmentDef[0],
                gameEndingDefs = new GameEndingDef[0],
                gameModePrefabs = new Run[0],
                itemDefs = new ItemDef[0],
                masterPrefabs = Modules.Prefabs.masterPrefabs.ToArray(),
                musicTrackDefs = new MusicTrackDef[0],
                networkedObjectPrefabs = new GameObject[0],
                networkSoundEventDefs = Modules.Assets.networkSoundEventDefs.ToArray(),
                projectilePrefabs = Modules.Prefabs.projectilePrefabs.ToArray(),
                sceneDefs = new SceneDef[0],
                skillDefs = Modules.Skills.skillDefs.ToArray(),
                skillFamilies = Modules.Skills.skillFamilies.ToArray(),
                surfaceDefs = new SurfaceDef[0],
                survivorDefs = Modules.Prefabs.survivorDefinitions.ToArray(),
                unlockableDefs = Modules.Unlockables.unlockableDefs.ToArray()
            };

            On.RoR2.ContentManager.SetContentPacks += AddContent;
        }

        private void AddContent(On.RoR2.ContentManager.orig_SetContentPacks orig, List<ContentPack> newContentPacks)
        {
            newContentPacks.Add(contentPack);
            orig(newContentPacks);
        }
    }
}