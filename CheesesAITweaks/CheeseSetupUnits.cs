using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Harmony;

public static class CheeseSetupUnits
{
    public static void SetupEjection(UnitSpawner __instance)
    {
        if (CheesesAITweaks.settings.enemyEjectorSeats)
        {
            Traverse unitSpawnerTraverse = Traverse.Create(__instance);
            UnitSpawn unitSpawn = unitSpawnerTraverse.Field("_spawnedUnit").GetValue<UnitSpawn>();
            switch (__instance.unitID)
            {
                case "ASF-30":
                    AIEjectPilot eject = CheesesAITweaks.instance.AddEjectorSeat(unitSpawn.actor.health, unitSpawn.GetComponent<Rigidbody>(), new Vector3(0, 0, 8));
                    GameObject canopy = unitSpawn.actor.transform.Find("enemyFighterNS").Find("canopy").gameObject;
                    eject.OnBegin.AddListener(delegate { canopy.SetActive(false); });

                    GameObject cockpit = CheesesAITweaks.instance.AddFACockpit(unitSpawn.transform, new Vector3(0, 0.27729f, 6.581f), Vector3.one * 100);
                    eject.OnBegin.AddListener(delegate { cockpit.SetActive(true); });
                    break;
                case "ASF-33":
                    AIEjectPilot eject2 = CheesesAITweaks.instance.AddEjectorSeat(unitSpawn.actor.health, unitSpawn.GetComponent<Rigidbody>(), new Vector3(0, 0.3f, 8));
                    GameObject canopy2 = unitSpawn.actor.transform.Find("lod0").Find("canopy").gameObject;
                    eject2.OnBegin.AddListener(delegate { canopy2.SetActive(false); });

                    GameObject cockpit2 = CheesesAITweaks.instance.AddFACockpit(unitSpawn.transform, new Vector3(0, 0.465f, 6.581f), Vector3.one * 100);
                    eject2.OnBegin.AddListener(delegate { cockpit2.SetActive(true); });
                    break;
                case "ASF-58":
                    AIEjectPilot eject3 = CheesesAITweaks.instance.AddEjectorSeat(unitSpawn.actor.health, unitSpawn.GetComponent<Rigidbody>(), new Vector3(0, 0.025f, 14.122f));
                    GameObject canopy3 = unitSpawn.actor.transform.Find("body").Find("canopy").gameObject;
                    eject3.OnBegin.AddListener(delegate { canopy3.SetActive(false); });

                    AIEjectPilot eject6 = CheesesAITweaks.instance.AddEjectorSeat(unitSpawn.actor.health, unitSpawn.GetComponent<Rigidbody>(), new Vector3(0, 0.41f, 12.126f));
                    eject6.OnBegin.AddListener(delegate { canopy3.SetActive(false); });

                    GameObject cockpit3 = CheesesAITweaks.instance.AddFACockpit(unitSpawn.transform, new Vector3(0, 0.107f, 11.57f), Vector3.one * 125);
                    eject3.OnBegin.AddListener(delegate { cockpit3.SetActive(true); });
                    eject6.OnBegin.AddListener(delegate { cockpit3.SetActive(true); });
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

                    unitSpawn.transform.Find("CockpitPart").GetComponent<VehiclePart>().detachDelay = new MinMax(3, 8);
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

    public static void SetupRWR(UnitSpawner __instance)
    {
        if (CheesesAITweaks.settings.allAIHaveRWR)
        {
            Traverse unitSpawnerTraverse = Traverse.Create(__instance);
            UnitSpawn unitSpawn = unitSpawnerTraverse.Field("_spawnedUnit").GetValue<UnitSpawn>();
            switch (__instance.unitID)
            {
                case "AIUCAV":
                case "GAV-25":
                case "ASF-30":
                case "ASF-33":
                case "ASF-58":
                case "EBomberAI":
                case "ABomberAI":
                case "E-4":
                case "KC-49":
                    GameObject rwrObject = new GameObject();
                    rwrObject.transform.parent = unitSpawn.actor.transform;
                    ModuleRWR rwr = rwrObject.AddComponent<ModuleRWR>();
                    unitSpawn.actor.gameObject.GetComponent<AIPilot>().moduleRWR = rwr;
                    break;
            }
        }
    }

    public static void SetupEvasiveManuevers(UnitSpawner __instance)
    {
        if (CheesesAITweaks.settings.allAICanEvade)
        {
            Traverse unitSpawnerTraverse = Traverse.Create(__instance);
            UnitSpawn unitSpawn = unitSpawnerTraverse.Field("_spawnedUnit").GetValue<UnitSpawn>();
            switch (__instance.unitID)
            {
                case "E-4":
                case "KC-49":
                    unitSpawn.actor.gameObject.GetComponent<AIPilot>().allowEvasiveManeuvers = true;
                    break;
            }
        }
    }

    public static void SetupVisualTargetFinders(UnitSpawner __instance)
    {
        if (CheesesAITweaks.settings.enemyEjectorSeats)
        {
            Traverse unitSpawnerTraverse = Traverse.Create(__instance);
            UnitSpawn unitSpawn = unitSpawnerTraverse.Field("_spawnedUnit").GetValue<UnitSpawn>();
            switch (__instance.unitID)
            {
            }
        }
    }
}
