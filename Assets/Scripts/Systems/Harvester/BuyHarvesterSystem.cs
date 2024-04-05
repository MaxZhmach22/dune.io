using DG.Tweening;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Dune.IO
{
    public class BuyHarvesterSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject _defaultWorld = default;
        private readonly EcsFilterInject<Inc<BuyHarvesterComponent>> _buyHarvesterFilter = default;
        private readonly EcsPoolInject<BuyHarvesterComponent> _poolInject = default;
        private readonly EcsPoolInject<HarvesterComponent> _harvesterPool = default;

        private ScoreService _scoreService;

        private GameObject _harvestersParentObject;
        
        public BuyHarvesterSystem(ScoreService scoreService)
        {
            _scoreService = scoreService;
            _harvestersParentObject = new GameObject("Harvesters");
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _buyHarvesterFilter.Value)
            {
                ref var buyHarvesterComponent = ref _poolInject.Value.Get(entity);

                if (ValidateSpice(buyHarvesterComponent.Price))
                {
                    var config = systems.GetShared<Configuration>();
                    ref var harvester = ref InitHarvester(config);
                }
                else
                {
                    Debug.Log("Don't have enough score to buy harvester");
                }
                
                _poolInject.Value.Del(entity);
            }
        }

        private bool ValidateSpice(int price)
        {
            return _scoreService.RemoveScore(price);
        }
        
        private ref HarvesterComponent InitHarvester(Configuration configuration)
        {
            var harvester = _defaultWorld.Value.NewEntity();
            ref var harvesterComponent = ref _harvesterPool.Value.Add(harvester);
            var harvesterView = Object.Instantiate(configuration.HarvesterPrefab, Vector3.zero, Quaternion.identity,  _harvestersParentObject.transform);
            harvesterComponent.HarvesterView = harvesterView.GetComponent<Harvester>();
            harvesterComponent.HarvesterView.HarvesterId = harvester;
            
            return ref harvesterComponent;
        }
    }
}