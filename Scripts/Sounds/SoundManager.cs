using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public static SoundManager Instance { get; private set; } // Singleton

    [SerializeField] private GameObject emitterPrefab; // AudioSource + Emitter component
    [SerializeField] private int initialPoolSize = 15;

    private Queue<SoundEmitter> pool = new Queue<SoundEmitter>();
    //private List<SoundEmitter> active = new List<SoundEmitter>();

    private void Awake() {
        if (Instance != null) Debug.LogWarning("More than one BuildManager detected");
        Instance = this;
        CreatePool();
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
