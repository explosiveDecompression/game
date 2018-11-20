using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserDisplay : MonoBehaviour {
    private float turn = 1f;
    public Text tc;
    public Text timer;
    private float count = 120f;
    public int currentDiceNumber = 0;
    private DiceValueCalc dvc;
    public GameObject tempDice;
    
    
	// Use this for initialization
	void Start () {
        tc.text = "";
        timer.text = count.ToString();
        
        

    }

    // Update is called once per frame
    void Update() {
        // dvc = tempDice.GetComponent<DiceValueCalc>();
        // tc.text =((int) (dvc.sideFacingUp)).ToString();

        tc.text = currentDiceNumber.ToString();
        

        if (((int)count - Time.time) > 0)
            timer.text = ((int)(count - Time.time)).ToString();
	}
}
