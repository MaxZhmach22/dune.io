using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UniRx;
using UniRx.Triggers;
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

        [field: Foldout("Reference")]
        [field: SerializeField]
        public List<HarvesterLandingPoint> HarvesterLandingPoint { get; private set; } = new();
        
        private void Awake()
        {
            HarvesterLandingPoint.AddRange(GetComponentsInChildren<HarvesterLandingPoint>(true));
        }
    }
}