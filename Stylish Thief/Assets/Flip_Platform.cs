using UnityEngine;
using System.Collections;

public class Flip_Platform : MonoBehaviour
{
    public GameObject FlipPlat;
    public float FlipSpeed = 1000f;
    public Vector3 rotate;

    void Start()
    {
        StartCoroutine(Flipping());
    }


    public IEnumerator Flipping()
    {

        while (FlipPlat.transform.rotation.eulerAngles.x > -180 && FlipPlat.transform.rotation.eulerAngles.x < 180)
        {
            FlipPlat.transform.Rotate(new Vector3(FlipSpeed * Time.deltaTime, 0f, 0f));
            yield return null;
        }

        FlipPlat.transform.rotation = Quaternion.identity;
        yield return new WaitForSeconds(3f);
        
        StartCoroutine(Flipping());
    }
}
