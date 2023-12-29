using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;

namespace HenryMod.Modules
{
    //consolidate contentaddition here in case something breaks and/or want to move to r2api
    internal class Content
    {
        public static void AddCharacterBodyPrefab(GameObject bprefab)
        {
            ContentPacks.bodyPrefabs.Add(bprefab);
        }

        public static void AddMasterPrefab(GameObject prefab)
        {
            ContentPacks.masterPrefabs.Add(prefab);
        }

        public static void AddProjectilePrefab(GameObject prefab)
        {
            ContentPacks.projectilePrefabs.Add(prefab);
        }

        public static void AddSurvivorDef(SurvivorDef survivorDef)
        {

            ContentPacks.survivorDefs.Add(survivorDef);
        }
        public static void CreateSurvivor(GameObject bodyPrefab, GameObject displayPrefab, Color charColor, string tokenPrefix) { CreateSurvivor(bodyPrefab, displayPrefab, charColor, tokenPrefix, null, 100f); }
        public static void CreateSurvivor(GameObject bodyPrefab, GameObject displayPrefab, Color charColor, string tokenPrefix, float sortPosition) { CreateSurvivor(bodyPrefab, displayPrefab, charColor, tokenPrefix, null, sortPosition); }
        public static void CreateSurvivor(GameObject bodyPrefab, GameObject displayPrefab, Color charColor, string tokenPrefix, UnlockableDef unlockableDef) { CreateSurvivor(bodyPrefab, displayPrefab, charColor, tokenPrefix, unlockableDef, 100f); }
        public static void CreateSurvivor(GameObject bodyPrefab, GameObject displayPrefab, Color charColor, string tokenPrefix, UnlockableDef unlockableDef, float sortPosition)
        {
            SurvivorDef survivorDef = ScriptableObject.CreateInstance<SurvivorDef>();
            survivorDef.bodyPrefab = bodyPrefab;
            survivorDef.displayPrefab = displayPrefab;
            survivorDef.primaryColor = charColor;

            survivorDef.cachedName = bodyPrefab.name.Replace("Body", "");
            survivorDef.displayNameToken = tokenPrefix + "NAME";
            survivorDef.descriptionToken = tokenPrefix + "DESCRIPTION";
            survivorDef.outroFlavorToken = tokenPrefix + "OUTRO_FLAVOR";
            survivorDef.mainEndingEscapeFailureFlavorToken = tokenPrefix + "OUTRO_FAILURE";

            survivorDef.desiredSortPosition = sortPosition;
            survivorDef.unlockableDef = unlockableDef;

            Modules.Content.AddSurvivorDef(survivorDef);
        }

        public static void AddUnlockableDef(UnlockableDef unlockableDef)
        {
            ContentPacks.unlockableDefs.Add(unlockableDef);
        }
        public static UnlockableDef CreateAndAddUnlockbleDef(string identifier, string nameToken, Sprite achievementIcon)
        {
            UnlockableDef unlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            unlockableDef.cachedName = identifier;
            unlockableDef.nameToken = nameToken;
            unlockableDef.achievementIcon = achievementIcon;

            AddUnlockableDef(unlockableDef);

            return unlockableDef;
        }

        public static void AddSkillDef(SkillDef skillDef)
        {
            ContentPacks.skillDefs.Add(skillDef);
        }

        public static void AddSkillFamily(SkillFamily skillFamily)
        {
            ContentPacks.skillFamilies.Add(skillFamily);
        }

        public static void AddEntityState(Type entityState)
        {
            ContentPacks.entityStates.Add(entityState);
        }

        public static void AddBuffDef(BuffDef buffDef)
        {
            ContentPacks.buffDefs.Add(buffDef);
        }
        // simple helper method
        internal static BuffDef CreateAndAddBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;

            AddBuffDef(buffDef);

            return buffDef;
        }

        public static void AddEffectDef(EffectDef effectDef)
        {
            ContentPacks.effectDefs.Add(effectDef);
        }
        //todo ser
        internal static EffectDef CreateAndAddEffectDef(GameObject effectPrefab, string soundName = "")
        {
            EffectDef effectDef = new EffectDef();
            effectDef.prefab = effectPrefab;
            effectDef.prefabEffectComponent = effectPrefab.GetComponent<EffectComponent>();
            effectDef.prefabName = effectPrefab.name;
            effectDef.prefabVfxAttributes = effectPrefab.GetComponent<VFXAttributes>();
            effectDef.spawnSoundEventName = soundName;

            AddEffectDef(effectDef);

            return effectDef;
        }

        public static void AddNetworkSoundEventDef(NetworkSoundEventDef networkSoundEventDef)
        {
            ContentPacks.networkSoundEventDefs.Add(networkSoundEventDef);
        }
        internal static NetworkSoundEventDef CreateAndAddNetworkSoundEventDef(string eventName)
        {
            NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            networkSoundEventDef.akId = AkSoundEngine.GetIDFromString(eventName);
            networkSoundEventDef.eventName = eventName;

            AddNetworkSoundEventDef(networkSoundEventDef);

            return networkSoundEventDef;
        }
    }
}