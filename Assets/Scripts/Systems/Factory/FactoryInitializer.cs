using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UniRx;
using UniRx.Triggers;
using UnityEngine;


namespace Dune.IO
{
    public class FactoryInitializer : IEcsInitSystem, IEcsDestroySystem
    {
        private readonly EcsWorldInject _world = default;
        private readonly EcsPoolInject<OrnithopterComponent> _pool = default;
        private readonly Factory _factory;
        private readonly CompositeDisposable _disposable = new();

        public FactoryInitializer(Factory factory)
        {
            _factory = factory;
        }
        
        
        public void Init(IEcsSystems systems)
        {
            FactoryInit();
            
        }
        
        private void FactoryInit()
        {
            _factory.HarvesterLandingPoint.ForEach(point =>
            {
                point.Collider.OnTriggerEnterAsObservable()
                    .Subscribe(col =>
                    {
                        var ornithopter = col.GetComponent<Ornithopter>();
                        ref var ornithopterComponent = ref _pool.Value.Get(ornithopter.EntityId);
                        var harvester = point.GetComponentInChildren<Harvester>();
                        
                        if (harvester != null && !ornithopterComponent.IsCarryingHarvester)
                        {
                            ornithopterComponent.IsCarryingHarvester = true;
                            harvester.transform.SetParent(ornithopter.transform);
                            harvester.transform.localPosition = new Vector3(0, -2, 0);
                        }

                        else if(harvester == null && ornithopterComponent.IsCarryingHarvester)
                        {
                            harvester = ornithopter.GetComponentInChildren<Harvester>(true);
                            ornithopterComponent.IsCarryingHarvester = false;
                            harvester.transform.SetParent(point.transform);
                            harvester.transform.localPosition = new Vector3(0, 0, 0);
                            var factory = point.GetComponentInParent<Factory>();
                            ref var factoryComponent = ref _world.Value.GetPool<FactoryComponent>().Add(harvester.HarvesterId);
                            factoryComponent.FactoryView = factory;
                        }
                        
                    })
                    .AddTo(_disposable);
            });
        }


        public void Destroy(IEcsSystems systems)
        {
            _disposable.Dispose();
        }
    }
}