using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New InventoryData", menuName = "Scriptable Objects/Inventory")]
public class InventoryData : ScriptableObject {

    public List<InventorySlot> Container = new List<InventorySlot>(); // For Items owned

    public List<InventorySlot> TurretContainer = new List<InventorySlot>(); // For Turrets owned not built

    public void AddItem(ItemData item, int amount) {
        bool IsInInventory = false;

        for (int i = 0; i < Container.Count; i++) {
            if (Container[i].Item == item) {
                IsInInventory = true;
                Container[i].AddAmount(amount);
                break;
            }
        }

        if (!IsInInventory) {
            Container.Add(new InventorySlot(item, amount));
        }
    }

    public void RemoveItem(ItemData item, int amount) {

        InventorySlot slot = null;

        for (int i = 0; i < Container.Count; i++) {  // Change direction i for optimisation

            slot = Container[i];
            if (slot.Item == item) {
                if (slot.Amount <= amount) {
                    Container.Remove(slot);
                }
                else {
                    slot.AddAmount(amount * -1);
                }
                break;
            }
        }
    }

    public void AddTurret(ItemData item, int amount) {
        bool IsInInventory = false;

        for (int i = 0; i < TurretContainer.Count; i++) {
            if (TurretContainer[i].Item == item) {
                IsInInventory = true;
                TurretContainer[i].AddAmount(amount);
                break;
            }
        }

        if (!IsInInventory) {
            TurretContainer.Add(new InventorySlot(item, amount));
        }
    }

    public void RemoveTurret(ItemData item, int amount) {        

        InventorySlot slot = null;

        for (int i = 0; i < TurretContainer.Count; i++) {

            slot = TurretContainer[i];
            if (slot.Item == item) {
                if (slot.Amount <= amount) {
                    TurretContainer.Remove(slot);
                } else {
                    slot.AddAmount(amount * -1);
                }
                break;
            }
        }
    }

    public void ApplyAllBoostToTurret(Turret turret) {
        InventorySlot inventorySlot;
        TurretBoostData turretBoostData;
        int amount;

        for (int i = 0; i < this.Container.Count; i++) {

            inventorySlot = this.Container[i];

            if (inventorySlot.Item.TypeOfItem == ItemType.Boost) {


                turretBoostData = inventorySlot.Item as TurretBoostData;

                amount = inventorySlot.Amount;
                Debug.Log("Amount : " + amount);

                for (int j = 0; j < amount; j++) {

                    turretBoostData.ApplyToTurret(turret);
                }
            }
        }
    }
}



[System.Serializable]
public class InventorySlot {

    public ItemData Item;
    public int Amount;

    public InventorySlot(ItemData item, int amount) {
        Item = item;
        Amount = amount;
    }

    public void AddAmount ( int value) {
        Amount += value;
    }
}