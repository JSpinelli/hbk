using UnityEngine;

public class ControlsView : MonoBehaviour
{
    // Properties
    public float blinkingRate;
    [ColorUsage(true, true)] public Color passiveControlColor;
    [ColorUsage(true, true)] public Color blinkControlColor;
    
    // References
    [Header("Scan")] 
    public GameObject scanGamepad;
    public GameObject scanKeyboard;

    [Header("Raise Sail")] 
    public GameObject raiseSailGamepad;
    public GameObject raiseSailKeyboard;

    [Header("Adjust Sail")] 
    public GameObject adjustSailGamepad;
    public GameObject adjustSailKeyboard;

    [Header("Move")] 
    public GameObject moveGamepad;
    public GameObject moveKeyboard;

    [Header("Anchor")] 
    public GameObject anchorGamepad;
    public GameObject anchorKeyboard;

    [Header("Steer")] 
    public GameObject steerGamepad;
    public GameObject steerKeyboard;

    [Header("Look")] 
    public GameObject lookGamepad;
    public GameObject lookKeyboard;

    [Header("Switch Camera")] 
    public GameObject cameraSwitchGamepad;
    public GameObject cameraSwitchKeyboard;

    [Header("Accelerate")] 
    public GameObject accelerateKeyboard;
    public GameObject accelerateGamepad;
    
    [Header("Next")] 
    public GameObject nextKeyboard;
    public GameObject nextGamepad;

    // State
        // Enabled controls
    private string _currentVal = "";
    private bool _lookEnabled;
    private bool _scanEnabled;
    private bool _raiseSailEnabled;
    private bool _adjustSailEnabled;
    private bool _moveEnabled;
    private bool _anchorEnabled;
    private bool _steerEnabled;
    private bool _cameraSwitchEnabled;
    private bool _accelerateEnabled;
    private bool _nextEnabled;
    
        // Blinking
    private string _blinkingInput;
    private float _blinkingTimer;
    private bool _blinkingState;
    private bool _blink;

    private void Start()
    {
        // Backup for lunching the game without going through main menu (Events get fired simultaneusly with subcriptions so Game Started doesn't get triggered)
        _lookEnabled = true;
        _scanEnabled = true;
        _raiseSailEnabled = true;
        _adjustSailEnabled = true;
        _moveEnabled = true;
        _anchorEnabled = true;
        _steerEnabled = true;
        _cameraSwitchEnabled = true;
        
        _nextEnabled = false;

        SetDefault();
        EventManager.Instance.Register<InputChange>((e) => { SetActiveObjects(_currentVal); });
        
        EventManager.Instance.Register<CameraSwitch>((e) =>
        {
            CameraSwitch cs = (CameraSwitch) e;
            _currentVal = cs.NewCam;
            SetActiveObjects(cs.NewCam);
        });
        
        EventManager.Instance.Register<GameStarted>((e) =>
        {
            _lookEnabled = true;
             _scanEnabled = true;
             _raiseSailEnabled = true;
             _adjustSailEnabled = true;
             _moveEnabled = true;
             _anchorEnabled = true;
             _steerEnabled = true;
             _cameraSwitchEnabled = true;
        });        
        
        EventManager.Instance.Register<StartTutorial>((e) =>
        {

             _lookEnabled = false;
             _scanEnabled = false;
             _raiseSailEnabled = false;
             _adjustSailEnabled = false;
             _moveEnabled = false;
             _anchorEnabled = false;
             _steerEnabled = false;
             _cameraSwitchEnabled = false;
             SetActiveObjects("none");
        });
        
        EventManager.Instance.Register<TutorialFinish>((e) =>
        {
            _blink = false;
            SetDefault();
        });
        
        EventManager.Instance.Register<SetInputActive>((e) =>
        {
            SetInputActive sia = (SetInputActive) e;
            if (sia.Enable)
            {
                SetDefault();
                _blinkingInput = sia.Input;
                _blink = true;
            }
            switch (sia.Input)
            {
                case "look":
                {
                    _lookEnabled = sia.Enable;
                    break;
                }
                case "move":
                {
                    _moveEnabled = sia.Enable;
                    break;
                }
                case "steer":
                {
                    _steerEnabled = sia.Enable;
                    break;
                }
                case "raise":
                {
                    _raiseSailEnabled = sia.Enable;
                    break;
                }
                case "adjust":
                {
                    _adjustSailEnabled = sia.Enable;
                    break;
                }
                case "scan":
                {
                    _scanEnabled = sia.Enable;
                    break;
                }
                case "camera":
                {
                    _cameraSwitchEnabled = sia.Enable;
                    break;
                }
                case "anchor":
                {
                    _anchorEnabled = sia.Enable;
                    break;
                }
                case "next":
                {
                    _nextEnabled = sia.Enable;
                    break;
                }
            }
            SetActiveObjects(_currentVal);
        });

        _currentVal = "none";
        SetActiveObjects(_currentVal);

        EventManager.Instance.Register<BubbleCollision>((e) =>
        {
            BubbleCollision bc = (BubbleCollision) e;
            _blinkingInput = "accelerate";
            _accelerateEnabled = bc.Inside;
            SetActiveObjects(_currentVal);
        });
    }

    private void Update()
    {
        if (_blink)
        {
            BlinkControl();
        }
    }

