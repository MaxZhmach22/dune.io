using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Dune.IO
{
    public class HarvesterSpicePointMovement : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<HarvesterComponent, MiningComponent>> _filter = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                MoveHarvesterOnSpicePoint(entity);
            }
        }

        private void MoveHarvesterOnSpicePoint(int entity)
        {
            ref var harvesterComponent = ref _filter.Pools.Inc1.Get(entity);
            
            if (harvesterComponent.MiningPoint == Vector3.zero)
            {
                ChooseNewRandomPoint(ref harvesterComponent);
            }
            else
            {
                MoveToMiningPoint(ref harvesterComponent);
            }
        }

        private void MoveToMiningPoint(ref HarvesterComponent harvesterComponent)
        {
            var transform = harvesterComponent.HarvesterView.transform;
            transform.localPosition = 
                Vector3.MoveTowards(transform.localPosition, 
                       harvesterComponent.MiningPoint, Time.deltaTime * harvesterComponent.HarvesterView.Speed);
            var distance = Vector3.Distance(transform.localPosition, harvesterComponent.MiningPoint);
            if(distance < 0.1f)
                harvesterComponent.MiningPoint = Vector3.zero;
        }

        private void ChooseNewRandomPoint(ref HarvesterComponent harvesterComponent) 
        {
            harvesterComponent.MiningPoint = 
                new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3));
        }
    }
}