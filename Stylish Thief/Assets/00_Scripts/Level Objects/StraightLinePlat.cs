using UnityEngine;

public class StraightLinePlat : MonoBehaviour
{
    [SerializeField] private Vector3 direction;
    [SerializeField] private float speed;
    [SerializeField] private float reverseTime;

    private float timer;
    private bool reversed;

    private void FixedUpdate()
    {
        if (!reversed)
        {
            transform.Translate(direction.normalized * speed);
            timer += Time.deltaTime;
            if(timer > reverseTime) { reversed = true; }
        }
        else
        {
            transform.Translate(direction.normalized * -speed);
            timer -= Time.deltaTime;
            if (timer < 0) { reversed = false; }
        }
    }
}