    private void SetActiveObjects(string activeCamera)
    {
        bool keyboardActive = GameManager.Instance.inputs.currentControlScheme == "Keyboard";
        // Scanner
        bool scanner = (activeCamera == "Top" || activeCamera == "Overboard");
        scanGamepad.SetActive(!keyboardActive && _scanEnabled && scanner);
        scanKeyboard.SetActive(keyboardActive && _scanEnabled && scanner);

        bool sail = (activeCamera == "Main" || activeCamera == "Front");
        //Raise Sail
        raiseSailGamepad.SetActive(!keyboardActive && _raiseSailEnabled && sail);
        raiseSailKeyboard.SetActive(keyboardActive && _raiseSailEnabled && sail);
        // Adjust Sail
        adjustSailGamepad.SetActive(!keyboardActive && _adjustSailEnabled && sail);
        adjustSailKeyboard.SetActive(keyboardActive && _adjustSailEnabled && sail);

        // Move Camera
        bool move = (activeCamera == "Main" || activeCamera == "Overboard" || activeCamera == "Front");
        moveGamepad.SetActive(!keyboardActive && _moveEnabled && move);
        moveKeyboard.SetActive(keyboardActive && _moveEnabled && move);

        bool steer = (activeCamera == "Steering");
        // Anchor
        anchorGamepad.SetActive(!keyboardActive && _anchorEnabled && steer);
        anchorKeyboard.SetActive(keyboardActive && _anchorEnabled && steer);
        // Steer
        steerGamepad.SetActive(!keyboardActive && _steerEnabled && steer);
        steerKeyboard.SetActive(keyboardActive && _steerEnabled && steer);
        // Accelerate
        accelerateGamepad.SetActive(!keyboardActive && _accelerateEnabled && steer);
        accelerateKeyboard.SetActive(keyboardActive && _accelerateEnabled && steer);

        // Look
        bool look = (activeCamera == "Steering" || activeCamera == "Overboard" || activeCamera == "Top");
        lookGamepad.SetActive(!keyboardActive && _lookEnabled && look);
        lookKeyboard.SetActive(keyboardActive && _lookEnabled && look);

        // Camera
        cameraSwitchGamepad.SetActive(!keyboardActive && _cameraSwitchEnabled);
        cameraSwitchKeyboard.SetActive(keyboardActive && _cameraSwitchEnabled);
        
        // Next
        nextGamepad.SetActive(!keyboardActive && _nextEnabled);
        nextKeyboard.SetActive(keyboardActive && _nextEnabled);
    }
    
    private void BlinkControl()
    {
        if (_blinkingTimer < blinkingRate)
        {
            _blinkingTimer += Time.deltaTime;
        }
        else
        {
            _blinkingState = !_blinkingState;
            SetColor(_blinkingInput, _blinkingState? passiveControlColor : blinkControlColor);
            _blinkingTimer = 0;
        }
    }

    private void SetDefault()
    {
        SetColor("look", passiveControlColor);
        SetColor("move", passiveControlColor);
        SetColor("steer", passiveControlColor);
        SetColor("raise", passiveControlColor);
        SetColor("adjust", passiveControlColor);
        SetColor("scan", passiveControlColor);
        SetColor("camera", passiveControlColor);
        SetColor("anchor", passiveControlColor);
        SetColor("accelerate", passiveControlColor);
        SetColor("next", passiveControlColor);
    }

    private void SetColor(string control, Color color)
    {
        switch (control)
        {
            case "look":
            {
                lookGamepad.GetComponent<ColorChanger>().ChangeColor(color);
                lookKeyboard.GetComponent<ColorChanger>().ChangeColor(color);
                break;
            }
            case "move":
            {
                moveGamepad.GetComponent<ColorChanger>().ChangeColor(color);
                moveKeyboard.GetComponent<ColorChanger>().ChangeColor(color);
                break;
            }
            case "steer":
            {
                steerGamepad.GetComponent<ColorChanger>().ChangeColor(color);
                steerKeyboard.GetComponent<ColorChanger>().ChangeColor(color);
                break;
            }
            case "raise":
            {
                raiseSailGamepad.GetComponent<ColorChanger>().ChangeColor(color);
                raiseSailKeyboard.GetComponent<ColorChanger>().ChangeColor(color);
                break;
            }
            case "adjust":
            {
                adjustSailGamepad.GetComponent<ColorChanger>().ChangeColor(color);
                adjustSailKeyboard.GetComponent<ColorChanger>().ChangeColor(color);
                break;
            }
            case "scan":
            {
                scanGamepad.GetComponent<ColorChanger>().ChangeColor(color);
                scanKeyboard.GetComponent<ColorChanger>().ChangeColor(color);
                break;
            }
            case "camera":
            {
                cameraSwitchGamepad.GetComponent<ColorChanger>().ChangeColor(color);
                cameraSwitchKeyboard.GetComponent<ColorChanger>().ChangeColor(color);
                break;
            }
            case "anchor":
            {
                anchorGamepad.GetComponent<ColorChanger>().ChangeColor(color);
                anchorKeyboard.GetComponent<ColorChanger>().ChangeColor(color);
                break;
            }
            case "accelerate":
            {
                accelerateGamepad.GetComponent<ColorChanger>().ChangeColor(color);
                accelerateKeyboard.GetComponent<ColorChanger>().ChangeColor(color);
                break;
            }
            case "next":
            {
                nextGamepad.GetComponent<ColorChanger>().ChangeColor(color);
                nextKeyboard.GetComponent<ColorChanger>().ChangeColor(color);
                break;
            }
        }
    }
}