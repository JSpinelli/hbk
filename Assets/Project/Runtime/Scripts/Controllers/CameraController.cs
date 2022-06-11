using System;
using Cinemachine;
using FMODUnity;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [HeaderAttribute("Movement Limits")] public float orbitAngleLimit = 45;
    public float pitchAngleLimit = 45;

    [HeaderAttribute("Enable Orbit Clamp")]
    public bool orbitClampEnabled = true;

    [HeaderAttribute("Enable Dolly Clamp")]
    public bool dollyClampEnabled = true;
    
    [HeaderAttribute("Camera Name")]
    public string cameraName;

    //References
    public StringReference activeCam;
    public StudioEventEmitter cameraMovementSound;
    public StudioEventEmitter cameraPanningSound;
    
    // Memory saving variables
    private float _orbitAngle;
    private float _pitchAngle;

    //Input Trackers
    private Vector2 _cameraDir;
    private float _dollyMovement;

    //Awake Init
    private CinemachineVirtualCamera _camera;
    private CinemachineTrackedDolly _cameraDolly;

    // Start Init
    private Config _sessionConfig;
    private float _startingXRot;
    private float _startingYRot;

    private void Awake()
    {
        _camera = GetComponent<CinemachineVirtualCamera>();
        _cameraDolly = _camera.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    private void Start()
    {
        InputManager.Instance.Register(OnPanning, "Panning",true,true);
        InputManager.Instance.Register(OnMovement, "Movement",true,true);
        
        Vector3 rot = transform.localRotation.eulerAngles;
        _startingXRot = rot.x;
        _startingYRot = rot.y;
        _sessionConfig = GameManager.Instance.currentConfig;
    }

    private void Update()
    {
        if (activeCam.Value != cameraName) return;
        if (_camera.isActiveAndEnabled)
        {
            if (Math.Abs(_cameraDir.x) > _sessionConfig.GetDeathZone())
                _orbitAngle += _cameraDir.x * _sessionConfig.GetCameraSensitivityX() * Time.deltaTime;
            if (Math.Abs(_cameraDir.y) > _sessionConfig.GetDeathZone())
                _pitchAngle -= _cameraDir.y * _sessionConfig.GetCameraSensitivityY() * Time.deltaTime;

            _pitchAngle = Mathf.Clamp(_pitchAngle, -pitchAngleLimit, pitchAngleLimit);
            if (orbitClampEnabled)
                _orbitAngle = Mathf.Clamp(_orbitAngle, -orbitAngleLimit, orbitAngleLimit);

            transform.localRotation = Quaternion.Euler(_startingXRot + _pitchAngle, _startingYRot + _orbitAngle, 0);

            if (_cameraDolly)
            {
                _cameraDolly.m_PathPosition += _dollyMovement * _sessionConfig.GetDollySensitivity();
                if (dollyClampEnabled)
                    _cameraDolly.m_PathPosition = Mathf.Clamp(_cameraDolly.m_PathPosition, 0, 1);
            }
        }
        else
        {
            _cameraDir = Vector2.zero;
        }
    }
    
    public void OnPanning(InputAction.CallbackContext value)
    {
        if (_camera.isActiveAndEnabled)
        {
            _cameraDir = value.ReadValue<Vector2>();
        }
    }

    public void OnMovement(InputAction.CallbackContext value)
    {
        _dollyMovement = value.ReadValue<float>();
    }
}