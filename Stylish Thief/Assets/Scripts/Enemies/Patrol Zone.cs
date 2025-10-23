using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PatrolZone : MonoBehaviour
{
    [SerializeField] private int playerInZone = 0;

    public UnityEvent OnPlayerEnter;
    public UnityEvent OnPlayerExit;

    private Collider[] colliders;

    private void Start()
    {
        colliders = GetComponents<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.GetComponentInChildren<PlayerStateDriver>() != null)
        {
            if (playerInZone == 0)
            {
                OnPlayerEnter?.Invoke();
            }
            playerInZone++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.GetComponentInChildren<PlayerStateDriver>() != null)
        {
            playerInZone--;
            if (playerInZone == 0)
            {
                OnPlayerExit?.Invoke();
            }
        }
    }

    public bool IsPlayerInZone()
    {
        return playerInZone > 0;
    }

    public bool IsPointInZone(Vector3 point)
    {
        foreach (Collider collider in colliders)
        {
            if (collider.ClosestPoint(point) == point)
            {
                return true;
            }
        }
        return false;
    }

    public Vector3 ClosestPoint(Vector3 point)
    {
        List<Vector3> points = new List<Vector3>();
        foreach(Collider collider in colliders)
        {
            points.Add(collider.ClosestPoint(point));
        }
        Vector3 closestPoint = Vector3.zero;
        float smallestDistance = Mathf.Infinity;
        foreach(Vector3 closest in points)
        {
            float distance = Vector3.Distance(point, closest);
            if(distance < smallestDistance)
            {
                smallestDistance = distance;
                closestPoint = closest;
            }
        }
        return closestPoint;
    }

    public Vector3 RandomPointInZone()
    {
        Bounds bounds = colliders[Random.Range(0, colliders.Length)].bounds;

        float minX = bounds.size.x * -0.5f;
        float minY = bounds.size.y * -0.5f;
        float minZ = bounds.size.z * -0.5f;

        return
            new Vector3(Random.Range(minX, -minX),
                Random.Range(minY, -minY),
                Random.Range(minZ, -minZ)
        ) + bounds.center;
    }
}
