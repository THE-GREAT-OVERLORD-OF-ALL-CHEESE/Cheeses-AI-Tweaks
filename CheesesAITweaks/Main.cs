using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Harmony;
using System.Reflection;
using Valve.Newtonsoft.Json;
using System.IO;
using UnityEngine.Events;

public class CheesesAISettings
{
    public bool controlNoiseEnabled = true;

    public float controlNoiseIntensity = 0.05f;
    public float controlNoiseFrequency = 2f;
    public float controlNoiseLargeAircraftFrequency = 5f;

    public int controlNoiseLayers = 3;
    public bool allowRefuelingNoise = false;

    public float formationDriftIntensity = 3f;
    public float formationDriftFrequency = 5f;

    public bool rockWingsOnContact = true;

    public CheesesAITweaks.DropTankMode dropTankMode = CheesesAITweaks.DropTankMode.DropOnContact;

    public bool enemyEjectorSeats = true;

    public CheesesAITweaks.CollisionMode collisionMode = CheesesAITweaks.CollisionMode.Normal;

    public bool rockShips = false;
    public float windSpeed = 50f;

    public bool allAIHaveRWR = true;
    public bool allAICanEvade = true;
    public bool tweakTargetFinders = true;

    public bool eyes = false;
    public bool invertedAI = false;
}

public class CheesesAITweaks : VTOLMOD
{
    public enum CollisionMode {
        Normal,
        CollidersAlwaysOn,
        CollidersAlwaysOff
    }

    public enum DropTankMode
    {
        Normal,
        DropOnContact
        //DropOnMerge
    }

    public static CheesesAISettings settings;
    public static CheesesAITweaks instance;

    public bool settingsChanged;


    public UnityAction<bool> controlNoiseEnabled_changed;

    public UnityAction<float> controlNoiseIntensity_changed;
    public UnityAction<float> controlNoiseFrequency_changed;
    public UnityAction<float> controlNoiseLargeAircraftFrequency_changed;

    public UnityAction<int> controlNoiseLayers_changed;
    public UnityAction<bool> allowRefuelingNoise_changed;

    public UnityAction<float> formationDriftIntensity_changed;
    public UnityAction<float> formationDriftFrequency_changed;

    public UnityAction<bool> rockWingsOnContact_changed;

    public UnityAction<int> dropTankMode_changed;

    public UnityAction<bool> enemyEjectorSeats_changed;

    public UnityAction<int> collisionMode_changed;

    public UnityAction<bool> rockShips_changed;
    public UnityAction<float> windSpeed_changed;

    public UnityAction<bool> allAIHaveRWR_changed;
    public UnityAction<bool> allAICanEvade_changed;
    public UnityAction<bool> tweakTargetFinders_changed;

    public UnityAction<bool> eyes_changed;
    public UnityAction<bool> invertedAI_changed;

    public static Dictionary<AutoPilot, CheeseAIHelper> apToHelper;
    public static Dictionary<AIPilot, CheeseAIHelper> aiToHelper;

    public GameObject googlyEyePrefab;

