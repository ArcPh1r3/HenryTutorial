using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace HenryMod.Modules
{
    internal static class ItemDisplays
    {
        private static Dictionary<string, GameObject> itemDisplayPrefabs = new Dictionary<string, GameObject>();

        internal static void PopulateDisplays()
        {
            PopulateFromBody("Commando");
            PopulateFromBody("Croco");
            PopulateFromBody("Mage");
        }

        private static void PopulateFromBody(string bodyName)
        {
            ItemDisplayRuleSet itemDisplayRuleSet = Resources.Load<GameObject>("Prefabs/CharacterBodies/" + bodyName + "Body").GetComponent<ModelLocator>().modelTransform.GetComponent<CharacterModel>().itemDisplayRuleSet;

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
                        string key = name?.ToLower();
                        if (!itemDisplayPrefabs.ContainsKey(key))
                        {
                            itemDisplayPrefabs[key] = followerPrefab;
                        }
                    }
                }
            }
        }

        internal static GameObject LoadDisplay(string name)
        {
            if (itemDisplayPrefabs.ContainsKey(name.ToLower()))
            {
                if (itemDisplayPrefabs[name.ToLower()]) return itemDisplayPrefabs[name.ToLower()];
            }

            return null;
        }
    }
}