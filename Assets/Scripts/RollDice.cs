using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollDice : MonoBehaviour {
    private Rigidbody rb;
    public ForceMode force;
    private float torqueForce = 10000f;
    private float movementForce = 150f;
    private MeshRenderer dieModel;
    
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        dieModel = GetComponent<MeshRenderer>();
	}

    // Update is called once per frame
    void Update() {
    }

    public void Roll()
    {
        dieModel.enabled = true;
        Vector3 rand = Random.onUnitSphere * movementForce;
        rand.Set(rand.x, Mathf.Abs(1*rand.y) + 10f, rand.z);
        rb.AddForce(rand);
        rb.AddTorque(Random.onUnitSphere * torqueForce);
    }

    public bool IsLanded()
    {
        if(rb.IsSleeping())
            dieModel.enabled = false;
        return rb.IsSleeping();
    }
}
