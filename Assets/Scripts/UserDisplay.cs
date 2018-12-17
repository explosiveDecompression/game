using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserDisplay : MonoBehaviour {
    public Text RemainingTurnsValue;
    public Text MoveCounterValue;
    public Text Objectives;
    public Text StatusReportValue;
    public int currentDiceNumber = 0;
    public int remainingTurns = 0;
    private TurnPhase tp;

    // Use this for initialization
    void Start() {
    }

    private class Objective
    {
        public Milestone milestone;
        public string objectiveText;

        public Objective(Milestone m, string o)
        {
            milestone = m;
            objectiveText = o;
        }

    }

    Objective[] objectives = new Objective[]
    {
        new Objective(Milestone.None, "Find the Control Panel"),
        new Objective(Milestone.ActivatedPanel, "Find the Teleporter"),
        new Objective(Milestone.ReachedTeleporter, "Successfully Escaped!"),
    };

    // Update is called once per frame
    void Update() {
        MoveCounterValue.text = currentDiceNumber.ToString();
        RemainingTurnsValue.text = remainingTurns.ToString();
        foreach (var objective in objectives)
        {
            if (GameManager.instance.IsMilestoneComplete(objective.milestone))
            {
                Objectives.text = objective.objectiveText;
            }
        }

        tp = GameManager.instance.CurrentPhase();
        MoveCounterValue.text = currentDiceNumber.ToString();
        RemainingTurnsValue.text = remainingTurns.ToString();
        StatusReportValue.text = DisplayStatus(tp);
    }

    string DisplayStatus(TurnPhase currPhase)
    {
        string displayString = "";
        if (currPhase == TurnPhase.RollDice)
            displayString = "Press Space to Roll Dice";
        else if (currPhase == TurnPhase.Movement)
            displayString = "Player Controls (Arrows):" + "\n" + "Up - Move Forward" + "\n" +
               "Left - Turn Left" + "\n" + "Right - Turn Right";

        return displayString;
    }
    
}