    public override void ModLoaded()
    {
        HarmonyInstance harmony = HarmonyInstance.Create("cheese.cheesesAITweaks");
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        Debug.Log("Patched the AI");

        instance = this;

        base.ModLoaded();
        VTOLAPI.SceneLoaded += SceneLoaded;
        VTOLAPI.MissionReloaded += MissionReloaded;

        apToHelper = new Dictionary<AutoPilot, CheeseAIHelper>();
        aiToHelper = new Dictionary<AIPilot, CheeseAIHelper>();

        settings = new CheesesAISettings();
        LoadFromFile();

        StartCoroutine(LoadAssetBundle());

        Settings modSettings = new Settings(this);
        modSettings.CreateCustomLabel("Cheese's AI Tweaks Settings");

        modSettings.CreateCustomLabel("");

        modSettings.CreateCustomLabel("Control Noise Settings");
        modSettings.CreateCustomLabel("The control noise adds random noise to the AIs inputs, to make them seem more human");

        controlNoiseEnabled_changed += controlNoiseEnabled_Setting;
        modSettings.CreateCustomLabel("Control Noise Enabled:");
        modSettings.CreateBoolSetting("(Default = true)", controlNoiseEnabled_changed, settings.controlNoiseEnabled);

        controlNoiseIntensity_changed += controlNoiseIntensity_Setting;
        modSettings.CreateCustomLabel("Control Noise Intensity:");
        modSettings.CreateCustomLabel("This is the size of the noise added to the inputs");
        modSettings.CreateFloatSetting("(Default = 5%)", controlNoiseIntensity_changed, settings.controlNoiseIntensity * 100, 0, 100);

        controlNoiseFrequency_changed += controlNoiseFrequency_Setting;
        modSettings.CreateCustomLabel("Control Noise Frequency:");
        modSettings.CreateCustomLabel("This is the frequency at which the noise changes");
        modSettings.CreateFloatSetting("(Default = 2)", controlNoiseFrequency_changed, settings.controlNoiseFrequency, 1, 60);

        controlNoiseLargeAircraftFrequency_changed += controlNoiseLargeAircraftFrequency_Setting;
        modSettings.CreateCustomLabel("Control Noise Frequency:");
        modSettings.CreateCustomLabel("This is the frequency at which the noise changes for large aircraft");
        modSettings.CreateFloatSetting("(Default = 5)", controlNoiseLargeAircraftFrequency_changed, settings.controlNoiseLargeAircraftFrequency, 0, 60);

        controlNoiseLayers_changed += controlNoiseLayers_Setting;
        modSettings.CreateCustomLabel("Control Noise Layers:");
        modSettings.CreateCustomLabel("This is the ammount of perlin noise layers used to create the noise");
        modSettings.CreateIntSetting("(Default = 3)", controlNoiseLayers_changed, settings.controlNoiseLayers, 0, 5);

        allowRefuelingNoise_changed += allowRefuelingNoise_Setting;
        modSettings.CreateCustomLabel("Allow noise when refueling another aircraft:");
        modSettings.CreateCustomLabel("Makes air to air refueling much harder");
        modSettings.CreateBoolSetting("(Default = false)", allowRefuelingNoise_changed, settings.allowRefuelingNoise);

        /*formationDriftIntensity_changed += formationDriftIntensity_Setting;
        modSettings.CreateCustomLabel("Formation Drift Intensity:");
        modSettings.CreateCustomLabel("This is the radius the aircraft can drift from its target");
        modSettings.CreateFloatSetting("(Default = 3m)", formationDriftIntensity_changed, settings.formationDriftIntensity, 0, 10);

        formationDriftFrequency_changed += formationDriftFrequency_Setting;
        modSettings.CreateCustomLabel("Formation Drift Frequency:");
        modSettings.CreateCustomLabel("This is the frequency at which the aircraft drifts in formation");
        modSettings.CreateFloatSetting("(Default = 5)", formationDriftFrequency_changed, settings.formationDriftFrequency, 1, 60);*/

        modSettings.CreateCustomLabel("");

        
        rockWingsOnContact_changed += rockWingsOnContact_Setting;
        modSettings.CreateCustomLabel("Rock wings on contact:");
        modSettings.CreateBoolSetting("(Default = true)", rockWingsOnContact_changed, settings.rockWingsOnContact);

        modSettings.CreateCustomLabel("");


        dropTankMode_changed += dropTankMode_Setting;
        modSettings.CreateCustomLabel("Drop Tank Mode:");
        modSettings.CreateIntSetting("(Default = 0)", dropTankMode_changed, (int)settings.dropTankMode, 0, 1);
        modSettings.CreateCustomLabel("0 = Normal (Only drop when empty)");
        modSettings.CreateCustomLabel("1 = Drop On Contact");

        modSettings.CreateCustomLabel("");


        enemyEjectorSeats_changed += enemyEjectorSeats_Setting;
        modSettings.CreateCustomLabel("Enemy Aircraft Ejector Seats:");
        modSettings.CreateBoolSetting("(Default = true)", enemyEjectorSeats_changed, settings.enemyEjectorSeats);

        modSettings.CreateCustomLabel("");


        collisionMode_changed += collisionMode_Setting;
        modSettings.CreateCustomLabel("Collsion Mode:");
        modSettings.CreateIntSetting("(Default = 0)", collisionMode_changed, (int)settings.collisionMode, 0, 2);
        modSettings.CreateCustomLabel("0 = Normal");
        modSettings.CreateCustomLabel("1 = Always On");
        modSettings.CreateCustomLabel("2 = Always Off");

        modSettings.CreateCustomLabel("");

        modSettings.CreateCustomLabel("AI aircraft don't have collisions on the ground normally,");
        modSettings.CreateCustomLabel("You can enable them, but the AI could get stuck");
        modSettings.CreateCustomLabel("on missions with many taxiing aircraft");

        modSettings.CreateCustomLabel("");
        modSettings.CreateCustomLabel("Detection Settings");

        allAIHaveRWR_changed += allAIHaveRWR_Setting;
        modSettings.CreateCustomLabel("RWR on all AI:");
        modSettings.CreateCustomLabel("Enemy AI do not normally have RWR");
        modSettings.CreateCustomLabel("This will give them a RWR and make them more situationally aware");
        modSettings.CreateBoolSetting("(Default = false)", allAIHaveRWR_changed, settings.allAIHaveRWR);

        allAICanEvade_changed += allAICanEvade_Setting;
        modSettings.CreateCustomLabel("RWR on all AI:");
        modSettings.CreateCustomLabel("AWACS and the KC-49 cannot normaly evade missiles");
        modSettings.CreateBoolSetting("(Default = true)", allAICanEvade_changed, settings.allAICanEvade);

        //rockShips_changed += rockShips_Setting;
        //modSettings.CreateCustomLabel("Rock Ships:");
        //modSettings.CreateCustomLabel("Should ships be rocked by the wind?");
        //modSettings.CreateBoolSetting("(Default = true)", rockShips_changed, settings.rockShips);

        //windSpeed_changed += windSpeed_Setting;
        //modSettings.CreateCustomLabel("Wind Speed (kts):");
        //modSettings.CreateCustomLabel("This is the windspeed used to work out how much to rock the ships");
        //modSettings.CreateFloatSetting("(Default = 50kts)", windSpeed_changed, settings.windSpeed, 0f, 100f);

        /*
        tweakTargetFinders_changed += tweakTargetFinders_Setting;
        modSettings.CreateCustomLabel("Tweak Visual Target Finders:");
        modSettings.CreateBoolSetting("(Default = true)", tweakTargetFinders_changed, settings.tweakTargetFinders);
        */

        modSettings.CreateCustomLabel("");

        modSettings.CreateCustomLabel("Joke Settings");

        modSettings.CreateCustomLabel("");

        eyes_changed += eyes_Setting;
        modSettings.CreateCustomLabel("Googly Eyes:");
        modSettings.CreateCustomLabel("Adds googly eyes to various AI units");
        modSettings.CreateBoolSetting("(Default = false)", eyes_changed, settings.eyes);

        invertedAI_changed += invertedAI_Setting;
        modSettings.CreateCustomLabel("Inverted AI:");
        modSettings.CreateCustomLabel("Causes the AI to fly upside down");
        modSettings.CreateBoolSetting("(Default = false)", invertedAI_changed, settings.invertedAI);

        modSettings.CreateCustomLabel("");

        modSettings.CreateCustomLabel("Please feel free to @ me on the discord if");
        modSettings.CreateCustomLabel("you think of any more features I could add!");
        VTOLAPI.CreateSettingsMenu(modSettings);
    }

