using Unity.VisualScripting;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    [SerializeField] private CapsuleCollider coll;
    [SerializeField] private Collider patrolZone;
    public GameObject playerFollower;
    public bool theChaseIsOn;
    Rigidbody rb;
    public float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //if (gameObject.)
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (patrolZone != null)
        {
            if (patrolZone.gameObject.GetComponent<PatrolZone>().playerIsHere)
            {
                theChaseIsOn = true;
            }
            else
            {
                theChaseIsOn = false;
            }
        }
    }
    private void FixedUpdate()
    {
        if (theChaseIsOn)
        {
            transform.Translate(Vector3.forward * speed);
            //transform.LookAt(playerFollower.transform.position - new Vector3(0, playerFollower.transform.position.y, 0));
            //var targetTransform;
            var targetPosition = playerFollower.transform.position;
            targetPosition.y = transform.position.y;
            transform.LookAt(targetPosition);

            //transform.rotation = new Quaternion(transform.rotation.eulerAngles - new Vector3(0, transform.rotation.eulerAngles.y, 0));
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Patrol Zone"))
        {
            Debug.Log("Registered");
            patrolZone = other;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Destroy(gameObject);
    }
}
