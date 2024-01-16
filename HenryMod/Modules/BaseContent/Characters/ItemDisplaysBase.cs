using RoR2;
using System.Collections.Generic;

namespace HenryMod.Modules.Characters
{
    public abstract class ItemDisplaysBase
    {
        public void SetItemDisplays(ItemDisplayRuleSet itemDisplayRuleSet)
        {
            List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules = new List<ItemDisplayRuleSet.KeyAssetRuleGroup>();

            ItemDisplays.LazyInit();

            SetItemDisplayRules(itemDisplayRules);

            itemDisplayRuleSet.keyAssetRuleGroups = itemDisplayRules.ToArray();

            ItemDisplays.DisposeWhenDone();
        }

        protected abstract void SetItemDisplayRules(List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules);
    }
}
