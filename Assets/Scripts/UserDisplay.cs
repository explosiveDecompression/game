using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserDisplay : MonoBehaviour {
    public Text RemainingTurnsValue;
    public Text MoveCounterValue;
    public int currentDiceNumber = 0;
    public int remainingTurns = 0;
    
	// Use this for initialization
	void Start () {
    }

    // Update is called once per frame
    void Update() {
        MoveCounterValue.text = currentDiceNumber.ToString();
        RemainingTurnsValue.text = remainingTurns.ToString();
	}
}
