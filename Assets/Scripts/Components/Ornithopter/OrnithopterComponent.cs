using UnityEngine;

namespace Dune.IO
{
    public struct OrnithopterComponent
    {
        public Ornithopter OrnithopterView;
        public Vector3 PreviousLookDirection;
        public bool IsLanding;
        public bool IsCarryingHarvester;
    }
}