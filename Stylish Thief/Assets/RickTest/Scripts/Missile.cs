using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public class Missile : MonoBehaviour
{
    public float speed;
    public float turnSpeed;
    public float explosionLength;
    public PlayerStateDriver player;
    private Vector3 target;
    [SerializeField] private DamageDealer explosion;
    [SerializeField] private GameObject visual;
    [SerializeField] private GameObject vfxPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponentInParent<CrimeSpreeManager>().playerInstance;
    }

    // Update is called once per frame
    void Update()
    {
        target = player.transform.position;
        target.y -= .5f;
        Vector3 lookDirection = player.transform.position - transform.position;
        lookDirection.Normalize();

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), turnSpeed * Time.deltaTime);
        transform.position += transform.forward * speed * Time.deltaTime;
    }
    public void BlowUp()
    {
        visual.SetActive(false);
        speed = 0;
        explosion.gameObject.SetActive(true);
        GameObject vfx = Instantiate(vfxPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject, explosionLength);
    }
}
//To do later:
//Make it slow down when the rotation angle would be super big or maybe make them just stop tracking you
