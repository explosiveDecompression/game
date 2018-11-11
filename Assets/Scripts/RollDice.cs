using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollDice : MonoBehaviour {
    private Rigidbody rb;
    public ForceMode force;
    private float torqueForce = 50f;
    private float movementForce = 250f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        bool moveDice = Input.GetKeyDown("d");
        rb = GetComponent<Rigidbody>();
        if (moveDice)
        {
            rb.AddForce(Random.onUnitSphere * movementForce);
            rb.AddTorque(Random.onUnitSphere * torqueForce, force);
            
        }
		
	}
}
