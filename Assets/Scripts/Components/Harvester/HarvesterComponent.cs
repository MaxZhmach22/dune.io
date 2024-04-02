using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;


namespace Dune.IO
{
    public struct HarvesterComponent
    {   
        public Harvester HarvesterView;
        public GameObject Target;
        public TweenerCore<Vector3,Vector3,VectorOptions> Tween;
        public int HarvesterId;
    }
}

