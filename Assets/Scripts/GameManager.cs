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
    private RollDice diceRoller;
    private DiceValueCalc diceValue;

	// Use this for initialization
	void Start () {
        movementController = player.GetComponent<MovementController>();
        uiController = ui.GetComponent<UserDisplay>();
        diceRoller = die.GetComponent<RollDice>();
        diceValue = die.GetComponent<DiceValueCalc>();
    }

    private bool waitingForLanded = false;

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

        if (waitingForLanded && diceRoller.IsLanded())
        {

            waitingForLanded = false;
            uiController.currentDiceNumber = diceValue.Value();
        }

        if (diceRoll) {
            diceRoller.Roll();
            waitingForLanded = true;
        }

    }
}
