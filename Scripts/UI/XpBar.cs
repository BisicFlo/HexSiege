using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// XpBar - Animated experience bar with level-up segmentation.
/// Supports multi-level XP gains in a single call with smooth fill animations. 
/// </summary>
public class XpBar : MonoBehaviour {

    [Header("References")]
    [SerializeField] private Slider slider;

    [Header("Animation")]
    [Tooltip("Duration to fill one full segment (0 to 1)")]
    [SerializeField] private float segmentDuration = 0.4f;      

    // Each segment: animate from A to B, then snap back to 0 if overflow
    private readonly Queue<(float from, float to, bool resetAfter)> segments = new();

    private Coroutine animCoroutine;

    /// <summary>
    /// Adds XP and queues the appropriate animations, handling level-ups automatically.
    /// </summary>
    /// <param name="amount">Amount of XP to add.</param>
    /// <param name="currentXp">Current XP before adding.</param>
    /// <param name="currentLevel">Current player level.</param>
    public void AddXp(int amount, int currentXp, int currentLevel ) {
        Debug.Log("AddXp : " + amount);

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
            Debug.Log("maxXp : " + maxXp + "space : " + space);


            if (remaining >= space) {
                Debug.Log("remaining : " + remaining);
                segments.Enqueue(((float)xp / maxXp, 1f, true));
                remaining -= space;
                xp = 0;
                currentLevel++;
            }
            else {
                Debug.Log("Target  : " + (float)(xp + remaining) / maxXp);
                segments.Enqueue(((float)xp / maxXp, (float)(xp + remaining) / maxXp, false));
                xp += remaining;
                remaining = 0;
            }
        }

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