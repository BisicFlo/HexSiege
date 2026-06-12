using UnityEngine;

// Unused 

public class Node : MonoBehaviour {
 
    //[SerializeField] private Renderer rend; //used to highlight

    public Turret TurretOnTop { get; private set; }


    //public Renderer GetRenderer() {
    //    return rend;
    //}


    public bool SetTurret(Turret turret) {

        if (turret!=null && TurretOnTop == null) {
            TurretOnTop = turret;
            return true;
        } else {
            return false;
        }
    }

    public bool IsEmpty() {
            return (TurretOnTop == null);    
    }
    public void ClearNode() {
         TurretOnTop = null;
    }


    //public void BuildSelectedTurret() {
    //    BuildTurret(BuildManager.Instance.SelectedTurret);
    //}

    //private void BuildTurret(GameObject turretPrefab) {
    //    if (turretOnTop != null) Debug.Log("Space already occupied");

    //    turretOnTop = (GameObject)Instantiate(turretPrefab, this.transform.position, Quaternion.identity);
    //}






}
