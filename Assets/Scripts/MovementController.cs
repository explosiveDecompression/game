using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MovementController : MonoBehaviour {

    private SphereCollider[] colliders = new SphereCollider[4];
    private Animator animator;

    private enum Direction {
        LEFT, BACK, RIGHT, FORWARD
    }


    // Use this for initialization
    void Start() {
        animator = this.GetComponent<Animator>();
        animator.Play("Idle");
        SphereCollider[] colliderSet = this.GetComponents<SphereCollider>();

        for (int i = 0; i < colliderSet.Length; i++) {
            Vector3 position = colliderSet[i].center;
            int x = Convert.ToInt32(position.x);
            int z = Convert.ToInt32(position.z);
            if (x == 4) {
                colliders[(int)Direction.LEFT] = colliderSet[i];
            } else if (x == -4) {
                colliders[(int)Direction.RIGHT] = colliderSet[i];
            } else if (z == 4) {
                colliders[(int)Direction.FORWARD] = colliderSet[i];
            } else if (z == -4) {
                colliders[(int)Direction.BACK] = colliderSet[i];
            }
        }
    }

    public float tilesPerSecond;
    public float tileSize;
    public float tilesPerMove;
    public float spinDegreesPerSecond;

    enum Move
    {
        Forward, Right, Left, Back, None
    }

    private Queue<Move> moveQueue = new Queue<Move>();
    private Move currentMove = Move.None;

    private float percentComplete;

    public GameObject GetFloor()
    {
        var gameObjects =
            Physics.OverlapSphere(transform.position, 1)
                   .Except<Collider>(GetComponents<Collider>())
                   .Select((c) => c.gameObject)
                   .ToArray();

        foreach (GameObject go in gameObjects) {
            if (go.tag == "Traversable") {
                return go;
            }
        }
        return null;
    }

    void Update()
    {
        if (currentMove == Move.None && moveQueue.Count > 0)
        {
            currentMove = moveQueue.Peek();
            percentComplete = -1;
        }

        switch (currentMove)
        {
            case Move.Forward:
                if (percentComplete < 0)
                {
                    animator.Play("Run");
                    percentComplete = 0.0f;
                }
                if (percentComplete < 1.0f)
                {
                    float thisFramePercent = (tilesPerMove / tilesPerSecond) * Time.deltaTime;
                    float amountThisFrame = tileSize * thisFramePercent;
                    percentComplete += thisFramePercent;
                    transform.Translate(new Vector3(0, 0, amountThisFrame));
                    // Debug.Log(forward);
                }
                break;
            case Move.Left:
                if (percentComplete < 0)
                {
                    animator.Play("Left");
                    percentComplete = 0.0f;
                }
                if (percentComplete < 1.0f)
                {
                    float thisFramePercent = (spinDegreesPerSecond / 90.0f) * Time.deltaTime;
                    float amountThisFrame = -90.0f * thisFramePercent;
                    transform.Rotate(Vector3.up, amountThisFrame);
                    percentComplete += thisFramePercent;
                    // Debug.Log(forward);
                }
                break;
            case Move.Right:
                if (percentComplete < 0)
                {
                    animator.Play("Right");
                    percentComplete = 0.0f;
                }
                if (percentComplete < 1.0f)
                {
                    float thisFramePercent = (spinDegreesPerSecond / 90.0f) * Time.deltaTime;
                    float amountThisFrame = 90.0f * thisFramePercent;
                    transform.Rotate(Vector3.up, amountThisFrame);
                    percentComplete += thisFramePercent;
                    // Debug.Log(forward);
                }
                break;
        }

        if (percentComplete >= 1.0f)
        {
            Vector3 rot = transform.localRotation.eulerAngles;
            rot.y = Snap(rot.y, 90.0f);
            transform.localRotation = Quaternion.Euler(rot);

            Vector3 pos = transform.position;
            pos.x = Snap(pos.x, 5.0f);
            pos.z = Snap(pos.z, 5.0f);
            transform.position = pos;

            currentMove = Move.None;
            moveQueue.Dequeue();
            animator.Play("Idle");
            percentComplete = -1f;
        }
    }

    private float Snap(float value, float snappingInterval)
    {
        return snappingInterval * Mathf.Round(value / snappingInterval);

    }

    public void TurnLeft()
    {
        moveQueue.Enqueue(Move.Left);
        Debug.Log("Added Turn Left Command");
    }

    public void TurnRight()
    {
        moveQueue.Enqueue(Move.Right);
        Debug.Log("Added Turn Right Command");
    }


    public bool IsNotMoving()
    {
        return moveQueue.Count == 0;
    }

    public bool CanMoveForward()
    {
        bool result = false;

        var moveList = moveQueue.ToList();

        Vector3 finalLocation = new Vector3();
        // This keeps all 4 directions in compass order such that turning
        // right increments the index by +1 and turning left by -1
        Vector3[] directions = new Vector3[] {
            new Vector3(0, 0, 5), // forward
            new Vector3(5, 0, 0), // right
            new Vector3(0, 0, -5), // back
            new Vector3(-5, 0, 0), // left
        };
        int currentDirectionIndex = 0;

        foreach (var move in moveList)
        {
            switch (move)
            {
                case Move.Forward:
                    finalLocation += directions[currentDirectionIndex];
                    break;
                case Move.Left:
                    currentDirectionIndex -= 1;
                    break;
                case Move.Right:
                    currentDirectionIndex += 1;
                    break;
                case Move.Back:
                    currentDirectionIndex += 2;
                    break;
            }

            currentDirectionIndex = currentDirectionIndex % directions.Length;
            if (currentDirectionIndex < 0)
            {
                currentDirectionIndex += directions.Length;
            }
            // direction index must always be positive and less than the number of directions we have.
            Debug.Assert(currentDirectionIndex < directions.Length && currentDirectionIndex >= 0);
        }
        finalLocation += directions[currentDirectionIndex];
        transform.Translate(finalLocation);
        var gameObjects =
            Physics.OverlapSphere(transform.position, 1)
                   .Except<Collider>(GetComponents<Collider>())
                   .Select((c) => c.gameObject)
                   .ToArray();
        foreach (GameObject go in gameObjects) {
            if (go.tag == "Traversable") {
                result = true;
                break;
            }
        }
        transform.Translate(finalLocation * -1);
        return result;
    }

    // Update is called once per frame
    public void MoveForward()
    {
        moveQueue.Enqueue(Move.Forward);
        Debug.Log("Added Move Forward Command");
    }
}

