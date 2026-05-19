using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InspectTurretManager : MonoBehaviour {
    public static InspectTurretManager Instance { get; private set; } // Singleton needed ? 

    [SerializeField] private ColorData rarityColors;

    // Physical / magical ?

    [SerializeField] private Button quitButton;
    [SerializeField] private Button destroyButton;

    [SerializeField] private Image turretIcon;
    [SerializeField] private Image turretTypeIcon;

    [SerializeField] private Text damageText;
    [SerializeField] private Text attackSpeedText;
    [SerializeField] private Text projectileSpeedText;
    [SerializeField] private Text criticalChanceText;
    [SerializeField] private Text criticalDamageText;
    [SerializeField] private Text curseChanceText;
    [SerializeField] private Text rangeText;

    private int baseDamage;
    private int baseAttackSpeed; 
    private int baseProjectileSpeed;
    private int baseRange;
    private int baseCriticalChance;
    private int baseCriticalDamage;
    private int baseCurseChance;

    private int damage;
    private int attackSpeed; 
    private int projectileSpeed;
    private int range;
    private int criticalChance;
    private int criticalDamage;
    private int curseChance;

    private int rarity;

    public Turret selectedTurret;

    private readonly WaitForSeconds waitBetweenDisplay = new WaitForSeconds(0.1f);

    private void Awake() {
        if (Instance != null) Debug.LogWarning("More than one InspectManager detected");
        Instance = this;
    }


    private void OnEnable() {

        if (selectedTurret!= null) Debug.Log("selectedTurret : " + selectedTurret.name);

        SetupButtonsEvents();

        //GetValuesFromTurret(selectedTurret);
        //UpdateTextUI();
        //ChangeColorImageFromRarity(turretIcon, rarity);
    }
    private void OnDisable() {
        ClearButtonsEvents();
    }

    public void DisplayTurret() {
        GetValuesFromTurretV2(selectedTurret);
        UpdateTextUI();
        ChangeTurretTypeIcon(selectedTurret); // New
        ChangeColorImageFromRarity(turretIcon, rarity);
    }

    private void SetupButtonsEvents() {   
        quitButton.onClick.AddListener(() => QuitInspectMenu());
        destroyButton.onClick.AddListener(() => DestroyTurretAndQuit());
    }

    private void ClearButtonsEvents() {
        quitButton.onClick.RemoveAllListeners();
        destroyButton.onClick.RemoveAllListeners();
    }

    //private void GetValuesFromTurret(Turret turret) {
    //    if (turret == null) {
    //        Debug.LogWarning("GetValuesFromTurret: Turret is null!");
    //        return;
    //    }

    //    this.damage = turret.attackDamage;
    //    this.attackSpeed = (int)turret.attackSpeed;
    //    this.projectileSpeed = turret.projectileSpeed;
    //    this.range = turret.range;
    //    this.criticalChance = turret.criticalChance;
    //    this.curseChance = turret.curseChance;

    //    this.rarity = turret.rarity;
    //}

    private void GetValuesFromTurretV2(Turret turret) {
        if (turret == null) {
            Debug.LogWarning("GetValuesFromTurret: Turret is null!");
            return;
        }

        // Attack Damage
        this.baseDamage = turret.AttackDamage.BaseValue;
        this.damage = turret.AttackDamage.Value;

        // Attack Speed
        this.baseAttackSpeed = turret.AttackSpeed.BaseValue;
        this.attackSpeed = turret.AttackSpeed.Value;

        // Projectile Speed
        this.baseProjectileSpeed = turret.ProjectileSpeed.BaseValue;
        this.projectileSpeed = turret.ProjectileSpeed.Value;

        // Critical Chance
        this.baseCriticalChance = turret.CriticalChance.BaseValue;
        this.criticalChance = turret.CriticalChance.Value;

        // Critical Damage
        this.baseCriticalDamage = turret.CriticalDamage.BaseValue;
        this.criticalDamage = turret.CriticalDamage.Value;

        // Curse Chance
        this.baseCurseChance = turret.CurseChance.BaseValue;
        this.curseChance = turret.CurseChance.Value;

        // Range
        this.baseRange = turret.Range.BaseValue;
        this.range = turret.Range.Value;

    }

    private void ChangeOneText(int baseValue , int value, Text text ) {
        // Damage
        int Bonus = value - baseValue;
        if (Bonus == 0) {
            text.text = baseValue.ToString();
        }
        else if (Bonus > 0) {
            text.text = $"{baseValue}<color=#9DED1A> (+{Bonus})</color>"; // light green
        }
        else {
            text.text = $"{baseValue}<color=red> ({Bonus})</color>";
        }
    }

    private void UpdateTextUI() {   // 5 (+3)   | 20 (-10)

        ChangeOneText(baseDamage, damage, damageText);
        ChangeOneText(baseDamage, damage, damageText);
        ChangeOneText(baseAttackSpeed, attackSpeed, attackSpeedText);
        ChangeOneText(baseProjectileSpeed, projectileSpeed, projectileSpeedText);
        ChangeOneText(baseRange, range, rangeText);
        ChangeOneText(baseCriticalChance, criticalChance, criticalChanceText);
        ChangeOneText(baseCriticalDamage, criticalDamage, criticalDamageText);
        ChangeOneText(baseCurseChance, curseChance, curseChanceText);
    }

    private void ChangeColorImageFromRarity(Image image, int rarity) {
        if (rarity < 1 || rarity > 5) return;
        image.color = rarityColors.ColorList[rarity-1];
    }
    private void ChangeTurretTypeIcon(Turret turret) {
        TurretTypeManager.Instance.SetCorrespondingSprite(turretTypeIcon, turret.turretType);
    }

    private void QuitInspectMenu() {
        UIManager.Instance.ShowScreen(ScreenType.HUD);
    }

    private void DestroyTurretAndQuit() {
        Destroy(selectedTurret.gameObject);
        // Remove from somewhere ? List ?
        QuitInspectMenu();
    }

    private IEnumerator ShowTextNeeded() {
        yield return waitBetweenDisplay;  
        damageText.transform.parent.gameObject.SetActive(damage > 0);

        yield return waitBetweenDisplay;
        attackSpeedText.transform.parent.gameObject.SetActive(attackSpeed > 0);

        yield return waitBetweenDisplay;
        projectileSpeedText.transform.parent.gameObject.SetActive(projectileSpeed > 0);

        yield return waitBetweenDisplay;
        rangeText.transform.parent.gameObject.SetActive(range > 0);

        yield return waitBetweenDisplay;
        criticalChanceText.transform.parent.gameObject.SetActive(criticalChance > 0);

        yield return waitBetweenDisplay;
        curseChanceText.transform.parent.gameObject.SetActive(curseChance > 0);
    }

    private IEnumerator HideAllStatText() {
        damageText.transform.parent.gameObject.SetActive(false);
        yield return null;
        attackSpeedText.transform.parent.gameObject.SetActive(false);
        yield return null;
        projectileSpeedText.transform.parent.gameObject.SetActive(false);
        yield return null;
        rangeText.transform.parent.gameObject.SetActive(false);
        yield return null;
        criticalChanceText.transform.parent.gameObject.SetActive(false);
        yield return null;
        curseChanceText.transform.parent.gameObject.SetActive(false);
    }

}
