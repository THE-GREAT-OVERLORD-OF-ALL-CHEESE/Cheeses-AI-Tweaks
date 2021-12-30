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

                    ModuleRWR rwr = unitSpawn.actor.gameObject.GetComponent<ModuleRWR>();

                    if (rwr == null)
                    {
                        rwr = rwrObject.AddComponent<ModuleRWR>();
                        unitSpawn.actor.gameObject.GetComponent<AIPilot>().moduleRWR = rwr;
                    }
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

    public static void SetupGooglyEyes(UnitSpawner __instance)
    {
        if (CheesesAITweaks.settings.eyes)
        {
            Traverse unitSpawnerTraverse = Traverse.Create(__instance);
            UnitSpawn unitSpawn = unitSpawnerTraverse.Field("_spawnedUnit").GetValue<UnitSpawn>();
            switch (__instance.unitID)
            {
                case "FA-26B AI":
                    CheesesAITweaks.instance.AddGooglyEye(unitSpawn.transform, new Vector3(0.66f, 1.254f, 5.922f), new Vector3(-20f, 70f, 0f), 1f, true);
                    break;
                case "F-45A AI":
                    CheesesAITweaks.instance.AddGooglyEye(unitSpawn.transform, new Vector3(0.536f, 0.907f, 5.73f), new Vector3(-20f, 70f, 0f), 1f, true);
                    break;
                case "MQ-31":
                    CheesesAITweaks.instance.AddGooglyEye(unitSpawn.transform, new Vector3(0.923f, 0.907f, 5.73f), new Vector3(-20f, 80f, 0f), 1f, true);
                    break;
                case "ASF-30":
                    CheesesAITweaks.instance.AddGooglyEye(unitSpawn.transform, new Vector3(0.675f, 1.13f, 8.16f), new Vector3(-20f, 80f, 0f), 1.2f, true);
                    break;
                case "ASF-33":
                    CheesesAITweaks.instance.AddGooglyEye(unitSpawn.transform, new Vector3(0.661f, 1.337f, 8.517f), new Vector3(-25f, 80f, 0f), 1.2f, true);
                    break;
                case "ASF-58":
                    CheesesAITweaks.instance.AddGooglyEye(unitSpawn.transform, new Vector3(1.01f, 1.215f, 12.74f), new Vector3(-30f, 80f, 0f), 2f, true);
                    break;
                case "AIUCAV":
                    CheesesAITweaks.instance.AddGooglyEye(unitSpawn.transform, new Vector3(1.01f, 0.63f, 2.65f), new Vector3(-70, 60, 0), 2f, true);
                    break;
                case "KC-49":
                    CheesesAITweaks.instance.AddGooglyEye(unitSpawn.transform, new Vector3(3.19f, 1.439f, 32.283f), new Vector3(-20, 80, 0), 3f, true);
                    CheesesAITweaks.instance.AddGooglyEye(unitSpawn.transform, new Vector3(1.349f, -2.849f, -24.2753f), new Vector3(60, 150, 0), 2f, true);
                    break;
                case "E-4":
                    CheesesAITweaks.instance.AddGooglyEye(unitSpawn.transform, new Vector3(3.19f, 1.439f, 32.283f), new Vector3(-20, 80, 0), 3f, true);
                    CheesesAITweaks.instance.AddGooglyEye(unitSpawn.transform, new Vector3(2f, 12.85f, -2.371f), new Vector3(0, 10, 0), 2f, true);
                    break;
                case "AV-42CAI":
                    CheesesAITweaks.instance.AddGooglyEye(unitSpawn.transform, new Vector3(0.988f, 0.119f, 5.543f), new Vector3(-5, 80, 0), 1f, true);
                    break;
                case "GAV-25":
                    CheesesAITweaks.instance.AddGooglyEye(unitSpawn.transform, new Vector3(0.872f, 0.472f, 4.464f), new Vector3(-15, 80, 0), 1f, true);
                    break;
                case "ABomberAI":
                case "EBomberAI":
                    CheesesAITweaks.instance.AddGooglyEye(unitSpawn.transform, new Vector3(0.788f, -1.08f, 22.465f), new Vector3(70, 90, 0), 1.5f, true);
                    break;
                default:
                    break;
            }
        }
    }
}
