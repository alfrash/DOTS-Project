using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Entities;
using Unity.Collections;

public class UnitSelectionManager : MonoBehaviour {

    private void Update() {
        // Check if left mouse button was clicked this frame
        if (Mouse.current.leftButton.wasPressedThisFrame) {
            // Get the position where the mouse clicked in world space
            Vector3 mousePosition = MouseWorldPosition.Instance.GetPosition();
            Debug.Log("Left mouse button was pressed." + mousePosition);

            // Get the EntityManager to access all entities in the ECS world
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
             
            // Create a query to find all entities that have the UnitMover component
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMover>().Build(entityManager);
            
            // Get all Entity objects matching the query (not used in this code, but kept for reference)
            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
             
            // Extract all UnitMover component data from matched entities into a modifiable array
            NativeArray<UnitMover> unitMoversArray = entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);
            
            // Loop through each unit and update its target position to where the mouse was clicked
            for (int i = 0; i < unitMoversArray.Length; i++) {
                // Get a copy of the current unit's data
                UnitMover unitMover = unitMoversArray[i];
                
                // Set the unit's destination to the clicked mouse position
                unitMover.targetPosition = mousePosition;
                
                // Write the updated unit data back into the array
                unitMoversArray[i] = unitMover;
            }
            
            // Copy all modified unit data back to the actual entities in the ECS world
            entityQuery.CopyFromComponentDataArray(unitMoversArray);
        }
    }
}