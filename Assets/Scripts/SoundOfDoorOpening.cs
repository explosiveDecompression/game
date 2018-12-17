using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOfDoorOpening : MonoBehaviour {
    
        public AudioClip DoorSoundsClip;
        public AudioSource DoorSoundsSource;
     
        void Start()
        {
            DoorSoundsSource.clip = DoorSoundsClip;
        }
        
        void Update()
        {
           DoorSoundsSource.Play();
        }
    }