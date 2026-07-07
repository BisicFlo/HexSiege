using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public static SoundManager Instance { get; private set; } // Singleton

    [SerializeField] private GameObject emitterPrefab; // AudioSource + Emitter component
    [SerializeField] private int initialPoolSize = 15;

    private Queue<SoundEmitter> pool = new Queue<SoundEmitter>();
    //private List<SoundEmitter> active = new List<SoundEmitter>();

    [Header("Ambiance")]
    [SerializeField] private AudioSource ambianceSource1;   // Main
    [SerializeField] private AudioSource ambianceSource2;   // For crossfading

    private AudioSource currentAmbiance;
    private AudioSource fadingAmbiance;

    private Coroutine ambianceFadeCoroutine;

    private void Awake() {
        if (Instance != null) Debug.LogWarning("More than one SoundManager detected");
        Instance = this;
        CreatePool();

        // Setup Ambiance Sources
        SetupAmbianceSources();
    }
    private void SetupAmbianceSources() {
        if (ambianceSource1 == null) {
            GameObject go = new GameObject("AmbianceSource1");
            go.transform.SetParent(transform);
            ambianceSource1 = go.AddComponent<AudioSource>();
        }

        if (ambianceSource2 == null) {
            GameObject go = new GameObject("AmbianceSource2");
            go.transform.SetParent(transform);
            ambianceSource2 = go.AddComponent<AudioSource>();
        }

        ambianceSource1.loop = true;
        ambianceSource2.loop = true;
        ambianceSource1.playOnAwake = false;
        ambianceSource2.playOnAwake = false;

        currentAmbiance = ambianceSource1;
    }

    public void PlayAmbiance(SoundData ambianceData, float fadeDuration = 2f) {
        if (ambianceData == null) return;

        AudioSource oldSource = currentAmbiance;
        AudioSource newSource = (currentAmbiance == ambianceSource1) ? ambianceSource2 : ambianceSource1;

        // Setup new ambiance
        AudioClip clip = ambianceData.clips[0]; // only 1 clip for ambiance
        newSource.clip = clip;
        newSource.volume = 0f;
        newSource.outputAudioMixerGroup = ambianceData.mixerGroup;
        newSource.Play();

        // Stop any in-progress fade so it can't fight this one
        if (ambianceFadeCoroutine != null)
            StopCoroutine(ambianceFadeCoroutine);

        currentAmbiance = newSource;
        fadingAmbiance = oldSource;

        float volumeMax = Random.Range(ambianceData.volumeRange.x, ambianceData.volumeRange.y);

        ambianceFadeCoroutine = StartCoroutine(CrossFadeAmbiance(newSource, oldSource, volumeMax, fadeDuration));
    }

    private IEnumerator CrossFadeAmbiance( AudioSource newSource, AudioSource oldSource,float volumeMax, float duration) {
        float timer = 0f;
        float startVolume = oldSource != null ? oldSource.volume : 0f;


        while (timer < duration) {
            timer += Time.deltaTime;
            float t = timer / duration;
            newSource.volume = Mathf.Lerp(0f, volumeMax, t);           // Fade in
            if (oldSource != null)
                oldSource.volume = Mathf.Lerp(startVolume, 0f, t); // Fade out
            yield return null;
        }

        // Finish cleanly
        newSource.volume = volumeMax;
        if (oldSource != null && oldSource != newSource) {
            oldSource.Stop();
            oldSource.volume = 0f;
        }

        ambianceFadeCoroutine = null;
    }

    public void StopAmbiance(float fadeOutTime = 2f) {
        if (currentAmbiance.isPlaying)
            StartCoroutine(FadeOutAmbiance(currentAmbiance, fadeOutTime));
    }

    private IEnumerator FadeOutAmbiance(AudioSource source, float duration) {
        float startVol = source.volume;
        float timer = 0f;

        while (timer < duration) {
            timer += Time.deltaTime;
            source.volume = Mathf.Lerp(startVol, 0f, timer / duration);
            yield return null;
        }

        source.Stop();
        source.volume = 0f;
    }

    private void CreatePool() {
        for (int i = 0; i < initialPoolSize; i++) {
            var go = Instantiate(emitterPrefab, transform);
            go.SetActive(false);
            pool.Enqueue(go.GetComponent<SoundEmitter>());
        }
    }

    public void PlaySFX(SoundData data, Vector3 position = default) {
        if (data == null || data.clips.Length == 0) return;

        // Optional: Throttle frequent sounds (hits)
        // ... logic here ...

        SoundEmitter emitter = GetFromPool();
        if (emitter == null) return; // New
        emitter.Play(data, position, this);
        //active.Add(emitter);
    }
    private SoundEmitter GetFromPool() {
        if (pool.Count > 0) {
            var e = pool.Dequeue();
            if (e == null) return null; // New
            e.gameObject.SetActive(true);
            return e;
        }
        // Expand pool if needed
        var newGo = Instantiate(emitterPrefab, transform);
        return newGo.GetComponent<SoundEmitter>();
    }

    // Call from emitter when finished
    public void ReturnToPool(SoundEmitter emitter) {
        emitter.gameObject.SetActive(false);
        //active.Remove(emitter);
        pool.Enqueue(emitter);
    }

    // Volume control example
   // public void SetSFXVolume(float value) => mixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);




}
