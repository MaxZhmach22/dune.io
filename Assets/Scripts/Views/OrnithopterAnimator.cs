using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;


namespace Dune.IO
{
    public class OrnithopterAnimator : MonoBehaviour
    {
        [field: Foldout("References")] [field: SerializeField] public Animator Animator { get; private set; }
        
        private void Awake()
        {
            Animator ??= GetComponent<Animator>();
        }
    }

}
