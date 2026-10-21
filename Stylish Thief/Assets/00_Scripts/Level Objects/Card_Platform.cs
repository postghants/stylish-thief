using System.Collections;
using UnityEngine;

public class Card_Platform : MonoBehaviour
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
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Spawner") || other.gameObject.CompareTag("Card"))
        {

        }
        else
        {
            speed = 0;
            acceleration = 0;
            StartCoroutine(DestroyAfterSeconds(4));
        }
    }

    private IEnumerator DestroyAfterSeconds(float time)
    {
        yield return new WaitForSeconds(4f);
        Destroy(this.gameObject);

    }

}
