using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EffectsHandler : MonoBehaviour
{
    public Volume volume;
    
    [HeaderAttribute("Normal Volume")] 
    public VolumeProfile normalVolume;
    
    [HeaderAttribute("Black Hole Effect")]
    public float blackHoleEffectDuration;
    public AnimationCurve lensDistortionTimeEffect;


    [HeaderAttribute("Camera Switch Effect")]
    public Material retroMat;
    public AnimationCurve staticCurve;
    public float cameraSwitchEffectDuration = 2f;
    private float _cameraSwitchTimer;
    private bool _switchEffectOn;
    
    [HeaderAttribute("Console Slide Effect")]
    public Material consoleMat;
    public AnimationCurve staticCurveConsole;
    public float consoleSlideEffectDuration = 2f;
    private float _consoleSlideTimer;
    private bool _consoleSlideEffectOn;

    
    // State
    private bool _blackHoleEffectActive;
    private float _blackHoleEffectTimer;
    private LensDistortion _lensDistortion;
    private ChromaticAberration _chromaticAberration;

    // CACHED REFERENCES
    private static readonly int StaticStrength = Shader.PropertyToID("_StaticStrength");
    private static readonly int DistortionStrength = Shader.PropertyToID("_DistortionStrength");

    private void Awake()
    {
        volume.profile = normalVolume;
    }

    private void Start()
    {
        volume.profile.TryGet(out _lensDistortion);
       volume.profile.TryGet(out _chromaticAberration);
       
       EventManager.Instance.Register<CameraSwitch>((e) => { _switchEffectOn = true;});
       EventManager.Instance.Register<ObjectScanned>((e) => { _consoleSlideEffectOn = true;});
       EventManager.Instance.Register<BlackHoleEffectActive>((e) =>
       {
           _blackHoleEffectActive = ((BlackHoleEffectActive) e).Active;
       });
    }

    private void Update()
    {
        if (_blackHoleEffectActive)
        {
            if (_blackHoleEffectTimer < blackHoleEffectDuration)
            {
                _blackHoleEffectTimer += Time.deltaTime;
                _lensDistortion.intensity.value = lensDistortionTimeEffect.Evaluate(_blackHoleEffectTimer / blackHoleEffectDuration);
            }
            else
            {
                _lensDistortion.active = false;
                _chromaticAberration.active = false;
                _blackHoleEffectActive = false;
            }
        }

        if (_switchEffectOn)
        {
            CameraSwitchEffect();
        }
        if (_consoleSlideEffectOn)
        {
            ConsoleSlideEffect();
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
    
    private void ConsoleSlideEffect()
    {
        if (_consoleSlideTimer < consoleSlideEffectDuration)
        {
            _consoleSlideTimer += Time.deltaTime;
            consoleMat.SetFloat(StaticStrength, staticCurveConsole.Evaluate(_consoleSlideTimer/consoleSlideEffectDuration));
            consoleMat.SetFloat(DistortionStrength, staticCurveConsole.Evaluate(_consoleSlideTimer/consoleSlideEffectDuration));
        }
        else
        {
            _consoleSlideEffectOn = false;
            _consoleSlideTimer = 0;
        }
    }
}