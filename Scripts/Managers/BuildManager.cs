using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour, IScreenManager {
    public static BuildManager Instance { get; private set; } // Singleton

    public InventoryData Inventory; // same ScriptableObject Inventory In "Player.cs"

    private List<GameObject> UITurrets = new List<GameObject>(); 
    public List<Turret> WorldTurrets = new List<Turret>(); //?

    [HideInInspector] public GameObject SelectedTurret;
    [HideInInspector] public GameObject SelectedTile;

    [SerializeField] private ColorData rarityColors; // New

    [SerializeField] private List<Button> buttonList = new List<Button>();
    [SerializeField] private List<Text> quantityTextList = new List<Text>();

    [SerializeField] private Button quitButton;

    private void Awake() {
        //Debug.Log("InstanceCreated");
        if (Instance != null) Debug.LogWarning("More than one BuildManager detected");
        Instance = this;
    }

    private void OnEnable() {
        SetupButtonsEvents();
    }

    private void OnDisable() {
        RemoveButtonsEvents();
    }

    public void OnScreenOpen() {
        // Switch Action Map
        //ActionMapManager.Instance.SwitchToUI(); # In UI manager now

        // 1) Instantiate Turrets to be rendered as icons on buttons  
        InstantiateInventoryAndSetupButton();
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
        Clear3DShop();
    }

    private void SetupButtonsEvents() {
        // Quit Button 
        quitButton.onClick.AddListener(() => QuitBuildMenu());

        // Build Buttons
        for (int i = 0; i < buttonList.Count; i++) {
            int buttonIndex = i; // used to save the index in the lambda expression  -> OnClick(i) stores OnClick(4)
            buttonList[i].onClick.AddListener(() => OnClick(buttonIndex)); 
        }
    }

    private void RemoveButtonsEvents() {
        // Quit Button 
        quitButton.onClick.RemoveAllListeners();

        // Build Buttons
        for (int i = 0; i < buttonList.Count; i++) {
            int buttonIndex = i; 
            buttonList[i].onClick.RemoveAllListeners();
        }
    }


    // Notes : instanciate turret -> put in Iconturret list -> changelayer UI3D

    // Called by Buttons of the Inventory
    public void OnClick(int index) {
        Debug.Log("OnClick : " +  index);   
        if (index >= Inventory.TurretContainer.Count) return;

        ItemData turret = Inventory.TurretContainer[index].Item;
        GameObject worldModel = turret.WorldModel;

        if (worldModel != null) {

            //ReloadNeeded = true;
            Turret createdTurret = BuildTurret(worldModel, SelectedTile.transform.position);
            // SetUp Intanciated Turret with Value in The scriptable Object ! <--------------------------------------------------------------------------

            // Desactivate the ring
            // SelectedTile.SetActive(false); // OLD 

            SetupNode(SelectedTile, createdTurret); // New

            //Inventory.ApplyAllBoostToTurret(createdTurret); // XXXXXXXXXX
            Debug.Log("Starting Coroutine");
            StartCoroutine(TempoBeforeApplyBoost(createdTurret));
            //Apply (createdTurret); // Test ?

            RemoveFromInventory(turret); //?
            RemoveFromDisplay(index); //?

            QuitBuildMenu();

            Clear3DShop(); //new
        }
    }

    public bool SetupNode(GameObject selectedTile, Turret createdTurret) {
        if (selectedTile == null || createdTurret == null) return false;

        Node node = selectedTile.GetComponent<Node>();

        if (node == null) return false;

        bool isTurretPlaced = node.SetTurret(createdTurret);

        return isTurretPlaced;
    }

    private void Apply(Turret turret ) {
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

    //private void SetupButton() {
    //    for (int i = 0; i < buttonList.Count; i++) {
    //        ChangeColorButtonFromRarity(buttonList[i], rarity);            
    //    }
    //}

    //public bool AddTurretToInventory(GameObject turret) {
    //    if (inventoryTurrets.Count >= maxTurret) {
    //        inventoryTurrets.Add(turret);
    //        return true;
    //    } else {
    //        return false;
    //    }
    //}

    //public bool RemoveTurretFromInventory(GameObject turret) {
    //    return inventoryTurrets.Remove(turret);
    //}

    
    private void InstantiateInventoryAndSetupButton() { // pb with item quantity >1
         // If Inventory.TurretContainer.Count > 4 ?????
        Vector3 pos = Vector3.zero;
        //int totalQuantity = GetTotalQuantityOfItem(); 

        for (int i = 0; i < Inventory.TurretContainer.Count; i++) { // 
            pos.x = (2 * i);
            pos.y = -11.1f;
            ItemData myItem = Inventory.TurretContainer[i].Item;

            GameObject turret = Instantiate(myItem.UIModel, pos, Quaternion.identity);

            turret.transform.localScale = Vector3.one * 0.6f;
            //ChangeLayerToUI3D(turret);  //done in prefab
            UITurrets.Add(turret);

            ChangeColorButtonFromRarity(buttonList[i], myItem.Rarity);
        }
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

    private int GetTotalQuantityOfItem() {
        int totalQuantity = 0;

        for (int i = 0;i < Inventory.TurretContainer.Count; i++) {
            totalQuantity += Inventory.TurretContainer[i].Amount;
        }

        return totalQuantity;

    }
    private void RemoveFromInventory(ItemData turret) {
        Inventory.RemoveTurret(turret, 1);
    }

    private void RemoveFromDisplay(int index) {
        GameObject turret = UITurrets[index];
        UITurrets.Remove(turret);
        Destroy(turret);
    }

    public Turret BuildTurret(GameObject prefab, Vector3 position) {
        if (prefab == null) return null;
        Turret myTurret = Instantiate(prefab, position, Quaternion.identity).GetComponent<Turret>(); 
        //

        WorldTurrets.Add(myTurret);

        return myTurret;
    }

    private void ChangeColorButtonFromRarity(Button button, int rarity) {
        //button.image.color = ShopManager.Instance.RarityColors[rarity - 1];
        if (rarity < 1 || rarity > 5) return; 
        button.image.color = rarityColors.ColorList[rarity - 1]; // New     
    }

    private void ChangeLayerToUI3D(GameObject prefab) {
        prefab.layer = 7;
    }

    private void UpdateUI() {

    }

    //private void CloseUI() {
    //    UIManager.Instance.ShowScreen("HUD");
    //}

    private void Clear3DShop() {

        for (int i = 0; i < UITurrets.Count; i++) {
            //RemoveFrom3DDisplay(i);
            GameObject turret = UITurrets[i];
            Destroy(turret);
        }

        UITurrets.Clear();
    }

    //private void UpdateTurretOfNodeFromTile(GameObject tile,Turret turret ) {
    //    if (tile.TryGetComponent<Node>(out var tileNode)) {
    //        tileNode.SetTurret(turret);
    //    }
    //}

    //private bool CheckIfTileEmpty(Vector3 position) {
    //    for (int i = 0; i < placedTurrets.Count; i++) {
    //        if (placedTurrets[i].transform.position == position) return false;
    //    }
    //    return true;
    //}

    //private void ShowTurretInventory() {
    //    for (int i = 0; i < inventoryTurrets.Count; i++) {
    //        inventoryTurrets[i].SetActive(true);
    //    }
    //}
    //private void HideTurretInventory() {
    //    for (int i = 0; i < inventoryTurrets.Count; i++) {
    //        inventoryTurrets[i].SetActive(false);
    //    }
    //}


    private System.Collections.IEnumerator TempoBeforeApplyBoost(Turret turret) {
        Debug.Log("Starting WaitForSeconds");
        yield return new WaitForSeconds(1);
        Debug.Log("Starting Apply");
        Apply(turret);
    }

}
