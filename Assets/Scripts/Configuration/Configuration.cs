﻿using NaughtyAttributes;
using UnityEngine;

namespace Dune.IO
{
    [CreateAssetMenu(fileName = "Config", menuName = "Configuration/Dune.io")]
    public sealed class Configuration : ScriptableObject
    {
        public string temp = "temp";
        [field: BoxGroup("Harvester settings")] [field: SerializeField] public Material HarvesterOutlineMaterial { get; private set; }
    }
}