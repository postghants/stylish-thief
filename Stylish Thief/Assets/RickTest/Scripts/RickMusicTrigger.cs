using Unity.VisualScripting;
using UnityEngine;
using FMODUnity;

public class RickMusicTrigger : MonoBehaviour
{
    //public int musicalState;
    //bool undoYourselfUponExit;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            Debug.Log("Player entered");
            RuntimeManager.StudioSystem.setParameterByName("areaType", 1);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            Debug.Log("Player left");
            RuntimeManager.StudioSystem.setParameterByName("areaType", 0);
        }
    }
}
