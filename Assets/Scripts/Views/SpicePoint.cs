using NaughtyAttributes;
using UnityEngine;

namespace Dune.IO
{
    public sealed class SpicePoint : MonoBehaviour
    {
        [field: BoxGroup("Settings")] [field: SerializeField] public float SpiceAmount { get; private set; }
    }
}
