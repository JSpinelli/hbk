using System;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogController : MonoBehaviour
{
    // References
    [HeaderAttribute("References")]
    public TextMeshProUGUI dialogText;
    public GameObject dialogObj;
    public StudioEventEmitter consoleTyping;

    // Properties
    [HeaderAttribute("Typing timer")]
    public float timePerCharacter = 0.1f;

    // State
    private bool _playing;
    private float _timer;
    private float _time;
    private Action _whenDone;
    private bool _disableWhenDone = true;
    private bool _hasLock;
    private bool _ignoreScans;
    
    private void Start()
    {

        InputManager.Instance.Register(OnAcceptDialog, "Select");
        
        EventManager.Instance.Register<UnlockDialog>(UnlockDialog);
        EventManager.Instance.Register<ObjectScanned>(OnObjectScanned);

        EventManager.Instance.Register<StartTutorial>((e) => { _ignoreScans = true; });
        EventManager.Instance.Register<TutorialFinish>((e) => { _ignoreScans = false; });

        EventManager.Instance.Register<BubbleCollision>((e) =>
        {
            BubbleCollision bc = (BubbleCollision) e;
            if (bc.Inside)
            {
                AnimateText("Entering atmosphere. Automatic sail disengage. Overboard motor started.", null);
            }
            else
            {
                AnimateText("Exiting atmosphere. Overboard motor stopped. Sail engaged.", null);
            }
        });
        
        dialogObj.SetActive(false);
        dialogText.text = "";
    }

    private void Update()
    {
        if (_playing)
        {
            if (dialogText.maxVisibleCharacters < dialogText.text.Length)
            {
                if (_time > _timer)
                {
                    dialogText.maxVisibleCharacters++;
                    consoleTyping.Play();
                    _time = 0;
                }
                else
                {
                    _time += Time.deltaTime;
                }
            }
            else
            {
                _playing = false;
                if (!_hasLock)
                {
                    EventManager.Instance.Fire(new SetInputActive("next",true));
                }
            }
        }
    }

    private void OnAcceptDialog(InputAction.CallbackContext e)
    {
        if (_playing) return;
        if (_hasLock) return;
        
        if (_disableWhenDone)
            dialogObj.SetActive(false);
        
        _time = 0;
        if (_whenDone != null)
            _whenDone.Invoke();
        
        EventManager.Instance.Fire(new SetInputActive("next",false));
    }

    private void UnlockDialog(HBKEvent e)
    {
        _time = 0;
        
        if (_disableWhenDone)
            dialogObj.SetActive(false);
        
        if (_whenDone != null)
            _whenDone.Invoke();
    }

    private void OnObjectScanned(HBKEvent e)
    {
        ObjectScanned os = (ObjectScanned) e;
        if (_ignoreScans) return;
        AnimateText(os.ScannedEntry.content, null);
    }

    public void AnimateText(string content, Action whenDone, bool disableWhenDone=true, Action lockAction = null)
    {
        //skipPrompt.SetActive(false);
        EventManager.Instance.Fire(new SetInputActive("next",false));
        if (lockAction != null)
        {
            _hasLock = true;
            lockAction.Invoke();
        }
        else
        {
            _hasLock = false;
        }

        _disableWhenDone = disableWhenDone;
        _playing = true;
        dialogText.text = content;
        dialogText.maxVisibleCharacters = 0;
        _timer = timePerCharacter;
        _time = 0;
        dialogObj.SetActive(true);
        _whenDone = whenDone;
    }
}