    private void SceneLoaded(VTOLScenes scene)
    {
        CheckSave();
    }

    private void MissionReloaded()
    {
        CheckSave();
    }

    private void OnApplicationQuit() {
        CheckSave();
    }

    private void CheckSave() {
        Debug.Log("Checking if settings were changed.");
        if (settingsChanged) {
            Debug.Log("Settings were changed, saving changes!");
            SaveToFile();
        }
    }

    public void LoadFromFile()
    {
        string address = ModFolder;
        Debug.Log("Checking for: " + address);

        if (Directory.Exists(address))
        {
            Debug.Log(address + " exists!");
            try
            {
                Debug.Log("Checking for: " + address + @"\settings.json");
                string temp = File.ReadAllText(address + @"\settings.json");

                settings = JsonConvert.DeserializeObject<CheesesAISettings>(temp);
                settingsChanged = false;
            }
            catch
            {
                Debug.Log("no json found, making one");
                SaveToFile();
            }
        }
        else
        {
            Debug.Log("Mod folder not found?");
        }
    }

    public void SaveToFile()
    {
        string address = ModFolder;
        Debug.Log("Checking for: " + address);

        if (Directory.Exists(address))
        {
            Debug.Log("Saving settings!");
            File.WriteAllText(address + @"\settings.json", JsonConvert.SerializeObject(settings));
            settingsChanged = false;
        }
        else
        {
            Debug.Log("Mod folder not found?");
        }
    }

