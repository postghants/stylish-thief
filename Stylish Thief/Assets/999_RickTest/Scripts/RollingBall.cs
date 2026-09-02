using UnityEngine;

public class RollingBall : MonoBehaviour
{
    public float rollSpeed;
    //Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //rb = GetComponentInChildren<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //rb.linearVelocity = transform.forward * rollSpeed;
        transform.position += transform.forward * rollSpeed * Time.deltaTime;
    }
    /*private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 22)
        {
            transform.rotation = other.transform.rotation;
            Debug.Log("Hello hello hello");
        }
    }*/
}
