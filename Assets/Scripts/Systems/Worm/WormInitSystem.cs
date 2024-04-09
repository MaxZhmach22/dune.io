using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Dune.IO
{
    public class WormInitSystem : IEcsInitSystem
    {
        private readonly EcsSharedInject<Configuration> _configuration = default;
        private readonly EcsWorldInject _world = default;
        private readonly EcsPoolInject<WormComponent> _wormPool = default;
        private readonly EcsPoolInject<ActiveComponent> _activeWormPool = default;

        public void Init(IEcsSystems systems)
        {
            var worms = Object.FindObjectsOfType<WormView>(true);
            
            foreach (var worm in worms)
            {
                var wormEntity = _world.Value.NewEntity();
                ref var wormComponent = ref _wormPool.Value.Add(wormEntity);
                if(worm.gameObject.activeSelf) _activeWormPool.Value.Add(wormEntity);
                wormComponent.WormView = worm;
                worm.EntityId = wormEntity;
            }
        }
    }
}