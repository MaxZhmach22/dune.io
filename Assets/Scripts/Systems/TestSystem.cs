using System;
using System.Collections.Generic;
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
        
        public void Init(IEcsSystems systems)
        {
            Debug.Log("Init");
        }
        
        
        public void Run(IEcsSystems systems)
        {
            
        }
        
    }
}