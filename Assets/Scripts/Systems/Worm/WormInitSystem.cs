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

        public void Init(IEcsSystems systems)
        {
            var wormView = Object.FindObjectOfType<WormView>();
            var wormEntity = _world.Value.NewEntity();
            ref var wormComponent = ref _wormPool.Value.Add(wormEntity);
            wormComponent.WormView = wormView;
            wormView.EntityId = wormEntity;
        }
    }
}