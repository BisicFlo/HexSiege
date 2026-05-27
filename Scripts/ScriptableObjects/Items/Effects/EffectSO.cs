using System;
using UnityEngine;

[Serializable]
public abstract class EffectSO : ScriptableObject {

    public string effectName;  
    public abstract void OnApply(ItemData itemData);   // when item is equipped
    public abstract void OnRemove(ItemData itemData );  // when item is unequipped
}
