using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject {

    public string Name;

    [Header("Stats")]
    public int StartingSpeed = 3;
    public int StartingLife = 4;
    public int DamageToPlayer = 4;
    public int MoneyWhenKilled = 1;
    public int XpWhenKilled = 1;

    [Header("Effects")]
    public GameObject DeathEffectPrefab = null;
    public Color DeathColor;



    [Header("Calculated Stats")]
    [SerializeField] private float threat;  // This will be non-editable in Inspector




    private void OnValidate() {
        //Debug.Log("onvalidate");

        threat = StartingLife * StartingSpeed + DamageToPlayer;


      //  EditorUtility.SetDirty(this);
    }

}
