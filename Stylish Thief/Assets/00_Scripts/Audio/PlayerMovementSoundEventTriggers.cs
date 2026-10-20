using System;
using UnityEngine;

public class PlayerMovementSoundEventTriggers : MonoBehaviour
{
public void TriggerWwiseJumpEvent()
    {
        AkUnitySoundEngine.PostEvent("Jump_Event", gameObject);
    }

public void TriggerWwiseRightFootstepEvent()
    {
        AkUnitySoundEngine.PostEvent("Right_Footstep_Event", gameObject);
    }

    public void TriggerWwiseLeftFootstepEvent()
    {
        AkUnitySoundEngine.PostEvent("Left_Footstep_Event", gameObject);
    }

    public void TriggerWwiseLandingEvent()
    {
        AkUnitySoundEngine.PostEvent("Landing_Event", gameObject);
    }

    public void TriggerWwiseDashEvent()
    {
        AkUnitySoundEngine.PostEvent("Dash_Event", gameObject);
    }
}
