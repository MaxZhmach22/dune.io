using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Dune.IO
{
    public class HarvesterOutlineSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject _defaultWorld = default;
        private readonly EcsPoolInject<OutlineComponent> _outlinePool = default;
        private readonly EcsPoolInject<HarvesterComponent> _harvesterPool = default;
        private readonly EcsFilterInject<Inc<HarvesterComponent, OutlineComponent>> _harvesterOutlineFilter = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _harvesterOutlineFilter.Value)
            {
                ref var outlineComponent = ref _outlinePool.Value.Get(entity);
                ref var harvesterComponent = ref _harvesterPool.Value.Get(entity);
                outlineComponent.OutlineMaterial = systems.GetShared<Configuration>().HarvesterOutlineMaterial;
                harvesterComponent.HarvesterView.GetComponentInChildren<Renderer>(true).material = outlineComponent.OutlineMaterial;
            }
            
        }
    }
}