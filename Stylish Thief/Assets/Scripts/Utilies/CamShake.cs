using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public static class CamShake
{
    public static IEnumerator ShakeCam(float amplitude, float frequency, float time, CinemachineCamera cam)
    {
        CinemachineBasicMultiChannelPerlin noise = (CinemachineBasicMultiChannelPerlin)cam.GetCinemachineComponent(CinemachineCore.Stage.Noise);
        if (noise == null) { yield break; }

        noise.AmplitudeGain = amplitude;
        noise.FrequencyGain = frequency;
        yield return new WaitForSeconds(time);
        noise.FrequencyGain = 0;
        noise.AmplitudeGain = 0;
    }
}
