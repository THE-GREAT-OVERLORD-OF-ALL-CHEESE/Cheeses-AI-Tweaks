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

    public bool rockWingsOnContact = true;

    public CheesesAITweaks.CollisionMode collisionMode = CheesesAITweaks.CollisionMode.Normal;

    public bool invertedAI = false;
}

public class CheesesAITweaks : VTOLMOD
{
    public enum CollisionMode {
        Normal,
        CollidersAlwaysOn,
        CollidersAlwaysOff
    }

    public static CheesesAISettings settings;

    public bool settingsChanged;


    public UnityAction<bool> controlNoiseEnabled_changed;

    public UnityAction<float> controlNoiseIntensity_changed;
    public UnityAction<float> controlNoiseFrequency_changed;
    public UnityAction<float> controlNoiseLargeAircraftFrequency_changed;

    public UnityAction<int> controlNoiseLayers_changed;

    public UnityAction<bool> rockWingsOnContact_changed;

    public UnityAction<int> collisionMode_changed;

    public UnityAction<bool> invertedAI_changed;


    public static Dictionary<AutoPilot, CheeseAIHelper> apToHelper;
    public static Dictionary<AIPilot, CheeseAIHelper> aiToHelper;

    public override void ModLoaded()
    {
        HarmonyInstance harmony = HarmonyInstance.Create("cheese.cheesesAITweaks");
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        Debug.Log("Patched the AI");

        base.ModLoaded();
        VTOLAPI.SceneLoaded += SceneLoaded;
        VTOLAPI.MissionReloaded += MissionReloaded;

        apToHelper = new Dictionary<AutoPilot, CheeseAIHelper>();
        aiToHelper = new Dictionary<AIPilot, CheeseAIHelper>();

        settings = new CheesesAISettings();
        LoadFromFile();

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
        modSettings.CreateCustomLabel("Control Noise Frequency:");
        modSettings.CreateCustomLabel("This is the ammount of perlin noise layers used to create the noise");
        modSettings.CreateIntSetting("(Default = 3)", controlNoiseLayers_changed, settings.controlNoiseLayers, 0, 5);

        modSettings.CreateCustomLabel("");

        
        rockWingsOnContact_changed += rockWingsOnContact_Setting;
        modSettings.CreateCustomLabel("Rock wings on contact:");
        modSettings.CreateBoolSetting("(Default = true)", controlNoiseEnabled_changed, settings.rockWingsOnContact);

        modSettings.CreateCustomLabel("");
        

        collisionMode_changed += collisionMode_Setting;
        modSettings.CreateCustomLabel("Collsion Mode:");
        modSettings.CreateIntSetting("(Default = 0)", collisionMode_changed, (int)settings.collisionMode, 0, 2);
        modSettings.CreateCustomLabel("0 = Normal");
        modSettings.CreateCustomLabel("1 = Always On");
        modSettings.CreateCustomLabel("2 = Always Off");

        modSettings.CreateCustomLabel("");

        invertedAI_changed += invertedAI_Setting;
        modSettings.CreateCustomLabel("Inverted AI:");
        modSettings.CreateCustomLabel("Causes AI the fly upside down");
        modSettings.CreateBoolSetting("(Default = false)", invertedAI_changed, settings.invertedAI);

        modSettings.CreateCustomLabel("");

        modSettings.CreateCustomLabel("AI aircraft don't have collisions on the ground normally,");
        modSettings.CreateCustomLabel("You can enable them, but the AI could get stuck on missions with many taxiing aircraft");

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

    public void rockWingsOnContact_Setting(bool newval)
    {
        settings.rockWingsOnContact = newval;
        settingsChanged = true;
    }

    public void collisionMode_Setting(int newval)
    {
        settings.collisionMode = (CollisionMode)newval;
        settingsChanged = true;
    }

    public void invertedAI_Setting(bool newval)
    {
        settings.invertedAI = newval;
        settingsChanged = true;
    }
}