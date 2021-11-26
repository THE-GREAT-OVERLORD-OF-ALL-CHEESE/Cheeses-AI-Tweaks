using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;

[HarmonyPatch(typeof(AirFormationLeader), "GetLocalFormationPosition")]
class Patch_AirFormationLeader_GetLocalFormationPosition
{
    [HarmonyPostfix]
    static void Postfix(AirFormationLeader __instance, ref Vector3 __result)
    {

    }
}