using NaughtyAttributes;
using UnityEngine;


namespace Dune.IO
{
    public class WormView : MonoBehaviour
    {
        [field: BoxGroup("Worm Settings:")] [field: SerializeField] public float WormSpeed { get; set; }
        [field: BoxGroup("Worm Settings:")] [field: SerializeField] public float WormFedSpeed { get; set; }
        [field: BoxGroup("Worm Settings:")] [field: SerializeField] public float WormFreeMovingSpeed { get; set; }
        [field: BoxGroup("Worm Settings:")] [field: SerializeField] public int EntityId { get; set; }
        
        [field: Foldout("State:")] [field: SerializeField] public Vector3 RandomMovingPoint { get; set; }

        [field: Foldout("References")] [field: SerializeField] public NextPointMove NextPointMove { get; private set; }

        private void Awake()
        {
            NextPointMove = new GameObject($"Worm {EntityId}").AddComponent<NextPointMove>();
        }
    }
}