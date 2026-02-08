using UnityEngine;
using UnityEngine.InputSystem;

public class MouseInput : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private InputActionReference pointInputAction;

    private bool isActionValid => pointInputAction != null && pointInputAction.action != null;
    Camera cam;

    void Awake()
    {
        cam = Camera.main;

        if (isActionValid)
            pointInputAction.action.Enable();
    }

    void Update()
    {
        if (!isActionValid)
            return;

        Vector2 screenPos = pointInputAction.action.ReadValue<Vector2>();
        Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
        worldPos.z = 0;

        transform.position = Vector3.Lerp(
            transform.position,
            worldPos,
            mouseSensitivity * Time.deltaTime
        );
    }
}
