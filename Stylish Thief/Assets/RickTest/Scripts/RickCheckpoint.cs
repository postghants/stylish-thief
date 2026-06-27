using UnityEngine;

public class RickCheckpoint : MonoBehaviour
{
    RickDeathManager manager;
    PlayerStateDriver playerStateDriver;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GetComponentInParent<RickDeathManager>();
    }

    // Update is called once per frame
    /*void Update()
    {if (playerStateDriver != null)
        {
            if (playerStateDriver.transform.parent.transform.localPosition.y < -5)
            {
                playerStateDriver.transform.parent.transform.localPosition = GetComponentInParent<RickDeathManager>().currentCheckpoint.transform.position;
            }
        }
    }*/
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            playerStateDriver = other.gameObject.GetComponentInParent<PlayerStateDriver>();
            manager.currentCheckpoint = gameObject;
        }
    }
}
