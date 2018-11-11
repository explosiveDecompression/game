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
    
	// Use this for initialization
	void Start () {
        tc.text = "";
        timer.text = count.ToString();
        
        

    }

    // Update is called once per frame
    void Update() {

        bool diceRoll = Input.GetKeyDown("space");
        if (!tc.text.Equals(currentDiceNumber.ToString())) {
            tc.text = currentDiceNumber.ToString();
        }

        if(((int)count - Time.time) > 0)
            timer.text = ((int)(count - Time.time)).ToString();
	}
}
