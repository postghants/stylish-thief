using Unity.VisualScripting;
using UnityEngine;

public class SniperLaser : MonoBehaviour
{
    public float verticalOffset;
    [SerializeField] Transform start;
    [SerializeField] Transform end;
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
}
