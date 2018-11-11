using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject player;
    private MovementController movementController;
    public GameObject die;
    public int turnsAllowed;
    public GameObject ui;
    private UserDisplay uiController;

	// Use this for initialization
	void Start () {
        movementController = player.GetComponent<MovementController>();
        uiController = ui.GetComponent<UserDisplay>();
	}
	
	// Update is called once per frame
	void Update () {
        // Input Management
        bool moveForward = Input.GetKeyDown("up");
        bool turnLeft = Input.GetKeyDown("left");
        bool turnRight = Input.GetKeyDown("right");
        bool diceRoll = Input.GetKeyDown("space");

        if (turnLeft) {
            movementController.TurnLeft();
        } else if (turnRight) {
            movementController.TurnRight();
        }

        if (moveForward && movementController.CanMoveForward() &&
            uiController.currentDiceNumber > 0
           ) {
            uiController.currentDiceNumber -= 1;
            movementController.MoveForward();
        }

        if (diceRoll) {
            uiController.currentDiceNumber = Random.Range(1, 7);
        }
    }
}
