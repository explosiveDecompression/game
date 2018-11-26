using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject player;
    private MovementController movementController;
    public GameObject die;
    public int turnsRemaining;
    public GameObject ui;
    public GameObject tileLightPrefab;

    public bool debugIgnoreMoveCounter;

    private UserDisplay uiController;
    private RollDice diceRoller;
    private DiceValueCalc diceValue;

	// Use this for initialization
	void Start () {
        movementController = player.GetComponent<MovementController>();
        uiController = ui.GetComponent<UserDisplay>();
        diceRoller = die.GetComponent<RollDice>();
        diceValue = die.GetComponent<DiceValueCalc>();
        var floors = GameObject.FindGameObjectsWithTag("Traversable");
        foreach (var floor in floors)
        {
            var light = Instantiate(tileLightPrefab);
            light.transform.parent = floor.transform;
            light.transform.localPosition = new Vector3(0.0f, 9.5f * 2.0f, 0.0f);
            light.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }

    enum TurnPhase
    {
        RollDice, WaitForDice, Movement, WaitForMovement, Finalize
    }

    private TurnPhase currentPhase = TurnPhase.RollDice;


    // Update is called once per frame
    void Update () {
        // Input Management
        bool moveForward = Input.GetKeyDown("up");
        bool turnLeft = Input.GetKeyDown("left");
        bool turnRight = Input.GetKeyDown("right");
        bool diceRoll = Input.GetKeyDown("space");

        switch (currentPhase)
        {
            case TurnPhase.RollDice:
                if (diceRoll)
                {
                    diceRoller.Roll();
                    currentPhase = TurnPhase.WaitForDice;

                }
                break;
            case TurnPhase.WaitForDice:
                if (diceRoller.IsLanded())
                {
                    uiController.currentDiceNumber = diceValue.Value();
                    currentPhase = TurnPhase.Movement;
                }
                break;
            case TurnPhase.Movement:
                if (turnLeft)
                {
                    movementController.TurnLeft();
                }

                if (turnRight)
                {
                    movementController.TurnRight();
                }

                var hasMoves = uiController.currentDiceNumber > 0 || debugIgnoreMoveCounter;
                if (moveForward && movementController.CanMoveForward() && hasMoves)
                {
                    uiController.currentDiceNumber -= 1;
                    movementController.MoveForward();
                }

                hasMoves = uiController.currentDiceNumber > 0 || debugIgnoreMoveCounter;
                if (!hasMoves)
                {
                    currentPhase = TurnPhase.WaitForMovement;
                }
                break;
            case TurnPhase.WaitForMovement:
                if (movementController.IsNotMoving())
                {
                    currentPhase = TurnPhase.Finalize;
                }
                break;
            case TurnPhase.Finalize:
                turnsRemaining -= 1;
                if (turnsRemaining < 1)
                {
                    // TODO: Game Over
                }
                currentPhase = TurnPhase.RollDice;
                break;
            default:
                Debug.Assert("Unhandled default case!" == "");
                break;
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        uiController.remainingTurns = turnsRemaining;
    }
}
