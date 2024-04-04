using DG.Tweening;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Dune.IO
{
    public class UploadingSpice : IEcsRunSystem
    {
        private readonly EcsWorldInject _defaultWorld = default;
        private readonly EcsPoolInject<HarvesterComponent> _harvesterPool = default;
        private readonly EcsPoolInject<FactoryComponent> _factoryPool = default;
        private readonly EcsPoolInject<MiningComponent> _miningPool = default;
        private readonly EcsFilterInject<Inc<FactoryComponent, HarvesterComponent>> _filter = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var factoryComponent = ref _filter.Pools.Inc1.Get(entity);
                ref var harvesterComponent = ref _harvesterPool.Value.Get(entity);

                if (harvesterComponent.HarvesterView.SpiceAmount > 0)
                {
                    harvesterComponent.HarvesterView.SpiceAmount -= Time.deltaTime * factoryComponent.FactoryView.UploadingSpeed;
                    factoryComponent.FactoryView.Capacity += Time.deltaTime * factoryComponent.FactoryView.UploadingSpeed;
                    Debug.Log($"Uploading spice {harvesterComponent.HarvesterView.SpiceAmount} / {factoryComponent.FactoryView.Capacity}");
                }
                else
                {
                    harvesterComponent.HarvesterView.SpiceAmount = 0;
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