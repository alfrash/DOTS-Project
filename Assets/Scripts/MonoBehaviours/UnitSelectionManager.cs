using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Entities;
using Unity.Collections;

public class UnitSelectionManager : MonoBehaviour {

    private void Update() {
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
}