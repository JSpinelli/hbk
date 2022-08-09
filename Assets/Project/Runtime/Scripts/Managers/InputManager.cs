using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // Global
    public static InputManager Instance;

    // References
    public PlayerInput input;

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

    public void Register(Action<InputAction.CallbackContext> action, string actionName, bool canceled = false,
        bool started = false)
    {
        input.actions[actionName].performed += action;
        if (canceled)
            input.actions[actionName].canceled += action;
        if (started)
            input.actions[actionName].started += action;
    }

    // public void Fire(string ar)
    // {
    //     input.ActivateInput(ar);
    // }
}