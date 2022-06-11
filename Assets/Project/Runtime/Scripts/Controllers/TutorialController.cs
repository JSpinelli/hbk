using System;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialController : MonoBehaviour
{
    // Delegate declaration
    private delegate bool Check();

    // References
    public DialogController dialog;
    public GameObject blackBackground;
    
    [Header("Sail status")]
    public FloatReference mainSailContribution;
    public FloatReference mainSailHeight;
    public FloatReference frontSailContribution;
    public FloatReference frontSailHeight;
    

    // State
    private bool _activeCheck;
    private Check _currentCheck;
    private int _textIndex;
    private TutorialDialog[] _startingText;
    private string _activeCam;

    // Input Checking Variables
    private int _panningCheck;
    private int _dollyCheck;
    private int _steeringCheck;
    private bool _scanningCheck;
    private bool _coreCheck;
    private bool _exitCoreCheck;
    private bool _anchorCheck;


    private void Awake()
    {

    }

    void Start()
    {
        InputManager.Instance.Register(OnPanning, "Panning");
        InputManager.Instance.Register(OnMovement, "Movement");
        InputManager.Instance.Register(OnTiller, "Tiller");
        InputManager.Instance.Register(OnAnchor, "Anchor");
        
        
        EventManager.Instance.Register<StartTutorial>(OnTutorialStart);
        EventManager.Instance.Register<ObjectScanned>((e) => _scanningCheck = true);
        EventManager.Instance.Register<CameraSwitch>((e) =>
        {
            CameraSwitch cw = (CameraSwitch) e;
            _activeCam = cw.NewCam;
            if (cw.NewCam == "Core")
            {
                _coreCheck = true;
            }

            if (cw.NewCam != "Core")
            {
                _exitCoreCheck = true;
            }
        });
        
        blackBackground.SetActive(false);
        
        _startingText = new[]
        {
            new TutorialDialog("Booting up...", RunNext),
            new TutorialDialog("Approaching destination...", RunNext),
            new TutorialDialog("Routine post-hibernation diagnostics needed...", RunNext),
            new TutorialDialog("Gravitational anchor engaged until diagnostics are completed...", RunNext),
            new TutorialDialog("Testing top camera (1) movement", RunNext, TopCameraCalibration),
            new TutorialDialog("Working", RunNext),
            new TutorialDialog("Proceeding to overboard camera (2)", RunNext,
                () =>
                {
                    EventManager.Instance.Fire(new SetCameraActive("Overboard", true));
                    EventManager.Instance.Fire(new SetInputActive("camera",true));
                    _activeCheck = true;
                    _currentCheck = () => (_activeCam == "Overboard");
                }),
            new TutorialDialog("Testing camera track movement", RunNext, OverboardCameraCalibration),
            new TutorialDialog("Nominal", RunNext),
            new TutorialDialog("Next step: Main sail controls (3)", RunNext,
                () =>
                {
                    EventManager.Instance.Fire(new SetCameraActive("Main", true));
                    _activeCheck = true;
                    _currentCheck = () => (_activeCam == "Main");
                }),
            new TutorialDialog("Raising sail to full capacity needed", RunNext,
                SailRaiseCheck),
            new TutorialDialog("No obstructions detected", RunNext),
            new TutorialDialog("Positioning sail for maximum wind efficiency", RunNext,
                SailPositionCheck),
            new TutorialDialog("Perfect", RunNext),
            new TutorialDialog("Next step: Front sail controls (4)", RunNext,
                () =>
                {
                    EventManager.Instance.Fire(new SetCameraActive("Front", true));
                    _activeCheck = true;
                    _currentCheck = () => (_activeCam == "Front");
                }),
            new TutorialDialog("Raising and correct positioning needed", RunNext,
                FrontSailCalibration),
            new TutorialDialog("Mint condition", RunNext),
            new TutorialDialog("Last step of calibration: Steering position (5)", RunNext,
                () =>
                {
                    EventManager.Instance.Fire(new SetCameraActive("Steering", true));
                    EventManager.Instance.Fire(new SetInputActive("steer",true));
                    _activeCheck = true;
                    _currentCheck = () => (_activeCam == "Steering");
                }),
            new TutorialDialog("Adjusting tiller for better wind angle", RunNext, SteeringCalibration),
            new TutorialDialog("Spectacular", RunNext),
            new TutorialDialog("Nearby item of interest found, scanning from top camera advised", RunNext,
                ScanningCalibration),
            new TutorialDialog("Scanned object added to database (6)", RunNext, CheckCore),
            new TutorialDialog("", RunNext, ExitCoreCheck),
            new TutorialDialog("Diagnostics completed. All systems working", RunNext),
            new TutorialDialog("Gravitational anchor ready to be lifted from steering position (5)", RunNext,
                () =>
                {
                    EventManager.Instance.Fire(new AnchorEnabled());
                    EventManager.Instance.Fire(new SetInputActive("anchor",true));
                    EventManager.Instance.Fire(new SetTutorialActive("anchor",true));
                    EventManager.Instance.Fire(new SetTutorialActive("speed",true));
                    EventManager.Instance.Fire(new SetCameraActive("Steering", true));
                    _activeCheck = true;
                    _anchorCheck = false;
                    _currentCheck = () => (_anchorCheck);
                })
        };
    }

    private void OnTutorialStart(HBKEvent e)
    {
        dialog.AnimateText(_startingText[0].Txt, _startingText[0].Trigger);
        blackBackground.SetActive(true);
    }

    private void RunNext()
    {
        _textIndex++;
        if (_textIndex < _startingText.Length)
        {
            dialog.AnimateText(_startingText[_textIndex].Txt, _startingText[_textIndex].Trigger, (_textIndex == _startingText.Length-1), _startingText[_textIndex].LockAction);
        }
        else
        {
            EventManager.Instance.Fire(new TutorialFinish());
        }
    }

    private void TopCameraCalibration()
    {
        _activeCheck = true;
        _panningCheck = 0;
        _currentCheck = () => (_panningCheck > 50);
        blackBackground.SetActive(false);
        EventManager.Instance.Fire(new InputAllowed());
        EventManager.Instance.Fire(new SetInputActive("look",true));
        EventManager.Instance.Fire(new CameraSwitch("Top"));
    }

    private void OverboardCameraCalibration()
    {
        _activeCheck = true;
        _dollyCheck = 0;
        _currentCheck = () => (_dollyCheck > 2);
        EventManager.Instance.Fire(new SetInputActive("move",true));
        EventManager.Instance.Fire(new CameraSwitchAllowed());
    }

    private void SailRaiseCheck()
    {
        _activeCheck = true;
        EventManager.Instance.Fire(new SetInputActive("raise",true));
        _currentCheck = () => (mainSailHeight > 0.95);
    }

    private void SailPositionCheck()
    {
        _activeCheck = true;
        EventManager.Instance.Fire(new SetInputActive("adjust",true));
        EventManager.Instance.Fire(new SetTutorialActive("mainSail",true));
        EventManager.Instance.Fire(new SetTutorialActive("boat",true));
        EventManager.Instance.Fire(new SetTutorialActive("wind",true));
        _currentCheck = () => (mainSailContribution > 0.8f);
    }

    private void ScanningCalibration()
    {
        _activeCheck = true;
        _scanningCheck = false;
        _currentCheck = () => _scanningCheck;
        EventManager.Instance.Fire(new SetInputActive("scan",true));
        EventManager.Instance.Fire(new SetCameraActive("Top", true));
        EventManager.Instance.Fire(new VisorEnabled());
    }

    private void CheckCore()
    {
        _activeCheck = true;
        _coreCheck = false;
        _currentCheck = () => _coreCheck;
        EventManager.Instance.Fire(new SetCameraActive("Core", true));
    }

    private void ExitCoreCheck()
    {
        _activeCheck = true;
        _exitCoreCheck = false;
        _currentCheck = () => _exitCoreCheck;
    }

    private void FrontSailCalibration()
    {
        _activeCheck = true;
        _currentCheck = () => ((frontSailContribution > 0.8f) && frontSailHeight > 0.95);
        EventManager.Instance.Fire(new SetTutorialActive("frontSail",true));
    }

    private void SteeringCalibration()
    {
        _activeCheck = true;
        _steeringCheck = 0;
        _currentCheck = () => (_steeringCheck > 2);
        EventManager.Instance.Fire(new SetTutorialActive("tiller",true));
    }
    
    private void Update()
    {
        if (_activeCheck)
        {
            if (_currentCheck.Invoke())
            {
                _activeCheck = false;
                EventManager.Instance.Fire(new UnlockDialog());
            }
        }
    }

    // INPUT HOOKS
    private void OnPanning(InputAction.CallbackContext value)
    {
        _panningCheck++;
    }

    private void OnMovement(InputAction.CallbackContext value)
    {
        _dollyCheck++;
    }

    public void OnTiller(InputAction.CallbackContext value)
    {
        _steeringCheck++;
    }

    public void OnAnchor(InputAction.CallbackContext value)
    {
        _anchorCheck = true;
    }


    // Tutorial Array Struct
    private struct TutorialDialog
    {
        public readonly string Txt;
        public readonly Action Trigger;
        public readonly Action LockAction;

        public TutorialDialog(string txt, Action act, Action lockAction = null)
        {
            Txt = txt;
            Trigger = act;
            LockAction = lockAction;
        }
    }
}