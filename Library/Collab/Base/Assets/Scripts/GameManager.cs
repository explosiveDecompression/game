using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

/// <summary>
/// Each flag specifies a particular phase of the game
/// </summary>
public enum Milestone : int
{
    /// <summary>
    /// No milestones yet achieved
    /// </summary>
    None,
    /// <summary>
    /// The player has reached and activated the control panel to enable the teleporter
    /// </summary>
    ActivatedPanel,
    /// <summary>
    /// The player has reached the teleporter and is beaming out
    /// </summary>
    ReachedTeleporter,
    /// <summary>
    /// The player has won
    /// </summary>
    Victory,
    Count,
}

public enum TurnPhase
{
    /// <summary>
    /// The player needs to roll the die to start their turn
    /// </summary>
    RollDice,
    /// <summary>
    /// The die is rolling
    /// </summary>
    WaitForDice,
    /// <summary>
    /// The player is inputting their movement commands
    /// </summary>
    Movement,
    /// <summary>
    /// The player has finished input, but the character is not to the final movement location
    /// </summary>
    WaitForMovement,
    /// <summary>
    /// The turn is over and the world state is being updated
    /// </summary>
    Finalize,
    Count,
}

public class GameManager : MonoBehaviour {
    static public GameManager instance;

    public GameObject player;
    private MovementController movementController;
    public GameObject die;
    public int turnsRemaining;
    public GameObject ui;

    // Prefabs
    public GameObject tileLightPrefab;
    public GameObject panelPrefab;
    public GameObject teleporterPrefab;
    public GameObject objectivePointerPrefab;
    public GameObject pointerFloorSystemPrefab;

    public bool debugIgnoreMoveCounter;

    private UserDisplay uiController;
    private RollDice diceRoller;
    private DiceValueCalc diceValue;
    private GameObject panelFloor;
    private GameObject escapeFloor;
    private GameObject teleporter;
    private ParticleSystem teleporterParticles;
    private GameObject pointer;
    private ParticleSystem pointerParticles;
    private Transform playerSpine;

    private List<GameObject> pointerFloors = new List<GameObject>();

    private Milestone lastMilestoneAchieved = Milestone.None;

	// Use this for initialization
	void Start () {
        GameManager.instance = this;
        movementController = player.GetComponent<MovementController>();
        uiController = ui.GetComponent<UserDisplay>();
        diceRoller = die.GetComponent<RollDice>();
        diceValue = die.GetComponent<DiceValueCalc>();
        var floors = GameObject.FindGameObjectsWithTag("Traversable");
        panelFloor = floors[UnityEngine.Random.Range(0, floors.Length)];


        var panel = Instantiate(panelPrefab);
        panel.transform.localPosition = panelFloor.transform.position + new Vector3(0f, 0f, 1.4f);
        panel.transform.localScale = new Vector3(1.0f, 0.75f, 1.0f);

        // Find floors far enough away from the panel floor
        var maximumDistance = 0.0f;
        foreach (var floor in floors)
        {
            foreach (var other in floors)
            {
                var thisDistance = (other.transform.position - floor.transform.position).sqrMagnitude;
                if (maximumDistance < thisDistance)
                {
                    maximumDistance = thisDistance;
                }
            }
        }

        maximumDistance = Mathf.Sqrt(maximumDistance) / 2.0f;

        // Try to find a suitable escape tile, but give up after a certain amount of tries
        var maxAttempts = 100;
        while (escapeFloor == null && maxAttempts >= 0)
        {
            escapeFloor = floors[UnityEngine.Random.Range(0, floors.Length)];
            if (maxAttempts != 0 && (escapeFloor.transform.position - panelFloor.transform.position).magnitude < maximumDistance)
            {
                escapeFloor = null;
            }
            maxAttempts -= 1;
        }

        foreach (var floor in floors)
        {
            var light = Instantiate(tileLightPrefab);
            var lightComponent = light.GetComponent<Light>();
            light.transform.parent = floor.transform;
            light.transform.localPosition = new Vector3(0.0f, 9.5f * 2.0f, 0.0f);
            light.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            if (floor == panelFloor)
            {
                var iController = light.GetComponent<RandomIntensity>();
                iController.minimumIntensity = 5.0f;
                iController.maximumIntensity = 5.0f;
                lightComponent.color = new Color(1.0f, 0.5f, 0.5f);
            } else if (floor == escapeFloor) {
                // Do nothing for now
            } else if (UnityEngine.Random.Range(0.0f, 100.0f) > 95.0f)
            {
                // Make a pointer floor
                var randIntensity = light.GetComponent<RandomIntensity>();
                randIntensity.enabled = false;
                lightComponent.color = new Color(1.0f, 1.0f, 1.0f);
                var particles = Instantiate(pointerFloorSystemPrefab);
                particles.transform.parent = floor.transform;
                particles.transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
                pointerFloors.Add(floor);
            }
        }
        pointer = Instantiate(objectivePointerPrefab);
        pointerParticles = pointer.GetComponent<ParticleSystem>();
        playerSpine = player.transform.Find("Pelvis/Spine1");
        var pPos = panelFloor.transform.position;
        pointer.transform.position = new Vector3(pPos.x, playerSpine.position.y, pPos.z);
    }


