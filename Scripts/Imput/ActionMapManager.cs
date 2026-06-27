using UnityEngine;
using UnityEngine.InputSystem;

// If you don't use PlayerInput component (or want more control):
// (direct access to InputActionAsset)
// Your EventSystem GameObject should have "InputSystemUIInputModule"


public class ActionMapManager : MonoBehaviour {
    public static ActionMapManager Instance { get; private set; } // Singleton

    [SerializeField] private InputActionAsset inputActions;   // drag .inputactions asset here

    private InputActionMap touchMap;
    private InputActionMap uiMap;

    private void Awake() {
        if (Instance != null) Debug.LogWarning("More than one ActionMapManager detected");
        Instance = this;
    }

    private void OnEnable() {
        //Setup
        touchMap = inputActions.FindActionMap("Touch");
        uiMap = inputActions.FindActionMap("UI");

        // Start with gameplay
        touchMap.Enable();
        uiMap.Disable();
    }

    private void OnDisable() {
        // Clean up 
        touchMap?.Disable();
        uiMap?.Disable();
    }

    public void SwitchToUI() {
        touchMap.Disable();
        uiMap.Enable();
    }

    public void SwitchToTouch() {
        uiMap.Disable();
        touchMap.Enable();
    }
}
