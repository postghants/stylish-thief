using UnityEngine;
using static UnityEditor.PlayerSettings;

public class CrimeSpreeAudioTrigger : MonoBehaviour
{
    void OnTriggerEnter()
    {
        AkUnitySoundEngine.PostEvent("Crime_Spree_Feedback", gameObject);
    }
}
