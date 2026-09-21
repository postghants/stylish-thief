using UnityEngine;

public class Card_Behavior : MonoBehaviour
{
    public GameObject Dealer;
    public Vector3 rotation;
    public float speed;
    public float fallspeed;
    public float acceleration;
    public Vector3 direction;

    private void Start()
    {
        direction = transform.forward;
    }

    void Update()
    {
        fallspeed += acceleration * Time.deltaTime;
        this.transform.Rotate(rotation * Time.deltaTime);
        Vector3 movement = speed * Time.deltaTime * direction;
        movement.y += fallspeed * Time.deltaTime;

        transform.position += movement;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Spawner"))
        {

        }
        else
        {
            Destroy(this.gameObject);
        }
    }

}
