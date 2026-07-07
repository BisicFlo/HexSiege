using UnityEngine;

public class SpikeTurret : Turret {

    [SerializeField] private GameObject spikePrefab = null;

    private Spike[] spikeArray;
    private int SpikeIndex = 0;

    // --------------------------------------------------------------
    //   MonoBehaviour
    // --------------------------------------------------------------

    protected override void Init() {
        base.Init();
        if (spikePrefab != null) spikeArray = CreateStockOf<Spike>(spikePrefab, 5);
    }

    void Update() {

        if (target != null) {

            bool readyToFire = CheckIfReadyToFire();

            if (readyToFire) {

                Attack();

            }
        }
    }

    protected override void Shoot() {
        bool isCritical = IsCritical();
        bool isCursed = IsCursed();
        int attackDamage = AttackDamage.Value;
        if (isCritical) attackDamage *= (int)CriticalDamage.Value;

        //if (isCursed) Debug.Log("isCursedHit");
        //if (isCritical) Debug.Log("IsCriticalHit");

        Spike mySpike = GetObjectFromIndex<Spike>(spikeArray, SpikeIndex);
        SpikeIndex++;
        SpikeIndex %= spikeArray.Length; // Redundant ?

        if (mySpike.gameObject.activeSelf) {
            Debug.Log("Spike Already In Used ");
            mySpike.HitTarget();
        }

        InstantiateAlternative(mySpike.gameObject, Vector3.zero, Quaternion.identity, Vector3.one, null); //firePoint.rotation
        if (target != null) {
            SoundManager.Instance.PlaySFX(shootSound, transform.position); // New

            mySpike.Init(this, target, enemyTargetted, attackDamage, ProjectileSpeed.Value, isCritical, isCursed);
            mySpike.ActivateSpikeAndDesactivateImpact();//  Redundant  : setActive

            mySpike.Erupt(target.position); //new
        }
    }

    private void OnDestroy() {
        DestroyStockOf(spikeArray);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Mathf.Sqrt(Range.Value)); // Mathf.Sqrt because sqrMagnitude
    }
}
