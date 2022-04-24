using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HenryMod.Modules
{
    internal static class ItemDisplays
    {
        private static Dictionary<string, GameObject> itemDisplayPrefabs = new Dictionary<string, GameObject>();

        internal static void PopulateDisplays()
        {
            PopulateFromBody("MageBody"); //commando is actually missing some displays
            PopulateFromBody("LunarExploderBody"); //solely for the perfected crown

            PopulateCustomLightningArm();

            //if you have any custom item displays to add here I would be very impressed
        }

        private static void PopulateFromBody(string bodyName)
        {
            ItemDisplayRuleSet itemDisplayRuleSet = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/" + bodyName).GetComponent<ModelLocator>().modelTransform.GetComponent<CharacterModel>().itemDisplayRuleSet;

            ItemDisplayRuleSet.KeyAssetRuleGroup[] item = itemDisplayRuleSet.keyAssetRuleGroups;

            for (int i = 0; i < item.Length; i++)
            {
                ItemDisplayRule[] rules = item[i].displayRuleGroup.rules;

                for (int j = 0; j < rules.Length; j++)
                {
                    GameObject followerPrefab = rules[j].followerPrefab;
                    if (followerPrefab)
                    {
                        string key = followerPrefab.name?.ToLowerInvariant();
                        if (!itemDisplayPrefabs.ContainsKey(key))
                        {
                            itemDisplayPrefabs[key] = followerPrefab;
                        }
                    }
                }
            }
        }

        private static void PopulateCustomLightningArm()
        {
            #region IgnoreThisAndRunAway
            //seriously you don't need this
            //I see you're still here, well if you do need this here's what you do
            //but again you don't need this
            //capacitor is hardcoded to track your "UpperArmR", "LowerArmR", and "HandR" bones.
            //this is for having the lightning on custom bones in your childlocator

            GameObject display = R2API.PrefabAPI.InstantiateClone(itemDisplayPrefabs["displaylightningarmright"], "DisplayLightningCustom", false);

            LimbMatcher limbMatcher = display.GetComponent<LimbMatcher>();

            limbMatcher.limbPairs[0].targetChildLimb = "LightningArm1";
            limbMatcher.limbPairs[1].targetChildLimb = "LightningArm2";
            limbMatcher.limbPairs[2].targetChildLimb = "LightningArmEnd";

            itemDisplayPrefabs["displaylightningarmcustom"] = display;
            #endregion
        }

        public static GameObject LoadDisplay(string name) {

            if (itemDisplayPrefabs.ContainsKey(name.ToLowerInvariant())) {

                if (itemDisplayPrefabs[name.ToLowerInvariant()]) {

                    GameObject display = itemDisplayPrefabs[name.ToLowerInvariant()];

                    return display;
                }
            }
            Log.Error("item display " + name + " returned null");
            return null;
        }
    }
}