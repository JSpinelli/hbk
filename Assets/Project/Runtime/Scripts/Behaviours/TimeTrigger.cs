using UnityEngine;

public class TimeTrigger : MonoBehaviour
{
    [RangeAttribute(0, 24)] public int hourStart;
    [RangeAttribute(0, 59)] public int minuteStart;
    public AnimationClip animClip;

    private Animator _myAnim;

    private int _hourEnd;
    private int _minuteEnd;
    private float _baseSpeed;
    
    private static readonly int MoveForward = Animator.StringToHash("MoveForward");
    private static readonly int MoveBackward = Animator.StringToHash("MoveBackward");
    private static readonly int GoingForward = Animator.StringToHash("GoingForward");
    private static readonly int SpeedForward = Animator.StringToHash("SpeedForward");
    private static readonly int SpeedBackward = Animator.StringToHash("SpeedBackward");

    // Start is called before the first frame update
    void Start()
    {
        _myAnim = GetComponent<Animator>();
        float targetEnd = animClip.length / TimeManager.Instance.secondsToMinute;
        _hourEnd = hourStart + ((int) Mathf.Floor(targetEnd / 60));
        _minuteEnd = minuteStart + ((int) targetEnd % 60);
        
        //This speed should be changed if at any moment time should stop / slow / accelerate
        _baseSpeed = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeManager.Instance.goingForward &&
            hourStart == TimeManager.Instance.hour &&
            minuteStart == TimeManager.Instance.minutes)
        {
            _myAnim.SetBool(MoveForward, true);
        }

        if (TimeManager.Instance.goingForward)
        {
            _myAnim.SetBool(MoveBackward, false);
            _myAnim.SetBool(GoingForward, true);
            _myAnim.SetFloat(SpeedForward, _baseSpeed);
            _myAnim.SetFloat(SpeedBackward, -_baseSpeed);
        }
        else
        {
            _myAnim.SetBool(GoingForward, false);
            _myAnim.SetBool(MoveForward, false);
            _myAnim.SetFloat(SpeedForward, -_baseSpeed);
            _myAnim.SetFloat(SpeedBackward, _baseSpeed);
        }


        if (!TimeManager.Instance.goingForward &&
            _hourEnd == TimeManager.Instance.hour &&
            _minuteEnd == TimeManager.Instance.minutes)
        {
            _myAnim.SetBool(MoveBackward, true);
        }
    }
}