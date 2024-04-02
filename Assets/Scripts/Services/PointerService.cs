using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UniRx;
using UnityEngine;


namespace Dune.IO
{
    public class PointerService
    {
        private readonly EcsWorld _world;
        
        public PointerService(EcsWorld world)
        {
            _world = world;
        }


        public PointerService Init(MonoBehaviour monoBehaviour)
        {
            var mainCamera = Camera.main;
            
            Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(0))
                .Subscribe(_ =>
                {
                    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        if (hit.transform.TryGetComponent<Harvester>(out var harvester))
                        {
                            //Add outline component 
                            var pool = _world.GetPool<OutlineComponent>();
                            var filter = _world.Filter<OutlineComponent>().End();
                            foreach (var entity in filter)
                            {
                                ref var entityOutline = ref pool.Get(entity);
                                harvester.GetComponentInChildren<Renderer>(true).material = entityOutline.DefaultMaterial;
                                pool.Del(entity);
                            }
                            
                            if (!pool.Has(harvester.HarvesterId))
                            {
                                ref OutlineComponent outline = ref pool.Add(harvester.HarvesterId);
                                outline.DefaultMaterial = harvester.GetComponentInChildren<Renderer>(true).material;
                            }
                            
                            Debug.Log("Harvester clicked");
                        }
                        else
                        {
                            var pool = _world.GetPool<OutlineComponent>();
                            var harvesterPool = _world.GetPool<HarvesterComponent>();
                            var filter = _world.Filter<OutlineComponent>().Inc<HarvesterComponent>().End();
                            foreach (var entity in filter)
                            {
                                ref var entityOutline = ref pool.Get(entity);
                                ref var harvesterComponent = ref harvesterPool.Get(entity);
                                harvesterComponent.HarvesterView.GetComponentInChildren<Renderer>(true).material = entityOutline.DefaultMaterial;
                                pool.Del(entity);
                            }
                        }
                    }
                })
                .AddTo(monoBehaviour);

            return this;
        }
    }
}