using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;

[HarmonyPatch(typeof(AIPilot), "Start")]
class Patch_AIPilot_Start
{
    [HarmonyPrefix]
    static void Postfix(AIPilot __instance)
    {
        __instance.gameObject.AddComponent<CheeseAIHelper>();

        Traverse aiTraverse = Traverse.Create(__instance);
        switch (CheesesAITweaks.settings.collisionMode)
        {
            case CheesesAITweaks.CollisionMode.Normal:
                break;
            case CheesesAITweaks.CollisionMode.CollidersAlwaysOn:
                aiTraverse.Method("SetCollidersToVessel").GetValue();
                break;
            case CheesesAITweaks.CollisionMode.CollidersAlwaysOff:
                aiTraverse.Method("SetCollidersForTaxi").GetValue();
                break;
        }
    }
}

[HarmonyPatch(typeof(AIPilot), "SetCollidersForTaxi")]
class Patch_AIPilot_SetCollidersForTaxi
{
    [HarmonyPrefix]
    static bool Prefix(AIPilot __instance)
    {
        switch (CheesesAITweaks.settings.collisionMode)
        {
            case CheesesAITweaks.CollisionMode.Normal:
                return true;
            case CheesesAITweaks.CollisionMode.CollidersAlwaysOn:
                return false;
            case CheesesAITweaks.CollisionMode.CollidersAlwaysOff:
                return true;
        }
        return true;
    }
}

[HarmonyPatch(typeof(AIPilot), "SetCollidersToVessel")]
class Patch_AIPilot_SetCollidersToVessel
{
    [HarmonyPrefix]
    static bool Prefix(AIPilot __instance)
    {
        switch (CheesesAITweaks.settings.collisionMode)
        {
            case CheesesAITweaks.CollisionMode.Normal:
                return true;
            case CheesesAITweaks.CollisionMode.CollidersAlwaysOn:
                return true;
            case CheesesAITweaks.CollisionMode.CollidersAlwaysOff:
                return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(AIPilot), "UpdateTargets")]
class Patch_AIPilot_UpdateTargets
{
    [HarmonyPostfix]
    static void Postfix(AIPilot __instance)
    {
        if (CheesesAITweaks.settings.rockWingsOnContact || CheesesAITweaks.settings.dropTankMode == CheesesAITweaks.DropTankMode.DropOnContact) {
            if (CheesesAITweaks.aiToHelper.ContainsKey(__instance))
            {
                CheeseAIHelper helper = CheesesAITweaks.aiToHelper[__instance];
                if (__instance.commandState == AIPilot.CommandStates.Combat && helper.lastInCombat == false) {
                    helper.BeginContact();
                }
            }
        }
    }
}