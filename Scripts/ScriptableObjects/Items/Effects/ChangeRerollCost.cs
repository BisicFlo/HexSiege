
using UnityEngine;

[CreateAssetMenu(fileName = "ChangeRerollCost", menuName = "Scriptable Objects/ChangeRerollCost")]
public class ChangeRerollCost : EffectSO {

    public int value;

    private int defaultValue;

    public override void OnApply(ItemData itemData) { // is (ItemData itemData) useful ?
        defaultValue = ShopManager.Instance.RefreshCost;
        ShopManager.Instance.RefreshCost = value;
    }

    public override void OnRemove(ItemData itemData) {     //Used ?
        ShopManager.Instance.RefreshCost = defaultValue;
    }
}
