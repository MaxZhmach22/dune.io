using System;
using NaughtyAttributes;
using UnityEngine;

namespace Dune.IO
{
    public class HarvesterLandingPoint : MonoBehaviour
    {
        [field: Foldout("Reference")]
        [field: SerializeField]
        public BoxCollider Collider { get; private set; }
        
        [field: Foldout("Reference")]
        [field: SerializeField]
        public Rigidbody Rigidbody { get; private set; }
        
        private void Awake()
        {
            Collider = GetComponent<BoxCollider>();
            Rigidbody = GetComponent<Rigidbody>();
        }
    }
}