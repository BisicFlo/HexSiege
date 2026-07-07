using UnityEngine;
using UnityEngine.Audio;

public class SoundEmitter : MonoBehaviour {
    [SerializeField] private AudioSource audioSource;

    private SoundData currentData;
    private SoundManager soundManager; // Reference back to manager to return to pool
    private float startTime;

    private void Awake() {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    /// <summary>
    /// Called by SoundManager when getting this emitter from the pool
    /// </summary>
    public void Play(SoundData data, Vector3 position = default, SoundManager manager = null) {
        if (data == null || data.clips.Length == 0) {
            ReturnToPool();
            return;
        }

        soundManager = manager;
        currentData = data;

        // Setup position (3D if position provided, otherwise 2D)
        if (position != default) {
            transform.position = position;
            audioSource.spatialBlend = .8f;   // 3D sound  // not 1 
        }
        else {
            audioSource.spatialBlend = 0f;   // 2D sound (UI, etc.)
        }

        // Random variation
        AudioClip clip = data.clips[Random.Range(0, data.clips.Length)];
        audioSource.clip = clip;
        audioSource.volume = Random.Range(data.volumeRange.x, data.volumeRange.y);
        audioSource.pitch = Random.Range(data.pitchRange.x, data.pitchRange.y);
        audioSource.outputAudioMixerGroup = data.mixerGroup;

        startTime = Time.time;
        audioSource.Play();

        // Auto-return to pool when finished
        Invoke(nameof(ReturnToPool), clip.length / audioSource.pitch + 0.1f);
    }

    private void ReturnToPool() {
        if (soundManager != null) {
            soundManager.ReturnToPool(this);
        }
        else {
            gameObject.SetActive(false);
        }

        currentData = null;
        soundManager = null;
    }

    // Optional: Manual stop
    public void Stop() {
        audioSource.Stop();
        CancelInvoke(nameof(ReturnToPool));
        ReturnToPool();
    }

    // For debugging / inspector
    public bool IsPlaying => audioSource.isPlaying;
    public SoundData CurrentData => currentData;
}