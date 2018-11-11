using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    public Transform player;

    public float transitionTime;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
    void Update () {

        var transitionSpeed = 1.0f / transitionTime;

        Vector3 behindPlayer = new Vector3(
            player.position.x,
            transform.position.y,
            player.position.z
        ) - (player.forward*4);

        Quaternion rot = Quaternion.Euler(
            transform.rotation.eulerAngles.x,
            player.rotation.eulerAngles.y,
            player.rotation.eulerAngles.z
        );

        transform.rotation = Quaternion.Slerp(transform.rotation,
                                              rot,
                                              Time.deltaTime * transitionSpeed);
        transform.position = Vector3.Lerp(transform.position,
                                          behindPlayer,
                                          Time.deltaTime * transitionSpeed);
	}
}
