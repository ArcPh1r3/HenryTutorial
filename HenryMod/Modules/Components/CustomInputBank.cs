using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HenryMod.Modules.Components
{
    public class CustomInputBank : MonoBehaviour
    {
        public InputBankTest.ButtonState weaponSwapSkill;

        internal static bool CheckAnyButtonDownOverrideHook(On.RoR2.InputBankTest.orig_CheckAnyButtonDown orig, InputBankTest self)
        {
            if (orig(self))
            {
                return true;
            }
            var extraInputBankTest = self.GetComponent<CustomInputBank>();
            if (extraInputBankTest) return extraInputBankTest.weaponSwapSkill.down;
            else return false;
        }
    }
}