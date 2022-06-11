using System.Collections.Generic;
using Shapes;
using TMPro;
using UnityEngine;

public class CamerasView : MonoBehaviour
{
    
    //Properties
    [ColorUsage(true, true)] public Color activeCameraColor;
    [ColorUsage(true, true)] public Color passiveCameraColor;
    [ColorUsage(true, true)] public Color blinkCameraColor;
    public float blinkingRate;

    //References
    public GameObject display;
    public GameObject activeCameraName;
    public List<TextMeshPro> cameraTexts;
    public List<ShapeRenderer> cameraCircles;
    public List<GameObject> cameraObjs;

    //State
    private int _blinkingCamera;
    private float _blinkingTimer;
    private bool _blinkingState;
    private bool _blink;

    private void Start()
    {   
        display.SetActive(false);
        
        EventManager.Instance.Register<GameStarted>((e) => { display.SetActive(true); });
        EventManager.Instance.Register<InputAllowed>((e) => { display.SetActive(true); });
        
        EventManager.Instance.Register<StartTutorial>((e) =>
        {
            display.SetActive(false);
            // Disable Cameras
            foreach (var obj in cameraObjs)
            {
                obj.SetActive(false);
            }
            cameraObjs[0].SetActive(true);
        });

        EventManager.Instance.Register<SetCameraActive>((e) =>
        {
            SetCameraActive sca = (SetCameraActive) e;
            SetActiveCam(sca.CameraName, sca.Active);
        });
        
        EventManager.Instance.Register<CameraSwitch>((e) => {            
            CameraSwitch cs = (CameraSwitch) e;
            TurnOnCam(cs.NewCam);
        });

        TurnOnCam("none");
    }

    private void Update()
    {
        if (_blink)
        {
            BlinkCamera();
        }
    }

    private void TurnOnCam(string cam)
    {
        
        // Top
        cameraTexts[0].color = cam == "Top"? activeCameraColor: passiveCameraColor;
        cameraCircles[0].Color = cam == "Top"? activeCameraColor: passiveCameraColor;
        
        // Overboard
        cameraTexts[1].color = cam == "Overboard"? activeCameraColor: passiveCameraColor;
        cameraCircles[1].Color = cam == "Overboard"? activeCameraColor: passiveCameraColor;
        
        // Front
        cameraTexts[2].color = cam == "Front"? activeCameraColor: passiveCameraColor;
        cameraCircles[2].Color = cam == "Front"? activeCameraColor: passiveCameraColor;
        
        // Main
        cameraTexts[3].color = cam == "Main"? activeCameraColor: passiveCameraColor;
        cameraCircles[3].Color = cam == "Main"? activeCameraColor: passiveCameraColor;
        
        // Steering
        cameraTexts[4].color = cam == "Steering"? activeCameraColor: passiveCameraColor;
        cameraCircles[4].Color = cam == "Steering"? activeCameraColor: passiveCameraColor;
        
        // Core
        cameraTexts[5].color = cam == "Core"? activeCameraColor: passiveCameraColor;
        cameraCircles[5].Color = cam == "Core"? activeCameraColor: passiveCameraColor;
        
        string name = cam == "Front"? "Front Sail": cam;
        name = name == "Main"? "Main Sail": name;
        activeCameraName.GetComponent<TextMeshPro>().text = name;
        //activeCameraName.GetComponent<ShapesScaler>().Scale();
        
        if (_blink)
        {
            switch (cam)
            {
                case "Top":
                {
                    if (_blinkingCamera == 0) _blink = false;
                    break;
                }
                case "Overboard":
                {
                    if (_blinkingCamera == 1) _blink = false;
                    break;
                }
                case "Front":
                {
                    if (_blinkingCamera == 2) _blink = false;
                    break;
                }
                case "Main":
                {
                    if (_blinkingCamera == 3) _blink = false;
                    break;
                }
                case "Steering":
                {
                    if (_blinkingCamera == 4) _blink = false;
                    break;
                }
                case "Core":
                {
                    if (_blinkingCamera == 5) _blink = false;
                    break;
                }
            } 
        }
    }

    private void SetActiveCam(string cam, bool active)
    {
        _blink = true;

        switch (cam)
        {
            case "Top":
            {
                _blinkingCamera = 0;
                cameraObjs[0].SetActive(active);
                break;
            }
            case "Overboard":
            {
                _blinkingCamera = 1;
                cameraObjs[1].SetActive(active);
                break;
            }
            case "Front":
            {
                _blinkingCamera = 2;
                cameraObjs[2].SetActive(active);
                break;
            }
            case "Main":
            {
                _blinkingCamera = 3;
                cameraObjs[3].SetActive(active);
                break;
            }
            case "Steering":
            {
                _blinkingCamera = 4;
                cameraObjs[4].SetActive(active);
                break;
            }
            case "Core":
            {
                _blinkingCamera = 5;
                cameraObjs[5].SetActive(active);
                break;
            }
        }
    }

    private void BlinkCamera()
    {
        if (_blinkingTimer < blinkingRate)
        {
            _blinkingTimer += Time.deltaTime;
        }
        else
        {
            _blinkingState = !_blinkingState;
            cameraTexts[_blinkingCamera].color = _blinkingState? blinkCameraColor: passiveCameraColor;
            cameraCircles[_blinkingCamera].Color = _blinkingState? blinkCameraColor: passiveCameraColor;
            _blinkingTimer = 0;
        }
    }
}