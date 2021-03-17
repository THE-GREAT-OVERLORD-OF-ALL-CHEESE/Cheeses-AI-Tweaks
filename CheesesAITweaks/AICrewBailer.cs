using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class AICrewBailer : MonoBehaviour
{
    public AIEjectPilot[] crew;

	public MinMax prebailTime;
	public MinMax bailInterval;

    public Vector3[] spawnPositions;

    public void SetupCrew(int crewAmmount, Rigidbody rb) {
        prebailTime = new MinMax(3, 5);
        bailInterval = new MinMax(0.5f, 1);

        spawnPositions = new Vector3[] { new Vector3(-4.69f, -0.392f, -15.84f), new Vector3(4.69f, -0.392f, -15.84f) };

        Debug.Log("Trying to get ejector seat!");
        GameObject ejectorSeatPrefab = UnitCatalogue.GetUnitPrefab("FA-26B AI").GetComponentInChildren<AIEjectPilot>(true).gameObject;
        Debug.Log("Got ejector seat!");


        crew = new AIEjectPilot[crewAmmount];
        for (int i = 0; i < crewAmmount;  i++) {
            GameObject ejectorSeat = Instantiate(ejectorSeatPrefab, transform);
            ejectorSeat.transform.localPosition = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Length-1)];
            ejectorSeat.transform.localRotation = Quaternion.identity;
            ejectorSeat.transform.parent = transform;

            AIEjectPilot ejectPilot = ejectorSeat.GetComponent<AIEjectPilot>();
            ejectPilot.parentRb = rb;
            ejectPilot.OnBegin.RemoveAllListeners();

            ejectPilot.booster.burnTime = 0;
            ejectPilot.delay = new MinMax(0,0);
            ejectPilot.ejectDelay = 0;
            ejectPilot.parachuteDelay = 2;

            crew[i] = ejectPilot;
        }
    }

    public void BeginBailout() {
		StartCoroutine(BailRoutine());
	}

	private IEnumerator BailRoutine()
	{
		yield return new WaitForSeconds(prebailTime.Random());

        foreach (AIEjectPilot bailedCrew in crew) {
            bailedCrew.BeginEjectSequence();
            yield return new WaitForSeconds(bailInterval.Random());
        }
		yield break;
	}
}
