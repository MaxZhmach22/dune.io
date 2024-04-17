using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;


namespace Dune.IO
{
    public class OrnithopterMovingSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<OrnithopterComponent>, Exc<LandingComponent>> _filter = default;
        private readonly EcsSharedInject<Configuration> _configuration = default;
        private readonly FixedJoystick _joystick;

        public OrnithopterMovingSystem(FixedJoystick joystick)
        {
            _joystick = joystick;
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var ornithopterComponent = ref _filter.Pools.Inc1.Get(entity);
                var forceDirection = _joystick.Direction;
                var localDirection = new Vector3(forceDirection.x, 0, forceDirection.y);
                var forceVector = ornithopterComponent.OrnithopterView.transform.TransformDirection(localDirection) * _configuration.Value.OrnithopterSpeedForce;
                ornithopterComponent.OrnithopterView.Rigidbody.AddForce(forceVector, ForceMode.VelocityChange);
                var clampedVelocity = Vector3.ClampMagnitude(ornithopterComponent.OrnithopterView.Rigidbody.velocity, _configuration.Value.OrnithopterMaxSpeed);
                ornithopterComponent.OrnithopterView.Rigidbody.velocity = clampedVelocity;
                if (forceDirection == Vector2.zero)
                {
                    ornithopterComponent.OrnithopterView.ParentModel.transform.forward =
                        ornithopterComponent.PreviousLookDirection;
                }
                else
                {
                    var normalized = forceVector.normalized;
                    ornithopterComponent.OrnithopterView.ParentModel.transform.forward = normalized;
                    ornithopterComponent.PreviousLookDirection = normalized;
                }
            }
        }
    }
}