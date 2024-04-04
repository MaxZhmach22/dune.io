using NaughtyAttributes;
using UnityEngine;

namespace Dune.IO
{
    public class Factory : MonoBehaviour
    {
        [field: BoxGroup("Settings")]
        [field: SerializeField]
        public float Capacity { get; set; } = 0;
        
        [field: BoxGroup("Settings")]
        [field: SerializeField]
        public float UploadingSpeed { get; set; } = 2;
    }
}