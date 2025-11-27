using UnityEngine;

public class RickRopeEnd : MonoBehaviour
{
    public RickRopePull pull;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6 && pull.pulling)
        {
            pull.StopPulling();
            Debug.Log("Touched");
        }
    }
}
