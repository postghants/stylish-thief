using UnityEngine;
using System.Collections;

public class Flip_Platform : MonoBehaviour
{
    public GameObject FlipPlat;
    public float FlipSpeed = 200f;
    public Vector3 rotate;

    void Start()
    {
        StartCoroutine(Flipping());
    }

    public IEnumerator Flipping()
    {
        while (rotate != new Vector3(FlipSpeed * Time.deltaTime, 0f, 0f))
        {
            transform.Rotate(new Vector3(FlipSpeed * Time.deltaTime, 0f, 0f));
        }
        yield return new WaitForSeconds(3f);
        StartCoroutine(Flipping());
    }
}
