
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [HideInInspector] public WaveSpawner waveSpawner = null; // set by WaveSpawner itself
    public GameObject DeathEffectPrefab = null;    

    [SerializeField] private Color deathColor ;    

    [SerializeField] private Transform Visual;

    [SerializeField] private int startingSpeed = 3;
    [SerializeField] private int startingLife = 4;
    [SerializeField] private int damageToPlayer = 4;
    [SerializeField] private int moneyWhenKilled = 1;
    [SerializeField] private int xpWhenKilled = 1;
    [SerializeField] private bool isInvinsible = false;

    private EnemyHitFlash enemyHitFlash;

    private GameObject deathEffect = null;
    private int currentWaypointIndex = 0;
    private Transform target;
    private int currentLife;
    private int currentSpeed;
    private float precision = 0.2f; // Area used to detect 
    Vector3 direction;

    private ParticleSystem.MainModule main; // 

    private void Start() {
        Spawn();
    }

    void Update() {

        direction = (target.position - transform.position).normalized;
        
        transform.Translate(0.1f * currentSpeed * Time.deltaTime * direction, Space.World);
    
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10f);

        // Reached waypoint?
        if ((transform.position - target.position).sqrMagnitude <= precision) {
            if (currentWaypointIndex >= Waypoints.points.Length - 1) {
                //  Reached end ? deal damage to player / destroy
                // OLD waveSpawner.DamagePlayer(damageToPlayer);
                Player.instance.TakeDamage(damageToPlayer);
                Die();
                return;
            }
            GetNextWaypoint();
        }
    }

    public void Spawn() {
        enemyHitFlash = GetComponent<EnemyHitFlash>();

        target = Waypoints.points[0];
        currentLife = startingLife;
        currentSpeed = startingSpeed;

        if (waveSpawner != null) {
            waveSpawner.EnemiesList.Add(this);
            this.transform.position = waveSpawner.transform.position;
        }

        StartCoroutine(PrepareDeathEffect());
    }

    public bool TakeDamage(int damage) {
        
        if (isInvinsible) return false;

        currentLife -= damage; // Lower Life
        if (currentLife <= 0) {
            Die();
            return true;
        } else {
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
        target = Waypoints.points[currentWaypointIndex];
    }

}

