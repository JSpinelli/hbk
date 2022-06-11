using UnityEngine;
using UnityEngine.InputSystem;

public class TrailerEffects : MonoBehaviour
{
    [HeaderAttribute("Camera Switch Effect")]
    public Material retroMat;
    public AnimationCurve staticCurve;
    public float cameraSwitchEffectDuration = 2f;
    private float _cameraSwitchTimer = 0f;
    private bool _switchEffectOn = false;

    private bool _effectTriggered;
    
    private bool _titleTriggered ;
    public DialogController text;

    // CACHED REFERENCES
    private static readonly int StaticStrength = Shader.PropertyToID("_StaticStrength");
    private static readonly int DistortionStrength = Shader.PropertyToID("_DistortionStrength");
    
    private void Update()
    {
        if (_switchEffectOn)
        {
            CameraSwitchEffect();
        }
    }

    private void CameraSwitchEffect()
    {
        if (_cameraSwitchTimer < cameraSwitchEffectDuration)
        {
            _cameraSwitchTimer += Time.deltaTime;
            retroMat.SetFloat(StaticStrength, staticCurve.Evaluate(_cameraSwitchTimer/cameraSwitchEffectDuration));
            retroMat.SetFloat(DistortionStrength, staticCurve.Evaluate(_cameraSwitchTimer/cameraSwitchEffectDuration));
        }
        else
        {
            _switchEffectOn = false;
            _cameraSwitchTimer = 0;
        }
    }

    private void OnNext(InputValue value)
    {
        if (!_effectTriggered)
        {
            _effectTriggered = true;
            _switchEffectOn = true;
            return;
        }
        if (!_titleTriggered)
        {
            _titleTriggered = true;
            _effectTriggered = false;
            text.AnimateText("Booting up...",null,false);
        }
        else
        {
            _titleTriggered = false;
            _effectTriggered = false;
            text.AnimateText("Approaching destination...",null,false);
        }
    }

}