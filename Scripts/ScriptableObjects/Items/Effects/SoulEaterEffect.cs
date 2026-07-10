
using Bisic.CharacterStats;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Only one Turret can have SoulEater effect , For the moment it's permanent 

[CreateAssetMenu(fileName = "SoulEaterEffect", menuName = "Scriptable Objects/Effects/SoulEaterEffect")]
public class SoulEaterEffect : EffectSO {

    public bool isPermanent;
    public int duration = 20;
    public int bonusPerKill = 10;
    public int maxStacks = 10;

    public TurretType turretType;

    private Dictionary<Turret, int> currentStacks = new Dictionary<Turret, int>();

    public override void OnApply(ItemData itemData) {     
        GameEvents.OnEnemyKilled += HandleKill;
    }

    public override void OnRemove(ItemData itemData) { //Used ?
        GameEvents.OnEnemyKilled -= HandleKill;
    }

    private void HandleKill(Turret t,Enemy e) {
        if (t == null) return;
        if (t.turretType == turretType) { // NullReferenceException: Obje...
            t.AttackSpeed.AddModifier(new StatModifier(bonusPerKill, StatModType.Flat, this));
         
        }
    }

}
