
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManagerV2 : MonoBehaviour, IScreenManager {
    public static ShopManagerV2 Instance { get; private set; } // Singleton // ?? Used

    public int RerollCost = 2;

    [SerializeField] private Text RerollCostText;

    #region --------- Inspector Fields ---------
    [SerializeField] private PlayerData playerData; // money 
    [SerializeField] private InventoryData inventory; // Items / Turrets
    //[SerializeField] private ColorData rarityColors; // New
    [SerializeField] ItemDatabase itemDatabase;

    [SerializeField] private List<CardConfig> CardList = new List<CardConfig>();

    [SerializeField] private Button quitButton; // NEw
    [SerializeField] private Button refreshButton;

    [SerializeField] private List<ItemData> ItemsForSale = new List<ItemData>(); // List of 4 objects from the shop
    //[SerializeField] private List<GameObject> UIItems = new List<GameObject>(); //  3D elements used in UI

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
    private int savedIndex = 0; // Used to save the index of the current button beeing display so it can be continued when reopening the shop
    #endregion

    private void Awake() {
        if (Instance != null) Debug.LogWarning("More than one ShopManagerV2 detected");
        Instance = this;
    }
    private void OnEnable() {
        SetupButtonsEvents();
    }
    private void OnDisable() {
        RemoveButtonsEvents();
    }
    public void OnScreenOpen() {
        if (firstTimeShop) {
            firstTimeShop = false;
            ClearAllCard();
            StartCoroutine(DisplayAllCards());
            RerollCostText.text = RerollCost.ToString();    
        }
    }
    public void OnScreenClose() {

    }

    private void SetupButtonsEvents() {
        // Refresh Button 
        refreshButton.onClick.AddListener(() => RefreshShop());

        // Quit Button 
        quitButton.onClick.AddListener(() => QuitShopMenu());

        // Buy Buttons
        for (int i = 0; i < CardList.Count; i++) {
            int buttonIndex = i; // used to save the index in the lambda expression  -> OnClick(i) stores OnClick(4)
            CardList[i].BuyButton.onClick.AddListener(() => OnClick(buttonIndex));
        }
    }

    private void RemoveButtonsEvents() {
        // Refresh Button 
        refreshButton.onClick.RemoveAllListeners();

        // Quit Button 
        quitButton.onClick.RemoveAllListeners();

        // Buy Buttons
        for (int i = 0; i < CardList.Count; i++) {
            int buttonIndex = i;
            CardList[i].BuyButton.onClick.RemoveAllListeners();
        }
    }

    private void QuitShopMenu() {
        // Change Canvas displayed
        UIManager.Instance.ShowScreen(ScreenType.HUD);
    }

    public void OnClick(int index) {

        ItemData item = ItemsForSale[index];

        if (BuySomething(item.Price)) { // If Player can buy -> buy -> 

            //Should Verify If enough space in inventory

            if (item.TypeOfItem == ItemType.Turret) {
                inventory.AddTurret(item, 1); // -> TurretContainer

            }
            else if (item.TypeOfItem == ItemType.Boost) {
                inventory.AddItem(item, 1); // -> Container
                TurretBoostData boost = item as TurretBoostData;
                boost.BoostAllCurrentTurret();


                if (boost.effects != null) { // new
                    for (int i = 0; i < boost.effects.Count; i++) {
                        boost.effects[i].OnApply(boost);
                    }
                }
            }
            CardList[index].gameObject.SetActive(false);// Add Image ? Closed Door ?
        }
    }

    private bool BuySomething(int price) {
        if (playerData.Money < price) {
            return false; // Not enough money
        }
        else {
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

    private ItemData PickItem() {
        return itemDatabase.GetRandomItem();
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
        //int rarity = PickRarity(playerData.Level);

        //bool selectTurret = Random.Range(0, 100) < 70; // 70 % chance

        //if (selectTurret || playerData.Level < 3) { // For Turrets 
        //    TurretData selectedTurret = PickTurret(rarity);
        //    ChangeColorButtonFromRarity(button, rarity);
        //    ChangePriceButton(button, selectedTurret.Price);
        //    button.gameObject.SetActive(true);

        //    ItemsForSale.Add(selectedTurret);

        //    InstantiateOne3DUI(savedIndex); // New
        //    ChangeImageButton(button, null, false);
        //}
        //else { // For Items  / -> TurretBoostData
        //    ItemData selectedItem = PickItem(); // ?? TurretBoostData
        //    ChangeColorButtonFromRarity(button, 6); // 6 for items
        //    ChangePriceButton(button, selectedItem.Price);
        //    button.gameObject.SetActive(true);

        //    ItemsForSale.Add(selectedItem);

        //    ChangeImageButton(button, selectedItem.Icon, true);

        //    UIItems.Add(null);
        //}
        //button.interactable = true; // ? ActivateInteractable(int index)
    }

    private void PassDataToCardConfig(CardConfig cardConfig, TurretData turretData ) {
        Debug.Log("TurretData");
        cardConfig.itemDataSelected = null;
        cardConfig.turretDataSelected = turretData;
        cardConfig.turretSelected = null;
        cardConfig.turretBoostSelected = null;
    }
    private void PassDataToCardConfig(CardConfig cardConfig, ItemData itemData) {
        Debug.Log("ItemData");
        cardConfig.itemDataSelected = null;
        cardConfig.turretDataSelected = null;
        cardConfig.turretSelected = null;
        cardConfig.turretBoostSelected = (TurretBoostData)itemData;
    }

    private void GetAndHandleOneItem(int index ) {
        int rarity = PickRarity(playerData.Level);

        bool isTurret = Random.Range(0, 100) < 70; // 70 % chance

        if (isTurret || playerData.Level < 3) { // For Turrets  // no item before lvl 3

            TurretData selectedTurret = PickTurret(rarity);
            ItemsForSale.Add(selectedTurret);
            PassDataToCardConfig(CardList[index], selectedTurret);
        }
        else {
            ItemData selectedItem = PickItem(); // ?? TurretBoostData
            Debug.Log("selectedItem : " + selectedItem.name);
            ItemsForSale.Add(selectedItem);
            PassDataToCardConfig(CardList[index], selectedItem);
        }
    }

    private void DisplayOneCard(int index) {
        CardConfig cardConfig = CardList[index];

        cardConfig.gameObject.SetActive(true);
        cardConfig.MainSetup();
    }

    private void ClearAllCard() {
        foreach (var card in CardList) {
            card.gameObject.SetActive(false);
        }
    }

    public void RefreshShop() {

        if (BuySomething(RerollCost)) {

            Debug.Log("Reroll");

            GameEvents.ShopRerolled();

            savedIndex = 0;

            refreshButton.gameObject.SetActive(false);
            ClearAllCard();
            ItemsForSale.Clear();
            StartCoroutine(DisplayAllCards());

            RerollCostText.text = RerollCost.ToString();
        }
    }

    private IEnumerator DisplayAllCards() {

        while (savedIndex < CardList.Count) {

            GetAndHandleOneItem(savedIndex);

            yield return new WaitForSeconds(.6f); // Should Optimize /!\ 

            DisplayOneCard(savedIndex);

            savedIndex++;
        }
        yield return new WaitForSeconds(1);

        refreshButton.gameObject.SetActive(true);
    }
}
