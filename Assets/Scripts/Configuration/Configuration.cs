using NaughtyAttributes;
using UnityEngine;

namespace Dune.IO
{
    [CreateAssetMenu(fileName = "Config", menuName = "Configuration/Dune.io")]
    public sealed class Configuration : ScriptableObject
    {
        [field: BoxGroup("Game settings")]
        [field: SerializeField]
        public int FinalGoal { get; private set; } = 150;
        
        [field: BoxGroup("Ornithopter settings")]
        [field: SerializeField]
        public float OrnithopterSpeedForce { get; private set; } = .3f;
        
        [field: BoxGroup("Ornithopter settings")]
        [field: SerializeField]
        public float OrnithopterMaxSpeed { get; private set; } = 5;        
        
        [field: BoxGroup("Ornithopter settings")]
        [field: SerializeField]
        public int LandingTime { get; private set; } = 500;        
        
        [field: BoxGroup("Ornithopter settings")]
        [field: SerializeField]
        public float VelocityDecreasingTime { get; private set; } = 0.5f;
        
        [field: BoxGroup("Ornithopter settings")]
        [field: SerializeField]
        public float OrnithopterRotationSpeed { get; private set; } = 5f;
        
        [field: BoxGroup("Score settings")]
        [field: SerializeField]
        public int StartScore { get; private set; } = 100;
        
        
        [field: BoxGroup("Harvester settings")] 
        [field: SerializeField] 
        public GameObject HarvesterPrefab { get; private set; }
        
        [field: BoxGroup("Harvester settings")] 
        [field: SerializeField] 
        public Material HarvesterOutlineMaterial { get; private set; }

        [field: BoxGroup("Harvester settings")]
        [field: SerializeField]
        public int StartHarvesterPrice { get; private set; } = 100;
        
        [field: BoxGroup("Harvester settings")]
        [field: SerializeField]
        public float StartMiningSpeed { get; private set; } = 2;
        
        [field: BoxGroup("Worm settings")]
        [field: SerializeField]
        public float WormStopDistance { get; private set; } = 1;

    }
}