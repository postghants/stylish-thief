using UnityEngine;

public class Arrow : MonoBehaviour
{
    public GameObject player;
    public CrimeSpreeManager manager;
    public int arrowNumber;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (manager.Valuables[arrowNumber] != null)
        {
            transform.LookAt(manager.Valuables[arrowNumber].transform);
        }
    }
}
