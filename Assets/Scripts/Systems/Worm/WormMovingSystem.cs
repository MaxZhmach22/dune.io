using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Dune.IO
{
    public class WormMovingSystem : IEcsRunSystem
    {
        private readonly EcsSharedInject<Configuration> _configuration = default;
        
        private readonly EcsFilterInject<Inc<WormComponent, ActiveComponent, MoveToTargetComponent>, Exc<WellFedComponent>> _wormsMovingToTargetFilter = default;
        private readonly EcsFilterInject<Inc<WormComponent, ActiveComponent>, Exc<MoveToTargetComponent, WellFedComponent>> _wormsFreeMovingFilter = default;
        private readonly EcsFilterInject<Inc<WormComponent, WellFedComponent>> _afterSwallowHarvesterFilter = default;
        private readonly EcsPoolInject<MoveToTargetComponent> _moveToTargetPool = default;
        private readonly EcsPoolInject<SwallowComponent> _swallowPool = default;

        private readonly EcsFilterInject<Inc<HarvesterComponent, MiningComponent>, Exc<SwallowComponent>> _miningHarvesterFilter = default; 
        private readonly EcsFilterInject<Inc<HarvesterComponent>, Exc<SwallowComponent, MiningComponent>> _finishMiningHarvesterFilter = default; 
        

        public void Run(IEcsSystems systems)
        {
            ClearTarget();
            FindTarget();
            FreeMoving();
            MovingToTarget();
            AfterSwallowMovement();
        }

        private void ClearTarget()
        {
            foreach (var harvesterEntity in _finishMiningHarvesterFilter.Value)
            {
                ref var harvesterComponent = ref _miningHarvesterFilter.Pools.Inc1.Get(harvesterEntity);
                foreach (var wormEntity in _wormsMovingToTargetFilter.Value)
                {
                    ref var wormComponent = ref _wormsMovingToTargetFilter.Pools.Inc1.Get(wormEntity);
                    if (wormComponent.Target != harvesterComponent.HarvesterView) continue;
                    wormComponent.HasTarget = false;
                    wormComponent.Target = null;
                    harvesterComponent.IsWormsTarget = false;
                    _wormsMovingToTargetFilter.Pools.Inc3.Del(wormEntity);
                }
            }
        }

        private void FindTarget()
        {
            foreach (var harvesterEntity in _miningHarvesterFilter.Value)
            {
                ref var harvesterComponent = ref _miningHarvesterFilter.Pools.Inc1.Get(harvesterEntity);
                FindNearestWorm(ref harvesterComponent);
            }
        }

        private void FindNearestWorm(ref HarvesterComponent harvesterComponent)
        {
            if(harvesterComponent.IsWormsTarget) return;
            
            var nearestDistance = float.MaxValue;
            var nearestWorm = 0;
            foreach (var wormEntity in _wormsFreeMovingFilter.Value)
            {
                ref var wormComponent = ref _wormsFreeMovingFilter.Pools.Inc1.Get(wormEntity);
                var distance = Vector3.Distance(wormComponent.WormView.transform.position,
                    harvesterComponent.HarvesterView.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestWorm = wormEntity;
                }
            }

            ref var nearestWormComponent = ref _wormsFreeMovingFilter.Pools.Inc1.Get(nearestWorm);
            nearestWormComponent.Target = harvesterComponent.HarvesterView;
            nearestWormComponent.HasTarget = true;
            harvesterComponent.IsWormsTarget = true;
            _moveToTargetPool.Value.Add(nearestWorm);
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
                var targetPosition = wormComponent.Target.transform.position;
                var endPosition = new Vector3(targetPosition.x, 0, targetPosition.z);
                var distance = Vector3.Distance(wormComponent.WormView.transform.position, endPosition);
                if (distance <= _configuration.Value.WormStopDistance)
                {
                    
                    Debug.Log("Worm reached target position!");
                    ref var harvesterComponent = ref _miningHarvesterFilter.Pools.Inc1.Get(wormComponent.Target.HarvesterId);
                    harvesterComponent.IsWormsTarget = false;
                    wormComponent.HasTarget = false;
                    ref var swallowComponent =  ref _swallowPool.Value.Add(wormComponent.Target.HarvesterId);
                    swallowComponent.TargetTransform = harvesterComponent.HarvesterView.transform;
                    _miningHarvesterFilter.Pools.Inc2.Del(wormComponent.Target.HarvesterId);
                    wormComponent.Target = null;
                    _wormsMovingToTargetFilter.Pools.Inc3.Del(wormEntity);
                    ref var fedComponent = ref _afterSwallowHarvesterFilter.Pools.Inc2.Add(wormEntity);
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
        }
        
        private void AfterSwallowMovement()
        {
            foreach (var wormEntity in _afterSwallowHarvesterFilter.Value)
            {
                ref var wormComponent = ref _afterSwallowHarvesterFilter.Pools.Inc1.Get(wormEntity);
                ref var fedComponent = ref _afterSwallowHarvesterFilter.Pools.Inc2.Get(wormEntity);
                
                var distance = Vector3.Distance(wormComponent.WormView.transform.position, fedComponent.Position);
                
                wormComponent.WormView.transform.position = Vector3.MoveTowards(
                    wormComponent.WormView.transform.position, fedComponent.Position,
                    wormComponent.WormView.WormFedSpeed * Time.deltaTime);
                
                if(distance <= 0.5)
                {
                    _afterSwallowHarvesterFilter.Pools.Inc2.Del(wormEntity);
                    Debug.Log("Worm down!");
                }
            }
        }
    }
}