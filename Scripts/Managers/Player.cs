using UnityEngine;

public class Player : MonoBehaviour {

    public static Player instance { get; private set; } // Singleton

    [SerializeField] private InventoryData inventory;

    [SerializeField] private PlayerData playerData;

    //[SerializeField] private HealthBar healthBar ; // Used to change HealthBar visual

    [SerializeField] private HUDManager hudManager;

    private void Start() {
        if (instance != null) Debug.LogWarning("More than one Player detected");
        instance = this;

        inventory.Container.Clear();// 
        inventory.TurretContainer.Clear();// 

        SetupPlayerData();
    }

    private void SetupPlayerData() {

        // - PlayerData - 
        playerData.Health = 10;
        playerData.MaxHealth = 10;
        playerData.Money = 10;
        playerData.Level = 1;
        playerData.Xp = 0;

        // - HealthBar - 
        //healthBar.SetMaxValue(playerData.MaxHealth);   // in  HUDManager
        //healthBar.SetValue(playerData.Health);

        hudManager.UpdateUI();
    }

    public void TakeDamage(int amount) {
        playerData.Health -= amount;
        if (playerData.Health <= 0) {
            //defeat
            UIManager.Instance.ShowScreen(ScreenType.GameOver);
        }
        hudManager.UpdateHealthUI(); // healthBar.SetValue(playerData.Health);

    }
    public void GainMoney(int amount) {
        //Debug.Log("GainMoney : " + amount + "=" + playerData.Money);
        playerData.GainMoney(amount);

        hudManager.UpdateMoneyUI();
    }

    public void GainXp(int amount) {
        //Debug.Log("GainXp : " + amount + " = " + playerData.Xp);
        playerData.GainXp(amount);

        hudManager.UpdateLevelUI();
    }




    // Used With Object In the Scene 
    public void AddItemToInventory(Item item) {
        inventory.AddItem(item.item, 1);
    }

    //overrloading , for object in the shop 
    public void AddItemToInventory(ItemData itemdata) {
        inventory.AddItem(itemdata, 1);
    }

    public void AddTurretToInventory(ItemData itemdata) {
        inventory.AddTurret(itemdata, 1);
    }

    public void AddTurretToInventory(Item item) {
        inventory.AddTurret(item.item, 1);
    }




    private void OnApplicationQuit() {
        //Inventory.Container.Clear();
    }
}
