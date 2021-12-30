using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheeseShipRocker : MonoBehaviour
{
    private Rigidbody childRb;
    private Transform tf;

    private float speedMultiplyer = 3f;

    private Vector3 boatSize;

    private Vector2 rockingAngle;

    private Vector2 rockingSpeed;
    private Vector2 rockingTimer;

    private void Start()
    {
        tf = transform;

        GetBoatSize();
        Reparent();

        CalculateRockingFrequncy();
        rockingTimer.x = Random.Range(-60f, 60f);
        rockingTimer.y = Random.Range(-60f, 60f);
    }

    private void GetBoatSize()
    {
        Quaternion originalBoatRot = tf.rotation;
        tf.rotation = Quaternion.identity;

        boatSize = Vector3.one * 20;

        Renderer[] renders = GetComponentsInChildren<Renderer>();
        foreach (Renderer render in renders)
        {
            if (boatSize.x < render.bounds.size.x)
            {
                boatSize.x = render.bounds.size.x;
            }
            if (boatSize.y < render.bounds.size.y)
            {
                boatSize.y = render.bounds.size.y;
            }
            if (boatSize.z < render.bounds.size.z)
            {
                boatSize.z = render.bounds.size.z;
            }
        }

        tf.rotation = originalBoatRot;
    }

    private void Reparent()
    {
        GameObject go = new GameObject();

        Transform childTf = go.transform;
        childTf.position = transform.position;
        childTf.rotation = transform.rotation;

        childRb = go.AddComponent<Rigidbody>();
        childRb.isKinematic = true;
        childRb.interpolation = RigidbodyInterpolation.Interpolate;

        FloatingOriginTransform fo = go.AddComponent<FloatingOriginTransform>();
        fo.SetRigidbody(childRb);

        

        Actor[] actors = GetComponentsInChildren<Actor>(true);
        foreach (Actor actor in actors)
        {
            actor.SetParentRigidbody(childRb);
            Debug.Log($"Set rigidbody on actor: {actor.gameObject.name}");
        }
        Gun[] guns = GetComponentsInChildren<Gun>(true);
        foreach (Gun gun in guns)
        {
            gun.SetParentRigidbody(childRb);
            Debug.Log($"Set rigidbody on gun: {gun.gameObject.name}");
        }
        CarrierCatapult[] cats = GetComponentsInChildren<CarrierCatapult>(true);
        foreach (CarrierCatapult cat in cats)
        {
            cat.parentRb = childRb;
            Debug.Log($"Set rigidbody on cat: {cat.gameObject.name}");
        }
        MovingPlatform[] platforms = GetComponentsInChildren<MovingPlatform>(true);
        foreach (MovingPlatform platform in platforms)
        {
            platform.SetParentRigidbody(childRb);
            Debug.Log($"Set rigidbody on platform: {platform.gameObject.name}");
        }
        //Runway[] runways = GetComponentsInChildren<Runway>();
        //foreach (Runway runway in runways)
        //{
        //    runway.parentActor = childRb;
        //}


        int childCount = tf.childCount;
        for (int i = 0; i < childCount; i++)
        {
            tf.GetChild(0).parent = childTf;
        }

        childTf.parent = transform;
    }

    private void CalculateRockingFrequncy()
    {
        rockingSpeed.x = speedMultiplyer / boatSize.x;
        rockingSpeed.y = speedMultiplyer / boatSize.z;
    }

    private float WindSpeedToWaveHeight(float kts)
    {
        return 0.003f * Mathf.Pow(kts, 2) + 0.06f * kts;
    }

    private void CalculateRockingAngle()
    {
        float height = WindSpeedToWaveHeight(CheesesAITweaks.settings.windSpeed);
        rockingAngle.x = Mathf.Atan2(height, boatSize.x) * Mathf.Rad2Deg;
        rockingAngle.y = Mathf.Atan2(height, boatSize.z) * Mathf.Rad2Deg;
    }

    private void FixedUpdate()
    {
        CalculateRockingAngle();

        rockingTimer.x += rockingSpeed.x * Time.deltaTime;
        rockingTimer.y += rockingSpeed.y * Time.deltaTime;

        childRb.MoveRotation(tf.rotation
            * Quaternion.AngleAxis(Mathf.Sin(rockingTimer.x * Mathf.PI * 2f) * rockingAngle.x, Vector3.forward)
            * Quaternion.AngleAxis(Mathf.Sin(rockingTimer.y * Mathf.PI * 2f) * rockingAngle.y, Vector3.right)
            );
        childRb.MovePosition(tf.position);
    }
}