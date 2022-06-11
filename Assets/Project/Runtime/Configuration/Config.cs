using UnityEngine;

[CreateAssetMenu(fileName = "Config", menuName = "Config Object", order = 1)]
public class Config : ScriptableObject
{
    [HeaderAttribute("Boat Values")] public float turningFactor;
    
    [HeaderAttribute("X Camera Sensitivity")]
    public float xCameraSensitivityKeyboard;
    public float xCameraSensitivityGamepad;

    [HeaderAttribute("Y Camera Sensitivity")]
    public float yCameraSensitivityKeyboard;  
    public float yCameraSensitivityGamepad;
    
    [HeaderAttribute("Dolly Sensitivity Overboard")]
    public float dollySensitivityKeyboardOverboard;
    public float dollySensitivityGamepadOverboard;
    
    [HeaderAttribute("Dolly Sensitivity Sails")]
    public float dollySensitivityKeyboardSails;
    public float dollySensitivityGamepadSails;
    
    [HeaderAttribute("Camera Death Zone")]
    public float cameraDeathZoneKeyboard;
    public float cameraDeathZoneGamepad;
    
    [HeaderAttribute("Sail Raise Sensitivity")]
    public float sailRaiseSensitivityKeyboard;
    public float  sailRaiseSensitivityGamepad;
    
    [HeaderAttribute("Tiller Sensitivity")]
    public float tillerSensitivityKeyboard;
    public float  tillerSensitivityGamepad;

    [HeaderAttribute("Front Sail Ranges")]
    public Vector2 runningRange;
    public Vector2 broadReachRange;
    public Vector2 beamReachRange;
    public Vector2 closeReachRange;
    public Vector2 closeHauledRange;
    
    [HeaderAttribute("Main Sail Ranges")]
    public Vector2 main_runningRange;
    public Vector2 main_broadReachRange;
    public Vector2 main_beamReachRange;
    public Vector2 main_closeReachRange;
    public Vector2 main_closeHauledRange;

    public float GetCameraSensitivityX()
    {
        if (GameManager.Instance.inputs.currentControlScheme == "Keyboard")
        {
            return xCameraSensitivityKeyboard;
        }
        return xCameraSensitivityGamepad;
    }    
    
    public float GetCameraSensitivityY()
    {
        if (GameManager.Instance.inputs.currentControlScheme == "Keyboard")
        {
            return yCameraSensitivityKeyboard;
        }
        return yCameraSensitivityGamepad;
    }
    
    public float GetDollySensitivity()
    {
        if (GameManager.Instance.inputs.currentControlScheme == "Keyboard")
        {
            return GameManager.Instance.inputs.currentActionMap.name == "Overboard Cameras" ? dollySensitivityKeyboardOverboard : dollySensitivityKeyboardSails;
        }
        return GameManager.Instance.inputs.currentActionMap.name == "Overboard Cameras" ? dollySensitivityGamepadOverboard : dollySensitivityGamepadSails;
    }
    
    public float GetDeathZone()
    {
        if (GameManager.Instance.inputs.currentControlScheme == "Keyboard")
        {
            return cameraDeathZoneKeyboard;
        }
        else
        {
            return cameraDeathZoneGamepad;
        }
    }

    public float GetSailRaiseSensitivity()
    {
        if (GameManager.Instance.inputs.currentControlScheme == "Keyboard")
        {
            return sailRaiseSensitivityKeyboard;
        }
        else
        {
            return sailRaiseSensitivityGamepad;
        }
    }    
    
    public float GetTillerSensitivity()
    {
        if (GameManager.Instance.inputs.currentControlScheme == "Keyboard")
        {
            return tillerSensitivityKeyboard;
        }
        else
        {
            return tillerSensitivityGamepad;
        }
    }

}
