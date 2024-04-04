using NaughtyAttributes;
using UnityEngine;


namespace Dune.IO
{
    public sealed class Harvester : MonoBehaviour
    {
        [field: BoxGroup("Settings")] [field: SerializeField] public float Speed { get; private set; }
        [field: BoxGroup("Settings")] [field: SerializeField] public int Level { get; private set; }
        [field: BoxGroup("Settings")] [field: SerializeField] public float SpiceCapacity { get; private set; }
        [field: BoxGroup("Settings")] [field: SerializeField] public float SpiceAmount { get; set; }
        
        
        //Id of the harvester
        [field: BoxGroup("Settings")] [field: SerializeField] public int HarvesterId { get; set; }
    }
}