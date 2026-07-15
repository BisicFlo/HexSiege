
using UnityEngine;

[CreateAssetMenu(fileName = "GreedEffect", menuName = "Scriptable Objects/Effects/GreedEffect")]
public class GreedEffect : EffectSO {

    //[SerializeField] private PlayerData PlayerData;

    [SerializeField] private bool canBeFatal;
    [SerializeField] private int lifeCost;

    public override void OnApply(ItemData itemData) {
        GameEvents.OnShopRerolled += HandleEvent;

        GameEvents.OnDefeat += RemoveAtTheEnd; // NEW
        GameEvents.OnVictory += RemoveAtTheEnd; // NEW
    }

    public override void OnRemove(ItemData itemData) { //Used ?
        GameEvents.OnShopRerolled -= HandleEvent;

        GameEvents.OnDefeat -= RemoveAtTheEnd; // NEW
        GameEvents.OnVictory -= RemoveAtTheEnd; // NEW
    }

    private void HandleEvent() {
        //PlayerData.TakeDamage(LifeCost);
        GameEvents.PlayerHit( e: null, lifeCost, canBeFatal); // enemy:null
    }

    private void RemoveAtTheEnd() {  // NEW
        GameEvents.OnShopRerolled -= HandleEvent;
        GameEvents.OnDefeat -= HandleEvent; 
        GameEvents.OnVictory -= HandleEvent; 
    }
}
