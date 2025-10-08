using UnityEngine;
using UnityEngine.Events;

public class PatrolZone : MonoBehaviour
{
    [SerializeField] private int playerInZone = 0;

    [HideInInspector]public UnityEvent OnPlayerEnter;
    [HideInInspector] public UnityEvent OnPlayerExit;

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
}
