using Bisic.CharacterStats;
using System.Collections;
using UnityEngine;

public struct TurretStats {
    public int attackDamage;
    public int attackSpeed;
    public int projectileSpeed;
    public int criticalChance;
    public int criticalDamage;
    public int curseChance;
    public int range;
}


public class Turret : MonoBehaviour {

    [HideInInspector] public Enemy enemyTargetted;
    [HideInInspector] public int rarity; // New

    // --------------------------------------------------------------
    //   Inspector Fields
    // -------------------------------------------------------------- 

    public TurretData turretData;

    public TurretType turretType;

    //[Header("New Stats System")]
    public CharacterStat AttackDamage;
    public CharacterStat AttackSpeed;  // 100 : normal 
    public CharacterStat ProjectileSpeed;
    public CharacterStat CriticalChance; // 0-100 
    public CharacterStat CriticalDamage; // 0-200     | ex :  200% / 90% / 140%
    public CharacterStat CurseChance;    // 0-100 
    public CharacterStat Range; // ~20

    public float timeBetweenUpdateTarget = 0.4f;

    [Header("Animation")]
    [SerializeField] private AnimateTurret animateTurret;

    [Header("Sound")]
    [SerializeField] protected SoundData shootSound; // New

    // --------------------------------------------------------------
    //   Private 
    // --------------------------------------------------------------

    private WaveSpawner waveSpawner;
    protected Transform target;
    private WaitForSeconds waitBetweenUpdateTarget;
    private float fireCountdown = 0;

    // --------------------------------------------------------------
    //   MonoBehaviour
    // --------------------------------------------------------------

    public TurretStats GetBaseValues() {

        return new TurretStats {
            attackDamage = AttackDamage.BaseValue,
            attackSpeed = AttackSpeed.BaseValue,
            projectileSpeed = ProjectileSpeed.BaseValue,
            criticalChance = CriticalChance.BaseValue,
            criticalDamage = CriticalDamage.BaseValue,
            curseChance = CurseChance.BaseValue,
            range = Range.BaseValue,
        };
    }
    public TurretStats GetTotalValues() {

        return new TurretStats {
            attackDamage = AttackDamage.Value,
            attackSpeed = AttackSpeed.Value,
            projectileSpeed = ProjectileSpeed.Value,
            criticalChance = CriticalChance.Value,
            criticalDamage = CriticalDamage.Value,
            curseChance = CurseChance.Value,
            range = Range.Value,
        };
    }

    protected void Start() {  // Don't Call Start() in derived classes so : "protected"
        Init();
        InitValues();
    }

    protected virtual void Init() {
        waveSpawner = WaveSpawner.Instance;
        waitBetweenUpdateTarget = new(timeBetweenUpdateTarget);
        StartCoroutine(UpdateTarget());
    }

    protected virtual void InitValues() {
        Debug.Log( " Init from " + this.name );
        AttackDamage = new CharacterStat(turretData.AttackDamage);
        AttackSpeed = new CharacterStat(turretData.AttackSpeed);
        ProjectileSpeed = new CharacterStat(turretData.ProjectileSpeed);
        CriticalChance = new CharacterStat(turretData.CriticalChance);
        CriticalDamage = new CharacterStat(turretData.CriticalDamage);
        CurseChance = new CharacterStat(turretData.CurseChance);
        Range = new CharacterStat(turretData.Range);

        rarity = turretData.Rarity;
        turretType = turretData.turretType; // New
    }

    protected bool IsCritical() {
        return Random.value < CriticalChance.Value / 100f;
    }

    protected bool IsCursed() {        
        return Random.value < CurseChance.Value / 100f;
    }

    protected void GetTarget() {
        
        float shortestDistance = 10000;
        GameObject nearestEnemy = null;
        
        if (target != null && target.gameObject.activeInHierarchy) {
            if ((this.transform.position - target.transform.position).sqrMagnitude < Range.Value)
                return; // -> no need to update 
        }

        foreach (Enemy enemy in waveSpawner.EnemiesList) {

            Transform visual = GetVisualFromEnemy(enemy.transform);
            if (visual == null) visual = enemy.transform;

            float distanceToEnemy = (this.transform.position - visual.position).sqrMagnitude;
            if (distanceToEnemy < Range.Value && distanceToEnemy < shortestDistance) {
                shortestDistance = distanceToEnemy;
                nearestEnemy = visual.gameObject;
                enemyTargetted = enemy; //we save a reference to the Enemy class so we dont use Getcomponent
            }
        }
        if (nearestEnemy != null) {
            target = nearestEnemy.transform;
        } else {
            target = null;
        }
    }

    protected bool CheckIfReadyToFire() {
        if (fireCountdown <= 0) {
            fireCountdown = 100f / AttackSpeed.Value; // (CharacterStat)
            //Debug.Log("fireCountdown : " + fireCountdown + " | AttackSpeed.Value : " + AttackSpeed.Value);
            return true;
        }
        fireCountdown -= Time.deltaTime;
        return false;
    }

    protected void Attack() {
        if (animateTurret != null) {
            StartCoroutine(animateTurret.Jiggle(Shoot, AttackSpeed.Value)); // Shoot Called after animation 
        }
        else {
            Shoot(); // no animation
        }
    }

    protected virtual void Shoot() {

    }

    protected Transform GetVisualFromEnemy(Transform parent) { // used with slimes
        if (parent == null) return null;

        // Check direct children first
        for (int i = 0; i < parent.childCount; i++) {
            Transform child = parent.GetChild(i);

            if (child.name == "Visual") {
                return child;
            }

            // Recurse into this child
            Transform found = GetVisualFromEnemy(child);
            if (found != null) {
                return found;
            }
        }
        return null;
    }

    protected void InstantiateAlternative(GameObject gameObject, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent) {
        gameObject.SetActive(true);
        gameObject.transform.SetPositionAndRotation(position, rotation);
        if (parent != null) gameObject.transform.SetParent(parent, true);
        gameObject.transform.localScale = scale;
    }

    protected void DestroyAlternative(GameObject gameObject) {
        gameObject.SetActive(false);
    }

    protected T[] CreateStockOf<T>(GameObject prefab, int count) where T : Object {
        T[] instances = new T[count];

        for (int i = 0; i < count; i++) {
            GameObject go = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            go.SetActive(false);

            instances[i] = typeof(T) == typeof(GameObject)
                ? (T)(object)go // Double Cast Trick
                : go.GetComponent<T>();
        }
        return instances;
    }

    protected T GetObjectFromIndex<T>(T[] array, int index) where T : Object {
        return array[index];
    }

    protected void DestroyStockOf<T>(T[] instances) where T : Object {
        for (int i = 0; i < instances.Length; i++) {
            if (instances[i] == null) continue;

            GameObject go = instances[i] is GameObject gameObj
                ? gameObj
                : (instances[i] as Component)?.gameObject;

            if (go != null) Destroy(go);
            instances[i] = null;
        }
    }

    protected IEnumerator UpdateTarget() {
        while (true) {
            yield return waitBetweenUpdateTarget;
            GetTarget();
        }
    }
}
