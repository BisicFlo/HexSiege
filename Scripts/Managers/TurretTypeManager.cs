using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TurretTypeManager : MonoBehaviour {
    public static TurretTypeManager Instance { get; private set; } // Singleton needed ? 

    [Header("Turret Type Sprites")]
    [SerializeField] private Sprite medievalSprite;
    [SerializeField] private Sprite magicalSprite;
    [SerializeField] private Sprite plantSprite;
    [SerializeField] private Sprite totemSprite;
    [SerializeField] private Sprite divineSprite;
    [SerializeField] private Sprite obscureSprite;

    [SerializeField] private Sprite allSprite;


    // Optional fallback sprite
    [SerializeField] private Sprite defaultSprite;

    // Optional: Dictionary for runtime lookup (auto-populated from inspector fields)
    private Dictionary<TurretType, Sprite> spriteMap = new Dictionary<TurretType, Sprite>();

    private void Awake() {
        if (Instance != null) Debug.LogWarning("More than one TurretTypeManager detected");
        Instance = this;
        InitializeSpriteMap();
    }

    private void InitializeSpriteMap() {
        spriteMap.Clear();

        if (medievalSprite != null) spriteMap[TurretType.Medieval] = medievalSprite;
        if (magicalSprite != null) spriteMap[TurretType.Magical] = magicalSprite;
        if (plantSprite != null) spriteMap[TurretType.Plant] = plantSprite;
        if (totemSprite != null) spriteMap[TurretType.Totem] = totemSprite;
        if (divineSprite != null) spriteMap[TurretType.Divine] = divineSprite;
        if (divineSprite != null) spriteMap[TurretType.Obscure] = obscureSprite;

        if (allSprite != null) spriteMap[TurretType.All] = allSprite;


        // Default fallback
        if (defaultSprite != null) {
            spriteMap[TurretType.None] = defaultSprite;
            spriteMap[TurretType.All] = defaultSprite;
        }
    }

    /// <summary>
    /// Assigns the correct sprite to the Image based on TurretType.
    /// </summary>
    /// <returns>True if sprite was successfully assigned.</returns>
    public bool SetCorrespondingSprite(Image image, TurretType type) {
        if (image == null)
            return false;

        if (spriteMap.TryGetValue(type, out Sprite sprite) && sprite != null) {
            image.sprite = sprite;
            return true;
        }

        // Fallback to default sprite if available
        if (defaultSprite != null) {
            image.sprite = defaultSprite;
            return true;
        }

        Debug.LogWarning($"No sprite found for TurretType: {type}");
        return false;
    }
}