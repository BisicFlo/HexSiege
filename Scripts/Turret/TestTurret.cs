using UnityEngine;

public class TestTurret : Turret {
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    protected override void Init() {
        base.Init();                 //  This calls the base version!

        Debug.Log("MagicTurret specific Init");
        // Add magic-specific setup here
    }

}
