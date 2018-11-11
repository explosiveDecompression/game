using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MovementController : MonoBehaviour {

    private Transform transform;
    private SphereCollider[] colliders = new SphereCollider[4];

    private enum Direction {
        LEFT, BACK, RIGHT, FORWARD
    }

    // Forward: Z+4
    // Back: Z-4
    // Right: X+4
    // Left: X-4

    // Use this for initialization
    void Start() {
        transform = this.GetComponent<Transform>();
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

    public void TurnLeft() {
        transform.Rotate(Vector3.up, -90);
    }

    public void TurnRight() {
        transform.Rotate(Vector3.up, 90);
    }

    public bool CanMoveForward() {
        bool result = false;
        Vector3 forward = new Vector3(0, 0, 5);
        Vector3 backward = new Vector3(0, 0, -5);
        transform.Translate(forward);
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
        transform.Translate(backward);
        return result;
    }

    // Update is called once per frame
    public void MoveForward() {
        Vector3 forward = new Vector3(0, 0, 5);
        transform.Translate(forward);
        SoundManager.instance.Play(SoundName.RobotFootsteps);
    }
}

