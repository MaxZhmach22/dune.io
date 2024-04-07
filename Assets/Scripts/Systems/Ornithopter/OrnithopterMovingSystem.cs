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
                ornithopterComponent.OrnithopterView.Rigidbody.AddForce(
                    new Vector3(forceDirection.x, 0, forceDirection.y) * _configuration.Value.OrnithopterSpeedForce, ForceMode.VelocityChange);
                var clampedVelocity = Vector3.ClampMagnitude(ornithopterComponent.OrnithopterView.Rigidbody.velocity, _configuration.Value.OrnithopterMaxSpeed);
                ornithopterComponent.OrnithopterView.Rigidbody.velocity = clampedVelocity;
            }
        }
    }
}