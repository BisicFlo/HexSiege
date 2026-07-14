using UnityEditor;
using UnityEngine;
public enum TurretType {
    None,
    Medieval, // Canon / Ballista
    Magical, // Eye // Cristal // Stellated octahedron
    Plant,
    Totem,
    Divine,
    Obscure,
    All,
}


[CreateAssetMenu(fileName = "New Turret Data", menuName = "Scriptable Objects/Items/TurretData")]
public class TurretData : ItemData {

    public void Awake() {
        TypeOfItem = ItemType.Turret;
    }

    // Attributes

    public TurretType turretType;

    public int AttackDamage;
    public int AttackSpeed;
    public int ProjectileSpeed;
    public int CriticalChance; 
    public int CriticalDamage;
    public int CurseChance;
    public int Range;

    [Header("Calculated Stats")]
    [SerializeField] private float dps;  // This will be non-editable in Inspector
    [SerializeField] private float curseTime;  // This will be non-editable in Inspector
    //[SerializeField] private float curseTime90percent;  // This will be non-editable in Inspector



    private void OnValidate() {
        //Debug.Log("onvalidate");

        // DPS = [((1-crit_chance) * regular_dmg) + (crit_chance * crit_dmg)] * attack_speed.
        float avHit = ((100 - CriticalChance) * AttackDamage) + (AttackDamage * CriticalChance * (CriticalDamage/100f)  );

        dps =  (avHit * (AttackSpeed / 100f) ) /100f ;

        if (AttackSpeed == 0 || CurseChance == 0) {
            curseTime = 0;
            //curseTime90percent = 0;
        }
        else {
            curseTime = 4f / ((AttackSpeed / 100f) * (CurseChance / 100f));// (CurseChance/100f) * AttackSpeed;
            //curseTime90percent = -Mathf.Log(0.1f) * 4f / (AttackSpeed / 100f) * (CurseChance / 100f);
        }
       // EditorUtility.SetDirty(this);
    }

    public TurretStats GetValues() {
        return new TurretStats {
            attackDamage = AttackDamage,
            attackSpeed = AttackSpeed,
            projectileSpeed = ProjectileSpeed,
            criticalChance = CriticalChance,
            criticalDamage = CriticalDamage,
            curseChance = CurseChance,
            range = Range,
        };
    }



}
