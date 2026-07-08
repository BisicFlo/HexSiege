using UnityEngine;

public class ChangeBackgroundColorOnStart : MonoBehaviour {

    [SerializeField] private ColorShifter colorShifter;

    [SerializeField] private int backgroundIndex;



    void Start() {

        // Change Background Color  
        colorShifter.SetBackGroundColorFromIndex(backgroundIndex);


    }


}
