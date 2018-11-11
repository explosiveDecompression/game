using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotWalkingSound : MonoBehaviour
{

    public AudioClip SoundRobotFootsteps;

    public AudioSource RobotFootstepsSource;
    // Use this for initialization
    void Start()
    {
        RobotFootstepsSource.clip = SoundRobotFootsteps;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
            RobotFootstepsSource.Play();
    }
}
