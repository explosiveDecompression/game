using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    public SoundName doorOpenSound;
    public SoundName doorCloseSound;
    Animator animator;
    bool doorOpen = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            doorOpen = true;
            DoorControl("Open");
            SoundManager.instance.Play(doorOpenSound, 0.4f);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (doorOpen)
        {
            doorOpen = false;
            DoorControl("Close");
            SoundManager.instance.Play(doorCloseSound, 0.1f);
        }
    }


    void DoorControl(string direction)
    {
        animator.SetTrigger(direction);
    }
}

	
