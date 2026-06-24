using UnityEngine;

public class CameraCullingButBad : MonoBehaviour
{
    public float verticalOffset;
    [SerializeField] Transform start;
    Transform end;
    public PlayerStateDriver player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerStateDriver>(); //Yes I know this is super bad for performance. It's only a test solution
        }
    }

    // Update is called once per frame
    void Update()
    {
        end = player.transform;
        float distance = Vector3.Distance(start.position, end.position);
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, distance);
        transform.position = (start.position + end.position) / 2;
        transform.LookAt(end.position + new Vector3(0, verticalOffset, 0));
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 0)
        {
            if (other.GetComponent<Renderer>() != null)
            {
                other.GetComponent<Renderer>().enabled = false;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 0)
        {
            if (other.GetComponent<Renderer>() != null)
            {
                other.GetComponent<Renderer>().enabled = true;
            }
        }
    }
}
