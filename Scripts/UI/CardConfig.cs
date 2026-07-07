using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Get a ItemData / TurretData / Turret , and display its properties ( Price, Stats , ... )

public class CardConfig : MonoBehaviour {

    #region --------- Inspector Fields ---------
    [SerializeField] private ColorData rarityColors; // New

    [Header("Images")]
    [SerializeField] private RectTransform FrontCard;
    [SerializeField] private RectTransform BackCard;

    [Header("Images")]
    [SerializeField] private Image MainImage;
    [SerializeField] private Image TypeImage;
    [SerializeField] private Image BackgroundColor;

    [Header("Texts")]
    [SerializeField] private Text ItemNameFront;
    [SerializeField] private Text ItemPriceFront;
    [SerializeField] private Text ItemNameBack;
    [SerializeField] private Text ItemPriceBack;

    [Header("Buttons")]
    [SerializeField] private Button FrontCardButton;
    [SerializeField] private Button BackCardButton;
    [SerializeField] private Button HelpButton; // Display Text instead of icons 
    public Button BuyButton; // Display Text instead of icons 


    [Header("Stats Sliders")]
    [SerializeField] private DoubleSliderBar damageSlider;
    [SerializeField] private DoubleSliderBar attackSpeedSlider;
    [SerializeField] private DoubleSliderBar projectileSpeedSlider; // unused ?
    [SerializeField] private DoubleSliderBar criticalChanceSlider;
    [SerializeField] private DoubleSliderBar criticalDamageSlider;
    [SerializeField] private DoubleSliderBar curseChanceSlider;
    [SerializeField] private DoubleSliderBar rangeSlider;

    //[Header("Turret Type Icons")]
    //[SerializeField] private GameObject medievalSprite;
    //[SerializeField] private GameObject magicalSprite;
    //[SerializeField] private GameObject plantSprite;
    //[SerializeField] private GameObject totemSprite;
    //[SerializeField] private GameObject divineSprite;
    //[SerializeField] private GameObject allSprite;

    private Dictionary<TurretType, GameObject> turretSpriteMap;

    [Header("Settings")]
    [SerializeField] private float flipDuration = 0.4f;
    [SerializeField] private AnimationCurve flipCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Sounds")]
    [SerializeField] private SoundData clickSound; // New



    //[Header("Debug")]
    //[SerializeField] private bool DisplayStats;
    //[SerializeField] private bool Animate;

    public ItemData itemDataSelected = null; // unused
    public TurretData turretDataSelected = null;  // hide in inspector
    public Turret turretSelected = null ;
    public TurretBoostData turretBoostSelected = null;


    #endregion

    private TurretStats turretBaseStats = new TurretStats();
    private TurretStats turretTotalStats = new TurretStats();

    private bool isFaceUp = true;
    private Coroutine currentFlip;

    // add max to stats : ex MaxDamage = 100

    // if Turret From Shop  / if inspector

    // Shop : clic on card : swap : details + "Buy" 



    //private void Update() { //temp
    //    if (DisplayStats) {
    //        DisplayStats = false;
    //        MainSetup();
    //    }
    //    if (Animate) {
    //        Animate = false;
    //        Flip();
    //    }
    //}

    private void OnEnable() {
        SetupButtonsEvents();
    }
    private void OnDisable() {
        RemoveButtonsEvents();
    }
    private void SetupButtonsEvents() {
        FrontCardButton.onClick.AddListener(() => Flip()); 
        //if (turretDataSelected!= null) FrontCardButton.onClick.AddListener(() => ChangeSlidersFromTurret(turretDataSelected));
        //else if (turretSelected != null) FrontCardButton.onClick.AddListener(() => ChangeSlidersFromTurret(turretSelected));

        BackCardButton.onClick.AddListener(() => Flip());
        //HelpButton
    }
    private void RemoveButtonsEvents() {
        FrontCardButton.onClick.RemoveAllListeners();
        BackCardButton.onClick.RemoveAllListeners();
        //HelpButton
    }
    //private void InitializeSpriteMap() {
    //    turretSpriteMap = new Dictionary<TurretType, GameObject> {
    //    { TurretType.Medieval, medievalSprite },
    //    { TurretType.Magical,  magicalSprite },
    //    { TurretType.Plant,    plantSprite },
    //    { TurretType.Totem,    totemSprite },
    //    { TurretType.Divine,   divineSprite },
    //    { TurretType.All,      allSprite }
    //    };
    //}


