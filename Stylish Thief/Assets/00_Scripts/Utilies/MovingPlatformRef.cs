using System;
using UnityEngine;

public class MovingPlatformRef : MonoBehaviour
{
    private Vector3 currentPos;
    private Quaternion currentRot;
    private Vector3 previousPos;
    private Quaternion previousRot;

    public GameObject movingPlatform;

    public void Init()
    {
        currentPos = transform.position;
        currentRot = transform.rotation;

        previousPos = transform.position;
        previousRot = transform.rotation;
    }

    public Vector3 GetVelocity()
    {
        return (currentPos - previousPos) / Time.deltaTime;
    }

    public void UpdatePosition(Vector3 velocity)
    {
        previousPos = currentPos + velocity * Time.deltaTime;
        previousRot = currentRot;

        velocity.y = 0;
        transform.position += velocity * Time.deltaTime;
        currentPos = transform.position;
        currentRot = transform.rotation;

    }
}
