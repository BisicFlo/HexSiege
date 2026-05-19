using UnityEngine;
public enum TurretType {
    None,
    Medieval, // Canon / Ballista
    Magical, // Eye // Cristal // Stellated octahedron
    Plant,
    Totem,
    Divine,
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


}
