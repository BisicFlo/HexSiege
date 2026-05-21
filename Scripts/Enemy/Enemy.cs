
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [SerializeField] private EnemyData enemyData;

    [SerializeField] private Transform Visual; // Part holding the renderer    

    #region SetByEnemyData
    private GameObject DeathEffectPrefab = null;
    private Color deathColor;

    private int startingSpeed = 3;
    private int startingLife = 4;
    private int damageToPlayer = 4;
    private int moneyWhenKilled = 1;
    private int xpWhenKilled = 1;
    #endregion

    #region SetByWaveSpawner 
    private WaveSpawner waveSpawner = null; // set by WaveSpawner itself // [HideInInspector] public
    private Waypoints selectedWaypoints; // set by WaveSpawner itself
    #endregion

    private EnemyHitFlash enemyHitFlash; // For visual Feedback // Setup automatically by GetComponent

    private bool isInvinsible = false;

    private GameObject deathEffect = null; //instance of deathEffectPrefab
    private int currentWaypointIndex = 0;
    private Transform target;
    private int currentLife;
    private int currentSpeed;
    private float precision = 0.2f; // Area used to detect waypoints
    Vector3 direction;

    private ParticleSystem.MainModule main; // 

    private void Start() {
        Spawn();
    }

    public void SetReferences(WaveSpawner waveSpawnerReference, Waypoints waypointsReference ) {
        this.waveSpawner = waveSpawnerReference;
        this.selectedWaypoints = waypointsReference;
    }

    void Update() {

        direction = (target.position - transform.position).normalized;

        transform.Translate(0.1f * currentSpeed * Time.deltaTime * direction, Space.World);

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10f);

        // Reached waypoint?
        if ((transform.position - target.position).sqrMagnitude <= precision) {
            if (currentWaypointIndex >= selectedWaypoints.points.Length - 1) { //
                //  Reached end ? deal damage to player / destroy
                // OLD waveSpawner.DamagePlayer(damageToPlayer);
                Player.instance.TakeDamage(damageToPlayer);
                Die();
                return;
            }
            GetNextWaypoint();
        }
    }

    private void InitValues() {
        this.currentLife = enemyData.startingLife;
        this.currentSpeed = enemyData.startingSpeed;
        this.damageToPlayer = enemyData.damageToPlayer;
        this. moneyWhenKilled = enemyData.moneyWhenKilled;
        this.xpWhenKilled = enemyData.xpWhenKilled;

        this.DeathEffectPrefab = enemyData.DeathEffectPrefab;
        this.deathColor = enemyData.deathColor;
    }

    public void Spawn() {
        InitValues();
        enemyHitFlash = GetComponent<EnemyHitFlash>();

        target = selectedWaypoints.points[1]; // target = Waypoints.points[0];
        currentLife = startingLife;
        currentSpeed = startingSpeed;

        if (waveSpawner != null) {
            waveSpawner.EnemiesList.Add(this);
            this.transform.position = selectedWaypoints.points[0].position;
        }
        StartCoroutine(PrepareDeathEffect());
    }

    public bool TakeDamage(int damage) {

        if (isInvinsible) return false;

        currentLife -= damage; // Lower Life
        if (currentLife <= 0) {
            Die();
            return true;
        }
        else {
            enemyHitFlash.TriggerFlash(); // Visual Feedback | should not be called if enemy dies
            return false;
        }
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
        if (waveSpawner.EnemiesList.Contains(this)) {
            waveSpawner.EnemiesList.Remove(this);
        }
        Player.instance.GainMoney(moneyWhenKilled);
        Player.instance.GainXp(xpWhenKilled);

        if (DeathEffectPrefab != null && Visual != null) {
            //GameObject deathEffect = Instantiate(_DeathEffectPrefab);

            deathEffect.transform.position = Visual.position;
            deathEffect.SetActive(true);

            Destroy(deathEffect, 3);
        }

        this.gameObject.SetActive(false);
        Destroy(this.gameObject, 1);
    }

    private void GetNextWaypoint() {
        currentWaypointIndex++;
        target = selectedWaypoints.points[currentWaypointIndex];
    }

}

