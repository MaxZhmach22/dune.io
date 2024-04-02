using NaughtyAttributes;
using UnityEngine;


namespace Dune.IO
{
    public sealed class Harvester : MonoBehaviour
    {
        [field: BoxGroup("Settings")] [field: SerializeField] public float Speed { get; private set; }
        [field: BoxGroup("Settings")] [field: SerializeField] public int Level { get; private set; }
    }
}