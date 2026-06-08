using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class XpBar : MonoBehaviour {
    public PlayerData playerData; // temp ! debug

    [SerializeField] private Slider slider;
    [SerializeField] private float segmentDuration = 0.4f;

    private int maxXp = 100;
    private int currentXp = 0;
    private Coroutine animCoroutine;

    // Each segment: animate from A to B, then snap back to 0 if overflow
    private readonly Queue<(float from, float to, bool resetAfter)> segments = new();

    public int GainXP;


    //private void Awake() {
    //    imageSliderBonus = sliderBonus.fillRect.GetComponent<Image>();
    //    imageHighlightBonus = sliderBonus.transform.GetChild(1).GetChild(0).GetComponent<Image>();
    //}

    private void Update() { //temp
        if (GainXP != 0) {
            AddXp(GainXP, playerData.Xp, playerData.Level);
            GainXP = 0;
        }
    }

    public void AddXp(int amount, int currentXp, int currentLevel ) {
        int remaining = amount;
        int xp = currentXp;

        while (remaining > 0) {
            // Cap at max level — no more leveling up
            if (currentLevel > PlayerData.xpRequired.Length) {
                xp = PlayerData.xpRequired[PlayerData.xpRequired.Length - 1]; // clamp display
                break;
            }

            int maxXp = PlayerData.xpRequired[currentLevel-1];
            int space = maxXp - xp;

            if (remaining >= space) {
                segments.Enqueue(((float)xp / maxXp, 1f, true));
                remaining -= space;
                xp = 0;
                currentLevel++;
            }
            else {
                segments.Enqueue(((float)xp / maxXp, (float)(xp + remaining) / maxXp, false));
                xp += remaining;
                remaining = 0;
            }
        }

        currentXp = xp;

        if (animCoroutine == null)
            animCoroutine = StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue() {
        while (segments.Count > 0) {
            var (from, to, resetAfter) = segments.Dequeue();

            // Lerp from -> to
            float elapsed = 0f;
            float duration = segmentDuration * (to - from); // scale by fill distance
            duration = Mathf.Max(duration, 0.05f);

            while (elapsed < duration) {
                elapsed += Time.deltaTime;
                slider.value = Mathf.Lerp(from, to, elapsed / duration);
                yield return null;
            }
            slider.value = to;

            // Brief flash at full before resetting (Minecraft feel)
            if (resetAfter) {
                yield return new WaitForSeconds(0.08f);
                slider.value = 0f;
            }
        }

        animCoroutine = null;
    }
}