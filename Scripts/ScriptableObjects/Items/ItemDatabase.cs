using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Scriptable Objects/ItemDatabase")]
public class ItemDatabase : ScriptableObject {

    [SerializeField] private List<TurretData> commonTurretList = new List<TurretData>();
    [SerializeField] private List<TurretData> uncommonTurretList = new List<TurretData>();
    [SerializeField] private List<TurretData> rareTurretList = new List<TurretData>();
    [SerializeField] private List<TurretData> epicTurretList = new List<TurretData>();
    [SerializeField] private List<TurretData> legendaryTurretList = new List<TurretData>();

    [SerializeField] private List<ItemData> itemList = new List<ItemData>();

    public TurretData GetRandomTurretFromRarity(int rarity) {
        List<TurretData> selectedList = GetListByRarity(rarity);

        if (selectedList == null || selectedList.Count == 0) {
            Debug.LogWarning($"No turrets available for rarity {rarity} or invalid rarity value");
            return null;
        }

        int randomIndex = Random.Range(0, selectedList.Count);
        return selectedList[randomIndex];
    }

    public ItemData GetRandomItem() {
        int randomIndex = Random.Range(0, itemList.Count);     // New
        return itemList[randomIndex];
    }

    private List<TurretData> GetListByRarity(int rarity) {
        switch (rarity) {
            case 1: return commonTurretList;
            case 2: return uncommonTurretList;
            case 3: return rareTurretList;
            case 4: return epicTurretList;
            case 5: return legendaryTurretList;

            default:
                Debug.LogWarning($"Invalid rarity value: {rarity}. Expected 1–5.");
                return null;
        }
    }

    #region Unused
    public TurretData GetTurretFromId(int id) {
        // Efficient linear search across all lists (fine for small lists < 100 items)
        List<TurretData>[] allTurretLists = {
        commonTurretList,
        uncommonTurretList,
        rareTurretList,
        epicTurretList,
        legendaryTurretList
        };

        foreach (var turretList in allTurretLists) {
            foreach (var turret in turretList) {
                if (turret.ID == id) {
                    return turret;
                }
            }
        }

        Debug.LogWarning($"Turret with ID {id} not found in any list!");
        return null;
    }

    public ItemData GetItemFromId(int id) {
        foreach (var item in itemList) {
            if (item.ID == id) {
                return item;
            }
        }
        return null;
    }
    #endregion


}


