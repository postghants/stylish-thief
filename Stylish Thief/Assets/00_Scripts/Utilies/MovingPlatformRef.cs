using System;
using UnityEngine;

public class MovingPlatformRef : MonoBehaviour
{
    private Vector3 currentPos;
    private Quaternion currentRot;
    private Vector3 previousPos;
    private Quaternion previousRot;

    public MovingPlatform movingPlatform;

    public void Init()
    {
        currentPos = transform.position;
        previousPos = transform.position - movingPlatform.GetVelocity(transform.position);

        currentRot = transform.rotation;
        previousRot = movingPlatform.GetPreviousRot();
    }

    public Vector3 GetVelocity()
    {
        return (currentPos - previousPos) / Time.deltaTime;
    }

    public void UpdatePosition(Vector3 movement)
    {
        movement.y = 0;
        previousPos = currentPos + movement;
        previousRot = currentRot;

        transform.position += movement;
        currentPos = transform.position;
        currentRot = transform.rotation;
        previousPos.y = currentPos.y;

    }
}
