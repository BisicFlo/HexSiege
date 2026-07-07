using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SoundData", menuName = "Scriptable Objects/SoundData")]
public class SoundData : ScriptableObject {

    public AudioClip[] clips;           // Support variations 
    public AudioMixerGroup mixerGroup;
    public Vector2 volumeRange = new Vector2(0.8f, 1f);
    public Vector2 pitchRange = new Vector2(0.95f, 1.05f);
    public bool isFrequent = false;     // For pooling limits (hits, footsteps)
    public int maxInstances = 8;        // Throttle spammy sounds
}
