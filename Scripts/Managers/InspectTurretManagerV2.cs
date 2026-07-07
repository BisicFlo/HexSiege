using UnityEngine;
using UnityEngine.UI;

public class InspectTurretManagerV2 : MonoBehaviour, IScreenManager {
    public static InspectTurretManagerV2 Instance { get; private set; } // Singleton needed ? 

    public Turret selectedTurret;

    [SerializeField] private ColorData rarityColors;

    //[SerializeField] private Button quitButton;
    [SerializeField] private Button destroyButton;

    [SerializeField] private CardConfig inspectCard;

    [Header("Sounds")]
    [SerializeField] private SoundData destroySound; // New

    private void Awake() {
        if (Instance != null) Debug.LogWarning("More than one InspectManager detected");
        Instance = this;
    }

    private void OnEnable() {
        SetupButtonsEvents();
    }
    private void OnDisable() {
        ClearButtonsEvents();
    }
    public void OnScreenOpen() {
        DisplayTurret();
    }
    public void OnScreenClose() {

    }

    public void DisplayTurret() {
        if (selectedTurret == null) return;
        PassDataToCardConfig(inspectCard, selectedTurret);

        inspectCard.gameObject.SetActive(true);
        inspectCard.MainSetup();
    }

    private void PassDataToCardConfig(CardConfig cardConfig, Turret turret) {
        Debug.Log("TurretData");
        cardConfig.itemDataSelected = null;
        cardConfig.turretDataSelected = null;
        cardConfig.turretSelected = turret;
        cardConfig.turretBoostSelected = null;
    }

    private void SetupButtonsEvents() {   
        //quitButton.onClick.AddListener(() => QuitInspectMenu());
        destroyButton.onClick.AddListener(() => DestroyTurretAndQuit());
    }

    private void ClearButtonsEvents() {
        //quitButton.onClick.RemoveAllListeners();
        destroyButton.onClick.RemoveAllListeners();
    }

    private void QuitInspectMenu() {
        UIManager.Instance.ShowScreen(ScreenType.HUD);
    }

    private void DestroyTurretAndQuit() {
        SoundManager.Instance.PlaySFX(destroySound); // New

        Destroy(selectedTurret.gameObject);
        // Remove from somewhere ? List ?
        QuitInspectMenu();
    }
}
