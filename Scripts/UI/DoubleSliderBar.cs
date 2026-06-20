using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class DoubleSliderBar : MonoBehaviour {

    [SerializeField] private Slider sliderBaseValue; // Main Bar : white 
    [SerializeField] private Slider sliderBonus; // Bonus Bar : Green | Red

    [SerializeField] private Image imageSliderBonus;     // main image Slider
    [SerializeField] private Image imageHighlightBonus;  // Small stripe of color for bright effect 

    //[SerializeField] private static Color RedColor = Color.red; // B2122D + E62E4D
    //[SerializeField] private static Color GreenColor = Color.green; // AAFF00 + E1FF77

    private static Color RedColor = ColorUtility.TryParseHtmlString("#B2122D", out Color color) ? color : Color.red;
    private static Color LightRedColor = ColorUtility.TryParseHtmlString("#E62E4D", out Color color) ? color : Color.red;
    private static Color GreenColor = ColorUtility.TryParseHtmlString("#AAFF00", out Color color) ? color : Color.green;
    private static Color LightGreenColor = ColorUtility.TryParseHtmlString("#E1FF77", out Color color) ? color : Color.green;

    //private Image imageSliderBonus;     // main image Slider
    //private Image imageHighlightBonus;  // Small stripe of color for bright effect 

    private Coroutine lerpCoroutine = null;
    //[SerializeField] private bool isDirty = false;
    [SerializeField] private float target;

    public bool takeDamage;
    public bool heal;
    private int health = 10;

    //private void Awake() {
    //    imageSliderBonus = sliderBonus.fillRect.GetComponent<Image>();
    //    imageHighlightBonus = sliderBonus.transform.GetChild(1).GetChild(0).GetComponent<Image>();
    //}

    private void Update() { //temp
        if (takeDamage) {
            takeDamage = false;
            health--;
            RemoveAmountWithEffect(health);
        }
        if (heal) {
            heal = false;
            health++;
            AddAmountWithEffect(health);
        }
    }

    public void SetValue(int value) {
        sliderBaseValue.value = value;
    }
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
    public void SetMaxValue(int newMaxHealth) {
        float oldMaxBase = sliderBaseValue.maxValue;
        float oldMaxBonus = sliderBonus.maxValue;

        sliderBaseValue.maxValue = newMaxHealth;
        sliderBonus.maxValue = newMaxHealth;

        sliderBaseValue.value += newMaxHealth - oldMaxBase;
        sliderBonus.value += newMaxHealth - oldMaxBonus;
    }

    //public void RemoveAmountWithEffectOLD(int value) { // Set amount : value

    //    //sliderBaseValue.value = value;
    //    //target = value;

    //    //if (lerpCoroutine == null) {
    //    //    isDirty = true;
    //    //    lerpCoroutine = StartCoroutine(LerpTo(sliderBonus, 1 ));
    //    //} else {
    //    //    isDirty = true;
    //    //}
    //}
    public void RemoveAmountWithEffect(int value) {
        sliderBaseValue.value = value;   // snap the BASE (bottom layer)
        target = value;
        EnsureCoroutineRunning(sliderBonus); // animate BONUS down to match
    }

    //public void AddAmountWithEffectOLD(int value) {

    //    //sliderBonus.value = value;
    //    //target = value;

    //    //if (lerpCoroutine == null) {
    //    //    isDirty = true;
    //    //    lerpCoroutine = StartCoroutine(LerpTo(sliderBaseValue, 1));
    //    //}
    //    //else {
    //    //    isDirty = true;
    //    //}
    //}

    public void AddAmountWithEffect(int value) {
        sliderBonus.value = value;       // snap the BONUS (top layer)
        target = value;
        EnsureCoroutineRunning(sliderBaseValue); // animate BASE up to match
    }

    private void EnsureCoroutineRunning(Slider slider) {
        if (lerpCoroutine != null)
            StopCoroutine(lerpCoroutine);

        lerpCoroutine = StartCoroutine(LerpTo(slider, 1f));
    }


    private IEnumerator LerpTo(Slider slider, float duration) {
        float start = slider.value;
        float myTarget = target;          // snapshot at coroutine start
        float elapsed = 0f;

        while (elapsed < duration) {
            // If target changed mid-flight, restart from current position
            if (!Mathf.Approximately(myTarget, target)) {
                start = slider.value;
                myTarget = target;
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


//    private IEnumerator LerpToOLD(Slider slider , float duration) {

//        while (isDirty) {
//            isDirty = false;

//            float start = slider.value;
//            float elapsed = 0f;

//            Debug.Log("Lerping from : " + start + " to : " + target);

//            while (elapsed < duration) {
//                elapsed += Time.deltaTime;
//                slider.value = Mathf.Lerp(start, target, elapsed / duration);
//                yield return null;
//            }
//            slider.value = target; // Ensure exact end value            
//        }
//        Debug.Log("CoroutineFinished"); 
//       lerpCoroutine = null;
//    }
//}
