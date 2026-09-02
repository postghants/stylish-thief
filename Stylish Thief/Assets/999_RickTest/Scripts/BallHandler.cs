using UnityEngine;

public class BallHandler : MonoBehaviour
{
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
        if (other.gameObject.layer == 22)
        {
            other.transform.rotation = transform.rotation;
            Debug.Log("Hello hello hello");
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 22)
        {
            collision.transform.rotation = transform.rotation;
            Debug.Log("Hello hello hello");
        }
    }
}
