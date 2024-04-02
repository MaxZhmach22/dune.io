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
        
        public BuyHarvesterSystem(ScoreService scoreService)
        {
            _scoreService = scoreService;
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _buyHarvesterFilter.Value)
            {
                ref var buyHarvesterComponent = ref _poolInject.Value.Get(entity);

                if (ValidateSpice(buyHarvesterComponent.Price))
                {
                    Debug.Log("Harvester bought");
                }
                else
                {
                    Debug.Log("Don't have enough score to buy harvester");
                }
                
                _poolInject.Value.Del(entity);
            }
        }

        private bool ValidateSpice(float price)
        {
            return _scoreService.RemoveScore(price);
        }
    }
}