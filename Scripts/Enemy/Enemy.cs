
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public int currentSpeed; // used in USlime

    [SerializeField] private PlayerData playerData;   

    [SerializeField] private EnemyData enemyData;

    [SerializeField] private Transform Visual; // Part holding the renderer    

    [SerializeField] private Curse curseSystem; 

    [SerializeField] private SoundData hitSound; // New
    [SerializeField] private SoundData deathSound; // New


    #region SetByEnemyData
    private GameObject DeathEffectPrefab = null;
    private Color deathColor;

    private int damageToPlayer;
    private int moneyWhenKilled;
    private int xpWhenKilled;
    #endregion

    #region SetByWaveSpawner 
    private WaveSpawner waveSpawner = null; // set by WaveSpawner itself // [HideInInspector] public
    private Waypoints selectedWaypoints; // set by WaveSpawner itself
    #endregion

    private EnemyHitFlash enemyHitFlash; // For visual Feedback // Setup automatically by GetComponent

    private bool isInvinsible = false; // not used yet

    private GameObject deathEffect = null; //instance of deathEffectPrefab
    private int currentWaypointIndex = 0;
    private Transform target;
    private int currentLife;

    private float precision = 0.2f; // Area used to detect waypoints
    Vector3 direction;

    private ParticleSystem.MainModule main; // 

    private void Start() {
        Spawn();
    }

    private void OnEnable() {
        if (curseSystem != null) curseSystem.OnCurseFallComplete += OnCurseCompleted;
    }

    private void OnDisable() {
        if (curseSystem != null) curseSystem.OnCurseFallComplete += OnCurseCompleted;
    }

    public void SetReferences(WaveSpawner waveSpawnerReference, Waypoints waypointsReference ) {
        this.waveSpawner = waveSpawnerReference;
        this.selectedWaypoints = waypointsReference;
    }

    void Update() {
        if (target == null) return; 
        direction = (target.position - transform.position).normalized;

        transform.Translate(0.1f * currentSpeed * Time.deltaTime * direction, Space.World);

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10f);

        // Reached waypoint?
        if ((transform.position - target.position).sqrMagnitude <= precision) {
            if (currentWaypointIndex >= selectedWaypoints.points.Length - 1) {
                //  Reached end ? deal damage to player / destroy
                // OLD waveSpawner.DamagePlayer(damageToPlayer);
                // OLD  Player.Instance.TakeDamage(damageToPlayer);
                HitPlayer();
                Die();
                return;
            }
            GetNextWaypoint();
        }
    }

    private void HitPlayer() {
        GameEvents.PlayerHit(this, damageToPlayer, true); 
    }

    private void InitValues() {
        this.currentLife = enemyData.StartingLife;
        this.currentSpeed = enemyData.StartingSpeed;
        this.damageToPlayer = enemyData.DamageToPlayer;
        this. moneyWhenKilled = enemyData.MoneyWhenKilled;
        this.xpWhenKilled = enemyData.XpWhenKilled;

        this.DeathEffectPrefab = enemyData.DeathEffectPrefab;
        this.deathColor = enemyData.DeathColor;
    }

    public void Spawn() {
        InitValues();
        enemyHitFlash = GetComponent<EnemyHitFlash>();

        target = selectedWaypoints.points[1]; // target = Waypoints.points[0];

        if (waveSpawner != null) {
            waveSpawner.EnemiesList.Add(this);
            this.transform.position = selectedWaypoints.points[0].position;
        }
        StartCoroutine(PrepareDeathEffect());
    }

    public bool TakeDamage(Turret t, int damage) {
        //if (t== null) Debug.Log("Turret is Null in : " +  this.gameObject.name);
        //else Debug.Log("Turret :" + t.gameObject.name);

        if (isInvinsible) return false;

        currentLife -= damage; // Lower Life
        if (currentLife <= 0) {

            GameEvents.EnemyKilled(t, this); // new

            Die();
            return true;
        }
        else if (damage > 0) {
            enemyHitFlash.TriggerFlash(); // Visual Feedback | should not be called if enemy dies
            SoundManager.Instance.PlaySFX(hitSound, transform.position); //NEw
        }

        return false;
    }

    public void TakeCurse(Turret t) {
        if (isInvinsible) return;
        if (curseSystem == null) return;

        if (curseSystem.ApplyCurse()) {
            // Enemy will die (4 curses)
            GameEvents.EnemyKilled(t, this);
        }
    }
    private void OnCurseCompleted() {
        Die();        
    }

    private IEnumerator PrepareDeathEffect() {
        if (DeathEffectPrefab != null) {

            deathEffect = Instantiate(DeathEffectPrefab);
            deathEffect.SetActive(false);

            ParticleSystem particuleSys = null;
            WaitForEndOfFrame w = new WaitForEndOfFrame();

            for (int i = 0; i < deathEffect.transform.childCount; i++) {
                particuleSys = deathEffect.transform.GetChild(i).GetComponent<ParticleSystem>();
                main = particuleSys.main;
                main.startColor = deathColor;
                yield return w;
            }
        }
    }

    private void Die() {
        if (waveSpawner.EnemiesList.Contains(this)) waveSpawner.EnemiesList.Remove(this);
        
        playerData.GainMoney(moneyWhenKilled); // NEW
        playerData.GainXp(xpWhenKilled); // was Player.Instance

        if (DeathEffectPrefab != null && Visual != null) {

            deathEffect.transform.position = Visual.position;
            deathEffect.SetActive(true);

            Destroy(deathEffect, 3);
        }

        SoundManager.Instance.PlaySFX(deathSound, transform.position); // NEw

        this.gameObject.SetActive(false);
        Destroy(this.gameObject, 1);
    }

    private void GetNextWaypoint() {
        currentWaypointIndex++;
        target = selectedWaypoints.points[currentWaypointIndex];
    }
}

