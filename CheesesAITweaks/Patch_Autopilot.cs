using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;

[HarmonyPatch(typeof(AutoPilot), "UpdateAutopilot")]
class Patch_Autopilot_UpdateAutopilot
{
    [HarmonyPrefix]
    static void Postfix(AutoPilot __instance)
    {
        if (CheesesAITweaks.settings.controlNoiseEnabled) {
            if (CheesesAITweaks.apToHelper.ContainsKey(__instance)) {
                CheeseAIHelper helper = CheesesAITweaks.apToHelper[__instance];

                Traverse apTraverse = new Traverse(__instance);
                Vector3 output = apTraverse.Field("output").GetValue<Vector3>() + helper.GetControlNoise();
                float outputThrottle = apTraverse.Field("outputThrottle").GetValue<float>() + helper.GetThottleNoise();
                for (int i = 0; i < __instance.outputs.Length; i++)
                {
                    __instance.outputs[i].SetPitchYawRoll(output);
                    __instance.outputs[i].SetThrottle(outputThrottle);
                }

                if (__instance.controlThrottle)
                {
                    foreach (ModuleEngine moduleEngine in __instance.engines)
                    {
                        moduleEngine.SetThrottle(outputThrottle);
                    }
                }
            }
        }
    }
}

[HarmonyPatch(typeof(AutoPilot), "RollTargetVersion2")]
class Patch_AutoPilot_RollTargetVersion2
{
    [HarmonyPrefix]
    static bool Prefix(AutoPilot __instance, ref Vector3 __result, Vector3 targetVector, Vector3 angularRollVector, Vector3 tfUp)
    {
        if (CheesesAITweaks.settings.invertedAI) {
            __instance.maxBank = 190;
            __result = (targetVector.normalized - __instance.rb.velocity.normalized) * -1f + (angularRollVector + 0.01f * tfUp) + __instance.rollUpBias * (125f / Mathf.Max(0.1f, __instance.currentSpeed)) * -Vector3.up;
            return false;
        }
        else {
            return true;
        }
    }
}

[HarmonyPatch(typeof(AutoPilot), "SetOverrideRollTarget")]
class Patch_AutoPilot_UpdateAutopilot
{
    [HarmonyPrefix]
    static bool Prefix(AutoPilot __instance, Vector3 rollTarget)
    {
        if (CheesesAITweaks.settings.invertedAI)
        {
            __instance.maxBank = 190;
            Traverse ap = new Traverse(__instance);
            ap.Field("useRollOverride").SetValue(true);
            ap.Field("overrideRollTarget").SetValue(-rollTarget);
            return false;
        }
        else
        {
            return true;
        }
    }
}