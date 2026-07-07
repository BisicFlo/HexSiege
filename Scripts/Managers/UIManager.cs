using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// Interface that screens can implement to receive open/close events.
/// </summary>
public interface IScreenManager {
    void OnScreenOpen();
    void OnScreenClose();
}

/// <summary>
/// Data container for each UI screen.
/// </summary>
[System.Serializable]
public class Screen {
    public ScreenType type;
    public GameObject panel; // 
    public MonoBehaviour manager; // MonoBehaviour so it's showned in the Inspector
    public ActionMap actionMap;
    public SoundData openMenuSound; // New
}

public enum ScreenType {
    None,
    StartScreen,
    MainMenu,
    LevelSelection,
    Pause,
    Settings,
    HUD,
    GameOver,
    Loading,
    Shop,
    Build,
    Blank,
    Victory,
    InspectTurret,
    InspectEnemy,
    Inventory,
    Debug
}

public enum ActionMap {
    None,
    Touch,
    UI
}

/// <summary>
/// UIManager - Central screen/panel management system for the game.
/// Handles showing/hiding UI screens, calling lifecycle events, and switching input action maps.
/// </summary>
public class UIManager : MonoBehaviour {
    public static UIManager Instance { get; private set; }

    [Header("Starting Screen")]
    [SerializeField] private ScreenType startingScreen;

    [Header("Registered Screens")]
    [SerializeField] private List<Screen> screens = new List<Screen>();

    private Screen currentScreen;

    private readonly Dictionary<string, ScreenType> nameToType = new() {
    { "StartScreen",    ScreenType.StartScreen       },
    { "MainMenu",       ScreenType.MainMenu          },
    { "LevelSelection", ScreenType.LevelSelection    },
    { "Pause",          ScreenType.Pause             },
    { "Settings",       ScreenType.Settings          },
    { "HUD",            ScreenType.HUD               },
    { "GameOver",       ScreenType.GameOver          },
    { "Shop",           ScreenType.Shop              },
    { "Build",          ScreenType.Build             },
    { "Blank",          ScreenType.Blank             },
    { "Victory",        ScreenType.Victory           },
    { "InspectTurret",  ScreenType.InspectTurret     },
    { "InspectEnemy",   ScreenType.InspectEnemy      },
    { "Inventory",      ScreenType.Inventory         },
    { "Debug",          ScreenType.Debug             },
    };
    

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }        
    }

    private void Start() {
        ShowScreen(startingScreen);
    }

    /// <summary>
    /// Displays the requested screen and hides the previous one.
    /// </summary>
    public void ShowScreen(ScreenType type) {
        var target = screens.Find(s => s.type == type);
        if (target == null) return;

        if (currentScreen != null) {
            // 1) Hide Current Canvas
            currentScreen.panel.SetActive(false);

            // 2) Trigger manager
            (currentScreen.manager as IScreenManager)?.OnScreenClose(); 
        }

        // 3) Display Next Canvas
        target.panel.SetActive(true);

        // 4) Trigger manager 
        (target.manager as IScreenManager)?.OnScreenOpen(); 

        // 5) Switch Action Map
        if (target.actionMap == ActionMap.UI) ActionMapManager.Instance.SwitchToUI();
        if (target.actionMap == ActionMap.Touch) ActionMapManager.Instance.SwitchToTouch();

        // 6) Play Sound (New)
        if (target.openMenuSound != null) SoundManager.Instance.PlaySFX(target.openMenuSound); // New        

        // 7) Update Current Screen
        currentScreen = target;
    }

    /// <summary>
    /// Overload to show screen using its string name.
    /// </summary>
    public void ShowScreen(string screenName) => ShowScreen(StringToType(screenName));

    private ScreenType StringToType(string name) {
        return nameToType.TryGetValue(name, out var type) ? type : ScreenType.None;
    }
}




