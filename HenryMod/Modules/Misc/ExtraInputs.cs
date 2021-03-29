using R2API.Utils;
using Rewired;
using Rewired.Data;
using RoR2;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace HenryMod.Modules.Misc
{
    internal static class ExtraInputs
    {
        internal static void AddActionsToInputCatalog()
        {
            var extraActionAxisPairFirst = new InputCatalog.ActionAxisPair(RewiredActions.WeaponSwapSkillName, AxisRange.Full);

            InputCatalog.actionToToken.Add(extraActionAxisPairFirst, HenryPlugin.developerPrefix + "_WEAPON_SWAP_SKILL");
        }

        internal static void AddCustomActions(Action<UserData> orig, UserData self)
        {
            var swapAction = CreateInputAction(RewiredActions.WeaponSwapSkill, RewiredActions.WeaponSwapSkillName);

            var actions = self.GetFieldValue<List<InputAction>>("actions");

            actions?.Add(swapAction);

            orig(self);
        }

        internal static InputAction CreateInputAction(int id, string name, InputActionType type = InputActionType.Button)
        {
            var action = new InputAction();

            action.SetFieldValue("_id", id);
            action.SetFieldValue("_name", name);
            action.SetFieldValue("_type", type);
            action.SetFieldValue("_descriptiveName", name);
            action.SetFieldValue("_behaviorId", 0);
            action.SetFieldValue("_userAssignable", true);
            action.SetFieldValue("_categoryId", 0);

            return action;
        }
    }
}