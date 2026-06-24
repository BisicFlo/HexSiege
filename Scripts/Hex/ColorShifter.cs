using System;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
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

    [Header("Fog/Gradient Material")]
    [Tooltip("Material that should be the same color as background")]
    [SerializeField] private Material fogMaterial;

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

        SetFogColor(BackgroundColors.ColorList[index]);
    }

    private void SetFogColor(Color color) {
        fogMaterial.color = color;
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


    /// <summary>
    /// Returns the current main texture offset of the material
    /// </summary>
    public Vector2 GetMaterialOffset() {
        return material.mainTextureOffset;
    }

    /// <summary>
    /// Returns the current main texture offset Index of the material:  x: [0-22] | y:[0-4]
    /// </summary>
    public Vector2 GetMaterialOffsetIndex() { 
        Vector2 index = material.mainTextureOffset;

        index.x *= 22;
        index.y *= 4;

        index.x %= 22;
        index.y %= 4;

        return index;
    }


    /// <summary>
    /// Returns the current main texture offset Index of the material: [0-87]
    /// </summary>
    public int GetMaterialOffsetGlobalIndex() {
        Vector2 offset = material.mainTextureOffset;

        int x = Mathf.RoundToInt(offset.x * 22f) % 22;
        int y = Mathf.RoundToInt(offset.y * 4f) % 4;

        if (x < 0) x += 22;
        if (y < 0) y += 4;

        return (int)(22 * y + x);
    }

    public int GetBackGroundColorIndex() {
        int index = -1;

        for (int i = 0; i < BackgroundColors.ColorList.Count; i++) {

            if (Camera.main.backgroundColor == BackgroundColors.ColorList[i]) {
                index = i; break;
            }

        }
        return index;
    }



    private void DisplayIndexesOnScreen() {
        if (indexDisplayText == null) return;
        indexDisplayText.text = "B : " + BackgroundIndex + " X : " +  colorIndexX + " Y : " + colorIndexY;
    }



}
