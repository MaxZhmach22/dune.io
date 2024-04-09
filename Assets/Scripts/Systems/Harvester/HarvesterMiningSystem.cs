using DG.Tweening;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Dune.IO
{
    public class HarvesterMiningSystem : IEcsRunSystem
    {
        private readonly EcsPoolInject<MiningComponent> _miningPool = default;
        private readonly EcsPoolInject<HarvesterComponent> _harvesterComponent = default;
        private readonly EcsPoolInject<FactoryComponent> _factoryPool = default;
        private readonly EcsFilterInject<Inc<MiningComponent, HarvesterComponent>, Exc<SwallowComponent>> _filter = default;
        private readonly EcsSharedInject<Configuration> _configuration = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var miningComponent = ref _miningPool.Value.Get(entity);
                ref var harvesterComponent = ref _harvesterComponent.Value.Get(entity);

                miningComponent.Amount += _configuration.Value.StartMiningSpeed * harvesterComponent.HarvesterView.Level * Time.deltaTime;
                harvesterComponent.HarvesterView.SpiceAmount = miningComponent.Amount;
                Debug.Log(harvesterComponent.HarvesterView.SpiceAmount);

                if (miningComponent.Amount >= harvesterComponent.HarvesterView.SpiceCapacity)
                {
                    harvesterComponent.HarvesterView.SpiceAmount = harvesterComponent.HarvesterView.SpiceCapacity;
                    Debug.Log($"Harvester is full {harvesterComponent.HarvesterView.SpiceAmount}");
                    _miningPool.Value.Del(entity);
                }
            }
        }
    }
}