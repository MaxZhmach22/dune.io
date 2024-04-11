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
                            LandHarvester(ornithopter, point, ref ornithopterComponent);
                        }
                        else
                        {
                            var harvester = point.GetComponentInChildren<Harvester>(true);
                            if (harvester != null)
                            {
                                PickUpHarvester(ornithopter, harvester, ref ornithopterComponent);
                            }
                            else
                            {
                                Debug.Log("No harvester on the ornithopter!");
                            }
                        }
                    })
                    .AddTo(_disposable);
            });
        }

        private void LandHarvester(Ornithopter ornithopter, SpicePoint point, ref OrnithopterComponent ornithopterComponent)
        {
            var harvester = ornithopter.GetComponentInChildren<Harvester>(true);
            harvester.transform.SetParent(point.transform);
            if(_miningPool.Value.Has(harvester.HarvesterId)) return;
            _miningPool.Value.Add(harvester.HarvesterId);
            ornithopterComponent.IsCarryingHarvester = false;
            Debug.Log("Spice point triggered! Harvester is start mining!", point);
        }

        private void PickUpHarvester(Ornithopter ornithopter, Harvester harvester, ref OrnithopterComponent ornithopterComponent)
        {
            harvester.transform.SetParent(ornithopter.transform);
            harvester.transform.localPosition = new Vector3(0, -2, 0);
            if(!_miningPool.Value.Has(harvester.HarvesterId)) return;
            _miningPool.Value.Del(harvester.HarvesterId);
            ornithopterComponent.IsCarryingHarvester = true;
            
            Debug.Log("Harvester is stop mining! Harvester is on the ornithopter!", harvester);
        }

        public void Destroy(IEcsSystems systems)
        {
            _disposable.Dispose();
        }
    }
}