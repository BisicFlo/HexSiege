using System.Collections.Generic;
using UnityEngine;

public class BackgroundColorChanger : MonoBehaviour {

    [SerializeField] private List<Color> BackgroundColorList;

    void Start() {
        //Camera.main.backgroundColor = newColor;
        //Camera.main.clearFlags = CameraClearFlags.SolidColor;
    }

    private void ChangeColorFromIndex(int index) {

        Camera.main.backgroundColor = BackgroundColorList[index];
    }
}
