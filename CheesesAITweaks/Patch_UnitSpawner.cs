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
                    AIEjectPilot eject = CheesesAITweaks.instance.AddEjectorSeat(unitSpawn.actor.health, unitSpawn.GetComponent<Rigidbody>(), new Vector3(0, 0, 8));
                    GameObject canopy = unitSpawn.actor.transform.Find("enemyFighterNS").Find("canopy").gameObject;
                    eject.OnBegin.AddListener(delegate { canopy.SetActive(false); });
                    break;
                case "ASF-33":
                    AIEjectPilot eject2 = CheesesAITweaks.instance.AddEjectorSeat(unitSpawn.actor.health, unitSpawn.GetComponent<Rigidbody>(), new Vector3(0, 0.3f, 8));
                    GameObject canopy2 = unitSpawn.actor.transform.Find("lod0").Find("canopy").gameObject;
                    eject2.OnBegin.AddListener(delegate { canopy2.SetActive(false); });
                    break;
                case "ASF-58":
                    AIEjectPilot eject3 = CheesesAITweaks.instance.AddEjectorSeat(unitSpawn.actor.health, unitSpawn.GetComponent<Rigidbody>(), new Vector3(0, 0.3f, 13));
                    GameObject canopy3 = unitSpawn.actor.transform.Find("body").Find("canopy").gameObject;
                    eject3.OnBegin.AddListener(delegate { canopy3.SetActive(false); });
                    break;
                case "GAV-25":
                    CheesesAITweaks.instance.AddEjectorSeat(unitSpawn.actor.health, unitSpawn.GetComponent<Rigidbody>(), new Vector3(0, -0.7f, 4.2f));
                    break;
                case "EBomberAI":
                    AIEjectPilot eject4 = CheesesAITweaks.instance.AddEjectorSeat(unitSpawn.actor.health, unitSpawn.GetComponent<Rigidbody>(), new Vector3(-0.629f, 0.397f, 23.806f));
                    AIEjectPilot eject5 = CheesesAITweaks.instance.AddEjectorSeat(unitSpawn.actor.health, unitSpawn.GetComponent<Rigidbody>(), new Vector3(0.629f, 0.397f, 23.806f));

                    GameObject canopy4 = unitSpawn.actor.transform.Find("CockpitPart").Find("ejectPanelLeft").gameObject;
                    eject4.OnBegin.AddListener(delegate { canopy4.SetActive(false); });

                    GameObject canopy5 = unitSpawn.actor.transform.Find("CockpitPart").Find("ejectPanelRight").gameObject;
                    eject5.OnBegin.AddListener(delegate { canopy5.SetActive(false); });

                    CheesesAITweaks.instance.AddBomberDoors(unitSpawn.actor.transform, eject4, false);
                    CheesesAITweaks.instance.AddBomberDoors(unitSpawn.actor.transform, eject5, true);
                    break;
                case "E-4":
                    AICrewBailer crewBailer = unitSpawn.actor.gameObject.AddComponent<AICrewBailer>();
                    crewBailer.SetupCrew(15, unitSpawn.GetComponent<Rigidbody>());
                    unitSpawn.actor.health.OnDeath.AddListener(crewBailer.BeginBailout);
                    break;
                case "KC-49":
                    AICrewBailer crewBailer2 = unitSpawn.actor.gameObject.AddComponent<AICrewBailer>();
                    crewBailer2.SetupCrew(3, unitSpawn.GetComponent<Rigidbody>());
                    unitSpawn.actor.health.OnDeath.AddListener(crewBailer2.BeginBailout);
                    break;
                default:
                    break;
            }
        }
    }
}
