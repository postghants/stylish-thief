using UnityEngine;
using System.Collections;


public class Blink_Block : MonoBehaviour
{
    public Collider Col;
    public Renderer Ren;


    void Start() 
    {
        StartCoroutine(Blinking());
    }

    public IEnumerator Blinking()
    {
        if (Col.enabled == true || Ren.enabled == true)
        {
            Col.enabled = false;
            Ren.enabled = false;

            yield return new WaitForSeconds(3f);
        }
        else
        {

        }
        Col.enabled = true;
        Ren.enabled = true;

        yield return new WaitForSeconds(3f);
        StartCoroutine(Blinking());
    }
}
