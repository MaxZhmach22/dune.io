using System;
using NaughtyAttributes;
using UnityEngine;


namespace Dune.IO
{
    public sealed class SpicePoint : MonoBehaviour
    {
        [field: BoxGroup("Settings")] [field: SerializeField] public float SpiceAmount { get; private set; }
        [field: Foldout("References")] [field: SerializeField] public BoxCollider BoxCollider { get; private set; }

        private void Awake()
        {
            BoxCollider = GetComponent<BoxCollider>();
        }
    }
}
