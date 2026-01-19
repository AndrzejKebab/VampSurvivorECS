using AndrzejKebab.Components;
using AndrzejKebab.Components.Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace AndrzejKebab.Jobs
{
    [BurstCompile]
    public partial struct AIControlJob : IJobEntity
    {
        [ReadOnly] public PhysicsWorld                                   PhysicsWorld;
        [ReadOnly] public ComponentLookup<CharacterComponent> CharacterLookup;
        [ReadOnly] public ComponentLookup<AIControllerComponent>         AILookup;
        [ReadOnly] public ComponentLookup<LocalTransform>                TransformLookup;
        [ReadOnly] public ComponentLookup<IsDeadTag>                     DeadLookup;
        
        private void Execute(Entity entity, ref CharacterControlComponent controlComponent, in AIControllerComponent ai,
                             in  LocalTransform              transform)
        {
            if (DeadLookup.HasComponent(entity))
            {
                controlComponent.MoveVector = float3.zero;
                return;
            }
            var distanceHits = new NativeList<DistanceHit>(Allocator.Temp);

            var hitsCollector = new AllHitsCollector<DistanceHit>(ai.DetectionDistance, ref distanceHits);

            var distInput = new PointDistanceInput
                            {
                                Position    = transform.Position,
                                MaxDistance = ai.DetectionDistance,
                                Filter = new CollisionFilter
                                         {
                                             BelongsTo    = CollisionFilter.Default.BelongsTo,
                                             CollidesWith = ai.DetectionFilter.Value
                                         }
                            };

            PhysicsWorld.CalculateDistance(distInput, ref hitsCollector);
            Entity selectedTarget = Entity.Null;

            for (var i = 0; i < hitsCollector.NumHits; i++)
            {
                Entity hitEntity = distanceHits[i].Entity;

                if (!CharacterLookup.HasComponent(hitEntity)) continue;
                if (AILookup.HasComponent(hitEntity)) continue;

                selectedTarget = hitEntity;
                break;
            }

            if (selectedTarget != Entity.Null)
            {
                float3 targetPos = TransformLookup[selectedTarget].Position;
                controlComponent.MoveVector = math.normalizesafe(targetPos - transform.Position);
            }
            else
            {
                controlComponent.MoveVector = float3.zero;
            }

            distanceHits.Dispose();
        }
    }
}
