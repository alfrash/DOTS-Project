using UnityEngine;
using UnityEngine.InputSystem;

public class MouseWorldPosition : MonoBehaviour
{
    public static MouseWorldPosition Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public Vector3 GetPosition()
    {
        if (Mouse.current == null) return Vector3.zero;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray mouseCameraRay = Camera.main.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(mouseCameraRay, out RaycastHit hitInfo))
        {
            return hitInfo.point;
        }
        return Vector3.zero;
    }
}
