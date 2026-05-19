using UnityEngine;
using System.Collections.Generic;

// IScreenManager.cs ?
public interface IScreenManager {
    void OnScreenOpen();
    void OnScreenClose();
}

public class UIManager : MonoBehaviour {
    public static UIManager Instance { get; private set; }

    [SerializeField] private ScreenType startingScreen;

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

    public void ShowScreen(ScreenType type) {
        var target = screens.Find(s => s.type == type);
        if (target == null) return;

        if (currentScreen != null) {
            //Debug.Log("currentScreen.panel " + currentScreen.panel.name);
            currentScreen.panel.SetActive(false);
            (currentScreen.manager as IScreenManager)?.OnScreenClose(); // NEW
        }
        else {
            Debug.Log("currentScreen null");
        }
        target.panel.SetActive(true);
        (target.manager as IScreenManager)?.OnScreenOpen(); // NEW
        currentScreen = target;
    }

    // Quick overload for string / enum
    public void ShowScreen(string screenName) => ShowScreen(StringToType(screenName));

    private ScreenType StringToType(string name) {
        return nameToType.TryGetValue(name, out var type) ? type : ScreenType.None;
    }
}

[System.Serializable]
public class Screen {
    public ScreenType type;
    public GameObject panel;// or CanvasGroup canvasGroup;
    public MonoBehaviour manager; // MonoBehaviour so it's showned in the Inspector
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