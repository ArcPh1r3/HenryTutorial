using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace HenryMod.Modules
{
    internal static class ItemDisplays
    {
        private static Dictionary<string, GameObject> itemDisplayPrefabs = new Dictionary<string, GameObject>();
        public static Dictionary<string, Object> itemDisplayCheckKeyAsset = new Dictionary<string, Object>();

        public static Dictionary<string, int> itemDisplayCheckCount = new Dictionary<string, int>();
        private static bool recording = false;

        internal static void PopulateDisplays()
        {
            //PopulateFromBody("CommandoBody");
            //PopulateFromBody("CrocoBody");
            PopulateFromBody("MageBody");
            PopulateFromBody("LunarExploderBody"); //solely for the perfected crown
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
                        string name = followerPrefab.name;
                        string key = name?.ToLowerInvariant();
                        if (!itemDisplayPrefabs.ContainsKey(key))
                        {
                            itemDisplayPrefabs[key] = followerPrefab;

                            itemDisplayCheckCount[key] = 0;
                            itemDisplayCheckKeyAsset[key] = item[i].keyAsset;
                        }
                    }
                }
            }
        }

        public static GameObject LoadDisplay(string name) {

            if (itemDisplayPrefabs.ContainsKey(name.ToLowerInvariant())) {

                if (itemDisplayPrefabs[name.ToLowerInvariant()]) {

                    if (recording) {
                        itemDisplayCheckCount[name.ToLowerInvariant()]++;
                    }

                    GameObject display = itemDisplayPrefabs[name.ToLowerInvariant()];

                    #region IgnoreThisAndRunAway
                    //seriously you don't need this
                    //I see you're still here, well if you do need this here's what you do
                    //but again you don't need this
                    //capacitor is hardcoded to track your "UpperArmR", "LowerArmR", and "HandR" bones.
                    //this is for having the lightning on custom bones in your childlocator
                    if (name == "DisplayLightningArmCustom") {
                        display = R2API.PrefabAPI.InstantiateClone(itemDisplayPrefabs["DisplayLightningArmRight"], "DisplayLightningCustom", false);

                        LimbMatcher limbMatcher = display.GetComponent<LimbMatcher>();

                        limbMatcher.limbPairs[0].targetChildLimb = "LightningArm1";
                        limbMatcher.limbPairs[1].targetChildLimb = "LightningArm2";
                        limbMatcher.limbPairs[2].targetChildLimb = "LightningArmEnd";
                    }
                    #endregion

                    return display;
                }
            }
            Log.Error("item display " + name + " returned null");
            return null;
        }

        #region check unused item displays
        public static void recordUnused() {
            recording = true;
        }
        public static void printUnused() {

            string yes = "used:";
            string no = "not used:";

            foreach (KeyValuePair<string, int> pair in itemDisplayCheckCount) {
                string thing = $"\n{itemDisplayPrefabs[pair.Key].name} | {itemDisplayCheckKeyAsset[pair.Key]} | {pair.Value}";

                if (pair.Value > 0) {
                    yes += thing;
                } else {
                    no += thing;
                }
            }
            Debug.LogWarning(no);

            //resetUnused();
        }

        private static void resetUnused() {
            foreach (KeyValuePair<string, int> pair in itemDisplayCheckCount) {
                itemDisplayCheckCount[pair.Key] = 0;
            }
            recording = false;
        }
        #endregion

    }
}