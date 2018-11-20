using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceValueCalc : MonoBehaviour {
    public Vector3[] diceResults;
    public SideResults sideFacingUp;
    public int faceIndex;
    public SideResults[] faceVector;
    public Vector3[] vectorPoints;

	// Use this for initialization
	void Start () {
		
	}

    public int Value()
    {
        return (int)sideFacingUp;
    }
	
	// Update is called once per frame
	void Update () {
        float pointingUp = -1;
        for(int i = 0; i < vectorPoints.Length; ++i)
        {
            var valueVector = vectorPoints[i];
            var worldSpaceVector = this.transform.localToWorldMatrix.MultiplyVector(valueVector);
            float dot = Vector3.Dot(worldSpaceVector, Vector3.up);
            if(dot > pointingUp)
            {
                pointingUp = dot;
                faceIndex = i;
            }
        }
        sideFacingUp = faceVector[faceIndex];
        

		
	}

}
public enum SideResults : int
{
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6
    };
