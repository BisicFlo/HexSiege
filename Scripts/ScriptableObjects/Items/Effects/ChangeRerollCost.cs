
using UnityEngine;

[CreateAssetMenu(fileName = "ChangeRerollCost", menuName = "Scriptable Objects/Effects/ChangeRerollCost")]
public class ChangeRerollCost : EffectSO {

    public int value;

    private int defaultValue;

    public override void OnApply(ItemData itemData) { // is (ItemData itemData) useful ?
        defaultValue = ShopManagerV2.Instance.RerollCost;
        ShopManagerV2.Instance.RerollCost = value;
    }

    public override void OnRemove(ItemData itemData) {     //Used ?
        ShopManagerV2.Instance.RerollCost = defaultValue;
    }
}
