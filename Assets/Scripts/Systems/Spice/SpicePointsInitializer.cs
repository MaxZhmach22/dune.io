using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Dune.IO
{
    public class SpicePointsInitializer : IEcsInitSystem, IEcsDestroySystem
    {
        private readonly List<SpicePoint> _spicePoints;
        private readonly CompositeDisposable _disposable = new();
        private readonly EcsWorldInject _world = default;
        private readonly EcsPoolInject<OrnithopterComponent> _pool = default;
        private readonly EcsPoolInject<MiningComponent> _miningPool = default;


        public SpicePointsInitializer(List<SpicePoint> spicePoints)
        {
            _spicePoints = spicePoints;
        }
        
        
        public void Init(IEcsSystems systems)
        {
            SpicePointsInit();
        }
        
        private void SpicePointsInit()
        {
            _spicePoints.ForEach(point =>
            {
                point.BoxCollider.OnTriggerEnterAsObservable()
                    .Subscribe(col =>
                    {   
                        var ornithopter = col.GetComponent<Ornithopter>();
                        if(ornithopter == null) return;

                        ref var ornithopterComponent = ref _pool.Value.Get(ornithopter.EntityId);
                        if (ornithopterComponent.IsCarryingHarvester)
                        {
                            var harvester = ornithopter.GetComponentInChildren<Harvester>(true);
                            harvester.transform.SetParent(point.transform);
                            if(_miningPool.Value.Has(harvester.HarvesterId)) return;
                            _miningPool.Value.Add(harvester.HarvesterId);
                            ornithopterComponent.IsCarryingHarvester = false;
                            Debug.Log("Spice point triggered! Harvester is start mining!");
                        }
                        else
                        {
                            Debug.Log("No harvester on the ornithopter!");
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