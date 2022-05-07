using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;

[HarmonyPatch(typeof(ShipMover), "Start")]
class Patch_ShipMover_Start
{
    [HarmonyPrefix]
    static void Postfix(ShipMover __instance)
    {
        if (CheesesAITweaks.settings.rockShips)
        {
            //__instance.gameObject.AddComponent<CheeseShipRocker>();
        }
    }
}