using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;


[HarmonyPatch(typeof(UnitSpawner), "CreateUnspawnedUnit")]
class Patch_UnitSpawner_CreateUnspawnedUnit
{
    [HarmonyPostfix]
    static void Postfix(UnitSpawner __instance)
    {
        CheeseSetupUnits.SetupEjection(__instance);
        CheeseSetupUnits.SetupRWR(__instance);
        CheeseSetupUnits.SetupEvasiveManuevers(__instance);
        CheeseSetupUnits.SetupVisualTargetFinders(__instance);
    }
}
