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
        private readonly EcsFilterInject<Inc<MiningComponent, HarvesterComponent>> _filter = default;
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

                    harvesterComponent.Target = Object.FindObjectOfType<Factory>().gameObject;
                    var distance = Vector3.Distance(harvesterComponent.HarvesterView.transform.position, harvesterComponent.Target.transform.position);
                    harvesterComponent.Tween =
                        harvesterComponent.HarvesterView.transform.DOMove(harvesterComponent.Target.transform.position, distance * harvesterComponent.HarvesterView.Speed);
                    harvesterComponent.Tween.SetUpdate(UpdateType.Manual);
                    harvesterComponent.Tween.SetEase(Ease.InOutSine);
                    harvesterComponent.Tween.OnComplete(() =>
                    {
                        //miningPool.Add(entity);
                        Debug.Log("Harvester arrived to factory, start unloading");
                    });
                }
            }
        }
    }
}