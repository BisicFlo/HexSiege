using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DoubleSliderBar : MonoBehaviour {

    [SerializeField] private Slider sliderBaseValue; // Main Bar : white 
    [SerializeField] private Slider sliderBonus; // Bonus Bar : Green | Red

    //[SerializeField] private static Color RedColor = Color.red; // B2122D + E62E4D
    //[SerializeField] private static Color GreenColor = Color.green; // AAFF00 + E1FF77

    private static Color RedColor = ColorUtility.TryParseHtmlString("#B2122D", out Color color) ? color : Color.red;
    private static Color LightRedColor = ColorUtility.TryParseHtmlString("#E62E4D", out Color color) ? color : Color.red;
    private static Color greenColor = ColorUtility.TryParseHtmlString("#AAFF00", out Color color) ? color : Color.green;
    private static Color LightGreenColor = ColorUtility.TryParseHtmlString("#E1FF77", out Color color) ? color : Color.green;

    private Image imageSliderBonus;
    private Coroutine lerpCoroutine = null;
    private bool isDirty = false;
    private float target;

    private void Start() {
        imageSliderBonus = sliderBonus.fillRect.GetComponent<Image>();
    }

    public void SetValue(int value) {
        sliderBaseValue.value = value;
    }
    public void SetValue(int value, int bonus) { // bonus is the total value : Base + bonus stat
        if (bonus > 0) {
            sliderBaseValue.value = value;
            sliderBonus.value = bonus;
            imageSliderBonus.color = RedColor;
        }
        else { 
            sliderBaseValue.value = bonus; // inverted so we can see the red part 
            sliderBonus.value = value;
            imageSliderBonus.color = GreenColor;
        }
    }
    public void SetMaxValue(int newMaxHealth) {
        float oldMaxBase = sliderBaseValue.maxValue;
        float oldMaxBonus = sliderBonus.maxValue;

        sliderBaseValue.maxValue = newMaxHealth;
        sliderBonus.maxValue = newMaxHealth;

        sliderBaseValue.value += newMaxHealth - oldMaxBase;
        sliderBonus.value += newMaxHealth - oldMaxBonus;
    }

    public void RemoveAmountWithEffect(int value) {
        //int startValue = (int)sliderBaseValue.value;

        sliderBaseValue.value = value;
        target = value;

        if (lerpCoroutine == null) {
            Debug.Log("Starting Lerp");
            isDirty = true;
            lerpCoroutine = StartCoroutine(LerpTo(sliderBonus, 1 ));
        } else {
            isDirty = true;
        }
    }

    public void AddAmountWithEffect(int value) {
        //int startValue = (int)sliderBaseValue.value;

        //sliderBaseValue.value = value;
        sliderBonus.value = value;
        target = value;

        if (lerpCoroutine == null) {
            isDirty = true;
            lerpCoroutine = StartCoroutine(LerpTo(sliderBaseValue, 1));
        }
        else {
            isDirty = true;
        }
    }

    private IEnumerator LerpTo(Slider slider , float duration) {
        while (isDirty) {
            isDirty = false;

            float start = slider.value;
            float elapsed = 0f;

            while (elapsed < duration) {
                elapsed += Time.deltaTime;
                slider.value = Mathf.Lerp(start, target, elapsed / duration);
                yield return null;
            }
            slider.value = target; // Ensure exact end value            
        }
        lerpCoroutine = null;
    }
}
