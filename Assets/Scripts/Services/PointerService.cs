using DG.Tweening;
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
                            SelectHarvester(harvester);
                        }
                        else if(hit.transform.TryGetComponent<SpicePoint>(out var spicePoint))
                        {
                            SelectSpicePoint(spicePoint);
                        }
                        else
                        {
                            DeselectAll();
                        }
                    }
                })
                .AddTo(monoBehaviour);

            return this;
        }

        /// <summary>
        /// Delete outline from all harvesters
        /// </summary>
        private void DeselectAll()
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
        
        /// <summary>
        /// Add outline to selected harvester
        /// </summary>
        /// <param name="harvester"></param>
        private void SelectHarvester(Harvester harvester)
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
                            
            if (!pool.Has(harvester.HarvesterId))
            {
                ref OutlineComponent outline = ref pool.Add(harvester.HarvesterId);
                outline.DefaultMaterial = harvester.GetComponentInChildren<Renderer>(true).material;
            }
                            
            Debug.Log("Harvester clicked");
        }

        /// <summary>
        /// Set target spice point for selected harvester
        /// </summary>
        /// <param name="spicePoint"></param>
        private void SelectSpicePoint(SpicePoint spicePoint)
        {
            var pool = _world.GetPool<OutlineComponent>();
            var harvesterPool = _world.GetPool<HarvesterComponent>();
            var miningPool = _world.GetPool<MiningComponent>();
            var filter = _world.Filter<OutlineComponent>().Inc<HarvesterComponent>().End();

            foreach (var entity in filter)
            {
                ref var entityOutline = ref pool.Get(entity);
                ref var harvesterComponent = ref harvesterPool.Get(entity);
                harvesterComponent.HarvesterView.GetComponentInChildren<Renderer>(true).material = entityOutline.DefaultMaterial;
                harvesterComponent.Target = spicePoint.gameObject;
                harvesterComponent.SelectedSpicePoint = spicePoint.gameObject;
                
                var distance = Vector3.Distance(harvesterComponent.HarvesterView.transform.position, harvesterComponent.Target.transform.position);
                harvesterComponent.Tween =
                    harvesterComponent.HarvesterView.transform.DOMove(harvesterComponent.Target.transform.position, distance * harvesterComponent.HarvesterView.Speed);
                harvesterComponent.Tween.SetUpdate(UpdateType.Manual);
                harvesterComponent.Tween.SetEase(Ease.InOutSine);
                harvesterComponent.Tween.OnComplete(() =>
                {
                    miningPool.Add(entity);
                    Debug.Log("Harvester arrived, start mining");
                });
                pool.Del(entity);
            }
        }
    }
}