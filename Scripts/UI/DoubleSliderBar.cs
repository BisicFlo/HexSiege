using UnityEngine;
using UnityEngine.UI;

public class DoubleSliderBar : MonoBehaviour {


    [SerializeField] private Slider sliderBaseValue; // Main Bar : white 
    [SerializeField] private Slider sliderBonus; // Bonus Bar : Green | Red

    [SerializeField] private Color RedColor = Color.red;
    [SerializeField] private Color GreenColor = Color.green;

    private Image imageSliderBonus;

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
            sliderBaseValue.value = bonus; // inversion so we can see the red part 
            sliderBonus.value = value;
            imageSliderBonus.color = GreenColor;
        }
    }
    public void SetMaxValue(int newMaxHealth) {
        sliderBaseValue.maxValue = newMaxHealth;
        sliderBonus.maxValue = newMaxHealth;

    }




}
