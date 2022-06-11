using UnityAtoms.BaseAtoms;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;
    
    public float secondsToMinute;
    
    public IntReference hour;
    public IntReference minutes;
    public BoolReference goingForward;

    private float _timer;
    
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

    private void Start()
    {
        hour.Value = 0;
        minutes.Value = 0;
    }

    private void FixedUpdate()
    {
        _timer += Time.fixedDeltaTime;
        if (_timer > secondsToMinute)
        {
            _timer = _timer - secondsToMinute;
            if (goingForward)
                minutes.Value++;
            else
                minutes.Value--;
            
            if (minutes.Value == 60)
            {
                minutes.Value = 0;
                hour.Value++;
            }
            if (minutes.Value == -1)
            {
                minutes.Value = 59;
                hour.Value--;
            }
        }
        if (hour.Value == 24)
        {
            TimeSwitch();
            hour.Value = 23;
            minutes.Value = 59;
        }
        if (hour == -1)
        {
            TimeSwitch();
            hour.Value = 0;
            minutes.Value = 0;
        }
        
    }

    public void TimeSwitch()
    {
        goingForward.Value = !goingForward.Value;
        //Effect goes here
        //EffectsManager.Instance.TriggerTimeSwitchEffect();
    }
}
