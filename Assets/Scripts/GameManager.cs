using UnityEngine;
using UnityEngine.SceneManagement;

public enum ProgressFlag : int
{
    ActivatedPanel,
    ReachedEscapePod,
    Count
}

public enum TurnPhase
{
    RollDice, WaitForDice, Movement, WaitForMovement, Finalize
}

public class GameManager : MonoBehaviour {
    static public GameManager instance;

    public GameObject player;
    private MovementController movementController;
    public GameObject die;
    public int turnsRemaining;
    public GameObject ui;
    public GameObject tileLightPrefab;
    public GameObject panelPrefab;
    public GameObject teleporterPrefab;

    public bool debugIgnoreMoveCounter;

    private UserDisplay uiController;
    private RollDice diceRoller;
    private DiceValueCalc diceValue;
    private GameObject panelFloor;
    private GameObject escapeFloor;
    private GameObject teleporter;
    private ParticleSystem teleporterParticles;

    private bool[] gameProgressFlags = new bool[(int)ProgressFlag.Count];

	// Use this for initialization
	void Start () {
        GameManager.instance = this;
        movementController = player.GetComponent<MovementController>();
        uiController = ui.GetComponent<UserDisplay>();
        diceRoller = die.GetComponent<RollDice>();
        diceValue = die.GetComponent<DiceValueCalc>();
        var floors = GameObject.FindGameObjectsWithTag("Traversable");
        panelFloor = floors[Random.Range(0, floors.Length)];


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
            escapeFloor = floors[Random.Range(0, floors.Length)];
            if (maxAttempts != 0 && (escapeFloor.transform.position - panelFloor.transform.position).magnitude < maximumDistance)
            {
                escapeFloor = null;
            }
            maxAttempts -= 1;
        }

        foreach (var floor in floors)
        {
            var light = Instantiate(tileLightPrefab);
            light.transform.parent = floor.transform;
            light.transform.localPosition = new Vector3(0.0f, 9.5f * 2.0f, 0.0f);
            light.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            if (floor == panelFloor)
            {
                var iController = light.GetComponent<RandomIntensity>();
                iController.minimumIntensity = 5.0f;
                iController.maximumIntensity = 5.0f;
                var lightComponent = light.GetComponent<Light>();
                lightComponent.color = new Color(1.0f, 0.5f, 0.5f);
            }
        }
    }


    public void ActivateTeleporter()
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


    // Update is called once per frame
    void Update () {
        // Input Management
        bool moveForward = Input.GetKeyDown("up");
        bool turnLeft = Input.GetKeyDown("left");
        bool turnRight = Input.GetKeyDown("right");
        bool actionButton = Input.GetKeyDown("space");

        switch (currentPhase)
        {
            case TurnPhase.RollDice:
                if (actionButton)
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

                if (actionButton)
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
                turnsRemaining -= 1;
                if (turnsRemaining < 1)
                {
                    // TODO: Game Over
                }
                if (movementController.GetFloor() == escapeFloor && gameProgressFlags[(int)ProgressFlag.ActivatedPanel])
                {
                    // Note: Win! There is a teleporter animation to play before receiving the win screen
                    animateTeleporter = true;
                }
                if (movementController.GetFloor() == panelFloor)
                {
                    gameProgressFlags[(int)ProgressFlag.ActivatedPanel] = true;
                    panelFloor.GetComponentInChildren<Light>().color = new Color(0.5f, 0.7f, 1.0f);
                    ActivateTeleporter();
                }
                currentPhase = TurnPhase.RollDice;
                break;
            default:
                Debug.Assert("Unhandled default case!" == "");
                break;
        }
        if (animateTeleporter)
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
        UpdateUI();
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
