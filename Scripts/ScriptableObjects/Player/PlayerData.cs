using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject {

    public int Health;
    public int MaxHealth;
    public int Money;
    public int Level;
    public int Xp;

    public readonly int[] xpRequired = { 2, 2, 6, 10, 20, 36, 60, 68, 80 };

    public void GainXp(int xpGained) {
        if (Level == 10) return; //Niveau Max

        Xp += xpGained;
        if (Xp >= xpRequired[Level - 1]) {
            Xp = 0;
            Level++;
        }
  
    }
    public void GainMoney(int money) {
        Money += money;
    }



}
