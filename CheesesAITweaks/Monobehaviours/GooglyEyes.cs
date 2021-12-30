using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GooglyEye : MonoBehaviour
{
    public Rigidbody rb;
    public Transform tf;
    public Transform pupil;
    private Vector2 localPosition;
    private Vector2 localVelocity;
    private Vector3 lastVelocity;

    public float range = 0.25f;
    public float scale = 1f;

    public float bouncyness = 0.8f;

    private void Start()
    {
        tf = transform;
        pupil = tf.Find("Pupil");
        rb = GetComponentInParent<Rigidbody>();

        lastVelocity = rb.GetPointVelocity(tf.position);
    }

    private void Update()
    {
        Gravity();
        Acceleration();
        CollisionsAndMovementUpdate();
    }

    private void Gravity()
    {
        Vector3 localGravity = tf.InverseTransformDirection(Physics.gravity);
        localVelocity += new Vector2(localGravity.x, localGravity.y) * Time.deltaTime;
    }

    private void Acceleration()
    {
        Vector3 currentVelocity = rb.GetPointVelocity(tf.position);
        Vector3 localAcceleration = Vector3.zero;

        if (Time.deltaTime > 0)
        {
            localAcceleration = -tf.InverseTransformDirection((currentVelocity - lastVelocity));
        }

        localVelocity += new Vector2(localAcceleration.x, localAcceleration.y);
        lastVelocity = currentVelocity;
    }

    private void CollisionsAndMovementUpdate()
    {
        localPosition += localVelocity * Time.deltaTime;

        if (localPosition.magnitude > range * scale)
        {
            localVelocity = Vector2.Reflect(localVelocity, -localPosition.normalized) * bouncyness;
            localPosition = Vector2.ClampMagnitude(localPosition, range * scale);
        }

        pupil.localPosition = localPosition / scale;
    }
}