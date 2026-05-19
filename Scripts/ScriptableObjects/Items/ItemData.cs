using UnityEngine;

public enum ItemType {
    Turret,
    Boost
}

public abstract class ItemData : ScriptableObject {


    [Header("Image")]

    public Sprite Icon;


    [Header("Item")]

    public int ID;
    public GameObject UIModel; // Prefab    
    public GameObject WorldModel; // Prefab Used to Instanciate In World 

    public string NameItem;
    public int Price;
    public int Rarity;

    public ItemType TypeOfItem;
    public string Description; // ?

}
