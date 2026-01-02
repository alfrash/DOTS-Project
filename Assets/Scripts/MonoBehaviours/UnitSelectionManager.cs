using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Entities;
using Unity.Collections;
using System;

public class UnitSelectionManager : MonoBehaviour {

    public event EventHandler OnSelectionAreaStart;
    public event EventHandler OnSelectionAreaEnd;
    private Vector2 selectionStartMousePosition;

    public static UnitSelectionManager Instance { get; private set; }
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Update() {
        if (Mouse.current.rightButton.wasPressedThisFrame) {
            selectionStartMousePosition = Mouse.current.position.ReadValue();
            Debug.Log("Selection Start Mouse Position: " + selectionStartMousePosition);
            OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);
        }
        if (Mouse.current.rightButton.wasReleasedThisFrame) {
            Vector2 selectionEndMousePosition = Mouse.current.position.ReadValue();
            Debug.Log("Selection End Mouse Position: " + selectionEndMousePosition);
            OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);
        }

        if (Mouse.current.leftButton.wasPressedThisFrame) {
            Vector3 mousePosition = MouseWorldPosition.Instance.GetPosition();

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<UnitMover, Selected>().Build(entityManager);

            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);

            NativeArray<UnitMover> unitMoversArray = entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);

            for (int i = 0; i < unitMoversArray.Length; i++) {
                UnitMover unitMover = unitMoversArray[i];
                unitMover.targetPosition = mousePosition;
                unitMoversArray[i] = unitMover;
            }
            entityQuery.CopyFromComponentDataArray(unitMoversArray);
        }
    }

    public Rect GetSelectionAreaRect() {
        Vector2 selectionEndMousePosition = Mouse.current.position.ReadValue();
        Vector2 lowerLeft = new Vector2(
            Mathf.Min(selectionStartMousePosition.x, selectionEndMousePosition.x),
            Mathf.Min(selectionStartMousePosition.y, selectionEndMousePosition.y)
        );
        Vector2 upperRight = new Vector2(
            Mathf.Max(selectionStartMousePosition.x, selectionEndMousePosition.x),
            Mathf.Max(selectionStartMousePosition.y, selectionEndMousePosition.y)
        );
        return new Rect(lowerLeft, upperRight - lowerLeft);
    }
}