    public void MainSetup() {
        ResetCardOrientation(); // New

        if (turretDataSelected != null) { // used In Shop
            ChangeBackgroundColor(turretDataSelected.Rarity);
            ChangeSlidersFromTurret(turretDataSelected);
            ChangeTypeImage(turretDataSelected.turretType);
            ChangeText(turretDataSelected);
            ChangeMainImage(turretDataSelected);
            
        }
        else if (turretSelected != null) { // used In Inspector
            ChangeBackgroundColor(turretSelected.rarity);
            ChangeSlidersFromTurret(turretSelected);
            ChangeTypeImage(turretSelected.turretData.turretType);
            ChangeText(turretSelected.turretData);
            ChangeMainImage(turretSelected.turretData);

        }
        else if (turretBoostSelected != null) { // used In Shop with Items
            ChangeBackgroundColor(6); // 6 = items
            ChangeSlidersFromTurret(turretBoostSelected);
            ChangeTypeImage(turretBoostSelected.turretType);
            ChangeText(turretBoostSelected);
            ChangeMainImage(turretBoostSelected);
        }
    }

    private void ChangeSlidersFromTurret(Turret turret) {
        if (turret == null) return;      

        turretBaseStats = turret.GetBaseValues();
        turretTotalStats = turret.GetTotalValues();

        ChangeOneSlider(damageSlider, turretBaseStats.attackDamage, turretTotalStats.attackDamage, 40); // 100
        ChangeOneSlider(attackSpeedSlider, turretBaseStats.attackSpeed, turretTotalStats.attackSpeed, 200);

        ChangeOneSlider(criticalChanceSlider, turretBaseStats.criticalChance, turretTotalStats.criticalChance, 100);
        ChangeOneSlider(criticalDamageSlider, turretBaseStats.criticalDamage -100, turretTotalStats.criticalDamage-100, 300);

        ChangeOneSlider(curseChanceSlider, turretBaseStats.curseChance, turretTotalStats.curseChance, 100);
        ChangeOneSlider(rangeSlider, turretBaseStats.range, turretTotalStats.range, 100);

    }
    private void ChangeSlidersFromTurret(TurretData turretData) {
        if (turretData == null) return;

        turretBaseStats = turretData.GetValues();

        ChangeOneSlider(damageSlider, turretBaseStats.attackDamage, turretBaseStats.attackDamage, 40);
        ChangeOneSlider(attackSpeedSlider, turretBaseStats.attackSpeed, turretBaseStats.attackSpeed, 200);

        ChangeOneSlider(criticalChanceSlider, turretBaseStats.criticalChance, turretBaseStats.criticalChance, 100);
        ChangeOneSlider(criticalDamageSlider, turretBaseStats.criticalDamage - 100, turretBaseStats.criticalDamage - 100, 300);

        ChangeOneSlider(curseChanceSlider, turretBaseStats.curseChance, turretBaseStats.curseChance, 100);
        ChangeOneSlider(rangeSlider, turretBaseStats.range, turretBaseStats.range, 100);

    }
    private void ChangeSlidersFromTurret(TurretBoostData turretBoostData) {
        if (turretBoostData == null) return;

        turretBaseStats = turretBoostData.GetValues(); //if (turretBaseStats == null) Debug.Log("!! turretBaseStats null ");

        UpdateStatSliderForItem(damageSlider, turretBaseStats.attackDamage);
        UpdateStatSliderForItem(attackSpeedSlider, turretBaseStats.attackSpeed);
        UpdateStatSliderForItem(criticalChanceSlider, turretBaseStats.criticalChance);
        UpdateStatSliderForItem(criticalDamageSlider, turretBaseStats.criticalDamage-100); // default : 140% -> 40% just for display
        UpdateStatSliderForItem(curseChanceSlider, turretBaseStats.curseChance);
        UpdateStatSliderForItem(rangeSlider, turretBaseStats.range);
    }

