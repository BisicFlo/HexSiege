
using UnityEngine;
using UnityEngine.UI;

public class DisplayInventory : MonoBehaviour {

    [SerializeField] private InventoryData inventory; // <- ScriptableObject

    [SerializeField] private Transform inventorySlotsFolder;


    private void OnEnable() {
        HideAllImages();
        Display();
    }

    private void HideAllImages() {
        for (int i = 0; i < inventorySlotsFolder.childCount; i++) {
            inventorySlotsFolder.GetChild(i).GetChild(0).GetComponent<Image>().enabled = false;

            inventorySlotsFolder.GetChild(i).GetChild(1).gameObject.SetActive(false);
        }
    }


    public void Display() {

        InventorySlot inventorySlot;
        ItemData itemData;
        int amount;

        int min = Mathf.Min(inventory.Container.Count, inventorySlotsFolder.childCount);


        for (int i = 0; i < min; i++) {

            inventorySlot = inventory.Container[i];

            if (inventorySlot.Item.TypeOfItem != ItemType.Turret) { // turrets are displayed as 3D

                itemData = inventorySlot.Item;

                amount = inventorySlot.Amount;

                // Change image UI  -> to be optimised
                inventorySlotsFolder.GetChild(i).GetChild(0).GetComponent<Image>().enabled = true;

                inventorySlotsFolder.GetChild(i).GetChild(0).GetComponent<Image>().sprite = itemData.Icon;

                // Change amount UI -> to be optimised
                if (amount > 1) {
                    inventorySlotsFolder.GetChild(i).GetChild(1).gameObject.SetActive(true);
                    inventorySlotsFolder.GetChild(i).GetChild(1).GetChild(0).GetComponent<Text>().text = "x" + amount;
                }
            }
        }
    }
}
