
using Bisic.CharacterStats;
using UnityEngine;

[CreateAssetMenu(fileName = "SoulEaterEffect", menuName = "Scriptable Objects/SoulEaterEffect")]
public class SoulEaterEffect : EffectSO {

    public bool isPermanent;
    public int duration;
    public int bonusPerKill = 10;
    public int maxStacks = 10;

    public TurretType turretType;

    public override void OnApply(ItemData itemData) {     
        GameEvents.OnEnemyKilled += HandleKill;
    }

    public override void OnRemove(ItemData itemData) { //Used ?
        GameEvents.OnEnemyKilled -= HandleKill;
    }

    private void HandleKill(Turret t,Enemy e) {
        if (t.turretType == turretType) { // NullReferenceException: Obje...
            t.AttackSpeed.AddModifier(new StatModifier(bonusPerKill, StatModType.Flat, this));
        }
    }
}
