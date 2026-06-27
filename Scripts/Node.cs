using UnityEngine;


public class Node : MonoBehaviour { 
    public Turret TurretOnTop { get; private set; }

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

}
