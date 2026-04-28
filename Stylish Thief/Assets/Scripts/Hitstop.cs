using System.Collections;
using UnityEngine;

public static class Hitstop
{
    public static void SetTimescale(float timescale)
    {
        Time.timeScale = timescale;
    }

    public static IEnumerator LerpTimescale(float target, float time)
    {
        float timer = 0;
        float startScale = Time.timeScale;
        while (timer < time)
        {
            timer += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(startScale, target, time / timer);

            yield return null;
        }
        Time.timeScale = target;
    }

    public static IEnumerator SmoothDampTimescale(float target, float time)
    {
        float timer = 0;
        float smoothDampRef = 0;
        while (timer < time)
        {
            timer += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.SmoothDamp(Time.timeScale, target, ref smoothDampRef, time);

            yield return null;
        }
        Time.timeScale = target;
    }

    public static IEnumerator WhooshTimescale(float target, float descentTime, float holdTime, float ascentTime)
    {
        float timer = 0;
        float smoothDampRef = 0;
        float startScale = Time.timeScale;
        while (timer < descentTime + holdTime)
        {
            timer += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.SmoothDamp(Time.timeScale, target, ref smoothDampRef, descentTime);

            yield return null;
        }

        while(timer < descentTime + holdTime + ascentTime)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        while (timer < descentTime + holdTime + ascentTime)
        {
            timer += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.SmoothDamp(Time.timeScale, startScale, ref smoothDampRef, ascentTime);

            yield return null;
        }

        Time.timeScale = startScale;
    }
}
