using FMODUnity;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.InputSystem;

public enum SailType
{
    Front,
    Main
}

public class SailController : MonoBehaviour
{
    [HeaderAttribute("Type")] public SailType sail;
    
    // References
    [HeaderAttribute("References")] 
    public Transform ship;
    public GameObject mast;
    public StudioEventEmitter sailSound;

    // Globals
    [HeaderAttribute("Atom Variables")] 
    public FloatReference sailContribution;
    public FloatReference sailHeight;
    public StringReference pointOfSail;
    public StringReference activeCam;

    // Properties
    [HeaderAttribute("Resistance")] 
    public float adjustmentFactor = 20f;
    public float ropeStep = 0.2f;
    public float windAttachmentFactor = 15f;
    public float windReturnFactor = 15f;
    public float tolerance = 2;

    [HeaderAttribute("Speed Contribution")]
    public float contributionLosingFactor;

    public AnimationCurve sailCurve;

    [HeaderAttribute("Sail Colors")] 
    public Color fullEfficiency;
    public Color noEfficiency;
    [ColorUsage(true, true)] public Color fullEfficiencyEmission;
    [ColorUsage(true, true)] public Color noEfficiencyEmission;

    // Start Init
    private Material _sailMat;
    private Config _sessionConfig;

    // State
    private float _rope;
    private float _angle;
    private float _windDirectionShip;
    private float _yRot;
    private Vector4 _sailVal;
    private float _sailRaiseValue;
    private bool _animateSails;
    private bool _loweringSails;

    // Input Tracker
    private float _sailRaise;
    private bool _leftSail;
    private bool _rightSail;

    // Memory Save
    private float _curvePoint;
    private float _ropeDiff;
    private float _sailAngle;

    // Const
    private float _maxRaiseMatValue = 7;
    private float _minRaiseMatValue = 14;
    private float _maxRope;
    private float _minRope;
    private float _time;
    private static readonly int Property = Shader.PropertyToID("_Plane");
    private static readonly int MainColor = Shader.PropertyToID("_MainColor");
    private static readonly int Emission = Shader.PropertyToID("_Emission");
    
    private void Start()
    {
        if (sail == SailType.Front)
        {
            _maxRaiseMatValue = -0.9f;
            _minRaiseMatValue = 5.3f;
            _maxRope = 80;
            _minRope = 2;
        }
        else
        {
            _maxRope = 56;
            _minRope = 2;
            _maxRaiseMatValue = 7;
            _minRaiseMatValue = 14;
        }

        _rope = 2f;
        sailSound.Play();
        sailSound.SetParameter("Sail Raising", 0);
        sailSound.SetParameter("Sail Movement", 0);
        _sailMat = GetComponent<MeshRenderer>().material;
        sailHeight.Value = 0;
        _sailVal = _sailMat.GetVector(Property);
        _sailRaise = 0;
        _sailVal.z = _minRaiseMatValue;
        _sailMat.SetVector(Property, _sailVal);
        _sessionConfig = GameManager.Instance.currentConfig;

        EventManager.Instance.Register<BubbleCollision>((e) =>
        {
            BubbleCollision bc = (BubbleCollision) e;
            _animateSails = true;
            _loweringSails = bc.Inside;
            if (bc.Inside)
            {
                _sailRaiseValue = _sailVal.z;
            }
        });
        
        InputManager.Instance.Register(OnSailRaise, "Sail Raise", true, true);
        InputManager.Instance.Register(OnLeftSail, "Left Sail");
        InputManager.Instance.Register(OnRightSail, "Right Sail");
    }

    private void Update()
    {
        _windDirectionShip = Vector2.Dot(new Vector2(ship.right.x, ship.right.z), WindManager.Instance.wind.normalized);
        _angle = Vector3.Angle(transform.forward, ship.forward);
        _ropeDiff = _rope - _angle;
        _yRot = transform.localRotation.y;
        _sailAngle = _yRot;
        _time = Time.fixedDeltaTime;
        
        // WENT TO FAR
        if (_ropeDiff < -tolerance)
        {
            transform.RotateAround(mast.transform.position, transform.up,
                ((Mathf.Sign(-_yRot) * adjustmentFactor)) * _time);
            return;
        }

        // WRONG SIDE
        if ((Mathf.Sign(_windDirectionShip) * Mathf.Sign(_yRot) > 0))
        {
            transform.RotateAround(mast.transform.position, transform.up,
                (Mathf.Sign(-_windDirectionShip) * windReturnFactor) *
                _time);
            return;
        }

        // GOING WITH THE WIND
        if (_ropeDiff > tolerance)
        {
            transform.RotateAround(mast.transform.position, transform.up,
                ((Mathf.Sign(-_windDirectionShip) * windAttachmentFactor)) *
                _time);
        }

        UpdateContribution();
        SailUpdateDegrees();

        sailSound.SetParameter("Sail Raising", _sailRaise != 0 ? 1 : 0);

        if (_animateSails)
        {
            if (_loweringSails)
            {
                _sailVal.z += _sessionConfig.GetSailRaiseSensitivity();
                if (Mathf.Abs(_sailVal.z - _minRaiseMatValue) < 0.001f) _animateSails = false;
            }
            else
            {
                _sailVal.z -= _sessionConfig.GetSailRaiseSensitivity();
                if (Mathf.Abs(_sailVal.z - _sailRaiseValue) < 0.001f) _animateSails = false;
            }
        }
        else
        {
            _sailVal.z += _sailRaise * _sessionConfig.GetSailRaiseSensitivity();
        }

        _sailVal.z = Mathf.Clamp(_sailVal.z, _maxRaiseMatValue, _minRaiseMatValue);
        sailHeight.Value = 1 - ((_sailVal.z - _maxRaiseMatValue) / (_minRaiseMatValue - _maxRaiseMatValue));
        _sailMat.SetVector(Property, _sailVal);
    }

