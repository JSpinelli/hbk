using System.Collections.Generic;
using Cinemachine;
using FMODUnity;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraPositionController : MonoBehaviour
{
    // Global
    public StringReference activeCamera;

    // References

    public StudioEventEmitter cameraSwitch;

    // Property
    public List<CinemachineVirtualCamera> cameras;
    public List<string> cameraNames;

    // State
    private int _cameraIndex;
    private bool[] _activeCams;

    private int verticalCounter;
    private int horizontalCounter;
    
    private void Start()
    {
        InputManager.Instance.Register(OnChangeCameraOne, "Change Camera One");
        InputManager.Instance.Register(OnChangeCameraTwo, "Change Camera Two");
        InputManager.Instance.Register(OnChangeCameraThree, "Change Camera Three");
        InputManager.Instance.Register(OnChangeCameraFour, "Change Camera Four");
        InputManager.Instance.Register(OnChangeCameraFive, "Change Camera Five");
        InputManager.Instance.Register(OnChangeCameraSix, "Change Camera Six");
        InputManager.Instance.Register(OnChangeCameraController, "Change Camera Controller");
        
        EventManager.Instance.Register<GameStarted>(OnGameStart);
        EventManager.Instance.Register<StartTutorial>(OnTutorialStart);
        EventManager.Instance.Register<SetCameraActive>(OnSetCameraActive);

        foreach (var cam in cameras)
        {
            cam.Priority = 0;
        }

        _activeCams = new bool[cameras.Count];
        for (int i = 0; i < _activeCams.Length; i++)
        {
            _activeCams[i] = true;
        }
    }

    private void OnGameStart(HBKEvent e)
    {
        _cameraIndex = 5;
        activeCamera.Value = "Steering";
        EventManager.Instance.Fire(new CameraSwitch("Steering"));
        SetActiveCam(activeCamera.Value, true);
    }

    private void OnTutorialStart(HBKEvent e)
    {
        cameras[0].Priority = 10;
        _cameraIndex = 0;
        activeCamera.Value = "Top";
        SetActiveCam(activeCamera.Value, true);
        for (int i = 1; i < _activeCams.Length; i++)
        {
            _activeCams[i] = false;
        }
    }

    private void OnSetCameraActive(HBKEvent e)
    {
        SetCameraActive sca = (SetCameraActive) e;
        SetActiveCam(sca.CameraName, true);
    }

    private void SetActiveCam(string cam, bool active)
    {
        switch (cam)
        {
            case "Top":
            {
                //activeCamera.Value = "Top";
                _activeCams[0] = active;
                break;
            }
            case "Overboard":
            {
                //activeCamera.Value = "Overboard";
                _activeCams[1] = active;
                break;
            }
            case "Main":
            {
                //activeCamera.Value = "Main";
                _activeCams[2] = active;
                break;
            }
            case "Front":
            {
                //activeCamera.Value = "Front";
                _activeCams[3] = active;
                break;
            }
            case "Steering":
            {
                //activeCamera.Value = "Steering";
                _activeCams[4] = active;
                break;
            }
            case "Core":
            {
                //activeCamera.Value = "Core";
                _activeCams[5] = active;
                break;
            }
        }
    }


    private void ChangeCamera(int cam) 
    {
        if (!_activeCams[cam]) return;
        cameras[_cameraIndex].Priority = 1;
        _cameraIndex = cam;
        cameras[cam].Priority = 10;
        EventManager.Instance.Fire(new CameraSwitch(cameraNames[cam]));
        cameraSwitch.Play();
        activeCamera.Value = cameraNames[cam];
    }

    private void OnChangeCameraOne(InputAction.CallbackContext value)
    {
        if (_activeCams[0])
            ChangeCamera(0);
    }

    private void OnChangeCameraTwo(InputAction.CallbackContext value)
    {
        if (_activeCams[1])
            ChangeCamera(1);
    }

    private void OnChangeCameraThree(InputAction.CallbackContext value)
    {
        if (_activeCams[2])
            ChangeCamera(2);
    }

    private void OnChangeCameraFour(InputAction.CallbackContext value)
    {
        if (_activeCams[3])
            ChangeCamera(3);
    }

    private void OnChangeCameraFive(InputAction.CallbackContext value)
    {
        if (_activeCams[4])
            ChangeCamera(4);
    }

    private void OnChangeCameraSix(InputAction.CallbackContext value)
    {
        if (_activeCams[5])
            ChangeCamera(5);
    }
    
    private void OnChangeCameraController(InputAction.CallbackContext value)
    {
        Vector2 val = value.ReadValue<Vector2>();
        int newCam = _cameraIndex;
        if (val.x != 0)
        {
            if (val.x < 0)
            {
                horizontalCounter++;
                if (horizontalCounter > 2)
                {
                    horizontalCounter = 0;
                }
            }

            if (val.x > 0)
            {
                horizontalCounter--;
                if (horizontalCounter < 0)
                {
                    horizontalCounter = 2;
                }
            }

            switch (horizontalCounter)
            {
                case 0:
                {
                    newCam = 4;
                    break;
                }

                case 1:
                {
                    newCam = 2;
                    break;
                }

                case 2:
                {
                    newCam = 3;
                    break;
                }
            }
        }
        else if (val.y != 0)
        {
            if (val.y < 0)
            {
                verticalCounter++;
                if (verticalCounter > 2)
                {
                    verticalCounter = 0;
                }
            }

            if (val.y > 0)
            {
                verticalCounter--;
                if (verticalCounter < 0)
                {
                    verticalCounter = 2;
                }
            }

            switch (verticalCounter)
            {
                case 0:
                {
                    newCam = 0;
                    break;
                }

                case 1:
                {
                    newCam = 5;
                    break;
                }

                case 2:
                {
                    newCam = 1;
                    break;
                }
            }
        }
        else
        {
            return;
        }
        if (_activeCams[newCam])
            ChangeCamera(newCam);
    }
    
}