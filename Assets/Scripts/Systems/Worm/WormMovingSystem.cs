using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dune.IO
{
    public class WormMovingSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<WormComponent, ActiveComponent>, Exc<SwallowComponent>> _filter = default;
        private readonly EcsFilterInject<Inc<WormComponent, WellFedComponent>> _swallowFilter = default;
        private readonly EcsPoolInject<SwallowComponent> _swallowPool = default;
        private readonly EcsFilterInject<Inc<HarvesterComponent, MiningComponent>> _miningFilter = default; 
        private readonly EcsSharedInject<Configuration> _configuration = default;

        public void Run(IEcsSystems systems)
        {
            MovingToTarget();
            AfterSwallowMovement();
        }
        
        private void MovingToTarget()
        {
            foreach(var miningEntity in _miningFilter.Value)
            {
                ref var harvesterComponent = ref _miningFilter.Pools.Inc1.Get(miningEntity);
                foreach (var entity in _filter.Value)
                {
                    ref var wormComponent = ref _filter.Pools.Inc1.Get(entity);
                    var targetPosition = harvesterComponent.HarvesterView.transform.position;
                    var endPosition = new Vector3(targetPosition.x, 0, targetPosition.z);
                    var distance = Vector3.Distance(wormComponent.WormView.transform.position, endPosition);
                    if(distance <= _configuration.Value.WormStopDistance)
                    {
                        Debug.Log("Worm reached target position!");
                        ref var swallowedComponent = ref _swallowPool.Value.Add(miningEntity);
                        swallowedComponent.TargetTransform = harvesterComponent.HarvesterView.transform;
                        ref var fedComponent = ref _swallowFilter.Pools.Inc2.Add(entity);
                        fedComponent.Position = wormComponent.WormView.transform.position + new Vector3(Random.Range(-7, 7), -1f, Random.Range(-7, 7));
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
        
        private void AfterSwallowMovement()
        {
            foreach (var wormEntity in _swallowFilter.Value)
            {
                ref var wormComponent = ref _swallowFilter.Pools.Inc1.Get(wormEntity);
                ref var fedComponent = ref _swallowFilter.Pools.Inc2.Get(wormEntity);
                
                var distance = Vector3.Distance(wormComponent.WormView.transform.position, fedComponent.Position);
                
                wormComponent.WormView.transform.position = Vector3.MoveTowards(
                    wormComponent.WormView.transform.position, fedComponent.Position,
                    wormComponent.WormView.WormFedSpeed * Time.deltaTime);
                
                if(distance <= 0.5)
                {
                    _swallowFilter.Pools.Inc2.Del(wormEntity);
                    Debug.Log("Worm down!");
                }
            }
        }
    }
}