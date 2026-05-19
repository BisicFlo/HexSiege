using UnityEngine;

public class SpikeTurret : Turret {

    // --------------------------------------------------------------
    //   Inspector Fields
    // -------------------------------------------------------------- 

    [SerializeField] private GameObject spikePrefab = null;

    // --------------------------------------------------------------
    //   Private 
    // --------------------------------------------------------------

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

        Spike mySpike = GetObjectFromIndex<Spike>(spikeArray, SpikeIndex);
        SpikeIndex++;
        SpikeIndex %= spikeArray.Length; // Redundant ?

        if (mySpike.gameObject.activeSelf) {
            Debug.Log("Spike Already In Used ");
            mySpike.HitTarget();
        }
        
        InstantiateAlternative(mySpike.gameObject, Vector3.zero, Quaternion.identity, Vector3.one, null); //firePoint.rotation
        if (target != null) {
            mySpike.Init(target, enemyTargetted, AttackDamage.Value, ProjectileSpeed.Value);
            mySpike.ActivateBulletAndDesactivateImpact();

            mySpike.Erupt(target.position); //new
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Mathf.Sqrt(Range.Value)); // Mathf.Sqrt because sqrMagnitude
    }
}
