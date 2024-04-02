using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Dune.IO
{
    public class TestSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsWorldInject _defaultWorld = default;
        private readonly EcsPoolInject<HarvesterComponent> _harvesterPool = default;
        private readonly EcsFilterInject<Inc<HarvesterComponent>> _filter = default;

        public void Init(IEcsSystems systems)
        {
            ref var harvester = ref InitHarvester();
            Debug.Log(harvester.HarvesterView.Level);

        }

        private ref HarvesterComponent InitHarvester()
        {
            var harvester = _defaultWorld.Value.NewEntity();
            ref var harvesterComponent = ref _harvesterPool.Value.Add(harvester);
            harvesterComponent.HarvesterView = Object.FindObjectOfType<Harvester>();
            harvesterComponent.Target = Object.FindObjectOfType<SpicePoint>().gameObject;
            harvesterComponent.HarvesterId = harvester;
            return ref harvesterComponent;
        }


        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref HarvesterComponent harvesterComponent = ref _harvesterPool.Value.Get(entity);
                var moveDirection = 
                    (harvesterComponent.Target.transform.position - harvesterComponent.HarvesterView.transform.position).normalized 
                    * harvesterComponent.HarvesterView.Speed * Time.deltaTime;
                var distance = Vector3.Distance(harvesterComponent.Target.transform.position, harvesterComponent.HarvesterView.transform.position);
                if (distance < 0.5f)
                {
                    Debug.Log("Harvester reached the target");
                    _harvesterPool.Value.Del(entity);
                }
                else
                {
                    harvesterComponent.HarvesterView.transform.transform.LookAt(harvesterComponent.Target.transform);
                    harvesterComponent.HarvesterView.transform.position += moveDirection;
                }
            }
        }
        
    }
}