using UnityEngine;

public class Eraser : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float eraseRadius;
    Vector3 lastPosition;

    void OnTriggerStay2D(Collider2D other)
    {
        Vector3 diff = this.transform.position - this.lastPosition;
        this.lastPosition = this.transform.position;
        if (diff.sqrMagnitude < 0.01f)
            return;

        if (other.TryGetComponent<TintErasable>(out var tint))
            tint.EraseAtWorldPosition(transform.position, eraseRadius);
    }
}
