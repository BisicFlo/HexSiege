using UnityEngine;
using UnityEngine.UI;

"Dont comp ty"

public class CardConfig : MonoBehaviour {

    [SerializeField] private Text ItemName;

    [SerializeField] private Text ItemPrice;


    [SerializeField] private DoubleSliderBar damageSlider;
    [SerializeField] private DoubleSliderBar attackSpeedSlider;
    [SerializeField] private DoubleSliderBar projectileSpeedSlider;
    [SerializeField] private DoubleSliderBar criticalChanceSlider;
    [SerializeField] private DoubleSliderBar criticalDamageSlider;
    [SerializeField] private DoubleSliderBar curseChanceSlider;
    [SerializeField] private DoubleSliderBar rangeSlider;


    // if Turret From Shop  / if inspector


    private TurretStats turretBaseStats = new TurretStats();
    private TurretStats turretTotalStats = new TurretStats();


    private void ChangeEveryValue(Turret turret) {
        if (turret == null) return;        


        turretBaseStats = turret.GetBaseValues();
        turretTotalStats = turret.GetTotalValues();




    }

}
