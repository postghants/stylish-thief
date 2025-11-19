using UnityEngine;

public class RickDeathTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per fuck
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            Debug.Log("DEATH");
            other.transform.parent.transform.localPosition = GetComponentInParent<RickDeathManager>().currentCheckpoint.transform.position;
        }
    }
}
