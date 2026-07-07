using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildManagerV2 : MonoBehaviour, IScreenManager {
    public static BuildManagerV2 Instance { get; private set; } // Singleton

    public InventoryData Inventory; // same ScriptableObject Inventory In "Player.cs"

    public List<Turret> WorldTurrets = new List<Turret>(); // Used For Boosts / Items

    [SerializeField] private List<CardConfig> CardList = new List<CardConfig>(); // New


    [HideInInspector] public GameObject SelectedTurret;
    [HideInInspector] public GameObject SelectedTile;

    [SerializeField] private ColorData rarityColors; // New

    [SerializeField] private List<Button> buttonList = new List<Button>();
    [SerializeField] private List<Text> quantityTextList = new List<Text>();

    //[SerializeField] private Button quitButton;

    [Header("Sounds")]
    [SerializeField] private SoundData BuildSound; // New

    private void Awake() {
        if (Instance != null) Debug.LogWarning("More than one BuildManagerV2 detected");
        Instance = this;
    }
    private void OnEnable() {
        SetupButtonsEvents();
    }
    private void OnDisable() {
        RemoveButtonsEvents();
    }
    public void OnScreenOpen() {

        // 1) Clear and Setup all cards 
        ClearAllCard();
        StartCoroutine(DisplayAllCards());

        // 2) Display Quantities
        SetupQuantityOnButton();
    }
    public void OnScreenClose() {
        // Switch Action Map
        //ActionMapManager.Instance.SwitchToTouch();  # In UI manager now
    }
    private void QuitBuildMenu() {
        //ActionMapManager.Instance.SwitchToTouch();
        UIManager.Instance.ShowScreen(ScreenType.HUD);
        //Clear3DShop();
    }
    private void SetupButtonsEvents() {
        // Quit Button 
        //quitButton.onClick.AddListener(() => QuitBuildMenu());

        // Build Buttons
        for (int i = 0; i < buttonList.Count; i++) {
            int buttonIndex = i; // used to save the index in the lambda expression  -> OnClick(i) stores OnClick(4)
            buttonList[i].onClick.AddListener(() => OnClick(buttonIndex)); 
        }
    }
    private void RemoveButtonsEvents() {
        // Quit Button 
        //quitButton.onClick.RemoveAllListeners();

        // Build Buttons
        for (int i = 0; i < buttonList.Count; i++) {
            int buttonIndex = i; 
            buttonList[i].onClick.RemoveAllListeners();
        }
    }

    public void OnClick(int index) { // Called by Buttons of the Inventory
        Debug.Log("OnClick : " + index);
        if (index >= Inventory.TurretContainer.Count) return;

        ItemData turret = Inventory.TurretContainer[index].Item; // Should set as TurretData
        GameObject model = turret.WorldModel;

        if (model == null) return;

        Turret createdTurret = BuildTurret(model, SelectedTile.transform.position);
        // SetUp Intanciated Turret with Value in The scriptable Object ! <--------------------------------------------------------------------------

        SetupNode(SelectedTile, createdTurret); // New

        Inventory.ApplyAllBoostToTurret(createdTurret); // XXXXXXXXXX

        Debug.Log("Starting Coroutine");
        StartCoroutine(TempoBeforeApplyBoost(createdTurret));

        //Apply (createdTurret); // Test ?

        SoundManager.Instance.PlaySFX(BuildSound); // New


        RemoveFromInventory(turret); //?

        QuitBuildMenu();
    }
    public bool SetupNode(GameObject selectedTile, Turret createdTurret) {
        if (selectedTile == null || createdTurret == null) return false;

        
        if (!selectedTile.TryGetComponent<Node>(out var node)) return false;

        bool isTurretPlaced = node.SetTurret(createdTurret);

        return isTurretPlaced;
    }
    private void ClearAllCard() {
        foreach (var card in CardList) {
            Debug.Log("Card hidden");
            card.gameObject.SetActive(false);
        }
    }
    private void PassDataToCardConfig(CardConfig cardConfig, TurretData turretData) {
        Debug.Log("TurretData");
        cardConfig.itemDataSelected = null;
        cardConfig.turretDataSelected = turretData;
        cardConfig.turretSelected = null;
        cardConfig.turretBoostSelected = null;
    }
    private void HandleOneItem(int index) {
        Debug.Log("Inventory.TurretContainer.Count : " + Inventory.TurretContainer.Count);
        if (index >= Inventory.TurretContainer.Count) { Debug.Log("index : " + index); return; }

        TurretData selectedTurret = (TurretData)Inventory.TurretContainer[index].Item;
        PassDataToCardConfig(CardList[index], selectedTurret);
    }
    private void DisplayOneCard(int index) {
        CardConfig cardConfig = CardList[index];

        cardConfig.gameObject.SetActive(true);
        cardConfig.MainSetup();
    }
    private void SetupQuantityOnButton() {
        int maxIndexTurret = Inventory.TurretContainer.Count; 
        for (int i = 0; i < quantityTextList.Count; i++) { // quantityTextList.Count == 4 

            if (i < maxIndexTurret) {                
                int quantity = Inventory.TurretContainer[i].Amount;
                //  Turret at this index -> Display Quantity Icon + Set Quantity
                quantityTextList[i].transform.parent.gameObject.SetActive(true);
                quantityTextList[i].text = "x" + quantity.ToString();                
            }
            else {
                // No Turret at this index -> Hide Quantity Icon
                quantityTextList[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }
    private int GetTotalQuantityOfItem() { // ?
        int totalQuantity = 0;

        for (int i = 0;i < Inventory.TurretContainer.Count; i++) {
            totalQuantity += Inventory.TurretContainer[i].Amount;
        }

        return totalQuantity;

    }
    private void RemoveFromInventory(ItemData turret) {
        Inventory.RemoveTurret(turret, 1);
    }
    public Turret BuildTurret(GameObject prefab, Vector3 position) {
        if (prefab == null) return null;
        Turret myTurret = Instantiate(prefab, position, Quaternion.identity).GetComponent<Turret>();         //

        WorldTurrets.Add(myTurret);

        return myTurret;
    }
    private IEnumerator DisplayAllCards() {
        int min = Mathf.Min(Inventory.TurretContainer.Count, CardList.Count);

        for (int i = 0; i < min; i++) {
            HandleOneItem(i);
            yield return null; // no tempo

            DisplayOneCard(i);

        }
    }
    private void ApplyAllBoostToTurret(Turret turret) { // For Items
        InventorySlot inventorySlot;
        TurretBoostData turretBoostData;
        int amount;

        for (int i = 0; i < Inventory.Container.Count; i++) {

            inventorySlot = Inventory.Container[i];

            if (inventorySlot.Item.TypeOfItem == ItemType.Boost) {

                turretBoostData = inventorySlot.Item as TurretBoostData;

                amount = inventorySlot.Amount;
                Debug.Log("Amount : " + amount);

                for (int j = 0; j < amount; j++) {

                    turretBoostData.ApplyToTurret(turret);
                }
            }
        }
    }
    private IEnumerator TempoBeforeApplyBoost(Turret turret) {
        Debug.Log("Starting WaitForSeconds");
        yield return new WaitForSeconds(1);
        Debug.Log("Starting Apply");
        ApplyAllBoostToTurret(turret);
    }

}
