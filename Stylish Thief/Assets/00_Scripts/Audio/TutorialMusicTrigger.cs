using Unity.VisualScripting;
using UnityEngine;
using FMODUnity;

public class TutorialMusicTrigger : MonoBehaviour
{
    public int tutorialMusicState;
    //bool undoYourselfUponExit;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            Debug.Log("Player entered");
            RuntimeManager.StudioSystem.setParameterByName("Tutorial_Progress", tutorialMusicState);
        }
    }
}
