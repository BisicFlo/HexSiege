using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {

    [SerializeField] private PlayerData playerData;

    [SerializeField] private Text moneyText;
    [SerializeField] private Text healthText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text xpText;

    [SerializeField] private DoubleSliderBar doubleHealthBar; // Used to change Health Bar visual
    [SerializeField] private XpBar simpleXpBar;    // Used to change XP Bar visual   

    private void OnEnable() {
        Debug.Log("HUDManager OnEnable called");

        playerData.OnStatChanged += UpdateUI;
        playerData.OnXpGained += UpdateLevelUI;


    }

    private void OnDisable() {
        playerData.OnStatChanged -= UpdateUI;
        playerData.OnXpGained -= UpdateLevelUI;
    }

    private void Start() {
        //UpdateLevelUI
        UpdateUI();//new
        UpdateLevelUI(0);
    }

    public void UpdateUI() {   // Overkill to change everything ??
        UpdateMoneyUI();
        UpdateHealthUI();
    }

    public void UpdateMoneyUI() {
        moneyText.text = playerData.Money.ToString();
    }

    public void UpdateHealthUI() {
        healthText.text = playerData.Health.ToString() + " / " + playerData.MaxHealth.ToString();

        doubleHealthBar.SetMaxValue(playerData.MaxHealth);
        doubleHealthBar.RemoveAmountWithEffect(playerData.Health);        
    }

    public void UpdateLevelUI(int xpGained) {
        int level = playerData.Level;
        if (level < 10) {
            int currentXp = playerData.Xp ; 

            Debug.Log(" !! Xp : " + currentXp);

            simpleXpBar.AddXp(xpGained, currentXp - xpGained, level); // Important : playerData.Xp already have + xpGained

            int xpRequired = PlayerData.xpRequired[level - 1]; // = playerData.xpRequired[level - 1];

            levelText.text = level.ToString();
            xpText.text = currentXp.ToString() + " / " + xpRequired.ToString();
        }
        else {
            xpText.text = ""; // could be optimized 
        }
    }
}
