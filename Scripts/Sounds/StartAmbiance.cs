using System.Diagnostics;
using UnityEngine;

public class StartAmbiance : MonoBehaviour {

    [SerializeField] private SoundData ambianceSound; 
    [SerializeField] private SoundData ambianceSound2; 


    public bool Debug = false;
    public bool Debug2 = false;

    private void Update() {
        if (Debug) {
            Debug = false;
            PlayMusic();
        }
        if (Debug2) {
            Debug2 = false;
            PlayMusic2();
        }
    }
    private void Start() {
        SoundManager.Instance.PlayAmbiance(ambianceSound, fadeDuration: 2f);
    }

    private void PlayMusic() {
        SoundManager.Instance.PlayAmbiance(ambianceSound2, fadeDuration: 2f);

    }

    private void PlayMusic2() {
        SoundManager.Instance.PlayAmbiance(ambianceSound, fadeDuration: 2f);

    }

}
