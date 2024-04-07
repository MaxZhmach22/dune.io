using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;


namespace Dune.IO
{
    public class OrnithopterInitSystem: IEcsInitSystem
    {
        private readonly EcsWorldInject _world = default;
        private readonly EcsSharedInject<Configuration> _configuration = default;
        private readonly EcsPoolInject<OrnithopterComponent> _poolInject = default;


        public void Init(IEcsSystems systems)
        {
            var ornithopter = _world.Value.NewEntity();
            ref var ornithopterComponent = ref _poolInject.Value.Add(ornithopter);
            var ornithopterView = Object.FindObjectOfType<Ornithopter>();
            ornithopterComponent.OrnithopterView = ornithopterView;
            ornithopterView.EntityId = ornithopter;
        }
    }
}