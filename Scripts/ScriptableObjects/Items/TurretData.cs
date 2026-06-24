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

    //public float RotationSpeed; // ?

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
