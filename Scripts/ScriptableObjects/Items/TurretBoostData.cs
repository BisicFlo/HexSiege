using Bisic.CharacterStats;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Boost Data", menuName = "Scriptable Objects/Items/TurretBoostData")]

public class TurretBoostData : ItemData {

    [Header("Bonus")]
    public TurretType turretType; // only apply to selected type

    // Stat Bonuses (0 = no boost)
    public int AttackDamage;
    public int AttackSpeed;
    public int ProjectileSpeed; // ?
    public int CriticalChance;
    public int CriticalDamage;
    public int CurseChance;
    public int Range;

    public List<EffectSO> effects; // new // uniques effects

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

    public void Awake() {
        TypeOfItem = ItemType.Boost;
    }

    public void BoostAllCurrentTurret() {
        if (BuildManager.Instance == null )  return;

        foreach (Turret turret in BuildManager.Instance.WorldTurrets) {
            ApplyToTurret(turret);
        }
    }

    // Only Boost one Turret
    public void ApplyToTurret(Turret turret) {
        if (turret == null) return;
        // We verify the type :  Magical boost only applies to Magical turrets
        if (this.turretType != turret.turretType && this.turretType != TurretType.All ) return;
        ApplyBoosts(turret);           
    }

    public void RemovefromTurret(Turret turret) {
        if (turret == null) return;
        RemoveBoosts(turret);
    }

    private void ApplyBoosts(Turret turret) {
        BoostStat(turret.AttackDamage, AttackDamage);
        BoostStat(turret.AttackSpeed, AttackSpeed);
        BoostStat(turret.ProjectileSpeed, ProjectileSpeed);
        BoostStat(turret.CriticalChance, CriticalChance);
        BoostStat(turret.CriticalDamage, CriticalDamage);
        BoostStat(turret.CurseChance, CurseChance);
        BoostStat(turret.Range, Range);
    }
    private void RemoveBoosts(Turret turret) {
        UnboostStat(turret.AttackDamage, AttackDamage);
        UnboostStat(turret.AttackSpeed, AttackSpeed);
        UnboostStat(turret.ProjectileSpeed, ProjectileSpeed);
        UnboostStat(turret.CriticalChance, CriticalChance);
        UnboostStat(turret.CriticalDamage, CriticalDamage);
        UnboostStat(turret.CurseChance, CurseChance);
        UnboostStat(turret.Range, Range);
    }

    private void BoostStat(CharacterStat stat, int value) {
        if (value != 0)
            stat.AddModifier(new StatModifier(value, StatModType.Flat, this));
    }
    private void UnboostStat(CharacterStat stat, int value) {
        if (value != 0)
            stat.RemoveAllModifiersFromSource(this);
    }

    //private void BoostAllCurrentTurretAttackSpeed(int value) {
    //    foreach (Turret turret in BuildManager.Instance.WorldTurrets) {
    //        turret.AttackSpeed.AddModifier(new StatModifier(value, StatModType.Flat, this));
    //    }
    //}

    //public void UnboostAllCurrentTurretAttackSpeed() {
    //    foreach (Turret turret in BuildManager.Instance.WorldTurrets) {
    //        turret.AttackSpeed.RemoveAllModifiersFromSource(this);
    //    }
    //}


    //public void Equip(Turret t, int value) {
    //    // Create the modifiers and set the Source to "this"
    //    // Note that we don't need to store the modifiers in variables anymore
    //    t.AttackDamage.AddModifier(new StatModifier(value, StatModType.Flat, this));
    //}


    //public void Unequip(Turret c) {
    //    // Remove all modifiers applied by "this" Item
    //    c.AttackDamage.RemoveAllModifiersFromSource(this);
    //}
}
