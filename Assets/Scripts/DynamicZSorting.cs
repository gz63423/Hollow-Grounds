using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DynamicZSorter : MonoBehaviour
{
    [Header("Optional Z Offset")]
    [Tooltip("Adjust to fine-tune render order. Positive moves object slightly behind, negative slightly in front.")]
    public float zOffset = 0f;

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        // Lower Y = visually in front
        // Higher Y = visually behind
        Vector3 pos = transform.position;
        pos.z = pos.y + zOffset; // Z position controls render order
        transform.position = pos;
    }
}
