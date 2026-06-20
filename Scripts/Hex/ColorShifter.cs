using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ColorShifter - Changes color using UV offset on a shared Gradient Atlas (22x4)
/// </summary>
public class ColorShifter : MonoBehaviour {

    [Header("Gradient Atlas Settings")]
    [Tooltip("The material using the gradient atlas texture")]
    [SerializeField] private Material material;

    [Tooltip("X offset (0 to 21) - Horizontal color position")]
    [Range(0, 21)]
    [SerializeField] private int colorIndexX = 0;

    [Tooltip("Y offset (0 to 3) - Vertical row position")]
    [Range(0, 3)]
    [SerializeField] private int colorIndexY = 0;

    //[Header("Animation Settings")]
    //[SerializeField] private bool animate = false;
    //[Tooltip("Speed of automatic color shifting")]
    //[SerializeField] private float animationSpeed = 1f;

    [Header("Background Colors")]
    //[SerializeField] private List<Color> BackgroundColorList;
    [SerializeField] private ColorData BackgroundColors; // NEW

    [SerializeField] private int BackgroundIndex;

    [SerializeField] private Text indexDisplayText;

    private Renderer rend;
    private Vector2 currentOffset;

    private void Awake() {
        rend = GetComponent<Renderer>();

        // Use the material from the renderer if none is assigned
        if (material == null && rend != null) {
            material = rend.sharedMaterial;
        }
    }

    /// <summary>
    /// Updates the texture offset based on grid position
    /// </summary>
    public void UpdateColorOffset() {
        if (material == null) return;

        // Calculate UV offset (each cell is 1/22 wide and 1/4 tall)
        float offsetX = colorIndexX / 22f;
        float offsetY = colorIndexY / 4f;

        currentOffset = new Vector2(offsetX, offsetY);
        material.mainTextureOffset = currentOffset;
    }

    /// <summary>
    /// Set a specific color by grid coordinates
    /// </summary>
    public void SetColor(int x, int y) {
        colorIndexX = Mathf.Clamp(x, 0, 21);
        colorIndexY = Mathf.Clamp(y, 0, 3);
        UpdateColorOffset();
    }

    /// <summary>
    /// Set color by index (flattens the 22x4 grid into 0-87)
    /// </summary>
    public void SetColorByIndex(int index) {
        int totalColors = 22 * 4;
        index = Mathf.Clamp(index, 0, totalColors - 1);
        colorIndexX = index % 22;
        colorIndexY = index / 22;
        UpdateColorOffset();
    }

    public void SetBackGroundColorFromIndex(int index) {

        if ( index < 0 || index >= BackgroundColors.ColorList.Count) return;     
        Camera.main.backgroundColor = BackgroundColors.ColorList[index]; // BackgroundColors
        BackgroundIndex = index;
    }

    public void ShiftBackgroundColor() {
        BackgroundIndex= (BackgroundIndex+1) % BackgroundColors.ColorList.Count;
        SetBackGroundColorFromIndex(BackgroundIndex);
        DisplayIndexesOnScreen();       
    }

    public void ShiftAtlasColor_X() {
        colorIndexX = (colorIndexX + 1) % 22;
        UpdateColorOffset() ;
        DisplayIndexesOnScreen();
    }

    public void ShiftAtlasColor_Y() {
        colorIndexY = (colorIndexY + 1) % 4;
        UpdateColorOffset();
        DisplayIndexesOnScreen();
    }


    public int GetCurrentColorIndex() {
        return colorIndexX + colorIndexY*22;
    }

    public int GetCurrentBackgroundColorIndex() {
        return BackgroundIndex;
    }

    private void DisplayIndexesOnScreen() {
        indexDisplayText.text = "B : " + BackgroundIndex + " X : " +  colorIndexX + " Y : " + colorIndexY;
    }
}