    public void controlNoiseEnabled_Setting(bool newval)
    {
        settings.controlNoiseEnabled = newval;
        settingsChanged = true;
    }

    public void controlNoiseIntensity_Setting(float newval)
    {
        settings.controlNoiseIntensity = newval/100f;
        settingsChanged = true;
    }

    public void controlNoiseFrequency_Setting(float newval)
    {
        settings.controlNoiseFrequency = newval;
        settingsChanged = true;
    }

    public void controlNoiseLargeAircraftFrequency_Setting(float newval)
    {
        settings.controlNoiseLargeAircraftFrequency = newval;
        settingsChanged = true;
    }

    public void controlNoiseLayers_Setting(int newval)
    {
        settings.controlNoiseLayers = newval;
        settingsChanged = true;
    }

    public void allowRefuelingNoise_Setting(bool newval)
    {
        settings.allowRefuelingNoise = newval;
        settingsChanged = true;
    }

    public void formationDriftIntensity_Setting(float newval)
    {
        settings.formationDriftIntensity = newval;
        settingsChanged = true;
    }

    public void formationDriftFrequency_Setting(float newval)
    {
        settings.formationDriftFrequency = newval;
        settingsChanged = true;
    }

    public void rockWingsOnContact_Setting(bool newval)
    {
        settings.rockWingsOnContact = newval;
        settingsChanged = true;
    }

    public void dropTankMode_Setting(int newval)
    {
        settings.dropTankMode = (DropTankMode)newval;
        settingsChanged = true;
    }

    public void enemyEjectorSeats_Setting(bool newval)
    {
        settings.enemyEjectorSeats = newval;
        settingsChanged = true;
    }

    public void collisionMode_Setting(int newval)
    {
        settings.collisionMode = (CollisionMode)newval;
        settingsChanged = true;
    }

    public void allAIHaveRWR_Setting(bool newval)
    {
        settings.allAIHaveRWR = newval;
        settingsChanged = true;
    }
    public void allAICanEvade_Setting(bool newval)
    {
        settings.allAICanEvade = newval;
        settingsChanged = true;
    }

    public void tweakTargetFinders_Setting(bool newval)
    {
        settings.tweakTargetFinders = newval;
        settingsChanged = true;
    }

    public void rockShips_Setting(bool newval)
    {
        settings.rockShips = newval;
        settingsChanged = true;
    }

    public void windSpeed_Setting(float newval)
    {
        settings.windSpeed = newval;
        settingsChanged = true;
    }

    public void eyes_Setting(bool newval)
    {
        settings.eyes = newval;
        settingsChanged = true;
    }

    public void invertedAI_Setting(bool newval)
    {
        settings.invertedAI = newval;
        settingsChanged = true;
    }