    private void UpdateStatSliderForItem(DoubleSliderBar doubleSlider, int value) {
        if (value > 0)
            ChangeOneSlider(doubleSlider, 0, value, 50);
        else
            ChangeOneSlider(doubleSlider, value, 0, 50);
    }

    private void ChangeOneSlider(DoubleSliderBar doubleSlider, int baseValue, int total , int maxValue) {
        if (doubleSlider == null) Debug.Log("!! silder null ");

        doubleSlider.SetMaxValue(maxValue); // Should be improve

        doubleSlider.SetValue(baseValue, total);
    }

    private void ChangeBackgroundColor(int rarity ) {
        if (rarity < 1 || rarity > 6) return; // 6 : items
        BackgroundColor.color = rarityColors.ColorList[rarity - 1]; // New  
    }

    private void ChangeTypeImage(TurretType turretType) {

        TurretTypeManager.Instance.SetCorrespondingSprite(TypeImage, turretType);
    }

    private void ChangeText(ItemData itemData) {
        if (itemData == null) {  Debug.Log("turretData is null in ChangeText() ")  ; return; }
        if (ItemPriceFront != null) ItemPriceFront.text = itemData.Price.ToString();
        if (ItemPriceBack != null) ItemPriceBack.text = itemData.Price.ToString();

        ItemNameFront.text = itemData.NameItem.ToString();
        ItemNameBack.text = itemData.NameItem.ToString();
    }

    private void ChangeMainImage(ItemData itemData) {
        MainImage.sprite = itemData.Icon;
    }
    private void ChangeMainImage(Turret turret) {
        MainImage.sprite = turret.turretData.Icon;
    }

    public void Flip() {
        //Debug.Log("Flip");

        if (currentFlip != null)
            StopCoroutine(currentFlip);

        currentFlip = StartCoroutine(FlipRoutine());

        SoundManager.Instance.PlaySFX(clickSound);
    }

    public void ResetCardOrientation() {
        isFaceUp = true;
        FrontCard.gameObject.SetActive(true);
        BackCard.gameObject.SetActive(false);
        FrontCard.localScale = Vector3.one;
    }
    private IEnumerator FlipRoutine() {
        Transform front ;
        Transform back ;

        if (isFaceUp) {
            front = FrontCard;
            back = BackCard;
        }
        else {
            back = FrontCard;
            front = BackCard;
        }        

        isFaceUp = !isFaceUp;
        float halfDuration = flipDuration / 2f;
        //RectTransform rt = GetComponent<RectTransform>();

        // 0) Scale down the back picture
        back.localScale = new Vector3(0f, back.localScale.y, back.localScale.z);

        // 1) Scale down to "edge"
        float elapsed = 0f;
        Vector3 startScale = front.localScale;
        Vector3 targetScale = new Vector3(0, startScale.y, startScale.z);

        while (elapsed < halfDuration) {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            t = flipCurve.Evaluate(t);

            front.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        // 2) Switch the image (at the "edge")
        if (FrontCard) FrontCard.gameObject.SetActive(isFaceUp); //  = isFaceUp;
        if (BackCard) BackCard.gameObject.SetActive(!isFaceUp);

        // Optional: swap sprite on single image instead
        // frontImage.sprite = isFaceUp ? frontSprite : backSprite;

        // 3) Scale back up
        elapsed = 0f;
        startScale = back.localScale;
        targetScale = new Vector3(1f, startScale.y, startScale.z); // or your original Y scale

        while (elapsed < halfDuration) {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            t = flipCurve.Evaluate(t);

            back.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        back.localScale = new Vector3(1f, back.localScale.y, back.localScale.z); // ensure clean value
        currentFlip = null;
    }
}
