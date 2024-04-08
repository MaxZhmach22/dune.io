using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Dune.IO
{
    public class WormMovingSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<WormComponent, WormMovingComponent>> _filter = default;
        private readonly EcsSharedInject<Configuration> _configuration = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var wormComponent = ref _filter.Pools.Inc1.Get(entity);
                ref var wormMovingComponent = ref _filter.Pools.Inc2.Get(entity);
                var targetPosition = wormMovingComponent.TargetTransform.position;
                var endPosition = new Vector3(targetPosition.x, 0, targetPosition.z);
                var distance = Vector3.Distance(wormComponent.WormView.transform.position, endPosition);
                if(distance <= _configuration.Value.WormStopDistance)
                {
                    Debug.Log("Worm reached target position!");
                    _filter.Pools.Inc2.Del(entity);
                }
                else
                {
                    wormComponent.WormView.transform.position = Vector3.MoveTowards(
                        wormComponent.WormView.transform.position, endPosition,
                        wormComponent.WormView.WormSpeed * Time.deltaTime);
                }

            }
        }
    }
}