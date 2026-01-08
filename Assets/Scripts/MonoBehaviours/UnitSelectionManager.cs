using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Entities;
using Unity.Collections;
using System;
using Unity.Transforms;
using Unity.Physics;

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
        // Right mouse button pressed - start selection
        if (Mouse.current.rightButton.wasPressedThisFrame) {
            selectionStartMousePosition = Mouse.current.position.ReadValue();
            OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);
        }
        // Right mouse button released - finalize selection
        if (Mouse.current.rightButton.wasReleasedThisFrame) {
            Vector2 selectionEndMousePosition = Mouse.current.position.ReadValue();

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            // First, disable all Selected components
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<Selected>().Build(entityManager);
            // Get all entities with Selected component
            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            // Disable all Selected components
            for (int i = 0; i < entityArray.Length; i++) {
                entityManager.SetComponentEnabled<Selected>(entityArray[i], false);
            }
            // Determine selection area rectangle
            Rect selectionAreaRect = GetSelectionAreaRect();
            float selectionAreaSize = selectionAreaRect.width + selectionAreaRect.height;
            float selectionSizeMin = 40f;
            bool isMultiSelect = selectionAreaSize > selectionSizeMin;
            Debug.Log($"Selection Area Size: {selectionAreaSize}, Is Multi-Select: {isMultiSelect}");

            if (isMultiSelect) {
                // Next, enable Selected components for units within selection area
                entityQuery = new EntityQueryBuilder(Allocator.Temp)
                    .WithAll<LocalTransform, Unit>().WithPresent<Selected>().Build(entityManager);
                // Get all entities with LocalTransform and Unit components and Selected component 
                entityArray = entityQuery.ToEntityArray(Allocator.Temp);
                // Get LocalTransform components of all units
                NativeArray<LocalTransform> localTransformArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
                // Check each unit's position against selection area
                for (int i = 0; i < localTransformArray.Length; i++) {
                    LocalTransform unitLocalTransform = localTransformArray[i];
                    // Convert unit world position to screen position
                    Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(unitLocalTransform.Position);
                    // Check if unit screen position is within selection area
                    if (selectionAreaRect.Contains(unitScreenPosition)) {
                        entityManager.SetComponentEnabled<Selected>(entityArray[i], true);
                    }
                }
            }else {
                // Single select - enable Selected component for unit under mouse cursor
                entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
                PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
                CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                UnityEngine.Ray mouseCameraRay = Camera.main.ScreenPointToRay(mousePosition);
                int unitLayerMask = 6; // Assuming units are on layer 6
                RaycastInput raycastInput = new RaycastInput {
                    Start = mouseCameraRay.origin,
                    End = mouseCameraRay.GetPoint(9999f),
                    Filter = new CollisionFilter {
                        BelongsTo = ~0u,
                        CollidesWith = 1u << unitLayerMask,
                        GroupIndex = 0
                    },
                };
                if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit raycastHit)) {
                    if(entityManager.HasComponent<Unit>(raycastHit.Entity)) {
                        // Enable Selected component for the unit hit by the raycast
                        entityManager.SetComponentEnabled<Selected>(raycastHit.Entity, true);
                    }
                }

            }

            OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);
        }

        if (Mouse.current.leftButton.wasPressedThisFrame) {
            Vector3 mousePosition = MouseWorldPosition.Instance.GetPosition();

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            // Query for all entities with UnitMover and Selected components
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<UnitMover, Selected>().Build(entityManager);
            // Get all entities with Selected component
            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            // Get UnitMover components of all selected units
            NativeArray<UnitMover> unitMoversArray = entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);
            // Update targetPosition of each selected unit to mouse position
            for (int i = 0; i < unitMoversArray.Length; i++) {
                UnitMover unitMover = unitMoversArray[i];
                unitMover.targetPosition = mousePosition;
                unitMoversArray[i] = unitMover;
            }
            // Apply the modified UnitMover components back to the entities
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