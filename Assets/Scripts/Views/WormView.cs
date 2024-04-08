using NaughtyAttributes;
using UnityEngine;

namespace Dune.IO
{
    public class WormView : MonoBehaviour
    {
        [field: BoxGroup("Worm Settings:")] [field: SerializeField] public float WormSpeed { get; set; }
        [field: BoxGroup("Worm Settings:")] [field: SerializeField] public int EntityId { get; set; }
    }
}