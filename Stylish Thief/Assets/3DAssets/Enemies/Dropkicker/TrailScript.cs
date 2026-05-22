using UnityEngine;

public class TrailScript : MonoBehaviour
{

    public TrailRenderer trailRendererL;
    public TrailRenderer trailRendererR;

    public void TrailGoesONR()
    {
        trailRendererR.emitting = true;
    }

    public void TrailGoesONL()
    {
        trailRendererL.emitting = true;
    }

    public void TrailGoesOFFR()
    {
        Debug.Log("Turning off trail R");
        trailRendererR.Clear();
        trailRendererR.emitting = false;
    }
    public void TrailGoesOFFL()
    {
        trailRendererR.Clear();
        trailRendererL.emitting = false;
    }
} //gameObject.GetComponent<TrailRenderer>().enabled=false; ?????
