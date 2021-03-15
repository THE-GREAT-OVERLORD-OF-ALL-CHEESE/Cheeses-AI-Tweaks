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
        if (CheesesAITweaks.settings.enemyEjectorSeats) {
            Traverse unitSpawnerTraverse = Traverse.Create(__instance);
            UnitSpawn unitSpawn = unitSpawnerTraverse.Field("_spawnedUnit").GetValue<UnitSpawn>();
            switch (__instance.unitID) {
                case "ASF-30":
                    CheesesAITweaks.instance.AddEjectorSeat(unitSpawn.actor.health, unitSpawn.GetComponent<Rigidbody>(), new Vector3(0, 0, 8));
                    break;
                case "ASF-33":
                    CheesesAITweaks.instance.AddEjectorSeat(unitSpawn.actor.health, unitSpawn.GetComponent<Rigidbody>(), new Vector3(0, 0.3f, 8));
                    break;
                case "ASF-58":
                    CheesesAITweaks.instance.AddEjectorSeat(unitSpawn.actor.health, unitSpawn.GetComponent<Rigidbody>(), new Vector3(0, 0.3f, 13));
                    break;
                case "GAV-25":
                    CheesesAITweaks.instance.AddEjectorSeat(unitSpawn.actor.health, unitSpawn.GetComponent<Rigidbody>(), new Vector3(0, -0.7f, 4.2f));
                    break;
                case "EBomberAI":
                    CheesesAITweaks.instance.AddEjectorSeat(unitSpawn.actor.health, unitSpawn.GetComponent<Rigidbody>(), new Vector3(-0.629f, 0.397f, 23.806f));
                    CheesesAITweaks.instance.AddEjectorSeat(unitSpawn.actor.health, unitSpawn.GetComponent<Rigidbody>(), new Vector3(0.629f, 0.397f, 23.806f));
                    break;
                case "E-4":
                    break;
                case "KC-49":
                    break;
                default:
                    break;
            }
        }
    }
}
