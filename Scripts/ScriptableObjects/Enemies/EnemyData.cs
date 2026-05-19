using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject {

    [Header("Stats")]
    public int startingSpeed = 3;
    public int startingLife = 4;
    public int damageToPlayer = 4;
    public int moneyWhenKilled = 1;
    public int xpWhenKilled = 1;

    [Header("Effects")]
    public GameObject DeathEffectPrefab = null;
    public Color deathColor;

}
