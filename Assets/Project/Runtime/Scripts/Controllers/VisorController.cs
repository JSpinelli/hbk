using System.Collections.Generic;
using FMODUnity;
using Shapes;
using UnityEngine;
using UnityEngine.InputSystem;

public class VisorController : MonoBehaviour
{
    // References
    public GameObject display;
    public Disc shutter;
    public StudioEventEmitter scanCompleteSound;
    public List<Line> lines;
    public Rectangle outline;

    // Properties
    public float timeToScan;
    public float animationSpeed;
    public Color foundColor;
    public Color normalColor;
    public Color scannedColor;

    public float lineStart;
    public float lineEnd;
    public float rectangleStart;
    public float rectangleEnd;
    public float lerpSpeed;

    // State
    private JournalEntry _currentEntry;
    private bool _scanning = false;
    private bool _haveObject = false;
    private float timer = 0;
    
    private void Start()
    {
        InputManager.Instance.Register(OnScan, "Scan",true,true);
        
        EventManager.Instance.Register<CameraSwitch>(OnCameraSwitch);
        EventManager.Instance.Register<ObjectFound>(OnObjectFound);
        EventManager.Instance.Register<ObjectLost>(OnObjectLost);
        ChangeColor(2);
    }

    private void Update()
    {
        LerpFrame(_haveObject);
        if (_scanning && _haveObject)
        {
            if (timer < timeToScan)
            {
                timer += Time.deltaTime;
                shutter.DashSpacing = Mathf.Lerp(shutter.DashSpacing, 0, (timer / timeToScan) * animationSpeed);
            }
            else
            {
                if (_currentEntry != null)
                {
                    scanCompleteSound.Play();
                    EventManager.Instance.Fire(new ObjectScanned(_currentEntry));
                    _currentEntry = null;
                    _scanning = false;
                }
            }
        }
        else
        {
            if (!_scanning && _haveObject)
            {
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                    shutter.DashSpacing = Mathf.Lerp(0.4f, shutter.DashSpacing, (timer / timeToScan) * animationSpeed);
                }
                else
                {
                    timer = 0;
                }
            }
            else
            {
                timer = 0;
            }
        }
    }


    private void OnCameraSwitch(HBKEvent e)
    {
        CameraSwitch cw = (CameraSwitch) e;

        if (cw.NewCam == "Overboard" || cw.NewCam == "Top")
        {
            display.SetActive(true);
        }
        else
        {
            display.SetActive(false);
        }
    }

    private void ChangeColor(int found)
    {
        Color targetColor;
        if (found == 0)
        {
            targetColor = scannedColor;
        }
        else
        {
            targetColor = found == 1 ? foundColor : normalColor;
        }

        shutter.Color = targetColor;
        outline.Color = targetColor;
        foreach (var line in lines)
        {
            line.Color = targetColor;
        }
    }

    private void LerpFrame(bool found)
    {
        float targetLine = found ? lineEnd : lineStart;
        float targetRectangle = found ? rectangleEnd : rectangleStart;

        foreach (var line in lines)
        {
            line.End = Vector3.Lerp(line.End, new Vector3(line.End.x, targetLine, line.End.z),
                Time.deltaTime * lerpSpeed);
        }

        outline.Width = Mathf.Lerp(outline.Width, targetRectangle, Time.deltaTime * lerpSpeed);
        outline.Height = Mathf.Lerp(outline.Height, targetRectangle, Time.deltaTime * lerpSpeed);
    }

    private void OnObjectFound(HBKEvent e)
    {
        ObjectFound cw = (ObjectFound) e;
        _currentEntry = cw.JournalEntry;
        _haveObject = true;
        ChangeColor(1);
    }

    private void OnObjectLost(HBKEvent e)
    {
        _currentEntry = null;
        _haveObject = false;
        ChangeColor(2);
    }

    private void OnScan(InputAction.CallbackContext value)
    {
        _scanning = value.ReadValueAsButton();
    }
}