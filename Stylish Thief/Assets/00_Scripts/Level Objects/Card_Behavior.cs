using UnityEngine;

public class Card_Behavior : MonoBehaviour
{
    public GameObject Dealer;
    public Vector3 rotation;
    public float speed;
    public float fallspeed;
    public float acceleration;

    void Update()
    {
        this.transform.Rotate(rotation * 1 * Time.deltaTime);
        transform.position += new Vector3(0, (fallspeed + acceleration) * Time.deltaTime, speed * Time.deltaTime);
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
