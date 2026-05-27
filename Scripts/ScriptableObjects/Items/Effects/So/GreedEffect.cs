
using UnityEngine;

[CreateAssetMenu(fileName = "GreedEffect", menuName = "Scriptable Objects/Effects/GreedEffect")]
public class GreedEffect : EffectSO {

    //[SerializeField] private PlayerData PlayerData;

    [SerializeField] private bool canBeFatal;
    [SerializeField] private int lifeCost;

    public override void OnApply(ItemData itemData) {
        GameEvents.OnShopRerolled += HandleEvent;
    }

    public override void OnRemove(ItemData itemData) { //Used ?
        GameEvents.OnShopRerolled -= HandleEvent;
    }

    private void HandleEvent() {
        //PlayerData.TakeDamage(LifeCost);
        GameEvents.PlayerHit(null, lifeCost, canBeFatal); // enemy:null
    }
}