    public void EnableTeleporter()
    {
        var iController = escapeFloor.GetComponentInChildren<RandomIntensity>();
        iController.minimumIntensity = 5.0f;
        iController.maximumIntensity = 5.0f;
        var lightComponent = escapeFloor.GetComponentInChildren<Light>();
        lightComponent.color = new Color(0.4f, 1.0f, 0.4f);
        teleporter = Instantiate(teleporterPrefab);
        teleporter.transform.parent = escapeFloor.transform;
        teleporter.transform.localPosition = new Vector3(0.0f, 0.25f, 0.0f);
        teleporter.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        teleporterParticles = teleporter.GetComponentInChildren<ParticleSystem>();
    }

    private bool animateTeleporter = false;
    private TurnPhase currentPhase = TurnPhase.RollDice;
    public TurnPhase CurrentPhase()
    {
        return currentPhase;
    }

    private struct TurnActions
    {
        public bool moveForward;
        public bool turnLeft;
        public bool turnRight;
        public bool actionButton;
    }

    void HandleTurn(TurnActions actions)
    {
        switch (currentPhase)
        {
            case TurnPhase.RollDice:
                if (actions.actionButton)
                {
                    diceRoller.Roll();
                    currentPhase = TurnPhase.WaitForDice;
                    SetPointerEnabled(false);
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
                if (actions.turnLeft)
                {
                    movementController.TurnLeft();
                }

                if (actions.turnRight)
                {
                    movementController.TurnRight();
                }


                var hasMoves = uiController.currentDiceNumber > 0 || debugIgnoreMoveCounter;
                if (actions.moveForward && movementController.CanMoveForward() && hasMoves)
                {
                    uiController.currentDiceNumber -= 1;
                    movementController.MoveForward();
                }

                if (actions.actionButton)
                {
                    uiController.currentDiceNumber = 0;
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
                var currentFloor = movementController.GetFloor();
                turnsRemaining -= 1;
                if (turnsRemaining < 1)
                {
                    // TODO: Game Over
                }
                if (movementController.GetFloor() == escapeFloor &&
                    IsMilestoneComplete(Milestone.ActivatedPanel))
                {
                    // Note: Win! There is a teleporter animation to play before receiving the win screen
                    CompleteMilestone(Milestone.ReachedTeleporter);
                }
                if (currentFloor == panelFloor)
                {
                    CompleteMilestone(Milestone.ActivatedPanel);
                    panelFloor.GetComponentInChildren<Light>().color = new Color(0.5f, 0.7f, 1.0f);
                    EnableTeleporter();
                } else if (pointerFloors.IndexOf(currentFloor) > -1)
                {
                    switch (lastMilestoneAchieved)
                    {
                        case Milestone.ActivatedPanel:
                            RevealLocation(escapeFloor);
                            break;
                        default:
                            RevealLocation(panelFloor);
                            break;
                    }
                }
                currentPhase = TurnPhase.RollDice;
                break;
            default:
                Debug.Assert("Unhandled default case!" == "");
                break;
        }
    }

    void SetPointerEnabled(bool enabled)
    {
        var emi = pointerParticles.emission;
        emi.enabled = enabled;
    }


    void RevealLocation(GameObject go) {
        pointer.transform.position = new Vector3(
            go.transform.position.x,
            playerSpine.transform.position.y,
            go.transform.position.z);
        pointer.transform.LookAt(playerSpine);
        SetPointerEnabled(true);
    }

    // Update is called once per frame
    void Update () {
        TurnActions actions = new TurnActions
        {
            moveForward = Input.GetKeyDown("up"),
            turnLeft = Input.GetKeyDown("left"),
            turnRight = Input.GetKeyDown("right"),
            actionButton = Input.GetKeyDown("space"),
        };

        if (IsMilestoneComplete(Milestone.ReachedTeleporter))
        {
            AnimateTeleporter();
        } else
        {
            HandleTurn(actions);
        }


        UpdateUI();
    }

    public void CompleteMilestone(Milestone flag)
    {
        lastMilestoneAchieved = flag;
    }

    public bool IsMilestoneComplete(Milestone flag)
    {
        return lastMilestoneAchieved >= flag;
    }

    public Milestone LastMilestone()
    {
        return lastMilestoneAchieved;
    }

    void AnimateTeleporter()
    {
        var maxSimSpeed = 10f;
        var emission = teleporterParticles.emission;
        emission.rateOverTimeMultiplier += 20 * Time.deltaTime;
        emission.rateOverTimeMultiplier = Mathf.Min(emission.rateOverTimeMultiplier, 100f);
        var main = teleporterParticles.main;
        main.simulationSpeed += 1.1f * Time.deltaTime;
        main.simulationSpeed = Mathf.Min(main.simulationSpeed, maxSimSpeed);
        if (main.simulationSpeed == maxSimSpeed)
        {
            Victory();
        }

    }

    void Victory()
    {
        SceneManager.LoadScene(2);
    }

    void UpdateUI()
    {
        uiController.remainingTurns = turnsRemaining;
    }
}
