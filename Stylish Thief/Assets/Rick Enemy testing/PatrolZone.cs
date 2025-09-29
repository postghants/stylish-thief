using UnityEngine;

public class PatrolZone : MonoBehaviour
{
    [SerializeField] private BoxCollider coll;
    public bool playerIsHere;
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
        if (other.gameObject.name.Contains("Player"))
        {
            playerIsHere = true;
            Debug.Log("Entered");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Contains("Player"))
        {
            playerIsHere = false;
            Debug.Log("Left");
        }
    }
}
