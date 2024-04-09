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

                        Debug.Log(col, col.transform);
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