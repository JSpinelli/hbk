using System;
using FMODUnity;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoatController : MonoBehaviour
{
    // Start Init
    private Rigidbody _rigidbody;

    // References
    [HeaderAttribute("References")] public Transform tillerPos;
    public StudioEventEmitter anchorSoundUp;
    public StudioEventEmitter anchorSoundDown;

    // Properties
    [HeaderAttribute("Speed")] 
    public float speedFactor = 50f;
    public float accelerationMagnitude;

    [HeaderAttribute("Slow Motion")] 
    public float slowMotionDuration = 10;
    public float slowMotionScale = 0.1f;
    public float slowMotionCooldown = 5f;
    
    [HeaderAttribute("Inertia Tensor")] 
    public Vector3 inertiaTensor;

    [HeaderAttribute("Anchor")] public float anchorDeceleration;
    public Vector3 minimumMovementAnchored = new Vector3(0.1f, 0.1f, 0.1f);

    [HeaderAttribute("Torque")] public float torqueModifier;
    public bool torqueEnabled;

    [HeaderAttribute("Atom Variables")] public IntReference speed;
    public StringReference typeOfSailing;
    public FloatReference mainSailContribution;
    public FloatReference frontSailContribution;
    public FloatReference currentTillerPos;
    public FloatReference mainSailHeight;
    public FloatReference frontSailHeight;

    // Memory Optimization
    private Vector3 _myForward;
    private Vector3 _myRight;
    private float _velMagnitude;
    private float _dot2;

    // Input Tracking
    private float _acceleration;
    private bool _slowMotionTriggered;
    private bool _slowMotionActive = false;
    

    // State
    private float _currentSpeed;
    public bool anchorDropped = true;
    private bool _anchorEnabled = true;
    private bool _speedBoatMode;
    private float _slowmoCooldown = 0;
    private float _slowmoDuration = 0;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        // HERE FOR BALANCING PURPOSES, THIS GET CHANGE AUTOMATICALLY WHEN ADDING A COLLIDER
        _rigidbody.inertiaTensor = inertiaTensor;
    }

    private void Start()
    {
        InputManager.Instance.Register(OnAccelerate, "Accelerate");
        InputManager.Instance.Register(OnAnchor, "Anchor");
        InputManager.Instance.Register(OnSlowmo, "Slowmo", true, true);
        
        EventManager.Instance.Register<StartTutorial>((e) =>
        {
            // freezing positions x and z
            _rigidbody.constraints = (RigidbodyConstraints) 10;
            _anchorEnabled = false;
        });
        EventManager.Instance.Register<SetTutorialActive>((e) =>
        {
            SetTutorialActive sta = (SetTutorialActive) e;
            switch (sta.Input)
            {
                case "tiller":
                {
                    _rigidbody.constraints = RigidbodyConstraints.None;
                    break;
                }
            }
        });
        EventManager.Instance.Register<GameStarted>((e) => { _rigidbody.constraints = RigidbodyConstraints.None; });

        EventManager.Instance.Register<TutorialFinish>((e) => { _rigidbody.constraints = RigidbodyConstraints.None; });
        EventManager.Instance.Register<AnchorEnabled>((e) => { _anchorEnabled = true; });
        EventManager.Instance.Register<BubbleCollision>((e) =>
        {
            BubbleCollision bc = (BubbleCollision) e;
            _speedBoatMode = bc.Inside;
        });
    }

    private void FixedUpdate()
    {
        _myForward = transform.forward;
        _myRight = transform.right;
        Vector2 sailDirection = new Vector2(_myForward.x, _myForward.z);
        Vector2 sailDirection2 = new Vector2(_myRight.x, _myRight.z);
        float dot = Vector2.Dot(sailDirection.normalized, WindManager.Instance.wind.normalized);
        //Direction of rotation of the hull
        _dot2 = Vector2.Dot(sailDirection2.normalized, WindManager.Instance.wind.normalized);
        // Force of the rotation based on the position of the sails
        float dot3 = Vector3.Dot(gameObject.transform.up, Vector3.right);
        _currentSpeed = 0;

        if (dot <= WindManager.Instance.noGo)
        {
            typeOfSailing.Value = "In Irons";
        }

        if (dot > WindManager.Instance.noGo && dot <= -0.7f)
        {
            typeOfSailing.Value = "Close Hauled";
        }

        if (dot > -0.7f && dot <= -0.1f)
        {
            typeOfSailing.Value = "Close Reach";
        }

        if (dot > -0.1f && dot <= 0.1f)
        {
            typeOfSailing.Value = "Beam Reach";
        }

        if (dot > 0.1f && dot <= 0.9f)
        {
            typeOfSailing.Value = "Broad Reach";
        }

        if (dot > 0.9f)
        {
            typeOfSailing.Value = "Running";
        }

        if (anchorDropped)
        {
            _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, minimumMovementAnchored,
                Time.deltaTime * anchorDeceleration);
        }
        else
        {
            if (!_speedBoatMode)
            {
                _currentSpeed = (mainSailContribution.Value * mainSailHeight.Value) +
                                (frontSailContribution.Value * frontSailHeight.Value);
                _currentSpeed *= WindManager.Instance.windMagnitude;
                _currentSpeed *= speedFactor;
            }
            else
            {
                _currentSpeed = _acceleration * accelerationMagnitude;
            }

            Vector3 forceDir = transform.forward * (_currentSpeed);

            _rigidbody.AddForce(forceDir, ForceMode.Force);
        }

        if (torqueEnabled)
        {
            _rigidbody.AddTorque(new Vector3(0, 0, -_dot2).normalized *
                                 (torqueModifier * WindManager.Instance.windMagnitude * _rigidbody.mass *
                                  (1 - Mathf.Abs(dot3))));
        }

        _velMagnitude = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z).magnitude;
        speed.Value = (int) (_velMagnitude * 100);

        //Boat Turning
        _rigidbody.AddForceAtPosition(
            transform.right * (currentTillerPos * Mathf.Clamp(_velMagnitude, 8, 40)),
            tillerPos.position);
        
        SlowMotion();
    }

    private void SlowMotion()
    {
        if (_slowMotionTriggered)
        {
            if (_slowmoDuration < (slowMotionDuration*slowMotionScale))
            {
                _slowmoDuration += Time.fixedDeltaTime;
                if (!_slowMotionActive)
                {
                    _slowMotionActive = true;
                    Time.timeScale = slowMotionScale;
                    _slowmoCooldown = 0;
                    EventManager.Instance.Fire(new SlowMotion(true));
                }
            }
            else
            {
                if (_slowMotionActive)
                {
                    _slowMotionActive = false;
                    Time.timeScale = 1;
                    EventManager.Instance.Fire(new SlowMotion(false));
                }
            }
        }
        else
        {
            if (_slowMotionActive)
            {
                _slowMotionActive = false;
                Time.timeScale = 1;
                EventManager.Instance.Fire(new SlowMotion(false));
            }
            if (_slowmoCooldown < slowMotionCooldown)
            {
                _slowmoCooldown += Time.fixedDeltaTime;
            }
            else
            {
                _slowmoDuration = 0;
            }

        }
    }

    // Input Handlers
    private void OnAnchor(InputAction.CallbackContext value)
    {
        if (!_anchorEnabled) return;
        if (anchorDropped)
        {
            anchorSoundUp.Play();
            anchorSoundDown.Stop();
        }
        else
        {
            anchorSoundDown.Play();
            anchorSoundUp.Stop();
        }

        anchorDropped = !anchorDropped;
    }

    private void OnAccelerate(InputAction.CallbackContext value)
    {
        _acceleration = -value.ReadValue<float>();
    }
    
    private void OnSlowmo(InputAction.CallbackContext value)
    {
        _slowMotionTriggered = value.ReadValue<float>() != 0;
    }
}