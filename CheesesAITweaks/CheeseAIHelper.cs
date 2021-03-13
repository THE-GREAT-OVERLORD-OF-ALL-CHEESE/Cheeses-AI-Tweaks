using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CheeseAIHelper : MonoBehaviour
{
    public AutoPilot ap;
    public AIPilot ai;
    public Rigidbody rb;

    public float pitchOffset;
    public float yawOffset;
    public float rollOffset;
    public float throttleOffset;

    public float pitchTimeOffset;
    public float yawTimeOffset;
    public float rollTimeOffset;
    public float throttleTimeOffset;

    public bool largeAircraft;
    public float frequncy;

    public bool lastInCombat;

    public Coroutine wingRockRoutine;

    private void Start() {
        Debug.Log("Setting up aircraft noise!");
        pitchOffset = UnityEngine.Random.Range(-10f, 10f);
        yawOffset = UnityEngine.Random.Range(-10f, 10f);
        rollOffset = UnityEngine.Random.Range(-10f, 10f);
        throttleOffset = UnityEngine.Random.Range(-10f, 10f);

        pitchTimeOffset = UnityEngine.Random.Range(-60f, 60f);
        yawTimeOffset = UnityEngine.Random.Range(-60f, 60f);
        rollTimeOffset = UnityEngine.Random.Range(-60f, 60f);
        throttleTimeOffset = UnityEngine.Random.Range(-60f, 60f);

        ap = GetComponent<AutoPilot>();
        if (CheesesAITweaks.apToHelper.ContainsKey(ap) == false) {
            CheesesAITweaks.apToHelper.Add(ap, this);
        }

        ai = GetComponent<AIPilot>();
        if (CheesesAITweaks.aiToHelper.ContainsKey(ai) == false)
        {
            CheesesAITweaks.aiToHelper.Add(ai, this);
        }

        rb = GetComponent<Rigidbody>();

        largeAircraft = ai.parkingSize >= 20;

        if (largeAircraft)
        {
            frequncy = CheesesAITweaks.settings.controlNoiseLargeAircraftFrequency;
        }
        else {
            frequncy = CheesesAITweaks.settings.controlNoiseFrequency;
        }
    }

    private void Update() {
        lastInCombat = ai.commandState == AIPilot.CommandStates.Combat;
    }

    public Vector3 GetControlNoise() {
        Vector3 output = new Vector3();
        output.x = VectorUtils.FullRangePerlinNoise((Time.time + pitchTimeOffset) / frequncy, pitchOffset) * CheesesAITweaks.settings.controlNoiseIntensity;
        output.y = VectorUtils.FullRangePerlinNoise((Time.time + yawTimeOffset) / frequncy, yawOffset) * CheesesAITweaks.settings.controlNoiseIntensity;
        output.z = VectorUtils.FullRangePerlinNoise((Time.time + rollTimeOffset) / frequncy, rollOffset) * CheesesAITweaks.settings.controlNoiseIntensity;
        return output;
    }

    public float GetThottleNoise()
    {
        return VectorUtils.FullRangePerlinNoise((Time.time + throttleTimeOffset) / frequncy, throttleOffset) * CheesesAITweaks.settings.controlNoiseIntensity;
    }

    public void RockWings() {
        if (wingRockRoutine == null) {
            ai.commandState = AIPilot.CommandStates.Override;
            wingRockRoutine = StartCoroutine(WingRockRoutine());
        }
    }

    private IEnumerator WingRockRoutine()
    {
        Debug.Log("Rocking wings!");

        float startTime = Time.time;
        Vector3 direction = rb.velocity.normalized;
        Vector3 right = Vector3.Cross(direction, Vector3.up);
        Vector3 up = Vector3.Cross(right, direction);

        while (true)
        {
            ai.autoPilot.steerMode = AutoPilot.SteerModes.Stable;
            ai.autoPilot.targetPosition = ai.transform.position + direction * 1000f;
            //ai.autoPilot.targetSpeed = ai.maxSpeed;

            ai.autoPilot.SetOverrideRollTarget(up + right * Mathf.Sin((Time.time - startTime) * Mathf.PI));

            if (Time.time - startTime > 2)
            {
                break;
            }
            yield return null;
        }

        Debug.Log("Wing rock complete!");
        ai.commandState = AIPilot.CommandStates.Combat;

        yield break;
    }

    private void OnDestroy() {
        CheesesAITweaks.apToHelper.Remove(ap);
    }
}
