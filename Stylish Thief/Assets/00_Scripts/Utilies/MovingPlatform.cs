using UnityEngine;


public class MovingPlatform : MonoBehaviour
{
    private Vector3 currentPos;
    private Quaternion currentRot;
    private Vector3 previousPos;
    private Quaternion previousRot;

    private void Start()
    {
        previousPos = transform.position;
        previousRot = transform.rotation;

        currentPos = transform.position;
        currentRot = transform.rotation;
    }

    private void FixedUpdate()
    {
        previousPos = currentPos;
        previousRot = currentRot;

        currentPos = transform.position;
        currentRot = transform.rotation;
    }

    public Vector3 GetVelocity(Vector3 childPoint)
    {
        Vector3 velocity = Vector3.zero;

        if(previousPos != currentPos)
        {
            velocity += currentPos - previousPos;
        }


        if(previousRot != currentRot)
        {
            Vector3 offset = childPoint - transform.position;

            Quaternion rotation = Quaternion.Euler(currentRot.eulerAngles - previousRot.eulerAngles);

            Vector3 rotated = rotation * offset;

            velocity += rotated - offset;
        }

        return velocity;
    }

    public Quaternion GetPreviousRot()
    {
        return previousRot;
    }
}