    private void SailUpdateDegrees()
    {
        if (_leftSail)
        {
            if (_sailAngle < 0)
                TakeRopeSail();
            else
                GiveRopeSail();
        }

        if (_rightSail)
        {
            if (_sailAngle > 0)
                TakeRopeSail();
            else
                GiveRopeSail();
        }
    }

    private void GiveRopeSail()
    {
        if (_rope < _maxRope)
        {
            _rope += ropeStep;
            _rope = Mathf.Clamp(_rope, _minRope + 0.1f, _maxRope - 0.1f);
        }
    }

    private void TakeRopeSail()
    {
        if (_rope >= 2)
        {
            _rope -= ropeStep;
            _rope = Mathf.Clamp(_rope, _minRope + 0.1f, _maxRope - 0.1f);
        }
    }

    void UpdateContribution()
    {
        Vector2 sailSpread = Vector2.zero;
        switch (pointOfSail.Value)
        {
            case "In Irons":
            {
                sailContribution.Value =
                    Mathf.Lerp(sailContribution.Value, 0, _time * contributionLosingFactor);
                break;
            }
            case "Close Hauled":
            {
                sailSpread = sail == SailType.Front
                    ? _sessionConfig.closeHauledRange
                    : _sessionConfig.main_closeHauledRange;
                break;
            }
            case "Close Reach":
            {
                sailSpread = sail == SailType.Front
                    ? _sessionConfig.closeReachRange
                    : _sessionConfig.main_closeReachRange;
                break;
            }
            case "Beam Reach":
            {
                sailSpread = sail == SailType.Front
                    ? _sessionConfig.beamReachRange
                    : _sessionConfig.main_beamReachRange;
                break;
            }
            case "Broad Reach":
            {
                sailSpread = sail == SailType.Front
                    ? _sessionConfig.broadReachRange
                    : _sessionConfig.main_broadReachRange;
                break;
            }
            case "Running":
            {
                sailSpread = sail == SailType.Front ? _sessionConfig.runningRange : _sessionConfig.main_runningRange;
                break;
            }
        }

        if (_rope < sailSpread.y && _rope > sailSpread.x)
        {
            _curvePoint = (_rope - sailSpread.x) /
                          (sailSpread.y - sailSpread.x);
            sailContribution.Value =
                Mathf.Lerp(sailContribution.Value, sailCurve.Evaluate(_curvePoint), _time);
        }
        else
        {
            sailContribution.Value = Mathf.Lerp(sailContribution.Value, .3f, _time * contributionLosingFactor);
        }

        float mappedValue = sailContribution.Value.Remap(0.3f, 1, 0, 1);
        _sailMat.SetColor(MainColor, Color.Lerp(noEfficiency, fullEfficiency, mappedValue));
        _sailMat.SetColor(Emission, Color.Lerp(noEfficiencyEmission, fullEfficiencyEmission, mappedValue));
    }

    private void OnSailRaise(InputAction.CallbackContext value)
    {
        if (_loweringSails) return;
        if ((activeCam.Value == "Front" && sail == SailType.Front) ||
            (activeCam.Value == "Main" && sail == SailType.Main))
        {
            _sailRaise = -value.ReadValue<float>();
        }
    }

    private void OnLeftSail(InputAction.CallbackContext value)
    {
        if (activeCam.Value == "Front" && sail == SailType.Front || activeCam.Value == "Main" && sail == SailType.Main)
        {
            _leftSail = value.ReadValueAsButton();
        }

        sailSound.SetParameter("Sail Movement", _leftSail ? 1 : 0);
    }

    private void OnRightSail(InputAction.CallbackContext value)
    {
        if (activeCam.Value == "Front" && sail == SailType.Front || activeCam.Value == "Main" && sail == SailType.Main)
        {
            _rightSail = value.ReadValueAsButton();
        }

        sailSound.SetParameter("Sail Movement", _rightSail ? 1 : 0);
    }
}