using System;
using DG.Tweening;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Dune.IO
{
    public class UploadingSpice : IEcsRunSystem
    {
        private readonly ScoreService _scoreService;
        private readonly EcsWorldInject _defaultWorld = default;
        private readonly EcsPoolInject<HarvesterComponent> _harvesterPool = default;
        private readonly EcsPoolInject<FactoryComponent> _factoryPool = default;
        private readonly EcsPoolInject<MiningComponent> _miningPool = default;
        private readonly EcsFilterInject<Inc<FactoryComponent, HarvesterComponent>> _filter = default;

        public UploadingSpice(ScoreService scoreService)
        {
            _scoreService = scoreService;
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var factoryComponent = ref _filter.Pools.Inc1.Get(entity);
                ref var harvesterComponent = ref _filter.Pools.Inc2.Get(entity);

                if (harvesterComponent.HarvesterView.SpiceAmount > 0)
                {
                    harvesterComponent.HarvesterView.SpiceAmount -= Time.deltaTime * factoryComponent.FactoryView.UploadingSpeed;
                    var spice = Time.deltaTime * factoryComponent.FactoryView.UploadingSpeed;
                    factoryComponent.FactoryView.Capacity += spice;
                    _scoreService.AddScore(spice);
                    Debug.Log($"Uploading spice {harvesterComponent.HarvesterView.SpiceAmount} / {factoryComponent.FactoryView.Capacity}");
                }
                else
                {
                    harvesterComponent.HarvesterView.SpiceAmount = 0;
                    factoryComponent.FactoryView.Capacity = (float)Math.Round(factoryComponent.FactoryView.Capacity);
                    _factoryPool.Value.Del(entity);
                    if (harvesterComponent.SelectedSpicePoint != null)
                    {
                        var distance = Vector3.Distance(harvesterComponent.HarvesterView.transform.position, harvesterComponent.SelectedSpicePoint.transform.position);
                        harvesterComponent.Tween =
                            harvesterComponent.HarvesterView.transform.DOMove(harvesterComponent.SelectedSpicePoint.transform.position, distance * harvesterComponent.HarvesterView.Speed);
                        harvesterComponent.Tween.SetUpdate(UpdateType.Manual);
                        harvesterComponent.Tween.SetEase(Ease.InOutSine);
                        harvesterComponent.Tween.OnComplete(() =>
                        {
                            _miningPool.Value.Add(entity);
                            Debug.Log("Harvester arrived, start mining");
                        }); 
                    }
                }
            }
        }


    }
}