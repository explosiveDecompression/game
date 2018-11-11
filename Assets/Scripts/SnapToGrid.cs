using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SnapToGrid : MonoBehaviour
{
#if UNITY_EDITOR
    public bool snapToGrid = true;
    public float snapValue = 5.0f;

    public Vector3 offset = new Vector3(0, 0, 0);

    public bool lockSize = true;
    public Vector3 sizeValue = new Vector3(5.0f, 0.5f, 5.0f);

    public void Update() {
        if (snapToGrid) {
            transform.position = RoundTransform(transform.position, snapValue);
        }
        if (lockSize) {
            transform.localScale = sizeValue;
        }
    }

    private Vector3 RoundTransform(Vector3 v, float snapValue) {
        return new Vector3(
            snapValue * Mathf.Round(v.x / snapValue),
            0.0f,
            snapValue * Mathf.Round(v.z / snapValue)
        ) + offset;
    }

#endif
}
