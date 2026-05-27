using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject {

    public event Action OnStatChanged;

    [SerializeField] private int StartingHealth;
    [SerializeField] private int StartingMaxHealth;
    [SerializeField] private int StartingMoney;
    [SerializeField] private int StartingXp;
    [SerializeField][Range(1, 10)] private int StartingLevel = 1;

    public int Health { get; private set; }
    public int MaxHealth { get; private set; }
    public int Money { get; private set; }
    public int Xp { get; private set; }
    public int Level { get; private set; } = 1;

    public readonly int[] xpRequired = { 2, 2, 6, 10, 20, 36, 60, 68, 80 };

    private void OnEnable() {
        Debug.Log("PlayerData OnEnable called");
        Health = StartingHealth;
        MaxHealth = StartingMaxHealth;
        Money = StartingMoney;
        Xp = StartingXp;
        Level = StartingLevel;
        OnStatChanged?.Invoke();
    }

    public void GainXp(int xpGained) {
        if (Level == 10) return; //Niveau Max

        Xp += xpGained;
        if (Xp >= xpRequired[Level - 1]) {
            Xp = 0;
            Level++;
        }
        OnStatChanged?.Invoke();
    }
    public void GainMoney(int money) {
        Money += money;
        OnStatChanged?.Invoke();
    }
    public void LoseMoney(int money) {
        Money -= money;
        OnStatChanged?.Invoke();
    }
    public void TakeDamage(int amount) {
        Health -= amount;

        if (Health > MaxHealth) {
            Health = MaxHealth; // Used for heals with (-amount )
        }
        OnStatChanged?.Invoke();
    }
    public void GainMaxHealth(int amount) {
        MaxHealth += amount;
        Health += amount;

        if (Health <= 0) {
            //defeat
            UIManager.Instance.ShowScreen(ScreenType.GameOver);
        }
        OnStatChanged?.Invoke();
    }
    public void SetPlayerLife(int value) {
        Health = value;
        OnStatChanged?.Invoke();
    }
}