    public AIEjectPilot AddEjectorSeat(Health health, Rigidbody rb, Vector3 position)
    {
        Debug.Log("Trying to get ejector seat!");
        GameObject ejectorSeatPrefab = UnitCatalogue.GetUnitPrefab("FA-26B AI").GetComponentInChildren<AIEjectPilot>(true).gameObject;
        Debug.Log("Got ejector seat!");

        GameObject ejectorSeat = Instantiate(ejectorSeatPrefab, health.transform);
        ejectorSeat.transform.localPosition = position;
        ejectorSeat.transform.localRotation = Quaternion.identity;

        AIEjectPilot ejectPilot = ejectorSeat.GetComponent<AIEjectPilot>();
        health.OnDeath.AddListener(ejectPilot.BeginEjectSequence);
        ejectPilot.parentRb = rb;
        ejectPilot.OnBegin.RemoveAllListeners();
        return ejectPilot;
    }

    public GameObject AddBomberDoors(Transform aircraft, AIEjectPilot ejector, bool rightDoor)
    {
        GameObject ejectorDoorPrefab;
        Debug.Log("Trying to get ejector door!");
        if (rightDoor)
        {
            ejectorDoorPrefab = UnitCatalogue.GetUnitPrefab("ABomberAI").transform.Find("CockpitPart").Find("ejectPanelRight.002").gameObject;
        }
        else {
            ejectorDoorPrefab = UnitCatalogue.GetUnitPrefab("ABomberAI").transform.Find("CockpitPart").Find("ejectPanelLeft.002").gameObject;
        }
        Debug.Log("Got ejector door!");

        GameObject ejectorDoor = Instantiate(ejectorDoorPrefab, aircraft);
        ejectorDoor.transform.localPosition = new Vector3(0, -3.582781f, 7.093267f);
        ejectorDoor.transform.localRotation = Quaternion.Euler(-90,0,0);

        ejectorDoor.SetActive(false);
        ejector.canopyObject = ejectorDoor;
        ejector.canopyBooster = ejectorDoor.GetComponentInChildren<SolidBooster>();
        ejector.OnBegin.AddListener(delegate { ejectorDoor.SetActive(true); });
        return ejectorDoor;
    }

    public GameObject AddFACockpit(Transform aircraftTransform, Vector3 position, Vector3 scale)
    {
        Debug.Log("Trying to get cockpit interior!");
        GameObject cockpitPrefab = FindObject(UnitCatalogue.GetUnitPrefab("FA-26B AI").transform, "lowPolyInterior").gameObject;
        Debug.Log("Got cockpit interior!");

        GameObject cockpit = Instantiate(cockpitPrefab, aircraftTransform);
        cockpit.transform.localPosition = position;
        cockpit.transform.localEulerAngles = new Vector3(-90, 0, -180f);
        cockpit.transform.localScale = scale;

        return cockpit;
    }

    public GameObject AddGooglyEye(Transform aircraftTransform, Vector3 position, Vector3 rotation, float scale, bool mirror = false)
    {
        GameObject googlyEye = Instantiate(googlyEyePrefab, aircraftTransform);
        googlyEye.transform.localPosition = position;
        googlyEye.transform.localRotation = Quaternion.Euler(rotation);
        googlyEye.transform.localScale = Vector3.one * scale;

        GooglyEye eye = googlyEye.AddComponent<GooglyEye>();
        eye.scale = scale;

        if (mirror) {
            AddGooglyEye(aircraftTransform, new Vector3(-position.x, position.y, position.z), new Vector3(rotation.x, -rotation.y, rotation.z), scale);
        }

        return googlyEye;
    }

    public Transform FindObject(Transform parent, string name)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child.name == name)
            {
                return child;
            }
        }
        return null;
    }

    private IEnumerator LoadAssetBundle()
    {
        Debug.Log("Checking " + ModFolder + @"\eyes.ab");
        if (!File.Exists(ModFolder + @"\eyes.ab"))
        {
            Debug.Log("Asset bundle does not exist");
            yield break;
        }
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(ModFolder + @"\eyes.ab");
        yield return request;

        AssetBundleRequest eyeRequest = request.assetBundle.LoadAssetAsync("Googly Eye", typeof(GameObject));
        yield return eyeRequest;
        if (eyeRequest.asset == null)
        {
            Debug.Log("Couldnt find googly eye prefab");
            yield break;
        }
        googlyEyePrefab = eyeRequest.asset as GameObject;

        Debug.Log("Googly Eye prefab loaded!");
    }
}