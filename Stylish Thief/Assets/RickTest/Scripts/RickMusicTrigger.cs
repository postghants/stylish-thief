using Unity.VisualScripting;
using UnityEngine;
using FMODUnity;

public class RickMusicTrigger : MonoBehaviour
{
    public int musicalState;
    //bool undoYourselfUponExit;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            Debug.Log("Player entered");
            RuntimeManager.StudioSystem.setParameterByName("gamestate", musicalState);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            Debug.Log("Player entered");
            RuntimeManager.StudioSystem.setParameterByName("gamestate", musicalState);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            Debug.Log("Player left");
            RuntimeManager.StudioSystem.setParameterByName("gamestate", 0);
        }
    }
}
