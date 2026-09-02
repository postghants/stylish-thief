using Unity.VisualScripting;
using UnityEngine;
using FMODUnity;

public class RickMusicTrigger : MonoBehaviour
{
    public int musicalState;
    private float parameterValue;
    //bool undoYourselfUponExit;
    public bool inside0;
    public bool inside1;
    public bool inside2;

    private void Update()
    {
        if (inside0 && !inside1 && !inside2)
        {
            RuntimeManager.StudioSystem.setParameterByName("gamestate", 0);
        }
        else if (!inside0 && inside1 && !inside2)
        {
            RuntimeManager.StudioSystem.setParameterByName("gamestate", 1);
        }
        else if (!inside0 && !inside1 && inside2)
        {
            RuntimeManager.StudioSystem.setParameterByName("gamestate", 2);
        }
    }
    /*private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            RuntimeManager.StudioSystem.getParameterByName("gamestate", out parameterValue);
            if (parameterValue != musicalState)
            {
                RuntimeManager.StudioSystem.setParameterByName("gamestate", musicalState);
            }
        }
    }*/
    /*private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            RuntimeManager.StudioSystem.getParameterByName("gamestate", out parameterValue);
            if (parameterValue != musicalState)
            {
                RuntimeManager.StudioSystem.setParameterByName("gamestate", musicalState);
            }
        }
    }*/
    /*private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            RuntimeManager.StudioSystem.getParameterByName("gamestate", out parameterValue);
            if (parameterValue != musicalState)
            {
                RuntimeManager.StudioSystem.setParameterByName("gamestate", 0);
            }
        }
    }*/
    public void Set0()
    {
        inside0 = true;
    }
    public void Set1()
    {
        inside1 = true;
    }
    public void Set2()
    {
        inside2 = true;
    }
    public void Unset0()
    {
        inside0 = false;
    }
    public void Unset1()
    {
        inside1 = false;
    }
    public void Unset2()
    {
        inside2 = false;
    }
}
