using UnityEngine;

public class Eraser : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent<TintErasable>(out var tint))
            tint.EraseAtWorldPosition(transform.position);
    }
}
