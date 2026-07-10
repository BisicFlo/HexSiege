
using UnityEngine;

[CreateAssetMenu(fileName = "ChangeRerollCost", menuName = "Scriptable Objects/Effects/XpOnBuy")]
public class XpOnBuy : EffectSO {

    public int value;

    public override void OnApply(ItemData itemData) { // is (ItemData itemData) useful ?
        Player.Instance.PlayerData.GainXp(value);
    }

    public override void OnRemove(ItemData itemData) {     //Used ?

    }
}
