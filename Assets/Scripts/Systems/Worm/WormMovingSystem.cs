using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Dune.IO
{
    public class WormMovingSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<WormComponent, ActiveComponent, MoveToTargetComponent>, Exc<SwallowComponent>> _wormsMovingToTargetFilter = default;
        private readonly EcsFilterInject<Inc<WormComponent, ActiveComponent>, Exc<SwallowComponent, MoveToTargetComponent>> _wormsFreeMovingFilter = default;
        private readonly EcsPoolInject<MoveToTargetComponent> _moveToTargetPool = default;

        private readonly EcsFilterInject<Inc<WormComponent, WellFedComponent>> _swallowFilter = default;
        private readonly EcsPoolInject<SwallowComponent> _swallowPool = default;
        private readonly EcsFilterInject<Inc<HarvesterComponent, MiningComponent>> _miningFilter = default; 
        private readonly EcsSharedInject<Configuration> _configuration = default;
        
        public void Run(IEcsSystems systems)
        {
            FreeMoving();
            MovingToTarget();
            AfterSwallowMovement();
        }

        private void FreeMoving()
        {
            foreach (var wormEntity in _wormsFreeMovingFilter.Value)
            {
                ref var wormComponent = ref _wormsFreeMovingFilter.Pools.Inc1.Get(wormEntity);
                if (wormComponent.WormView.RandomMovingPoint == Vector3.zero)
                {
                    wormComponent.WormView.RandomMovingPoint = new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20));
                    wormComponent.WormView.NextPointMove.transform.position = wormComponent.WormView.RandomMovingPoint;
                    wormComponent.WormView.NextPointMove.name = $"Worm " + wormComponent.WormView.EntityId;
                }
                else
                {
                    var distance = Vector3.Distance(wormComponent.WormView.transform.position,
                        wormComponent.WormView.RandomMovingPoint);

                    if (distance <= 0.5)
                    {
                        wormComponent.WormView.RandomMovingPoint = Vector3.zero;
                    }
                    else
                    {
                        wormComponent.WormView.transform.localPosition = Vector3.MoveTowards(
                            wormComponent.WormView.transform.localPosition, wormComponent.WormView.RandomMovingPoint,
                            wormComponent.WormView.WormFreeMovingSpeed * Time.deltaTime);
                    }
                }
            }
        }

        private void MovingToTarget()
        {
            foreach (var wormEntity in _wormsMovingToTargetFilter.Value)
            {
                ref var wormComponent = ref _wormsMovingToTargetFilter.Pools.Inc1.Get(wormEntity);
                MovingToTarget(wormComponent, wormEntity);
            }
        }
        

        private void MovingToTarget(WormComponent wormComponent, int wormEntity)
        {
            var targetPosition = wormComponent.Target.transform.position;
            var endPosition = new Vector3(targetPosition.x, 0, targetPosition.z);
            var distance = Vector3.Distance(wormComponent.WormView.transform.position, endPosition);
            if (distance <= _configuration.Value.WormStopDistance)
            {
                Debug.Log("Worm reached target position!");
                ref var swallowedComponent = ref _swallowPool.Value.Add(wormComponent.Target.HarvesterId);
                swallowedComponent.TargetTransform = wormComponent.Target.transform;
                ref var fedComponent = ref _swallowFilter.Pools.Inc2.Add(wormEntity);
                fedComponent.Position = wormComponent.WormView.transform.position +
                                        new Vector3(Random.Range(-7, 7), -1f, Random.Range(-7, 7));
            }
            else
            {
                wormComponent.WormView.transform.position = Vector3.MoveTowards(
                    wormComponent.WormView.transform.position, endPosition,
                    wormComponent.WormView.WormSpeed * Time.deltaTime);
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