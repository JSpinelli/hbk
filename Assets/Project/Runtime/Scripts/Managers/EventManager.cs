using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    // Global
    public static EventManager Instance;

    // State
    private readonly Dictionary<Type, HBKEvent.Handler> _registeredHandlers = new Dictionary<Type, HBKEvent.Handler>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.Log("Should not be another class");
            Destroy(this);
        }
    }

    public void Register<T>(HBKEvent.Handler handler) where T : HBKEvent
    {
        var type = typeof(T);
        if (_registeredHandlers.ContainsKey(type))
        {
            if (!IsEventHandlerRegistered(type, handler))
                _registeredHandlers[type] += handler;
        }
        else
        {
            _registeredHandlers.Add(type, handler);
        }
    }

    public void Unregister<T>(HBKEvent.Handler handler) where T : HBKEvent
    {
        var type = typeof(T);
        if (!_registeredHandlers.TryGetValue(type, out var handlers)) return;

        handlers -= handler;

        if (handlers == null)
        {
            _registeredHandlers.Remove(type);
        }
        else
        {
            _registeredHandlers[type] = handlers;
        }
    }

    public void Fire(HBKEvent e)
    {
        var type = e.GetType();

        if (_registeredHandlers.TryGetValue(type, out var handlers))
        {
            handlers(e);
        }
    }

    private bool IsEventHandlerRegistered(Type typeIn, Delegate prospectiveHandler)
    {
        return _registeredHandlers[typeIn].GetInvocationList()
            .Any(existingHandler => existingHandler == prospectiveHandler);
    }
}

public abstract class HBKEvent
{
    public delegate void Handler(HBKEvent e);
}

// Meta Game State Event
public class GameStarted : HBKEvent
{
}

public class SetUIActive : HBKEvent
{
}


public class GamePaused : HBKEvent
{
}

public class GameResumed : HBKEvent
{
}

public class VisorEnabled : HBKEvent
{
}

public class BlackHoleEffectActive : HBKEvent
{
    public readonly bool Active;

    public BlackHoleEffectActive(bool active)
    {
        Active = active;
    }
}

public class SetCameraActive : HBKEvent
{
    public readonly string CameraName;
    public readonly bool Active;

    public SetCameraActive(string name, bool active)
    {
        CameraName = name;
        Active = active;
    }
}

public class BubbleCollision : HBKEvent
{
    public readonly bool Inside;
    public BubbleCollision(bool inside)
    {
        Inside = inside;
    }
}

public class InputChange : HBKEvent
{
}

// Game Event
public class CameraSwitch : HBKEvent
{
    public readonly string NewCam;

    public CameraSwitch(string newCam)
    {
        NewCam = newCam;
    }
}

public class SetInputActive : HBKEvent
{
    public readonly string Input;
    public readonly bool Enable;

    public SetInputActive(string inputToEnable, bool enable)
    {
        Input = inputToEnable;
        Enable = enable;
    }
}

public class SetTutorialActive : HBKEvent
{
    public readonly string Input;
    public readonly bool Enable;

    public SetTutorialActive(string inputToEnable, bool enable)
    {
        Input = inputToEnable;
        Enable = enable;
    }
}

public class StartTutorial : HBKEvent
{
}

public class AnchorEnabled : HBKEvent
{
}

public class InputAllowed : HBKEvent
{
}

public class CameraSwitchAllowed : HBKEvent
{
}

public class TutorialFinish : HBKEvent
{
}

public class UnlockDialog : HBKEvent
{
}

public class ObjectFound : HBKEvent
{
    public readonly JournalEntry JournalEntry;

    public ObjectFound(JournalEntry e)
    {
        JournalEntry = e;
    }
}

public class ObjectLost : HBKEvent
{
}

public class ObjectScanned : HBKEvent
{
    public readonly JournalEntry ScannedEntry;

    public ObjectScanned(JournalEntry je)
    {
        ScannedEntry = je;
    }
}