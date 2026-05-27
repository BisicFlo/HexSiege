
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour, IScreenManager {
    public static ShopManager Instance { get; private set; } // Singleton

    public int RefreshCost = 2;

    #region --------- Inspector Fields ---------
    [SerializeField] private PlayerData playerData; // money 

    [SerializeField] private InventoryData inventory; // Items / Turrets

    //[SerializeField] public  List<Color> RarityColors = new List<Color>(); // OLD =>  replaced by background images list  
    [SerializeField] private ColorData rarityColors; // New

    [SerializeField] ItemDatabase itemDatabase;

    [SerializeField] private List<Button> buttonList = new List<Button>();

    [SerializeField] private Button quitButton; // NEw

    [SerializeField] private Button refreshButton;

    [SerializeField] private List<ItemData> ItemsForSale = new List<ItemData>(); // List of 4 objects from the shop

    [SerializeField] private List<GameObject> UIItems = new List<GameObject>(); //  3D elements used in UI

    [SerializeField] private List<Image> imageList = new List<Image>(); // Temp -> improved ? // Used to display items 
    #endregion

    #region --------- Private Fields ---------
    // 1:Common | 2:Uncommon | 3:Rare | 4:Epic | 5:Legendary
    private static readonly int[,] weights =
       {{ 100,  0,  0,  0,  0 },  // Lvl 1
        {  75, 25,  0,  0,  0 },  // Lvl 2
        {  55, 30, 15,  0,  0 },  // Lvl 3
        {  45, 33, 20,  2,  0 },  // Lvl 4
        {  30, 40, 25,  5,  0 },  // Lvl 5
        {  16, 30, 43, 10,  1 },  // Lvl 6
        {  15, 20, 32, 30,  3 },  // Lvl 7
        {  10, 17, 25, 33, 15 },  // Lvl 8
        {   5, 10, 20, 40, 25 },  // Lvl 9
        {   1,  2, 12, 50, 35 }}; // Lvl 10
          
    private bool firstTimeShop = true;
    private int savedIndex = 0 ; // Used to save the index of the current button beeing display so it can be continued when reopening the shop
    #endregion

    private void Awake() {
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
        ActionMapManager.Instance.SwitchToUI();  // -> Should be UIManager

        if (firstTimeShop) {
            firstTimeShop = false;
            ClearAllButtons();
            StartCoroutine(SetupAllButtons());
        }
        else {
            Show3DUI(); // Enable visual 3D elements 
        }
    }
    public void OnScreenClose() {
        // Switch Action Map
        ActionMapManager.Instance.SwitchToTouch();
    }

    private void SetupButtonsEvents() {
        // Refresh Button 
        refreshButton.onClick.AddListener(() => RefreshShop());

        // Quit Button 
        quitButton.onClick.AddListener(() => QuitShopMenu());

        // Buy Buttons
        for (int i = 0; i < buttonList.Count; i++) {
            int buttonIndex = i; // used to save the index in the lambda expression  -> OnClick(i) stores OnClick(4)
            buttonList[i].onClick.AddListener(() => OnClick(buttonIndex));
        }
    }

    private void RemoveButtonsEvents() {
        // Refresh Button 
        refreshButton.onClick.RemoveAllListeners(); 

        // Quit Button 
        quitButton.onClick.RemoveAllListeners();

        // Buy Buttons
        for (int i = 0; i < buttonList.Count; i++) {
            int buttonIndex = i;
            buttonList[i].onClick.RemoveAllListeners();
        }
    }

    private void QuitShopMenu() {
        // Switch Action Map
        ActionMapManager.Instance.SwitchToTouch();

        // Change Canvas displayed
        UIManager.Instance.ShowScreen(ScreenType.HUD);

        //Clear3DShop();
        Hide3DUI();
    }

    public void OnClick(int index) {

        ItemData item = ItemsForSale[index];
        //GameObject worldModel = turret.WorldModel;

        if (BuySomething(item.Price)) { // If Player can buy -> buy -> 

            //Should Verify If enough space in inventory

            if (item.TypeOfItem == ItemType.Turret) {
                inventory.AddTurret(item, 1); // -> TurretContainer
                if (UIItems[index] != null) UIItems[index].SetActive(false);

            }
            else if (item.TypeOfItem == ItemType.Boost) {
                inventory.AddItem(item, 1); // -> Container
                TurretBoostData boost = item as TurretBoostData;
                boost.BoostAllCurrentTurret();
                ChangeImageButton(buttonList[index], null, false); // hide sprite // NEW

                if (boost.effects != null) { // new
                    for (int i = 0; i < boost.effects.Count; i++) {
                        boost.effects[i].OnApply(boost);
                    }
                }
            }
            DesactivateInteractable(index);// Add Image ? Closed Door ?
        }
    }

    private void Hide3DUI() {
        for (int i = 0; i < UIItems.Count; i++) {
            if (UIItems[i] != null)
                UIItems[i].SetActive(false);
        }
    }
    private void Show3DUI() {
        for (int i = 0; i < UIItems.Count; i++) {
            // -> If interactable == false -> Button has been desactivated -> no need to display 3D Icon
            if (buttonList[i].GetComponent<Button>().interactable == true) {
                if (UIItems[i] != null) {
                    UIItems[i].SetActive(true);
                }
            }
        }
    }
    
    private void DesactivateOneButton(int index) {
        buttonList[index].gameObject.SetActive(false);
    }
    
    private void DesactivateInteractable(int index) {
        buttonList[index].GetComponent<Button>().interactable = false;
    }
    
    private void ActivateInteractable(int index) {
        buttonList[index].GetComponent<Button>().interactable = true;
    }

    private bool BuySomething(int price) {
        if (playerData.Money < price) {
            return false; // Not enough money
        } else {
            playerData.LoseMoney(price);
            return true;
        }
    }

    private int PickRarity(int level) {

        int rarity = 1;
        int randomNumber = UnityEngine.Random.Range(1, 101); // (int minInclusive, int maxExclusive);
        int length = weights.GetLength(1); // => 5

        for (int i = 0; i < length; i++) {
            randomNumber -= weights[level - 1, i];
            if (randomNumber <= 0) return rarity;
            rarity++;
        }
        return -1;
    }

    private TurretData PickTurret(int rarity) {
        return itemDatabase.GetRandomTurretFromRarity(rarity);
    }

    private ItemData PickItem( ) {
        return itemDatabase.GetRandomItem();
    }

    private void ChangeColorButtonFromRarity(Button button, int rarity) {
        if (rarity < 1 || rarity > 6) return; // 6 : items
        button.image.color = rarityColors.ColorList[rarity - 1]; // New        
    }

    private void ChangePriceButton(Button button, int price) {
        button.transform.GetChild(0).GetComponent<Text>().text = price.ToString();
    }

    private void ChangeImageButton(Button button, Sprite sprite, bool setActive) {
        if (setActive) {
            button.transform.GetChild(2).gameObject.SetActive(true);
            button.transform.GetChild(2).GetComponent<Image>().sprite = sprite;
        }
        else {
            button.transform.GetChild(2).gameObject.SetActive(false);
        }

    }

    private void SetupOneButton(Button button) {
        int rarity = PickRarity(playerData.Level);

        bool selectTurret =  Random.Range(0, 100) < 70; // 70 % chance
        
        if (selectTurret || playerData.Level < 3) { // For Turrets 
            TurretData selectedTurret = PickTurret(rarity);
            ChangeColorButtonFromRarity(button, rarity);
            ChangePriceButton(button, selectedTurret.Price);
            button.gameObject.SetActive(true);

            ItemsForSale.Add(selectedTurret);

            InstantiateOne3DUI(savedIndex); // New
            ChangeImageButton(button, null, false);
        }
        else { // For Items  / -> TurretBoostData
            ItemData selectedItem = PickItem(); // ?? TurretBoostData
            ChangeColorButtonFromRarity(button, 6); // 6 for items
            ChangePriceButton(button, selectedItem.Price);
            button.gameObject.SetActive(true);

            ItemsForSale.Add(selectedItem);

            ChangeImageButton(button, selectedItem.Icon, true);

            UIItems.Add(null);
        }
        button.interactable = true; // ? ActivateInteractable(int index)
    }

    private void ChangeButtonImage( int index ) {
        //BoostData boostData = ItemsForSale as BoostData;
        //if (boostData != null) {
        //    Sprite image = boostData.Image;
        //}

        //imageList

        //    ItemsForSale.
        //    TurretBoostData boost
    }

    private void InstantiateAllShop() {
        Vector3 pos = Vector3.zero;

        for (int i = 0; i < ItemsForSale.Count; i++) {
            pos.x = (2 * i);
            pos.y = -10;

            GameObject turret = Instantiate(ItemsForSale[i].UIModel, pos, Quaternion.identity);
            //ChangeLayerToUI3D(turret);  //done in prefab
            UIItems.Add(turret);
        }
    }

    private void InstantiateOne3DUI(int index) {
        Vector3 pos = Vector3.zero;
        pos.x = (2 * index);
        pos.y = -11; // -> Scriptable ItemData -> "Height to be spawned"
        GameObject turret = Instantiate(ItemsForSale[index].UIModel, pos, Quaternion.identity);
        turret.transform.localScale = Vector3.one * 0.6f ;
        //ChangeLayerToUI3D(turret);  //done in prefab
        UIItems.Add(turret);
    }

    private void RemoveFrom3DDisplay(int index) {
        GameObject turret = UIItems[index];
        UIItems.Remove(turret);
        Destroy(turret);
    }

    private void Clear3DShop() {
        for (int i = 0; i < UIItems.Count; i++) {
            //RemoveFrom3DDisplay(i);
            GameObject turret = UIItems[i];
            Destroy(turret);
        }
        UIItems.Clear();    
    }

    private void ClearAllButtons() {      
        foreach (var button in buttonList) {
            button.gameObject.SetActive(false); 
        }           
    }

    public void RefreshShop() {

        if (BuySomething(RefreshCost)) {

            GameEvents.ShopRerolled();

            savedIndex = 0;

            //RefreshButton.gameObject.SetActive(false);

            ClearAllButtons();
            Clear3DShop();
            ItemsForSale.Clear();
            StartCoroutine(SetupAllButtons());
        }
    }

    private IEnumerator SetupAllButtons() {

        while (savedIndex < buttonList.Count) {
            yield return new WaitForSeconds(0.5f); // Should Optimize

            SetupOneButton(buttonList[savedIndex]);
            //InstantiateOne3DUI(savedIndex); // Only if it's a turret  -> in 
            savedIndex++;
        }
        yield return new WaitForSeconds(1);
        refreshButton.gameObject.SetActive(true);
    }
}
