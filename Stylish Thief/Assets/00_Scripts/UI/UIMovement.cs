using UnityEngine;

public class UIMovement : MonoBehaviour
{
    public Vector2 target;
    public float speed;
    public bool move;

    private RectTransform rect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (new Vector2(transform.position.x, transform.position.y) != target)
        {
            if (move)
            {
                transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);
            }
        }
        else
        {
            move = false;
        }
    }
}
