using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // References
    [HeaderAttribute("General UI References")]
    public GameObject pauseMenu;
    public GameObject mainMenu;
    public GameObject menuCamera;
    
    public List<GameObject> gameUI = new List<GameObject>();
    
    // State
    private bool _enabledUI;
    private void Start()
    {
        EventManager.Instance.Register<GameStarted>(OnGameStart);
        EventManager.Instance.Register<GamePaused>(OnGamePaused);
        EventManager.Instance.Register<GameResumed>(OnGameResumed);
        EventManager.Instance.Register<StartTutorial>(OnTutorialStart);
        EventManager.Instance.Register<SetUIActive>((e) =>
        {
            _enabledUI = !_enabledUI;
            foreach (var obj in gameUI)
            {
                obj.SetActive(_enabledUI); 
            }
        });
        pauseMenu.SetActive(false);
        mainMenu.SetActive(true);
        menuCamera.SetActive(true);
        
        foreach (var obj in gameUI)
        {
            obj.SetActive(_enabledUI); 
        }
    }

    // Events Handlers
    private void OnTutorialStart(HBKEvent e)
    {
        _enabledUI = true;
        foreach (var obj in gameUI)
        {
            obj.SetActive(_enabledUI); 
        }
        mainMenu.SetActive(false);
        pauseMenu.SetActive(false);
        menuCamera.SetActive(false);
    }

    private void OnGameStart(HBKEvent e)
    {
        _enabledUI = true;
        foreach (var obj in gameUI)
        {
            obj.SetActive(_enabledUI); 
        }
        mainMenu.SetActive(false);
        menuCamera.SetActive(false);
    }

    private void OnGameResumed(HBKEvent e)
    {
        pauseMenu.SetActive(false);
        menuCamera.SetActive(false);
        _enabledUI = true;
        foreach (var obj in gameUI)
        {
            obj.SetActive(_enabledUI); 
        }
    }

    private void OnGamePaused(HBKEvent e)
    {
        pauseMenu.SetActive(true);
        menuCamera.SetActive(true);
        _enabledUI = false;
        foreach (var obj in gameUI)
        {
            obj.SetActive(_enabledUI); 
        }
    }
    
}