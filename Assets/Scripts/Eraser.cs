using UnityEngine;

public class Eraser : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float eraseRadius;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent<TintErasable>(out var tint))
            tint.EraseAtWorldPosition(transform.position, eraseRadius);
    }
}
