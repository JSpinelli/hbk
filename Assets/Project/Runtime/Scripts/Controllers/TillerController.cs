using FMODUnity;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.InputSystem;

public class TillerController : MonoBehaviour
{
    // Global
    public FloatReference tillerPos;
    
    // Properties
    public Transform tillerOrigin;
    public AnimationCurve tillerVelocity;
    public StudioEventEmitter tillerAudio;
    public StringReference activeCam;
    public float recenterCorrection;

    // Input Tracker
    private float _tillerDir;
    
    // Memory Save
    private float _tillerVal;

    // Start Init
    private Config _sessionConfig;

    void Start()
    {
        _sessionConfig = GameManager.Instance.currentConfig;
        InputManager.Instance.Register(OnTiller, "Tiller",true,true);
    }

    private void Update()
    {
        if (activeCam.Value != "Steering" && transform.localRotation.eulerAngles.y != 0)
        {
            transform.RotateAround(tillerOrigin.position, tillerOrigin.up,
                Mathf.Sign(transform.localRotation.eulerAngles.y - 65) * recenterCorrection);
        }

        if (_tillerDir > 0 &&
            (transform.localRotation.eulerAngles.y < 60 || transform.localRotation.eulerAngles.y > 295))
        {
            transform.RotateAround(tillerOrigin.position, tillerOrigin.up,
                _tillerDir * _sessionConfig.GetTillerSensitivity());
        }

        if (_tillerDir < 0 &&
            (transform.localRotation.eulerAngles.y > 300 || transform.localRotation.eulerAngles.y < 65))
        {
            transform.RotateAround(tillerOrigin.position, tillerOrigin.up,
                _tillerDir * _sessionConfig.GetTillerSensitivity());
        }

        _tillerVal = Mathf.Sign(transform.localRotation.y) *
                     tillerVelocity.Evaluate(Mathf.Abs(transform.localRotation.y));

        tillerPos.Value = _tillerVal * _sessionConfig.turningFactor;
    }

    private void OnTiller(InputAction.CallbackContext value)
    {
        _tillerDir = value.ReadValue<float>();
    }
    
}