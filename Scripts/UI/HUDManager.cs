using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {

    [SerializeField] private PlayerData playerData;

    [SerializeField] private Text moneyText;
    [SerializeField] private Text healthText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text xpText;

    [SerializeField] private DoubleSliderBar doubleHealthBar; // Used to change Health Bar visual
    [SerializeField] private DoubleSliderBar doubleXpBar;    // Used to change XP Bar visual

    private void OnEnable() {
        Debug.Log("HUDManager OnEnable called");

        playerData.OnStatChanged += UpdateUI;
        UpdateUI();//new
    }

    private void OnDisable() {
        playerData.OnStatChanged -= UpdateUI;
    }

    public void UpdateUI() {   // Overkill to change everything ??
        UpdateMoneyUI();
        UpdateHealthUI();
        UpdateLevelUI();
    }

    public void UpdateMoneyUI() {
        moneyText.text = playerData.Money.ToString();
    }

    public void UpdateHealthUI() {
        healthText.text = playerData.Health.ToString() + " / " + playerData.MaxHealth.ToString();
        //healthBar.SetMaxValue(playerData.MaxHealth);
        //healthBar.SetValue(playerData.Health);
        doubleHealthBar.SetMaxValue(playerData.MaxHealth);
        doubleHealthBar.RemoveAmountWithEffect(playerData.Health);        
    }

    public void UpdateLevelUI() {
        int level = playerData.Level;
        int xp = playerData.Xp;
        int xpRequired = PlayerData.xpRequired[level - 1]; // = playerData.xpRequired[level - 1];

        doubleXpBar.SetMaxValue(xpRequired);
        doubleXpBar.AddAmountWithEffect(xp);
        levelText.text = level.ToString();
        xpText.text = xp.ToString() + " / " + xpRequired.ToString();
    }
}
