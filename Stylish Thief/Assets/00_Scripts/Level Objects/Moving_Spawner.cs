using System.Collections;
using UnityEngine;

public class Moving_Spawner : MonoBehaviour
{
    [SerializeField] GameObject pointA;
    [SerializeField] GameObject pointB;
    [SerializeField] float speed = 10f;

    private Vector3 targetPos;
    void Start()
    {
        transform.position = pointA.transform.position;
        targetPos = pointB.transform.position;
        StartCoroutine(MovePlatform());
    }


    IEnumerator MovePlatform()
    {
        while (true)
        {
            if ((targetPos - transform.position).sqrMagnitude > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                
               
                yield return null;
            }
            else
            {
                Destroy(this.gameObject);
            }
            yield return null;
        }
       
    }
}
