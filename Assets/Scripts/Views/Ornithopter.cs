using System;
using UnityEngine;

namespace Dune.IO
{
    public class Ornithopter : MonoBehaviour
    {
        public Rigidbody Rigidbody {get; private set;}
        public int EntityId { get; set; }
        
        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }
    }
}