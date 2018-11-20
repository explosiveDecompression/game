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
    private class AudioBundle
    {
        public Dictionary<SoundName, AudioClip> audioTable;
        public AudioSource source;

        public bool IsPlaying()
        {
            return source.isPlaying;
        }

        public void Play(SoundName sound, float volume)
        {
            source.volume = volume;
            source.PlayOneShot(audioTable[sound]);
        }

    }

    private List<AudioBundle> GlobalSources;
    public int maxGlobalSounds;

    public static SoundManager instance = null;

    // Use this for initialization
    void Start() {
        instance = this;
        GlobalSources = new List<AudioBundle>
        {
            NewAudioSource()
        };
    }

    private AudioBundle NewAudioSource()
    {
        var result = new AudioBundle()
        {
            source = gameObject.AddComponent<AudioSource>(),
            audioTable = new Dictionary<SoundName, AudioClip>()
        };
        foreach (Sound s in sounds) {
            result.audioTable[s.name] = Instantiate(s.audio);
        }
        return result;

    }

    public void Play(SoundName sound, float volume = 1f) {
        foreach (AudioBundle b in GlobalSources)
        {
            if (b.IsPlaying()) { continue; }
            b.Play(sound, volume);
            return;
        }

        if (GlobalSources.Count >= maxGlobalSounds)
        {
            Debug.Log("Max Sounds Playing at once, refusing to play: " + sound);
            return;
        }
        var bundle = NewAudioSource();
        GlobalSources.Add(bundle);
        bundle.Play(sound, volume);
    }

    // Update is called once per frame
    void Update() {

    }
}
