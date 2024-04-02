using DG.Tweening;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Dune.IO
{
    public class HarvesterMovingSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsWorldInject _defaultWorld = default;
        private readonly EcsPoolInject<HarvesterComponent> _harvesterPool = default;
        private readonly EcsFilterInject<Inc<HarvesterComponent>> _filter = default;

        public void Init(IEcsSystems systems)
        {
            ref var harvester = ref InitHarvester();
        }

        private ref HarvesterComponent InitHarvester()
        {
            var harvester = _defaultWorld.Value.NewEntity();
            ref var harvesterComponent = ref _harvesterPool.Value.Add(harvester);
            harvesterComponent.HarvesterView = Object.FindObjectOfType<Harvester>();
            harvesterComponent.Target = Object.FindObjectOfType<SpicePoint>().gameObject;
            harvesterComponent.HarvesterId = harvester;
            harvesterComponent.Tween =
                harvesterComponent.HarvesterView.transform.DOMove(harvesterComponent.Target.transform.position, harvesterComponent.HarvesterView.Speed);
            harvesterComponent.Tween.SetUpdate(UpdateType.Manual);
            harvesterComponent.Tween.SetEase(Ease.InOutSine);
            harvesterComponent.Tween.SetLoops(loops: -1, loopType: LoopType.Yoyo);
            harvesterComponent.Tween.OnComplete(() => { Debug.Log("Harvester arrived"); });
            return ref harvesterComponent;
        }


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