using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;


namespace Dune.IO
{
    public class SwallowTargetSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<SwallowComponent, HarvesterComponent>> _filter = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var swallowComponent = ref _filter.Pools.Inc1.Get(entity);
                swallowComponent.TargetTransform.gameObject.SetActive(false);
                _filter.Pools.Inc1.Del(entity);
                _filter.Pools.Inc2.Del(entity);
                Debug.Log("Swallowed target!");
            }
        }
    }
}