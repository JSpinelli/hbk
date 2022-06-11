using FMODUnity;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WindManager : MonoBehaviour
{
    public static WindManager Instance;
    
    // References
    [HeaderAttribute("References")]
    public StudioEventEmitter windSound;
    
    // Properties
    [HeaderAttribute("Starting Values")]
    public Vector2 startingWind;
    public float startingMagnitude;
    
    [HeaderAttribute("Current Values")]
    public Vector2 wind;
    public float windMagnitude;
    
    [HeaderAttribute("Random Change Parameters")]
    public float minimumTimeBeforeChange;
    public float maximumTimeBeforeChange;
    public float maximumWindChange = 180;
    public float minimumWindChange = 10;
    public float maximumWindMagnitude = 10;
    public float minimumWindMagnitude = 1;
    public float lerpSpeed = 0.5f;
    
    [HeaderAttribute("Global Values")]
    public float noGo = -0.45f;
    public bool windChangeEnable;
    public bool randomizeStart;
    
    [HeaderAttribute("Target Values")]
    public float targetMagnitude;
    public Vector2 targetDirection;

    private float _timeBeforeChange;
    private float _windChangeTimer;

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
        //EventManager.Instance.Register<GameStarted>((e) => {  windSound.Play();});
        targetDirection = startingWind;
        wind = startingWind;
        targetMagnitude = startingMagnitude;
        windMagnitude = startingMagnitude;
        _windChangeTimer = 0;
        if (randomizeStart)
            RandomizeStart();
    }

    public void SetWind(Vector2 direction, float magnitude, float timeToChange)
    {
        targetDirection = direction.normalized;
        targetMagnitude = magnitude;
        _timeBeforeChange = timeToChange;
    }

    private void RandomizeStart()
    {
        _timeBeforeChange = Random.Range(minimumTimeBeforeChange, maximumTimeBeforeChange);
        targetMagnitude = Random.Range(minimumWindMagnitude, maximumWindMagnitude);
        RotateWind();
    }

    private void Update()
    {
        if (windChangeEnable)
        {
            if (_windChangeTimer < _timeBeforeChange)
            {
                _windChangeTimer += Time.deltaTime;
            }
            else
            {
                _windChangeTimer = 0;
                _timeBeforeChange = Random.Range(minimumTimeBeforeChange, maximumTimeBeforeChange);
                targetMagnitude = Random.Range(minimumWindMagnitude, maximumWindMagnitude);
                RotateWind();
            }
        }

        if (Vector2.Distance(targetDirection, wind) > 0.01f)
        {
            wind = Vector2.Lerp(wind, targetDirection, Time.deltaTime * lerpSpeed);
        }

        if (Mathf.Abs(targetMagnitude - windMagnitude) > 0.001f)
        {
            windMagnitude = Mathf.Lerp(windMagnitude, targetMagnitude, Time.deltaTime * lerpSpeed);
        }
    }

    public Vector2 RandomizeWind()
    {
        Vector2 newWind = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        if (newWind == Vector2.zero)
            return RandomizeWind();
        return newWind;
    }

    public void RotateWind()
    {
        targetDirection = Quaternion.Euler(0, 0, Random.Range(minimumWindChange, maximumWindChange)) * wind;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(WindManager))]
public class DrawWindManager: Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WindManager manager = (WindManager)target;
        if(GUILayout.Button("Change Wind"))
        {
            manager.RotateWind();
        }
    }
}
#endif