using Unity.VisualScripting;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    [SerializeField]
    private AK.Wwise.RTPC locationCheck;


    void OnTriggerEnter()
    {
        locationCheck.SetGlobalValue(1);
    }

    private void OnTriggerExit()
    {
        locationCheck.SetGlobalValue(0);
    }
}
