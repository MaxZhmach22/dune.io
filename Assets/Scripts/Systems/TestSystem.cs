using System;
using Leopotam.EcsLite;
using UnityEngine;

namespace Dune.IO
{
    public class TestSystem : IEcsRunSystem, IEcsInitSystem
    {
        
        
        public void Init(IEcsSystems systems)
        {
            Debug.Log("Init");
        }
        
        
        public void Run(IEcsSystems systems)
        {
            
        }
        
    }
}