using RoR2;
using RoR2.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HenryMod.Modules
{
    internal static class ItemDisplayCheck
    {
        public static List<Object> allDisplayedItems = null;

        /// <summary>
        /// prints in the console code for new item displays from vanilla characters that are not in the passed in rules, formatted for use in this project
        /// </summary>
        /// <param name="itemDisplayRuleSet">the IDRS of the charactermodel with existing displays. pass in null to print all displays</param>
        /// <param name="bodyName">decoration string for the console, if you're printing for multiple bodies</param>
        public static void PrintUnused(ItemDisplayRuleSet itemDisplayRuleSet, string bodyName = "")
        {
            PrintUnused(itemDisplayRuleSet.keyAssetRuleGroups.ToList(), bodyName);
        }
        /// <summary>
        /// prints in the console code for new item displays from vanilla characters that are not in the passed in rules, formatted for use in this project
        /// </summary>
        /// <param name="ruleSet">list of existing item displays. pass in null to print all item displays</param>
        /// <param name="bodyName">decoration string for the console, if you're printing for multiple bodies</param>
        public static void PrintUnused(IEnumerable<ItemDisplayRuleSet.KeyAssetRuleGroup> ruleSet = null, string bodyName = "")
        {
            string missingDisplays = $"generating item displays for {bodyName}";

            //grab all keyassets
            if(allDisplayedItems == null)
                LazyGatherAllItems();

            //start with all items, and remove from the list items that we already have displays for
            List<Object> missingKeyAssets = new List<Object>(allDisplayedItems);

            string firstCompatibleChild = "";
            if (ruleSet != null)
            {
                foreach (ItemDisplayRuleSet.KeyAssetRuleGroup ruleGroup in ruleSet)
                {
                    if (ruleGroup.displayRuleGroup.rules.Length == 0)
                        continue;

                    missingKeyAssets.Remove(ruleGroup.keyAsset);

                    if (string.IsNullOrEmpty(firstCompatibleChild))
                    {
                        firstCompatibleChild = ruleGroup.displayRuleGroup.rules[0].childName;
                    }
                }
            }

            if (string.IsNullOrEmpty(firstCompatibleChild))
            {
                firstCompatibleChild = "Chest";
            }

            //print all display rules
            foreach (Object keyAsset in missingKeyAssets)
            {
                string thing = $"";
                if (ItemDisplays.KeyAssetDisplayPrefabs.ContainsKey(keyAsset))
                {
                    //if we have a displayprefab for it (Populated in ItemDisplays.PopulateDisplays),
                        //generate a rule formatted to the code in this project
                    thing += SpitOutNewRule(keyAsset, firstCompatibleChild, ItemDisplays.KeyAssetDisplayPrefabs[keyAsset]);
                }
                else
                {
                    Log.Error($"COULD NOT FIND DISPLAY PREFABS FOR KEYASSET {keyAsset}");
                }
                missingDisplays += thing;
            }
            //add them all to a giant string and print it out, formatted
            Log.Message(missingDisplays);
        }

        private static void LazyGatherAllItems()
        {
            allDisplayedItems = new List<Object>(ItemDisplays.KeyAssetDisplayPrefabs.Keys);

            allDisplayedItems.Sort((item1, item2) => {
                //sort defs by keyasset so it shows up in the same order as the idph
                if (item1 is ItemDef && item2 is ItemDef)
                {
                    return item1.name.CompareTo(item2.name);
                }
                if (item1 is EquipmentDef && item2 is EquipmentDef)
                {
                    return item1.name.CompareTo(item2.name);
                }
                if (item1 is ItemDef && item2 is EquipmentDef)
                {
                    return -1;
                }
                if (item1 is EquipmentDef && item2 is ItemDef)
                {
                    return 1;
                }

                return 0;
            });
        }

        private static string SpitOutNewRule(Object asset, string firstCompatibleChild, ItemDisplayRule[] displayRules)
        {
            if (displayRules.Length == 0)
                return $"\n[NO DISPLAY RULES FOUND FOR THE KEYASSET {asset}";

            //generate item display rule
            string newRule = $"\n            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets[\"{asset.name}\"]";
            for (int i = 0; i < displayRules.Length; i++)
            {
                if(displayRules[i].limbMask == LimbFlags.None)
                {
                    newRule += ",\n" + 
                              $"                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay(\"{displayRules[i].followerPrefab.name}\"),\n" +
                              $"                    \"{firstCompatibleChild}\",\n" +
                               "                    new Vector3(2, 2, 2),\n" +
                               "                    new Vector3(0, 0, 0),\n" +
                               "                    new Vector3(1, 1, 1)\n" +
                               "                    )";
                } 
                else
                {
                    newRule += ",\n" + 
                              $"                ItemDisplays.CreateLimbMaskDisplayRule(LimbFlags.{displayRules[i].limbMask})";
                }
            }
            newRule += "\n                ));";
            return newRule;
        }
    }
}