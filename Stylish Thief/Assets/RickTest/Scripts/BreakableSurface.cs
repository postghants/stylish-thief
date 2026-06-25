using UnityEngine;

public class BreakableSurface : MonoBehaviour
{
    [Header("Crime stuff")]
    [Tooltip("How many points does the player get from this?")] public float givenScore;
    [Tooltip("What is the name of this crime?")] public string crime;
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
        if (other.gameObject.layer == 6)
        {
            Destroy(transform.parent.gameObject);
            CrimeSpreeManager.instance.DoMinorCrime(givenScore, crime, gameObject);
        }
    }
}
