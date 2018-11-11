using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum SoundName {
    RobotFootsteps
}

[Serializable]
public class Sound {
    public SoundName name;
    public AudioClip audio;
}

public class SoundManager : MonoBehaviour {

    public Sound[] sounds;
    private Dictionary<SoundName, AudioClip> audioTable;

    public AudioSource GlobalSource;

    public static SoundManager instance = null;

    // Use this for initialization
    void Start() {
        instance = this;
        audioTable = new Dictionary<SoundName, AudioClip>();
        foreach (Sound s in sounds) {
            audioTable[s.name] = s.audio;
        }
    }

    public void Play(SoundName sound, AudioSource source = null) {
        if (source == null) {
            source = GlobalSource;
        }
        source.clip = audioTable[sound];
        source.Play();
    }

    // Update is called once per frame
    void Update() {

    }
}
