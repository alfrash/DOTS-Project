using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using UnityEngine;

partial struct TestingSystem : ISystem {

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        /*
        int unitCount = 0;
        foreach ((
            RefRW<LocalTransform> localTransform,
            RefRO<UnitMover> unitMover,
            RefRW<PhysicsVelocity> physicsVelocity,
            RefRO<Selected> selected
            )
            in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRO<UnitMover>,
                RefRW<PhysicsVelocity>,
                RefRO<Selected>
                >()
            ) {
            unitCount++;
            
        }
        Debug.Log($"Total units processed: {unitCount}");
        */
    }

}

/*
        // Legacy immediate-mode query (kept for reference). This iterates on main thread:
        foreach ((
            RefRW<LocalTransform> localTransform,
            RefRO<UnitMover> unitMover,
            RefRW<PhysicsVelocity> physicsVelocity)
            in SystemAPI.Query<
            RefRW<LocalTransform>,
            RefRO<UnitMover>,
            RefRW<PhysicsVelocity>
            >()) {

            // Compute direction from current position to target and normalize
            float3 moveDirection = unitMover.ValueRO.targetPosition - localTransform.ValueRO.Position;
            moveDirection = math.normalize(moveDirection);

            // Smoothly rotate transform to face movement direction
            localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation,
                quaternion.LookRotationSafe(moveDirection, math.up()),
                SystemAPI.Time.DeltaTime * unitMover.ValueRO.rotationSpeed);

            // Apply linear movement and stop angular motion
            physicsVelocity.ValueRW.Linear = moveDirection * unitMover.ValueRO.moveSpeed;
            physicsVelocity.ValueRW.Angular = float3.zero;
        }
        */
