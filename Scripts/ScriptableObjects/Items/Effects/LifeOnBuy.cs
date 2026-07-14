
using UnityEngine;

[CreateAssetMenu(fileName = "LifeOnBuy", menuName = "Scriptable Objects/Effects/LifeOnBuy")]
public class LifeOnBuy : EffectSO {

    public int value;

    public override void OnApply(ItemData itemData) { // is (ItemData itemData) useful ?
       // Player.Instance.PlayerData.TakeDamage(value); OLD
        GameEvents.PlayerHit(null, value, true); // New

    }

    public override void OnRemove(ItemData itemData) {     //Used ?

    }
}
