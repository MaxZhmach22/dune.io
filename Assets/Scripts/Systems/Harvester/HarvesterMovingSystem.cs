using DG.Tweening;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;


namespace Dune.IO
{
    public class HarvesterMovingSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject _defaultWorld = default;
        private readonly EcsPoolInject<HarvesterComponent> _harvesterPool = default;
        private readonly EcsFilterInject<Inc<HarvesterComponent>, Exc<SwallowComponent>> _filter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var harvesterComponent = ref _harvesterPool.Value.Get(entity);
                harvesterComponent.Tween.ManualUpdate(Time.deltaTime, Time.unscaledDeltaTime);
            }
        }
    }
}