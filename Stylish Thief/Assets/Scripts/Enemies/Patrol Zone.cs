using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PatrolZone : MonoBehaviour
{
    [SerializeField] private int playerInZone = 0;

    [HideInInspector]public UnityEvent OnPlayerEnter;
    [HideInInspector] public UnityEvent OnPlayerExit;

    private Collider[] colliders;

    private void Start()
    {
        colliders = GetComponents<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.GetComponentInChildren<PlayerStateDriver>() != null)
        {
            if(playerInZone == 0)
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
        foreach(Collider collider in colliders)
        {
            if (collider.ClosestPoint(point) == point)
            {
                return true;
            }
        }
        return false;
    }
}
