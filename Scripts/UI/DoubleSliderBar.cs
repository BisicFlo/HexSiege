using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// DoubleSliderBar - A specialized UI component for displaying values with bonus/penalty overlays.
/// Commonly used for health bars, damage previews, or stat comparisons.
/// 
/// Features:
/// - Base value (bottom layer)
/// - Bonus/Total value (top layer) with color feedback
/// - Smooth animated transitions
/// - Supports both positive (green) and negative (red) bonuses
/// </summary>
public class DoubleSliderBar : MonoBehaviour {

    [Header("Sliders")]
    [Tooltip("Main base value slider (usually white or gray)")]
    [SerializeField] private Slider sliderBaseValue; 

    [Tooltip("Secondary/Bonus slider, behind the main slider")]
    [SerializeField] private Slider sliderBonus;     

    [Tooltip("Main fill image of the bonus slider")]
    [SerializeField] private Image imageSliderBonus;

    [Tooltip("Highlight stripe for extra visual pop")]
    [SerializeField] private Image imageHighlightBonus;

    // Predefined colors (HTML format for easy tweaking)
    private static Color RedColor = ColorUtility.TryParseHtmlString("#B2122D", out Color color) ? color : Color.red;
    private static Color LightRedColor = ColorUtility.TryParseHtmlString("#E62E4D", out Color color) ? color : Color.red;
    private static Color GreenColor = ColorUtility.TryParseHtmlString("#AAFF00", out Color color) ? color : Color.green;
    private static Color LightGreenColor = ColorUtility.TryParseHtmlString("#E1FF77", out Color color) ? color : Color.green;

    private Coroutine lerpCoroutine = null;
     private float targetValue;


    public void SetValue(int value) {
        sliderBaseValue.value = value;
    }

    /// <summary>
    /// Sets both base and total value with automatic color switching.
    /// </summary>
    public void SetValue(int baseValue, int total) { // bonus is the total value : Base + bonus stat
        if (total - baseValue >= 0) {
            sliderBaseValue.value = baseValue;
            sliderBonus.value = total;
            imageSliderBonus.color = GreenColor;
            imageHighlightBonus.color = LightGreenColor;
        }
        else {
            sliderBaseValue.value = total; // inverted so we can see the red part 
            sliderBonus.value = baseValue;
            imageSliderBonus.color = RedColor;
            imageHighlightBonus.color = LightRedColor;
        }
    }

    /// <summary>
    /// Updates the maximum value of both sliders (e.g. when max health increases).
    /// </summary>
    public void SetMaxValue(int newMaxHealth) {
        float oldMaxBase = sliderBaseValue.maxValue;
        float oldMaxBonus = sliderBonus.maxValue;

        sliderBaseValue.maxValue = newMaxHealth;
        sliderBonus.maxValue = newMaxHealth;

        // Preserve current fill ratio when max increases
        sliderBaseValue.value += newMaxHealth - oldMaxBase;
        sliderBonus.value += newMaxHealth - oldMaxBonus;
    }


    /// <summary>
    /// Removes value with smooth animation on the bonus layer.
    /// </summary>
    public void RemoveAmountWithEffect(int value) {
        sliderBaseValue.value = value;   // snap the BASE (bottom layer)
        targetValue = value;
        EnsureCoroutineRunning(sliderBonus); // animate BONUS down to match
    }

    /// <summary>
    /// Adds value with smooth animation on the base layer.
    /// </summary>
    public void AddAmountWithEffect(int value) {
        sliderBonus.value = value;       // snap the BONUS (top layer)
        targetValue = value;
        EnsureCoroutineRunning(sliderBaseValue); // animate BASE up to match
    }

    private void EnsureCoroutineRunning(Slider slider) {
        if (lerpCoroutine != null)
            StopCoroutine(lerpCoroutine);

        lerpCoroutine = StartCoroutine(LerpTo(slider, 1f));
    }

    /// <summary>
    /// Smoothly lerps a slider toward the current target value.
    /// </summary>
    private IEnumerator LerpTo(Slider slider, float duration) {
        float start = slider.value;
        float myTarget = targetValue;          // snapshot at coroutine start
        float elapsed = 0f;

        while (elapsed < duration) {
            // If target changed mid-flight, restart from current position
            if (!Mathf.Approximately(myTarget, targetValue)) {
                start = slider.value;
                myTarget = targetValue;
                elapsed = 0f;
            }

            elapsed += Time.deltaTime;
            slider.value = Mathf.Lerp(start, myTarget, elapsed / duration);
            yield return null;
        }

        slider.value = myTarget;
        lerpCoroutine = null;
    }
}


