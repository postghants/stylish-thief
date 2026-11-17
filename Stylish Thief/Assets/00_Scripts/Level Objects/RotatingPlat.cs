using UnityEngine;

public class RotatingPlat : MonoBehaviour
{
    public float speed;

    private void Update()
    {
        transform.Rotate(0, speed * Time.deltaTime, 0);
    }
}
