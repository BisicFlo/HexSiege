using UnityEngine;

public class Player : MonoBehaviour {
    public static Player Instance { get; private set; } // Singleton

    [SerializeField] private InventoryData inventory;

    public PlayerData PlayerData;

    //[SerializeField] private HealthBar healthBar ; // Used to change HealthBar visual

    //[SerializeField] private HUDManager hudManager;

    //public int Health { get; private set; }
    //public int MaxHealth { get; private set; }
    //public int Money { get; private set; }
    //public int Level { get; private set; }
    //public int Xp { get; private set; }

    private void OnEnable() {
        GameEvents.OnPlayerHit += HandleOnPlayerHit;
    }

    private void OnDisable() {
        GameEvents.OnPlayerHit -= HandleOnPlayerHit;
    }

    private void Awake() {
        if (Instance != null) Debug.LogWarning("More than one Player detected");
        Instance = this;

        inventory.Container.Clear();// 
        inventory.TurretContainer.Clear();// 

        //SetupPlayerData();

        PlayerData.Init(); //New
    }

    private void HandleOnPlayerHit(Enemy e, int dmg, bool canBeFatal) {

        PlayerData.TakeDamage(dmg);

        if (PlayerData.Health <= 0) {
            if (!canBeFatal) PlayerData.SetPlayerLife(1);
            else {
                //defeat
                UIManager.Instance.ShowScreen(ScreenType.GameOver);
            }
        }
    }



    //private void SetupPlayerData() {

    //    //Health = playerData.Health;
    //    //MaxHealth = playerData.MaxHealth;
    //    //Money  = playerData.Money;
    //    //Level = playerData.Level;
    //    //Xp = playerData.Xp;


    //    // - HealthBar - 
    //    //healthBar.SetMaxValue(playerData.MaxHealth);   // in  HUDManager
    //    //healthBar.SetValue(playerData.Health);

    //    hudManager.UpdateUI();
    //}

    //public void TakeDamage(int amount) {
    //    playerData.Health -= amount;
    //    if (playerData.Health <= 0) {
    //        //defeat
    //        UIManager.Instance.ShowScreen(ScreenType.GameOver);
    //    }
    //    hudManager.UpdateHealthUI(); // healthBar.SetValue(playerData.Health);

    //}
    //public void GainMoney(int amount) {
    //    //Debug.Log("GainMoney : " + amount + "=" + playerData.Money);
    //    playerData.GainMoney(amount);

    //    hudManager.UpdateMoneyUI();
    //}
    //public void GainXp(int amount) {
    //    //Debug.Log("GainXp : " + amount + " = " + playerData.Xp);
    //    playerData.GainXp(amount);

    //    hudManager.UpdateLevelUI();
    //}
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
}
