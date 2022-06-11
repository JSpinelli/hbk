#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Global
    public static GameManager Instance;

    // Properties
    public List<string> scenesToLoad;
    public bool startPaused = true;
    public bool startSailing = true;
    public bool playTutorial = true;
    
    [Header("Fog Transition Values")]
    public Color baseFogColor;
    public float fogTransitionSpeed;
    public float fogBaseValue;
    public float blackHoleFog = 0.1f;

    // References
    public Config currentConfig;
    public CinemachineVirtualCamera mainMenuCam;
    
    [Header("Particle Systems")]
    public ParticleSystem centerStars;
    public ParticleSystem edgeStars;
    public ParticleSystem centerWind;
    public ParticleSystem edgeWind;
    public GameObject windParticles;
    public GameObject starParticles;
    public GameObject blackHoleTransitionParticles;

    // Start Init
    [HideInInspector]
    public PlayerInput inputs;
    
    // State
    private string _prevInputSate =" ";
    private bool _inMainMenu = true;
    private bool _gamePaused = false;
    private bool _fogChanging;
    private bool _blackHoleTransition;
    private bool _blackHoleTransitionEnded = false;
    private Color _fogTarget;
    

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
        inputs = GetComponent<PlayerInput>();
        _prevInputSate = inputs.currentControlScheme;
        Cursor.lockState = CursorLockMode.Locked;
        mainMenuCam.Priority = 11;
        
        InputManager.Instance.Register(OnStart, "Start");
        InputManager.Instance.Register(OnExit, "Exit");
        InputManager.Instance.Register(OnSkipTutorial, "Skip Tutorial");
        InputManager.Instance.Register(OnPause, "Pause");
        InputManager.Instance.Register(OnHideUI, "Hide UI");
    }

    private void Start()
    {
        WaveManager.Instance.ChangeWaveValues(2,20,0.2f,15);
        blackHoleTransitionParticles.SetActive(false);
        RenderSettings.fogColor = baseFogColor;
        SetParticles(true);
        if (startSailing)
        {
            _inMainMenu = false;
            StartGame();
        }

        if (startPaused)
        {
            StartGame();
            OnPause(new InputAction.CallbackContext());
        }
    }

    public void StartBlackHoleTransition()
    {
        EventManager.Instance.Fire(new BlackHoleEffectActive(true));
        _blackHoleTransition = true;
        blackHoleTransitionParticles.SetActive(true);
    }
    
    public void StopBlackHoleTransition()
    {
        _blackHoleTransitionEnded = true;
        _blackHoleTransition = false;
        SetParticles(true);
        EventManager.Instance.Fire(new BlackHoleEffectActive(false));
        WaveManager.Instance.ChangeWaveValues(2,20,0.2f,15);
        var emissionModule = blackHoleTransitionParticles.GetComponent<ParticleSystem>().emission;
        emissionModule.rateOverTime = 0;
    }

    private void SetParticles(bool active)
    {
        windParticles.SetActive(active);
        starParticles.SetActive(active);
    }

    public void StartGame()
    {
        SetParticles(false);
        WaveManager.Instance.ChangeWaveValues(0.1f,20,0.1f,15);
        RenderSettings.fogDensity = 0;
        _inMainMenu = false;
        EventManager.Instance.Fire(new GameStarted());
        if (playTutorial)
        {
            EventManager.Instance.Fire(new StartTutorial());
        }
        else
        {
            EventManager.Instance.Fire(new TutorialFinish());
        }

        foreach (var scene in scenesToLoad)
        {
            StartCoroutine(LoadYourAsyncScene(scene));
            
        }
        mainMenuCam.Priority = -1;
    }
    
    IEnumerator LoadYourAsyncScene(string scene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene,LoadSceneMode.Additive);
        yield return null;
    }

    private void Update()
    {
        if (inputs.currentControlScheme != _prevInputSate)
        {
            EventManager.Instance.Fire(new InputChange());
            _prevInputSate = inputs.currentControlScheme;
        }

        if (_fogChanging)
        {
            RenderSettings.fogColor =
                Color.Lerp(RenderSettings.fogColor, _fogTarget, Time.deltaTime * fogTransitionSpeed);
            if (RenderSettings.fogColor == _fogTarget)
            {
                _fogChanging = false;
            }
        }

        if (_blackHoleTransitionEnded)
        {
            RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, fogBaseValue, Time.deltaTime);
        }        
        
        if (_blackHoleTransition && !_blackHoleTransitionEnded)
        {
            RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, blackHoleFog, Time.deltaTime);
        }
        
    }

    public void Pause()
    {
        _gamePaused = !_gamePaused;
        if (_gamePaused)        
        {
            EventManager.Instance.Fire(new GamePaused());
            Time.timeScale = 0;
        }
        else
        {
            EventManager.Instance.Fire(new GameResumed());
            Time.timeScale = 1;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void AddBubbleTrigger(SphereCollider sc)
    {
        edgeStars.trigger.AddCollider(sc);
        centerStars.trigger.AddCollider(sc);
        centerWind.trigger.AddCollider(sc);
        edgeWind.trigger.AddCollider(sc);
    }

    public void FogChange(Color fogColor)
    {
        _fogChanging = true;
        _fogTarget = fogColor;
    }
    
    public void FogRevert()
    {
        _fogChanging = true;
        _fogTarget = baseFogColor;
    }
    
    // Input Handlers
    private void OnExit(InputAction.CallbackContext val)
    {
        ExitGame();
    }
    
    private void OnPause(InputAction.CallbackContext value)
    {
        Pause();
    }
    
    private void OnHideUI(InputAction.CallbackContext value)
    {
        EventManager.Instance.Fire(new SetUIActive());
    }
    
    private void OnStart(InputAction.CallbackContext val)
    {
        if (!_inMainMenu) return;
        StartGame();
    }
    
    private void OnSkipTutorial(InputAction.CallbackContext val)
    {
        if (!_inMainMenu) return;
        playTutorial = false;
        StartGame();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameManager))]
public class DrawGameManager: Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameManager manager = (GameManager)target;
    }
}
#endif
