using UnityEngine;
using System;
using System.Collections;
public static class Helpers
{

}

public static class ExtensionMethods {
 
    public static float Remap (this float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    public static Texture2D ToTexture2D(this RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        tex.wrapMode = TextureWrapMode.Clamp;
            // ReadPixels looks at the active RenderTexture.
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }
}
// Mut code taken from: https://gitlab.com/mutmedia/mutcommon/-/tree/master/Runtime 
public static class MonoBehaviourExtensions
{
    public static void DoAfterTime(this MonoBehaviour obj, float time, Action action)
    {
        obj.StartCoroutine(CoroutineHelpers.DoAfterTimeCoroutine(time, action));
    }

    public static void DoNextFrame(this MonoBehaviour obj, Action action) => DoAfterFrames(obj, 1, action);

    public static void DoAfterFrames(this MonoBehaviour obj, int frames, Action action)
    {
        obj.StartCoroutine(CoroutineHelpers.SkipFramesCoroutine(frames, action));
    }
}

public static class CoroutineHelpers
{
    public static IEnumerator InterpolateByTime(float time, System.Action<float> interpolator, Action callback = null)
        => InterpolateByTimeCustom(() => Time.deltaTime, time, interpolator, callback);

    public static IEnumerator InterpolateByTimeFixed(float time, System.Action<float> interpolator, Action callback = null)
        => InterpolateByTimeCustom(() => Time.fixedDeltaTime, time, interpolator, callback);

    public static IEnumerator InterpolateByUnscaledTimeFixed(float time, System.Action<float> interpolator, Action callback = null)
        => InterpolateByTimeCustom(() => Time.fixedUnscaledDeltaTime, time, interpolator, callback);

    public static IEnumerator InterpolateByUnscaledTime(float time, System.Action<float> interpolator, Action callback = null)
        => InterpolateByTimeCustom(() => Time.unscaledDeltaTime, time, interpolator, callback);

    public static IEnumerator InterpolateByTimeCustom(Func<float> deltaTimeGetter, float time, System.Action<float> interpolator, Action callback = null)
    {
        for (float t = 0f; t < time; t += deltaTimeGetter())
        {
            var k = t / time;
            interpolator(k);
            yield return null;
        }
        interpolator(1);
        callback?.Invoke();
    }

    public static IEnumerator DoAfterTimeCoroutine(float time, Action action)
    {
        yield return new WaitForSeconds(time);

        action();
    }

    public static IEnumerator DoAfterRealtimeTimeCoroutine(float time, Action action)
    {
        yield return new WaitForSecondsRealtime(time);

        action();
    }

    public static IEnumerator SkipFramesCoroutine(int frames, Action action)
    {
        for (int i = 0; i < frames; i++)
            yield return null;

        action();
    }
}


