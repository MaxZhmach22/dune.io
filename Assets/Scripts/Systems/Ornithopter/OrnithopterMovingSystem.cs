using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Dune.IO
{
    public class OrnithopterMovingSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<OrnithopterComponent>, Exc<LandingComponent, OutOfBorderComponent>>
            _movingForwardFilter = default;

        private readonly EcsFilterInject<Inc<OrnithopterComponent, OutOfBorderComponent>, Exc<LandingComponent>>
            _movingBackwardFilter = default;

        private readonly EcsFilterInject<Inc<OrnithopterComponent>> _ornithopterFilter = default;
        private readonly EcsSharedInject<Configuration> _configuration = default;
        private readonly EcsPoolInject<OutOfBorderComponent> _outOfBorderPoll = default;
        private readonly FixedJoystick _joystick;

        public OrnithopterMovingSystem(FixedJoystick joystick)
        {
            _joystick = joystick;
        }

        public void Run(IEcsSystems systems)
        {
            CheckBorders();
            Move();
        }

        private void CheckBorders()
        {
            foreach (var entity in _ornithopterFilter.Value)
            {
                ref var ornithopterComponent = ref _ornithopterFilter.Pools.Inc1.Get(entity);
                var distance = Vector3.Distance(ornithopterComponent.OrnithopterView.transform.position, Vector3.zero);
                if (distance > 60)
                {
                    if (_outOfBorderPoll.Value.Has(entity)) continue;
                    _outOfBorderPoll.Value.Add(entity);
                    Debug.Log("Out of border!");
                }
                else
                {
                    if (!_outOfBorderPoll.Value.Has(entity)) continue;
                    _outOfBorderPoll.Value.Del(entity);
                    Debug.Log("Back to the border!");
                }
            }
        }

        private void Move()
        {
            foreach (var ornithopterEntity in _ornithopterFilter.Value)
            {
                ref var ornithopterComponent = ref _movingForwardFilter.Pools.Inc1.Get(ornithopterEntity);
                var forceDirection = _joystick.Direction;
                var localDirection = new Vector3(forceDirection.x, 0, forceDirection.y);
                var forceVector =
                    ornithopterComponent.OrnithopterView.transform.TransformDirection(localDirection) *
                    _configuration.Value.OrnithopterSpeedForce;
                
                foreach (var entity in _movingForwardFilter.Value)
                {
                    ornithopterComponent.OrnithopterView.Rigidbody.AddForce(forceVector, ForceMode.VelocityChange);
                    var clampedVelocity = Vector3.ClampMagnitude(
                        ornithopterComponent.OrnithopterView.Rigidbody.velocity,
                        _configuration.Value.OrnithopterMaxSpeed);
                    ornithopterComponent.OrnithopterView.Rigidbody.velocity = clampedVelocity;
                    if (forceDirection == Vector2.zero)
                    {
                        ornithopterComponent.OrnithopterView.ParentModel.transform.forward =
                            ornithopterComponent.PreviousLookDirection;
                    }
                    else
                    {
                        var normalized = forceVector.normalized;
                        var currentRotation = ornithopterComponent.OrnithopterView.ParentModel.transform.rotation;
                        var targetRotation = Quaternion.LookRotation(normalized);
                        ornithopterComponent.OrnithopterView.ParentModel.transform.rotation = Quaternion.Slerp(
                            currentRotation,
                            targetRotation, _configuration.Value.OrnithopterRotationSpeed * Time.deltaTime);
                        ornithopterComponent.PreviousLookDirection =
                            ornithopterComponent.OrnithopterView.ParentModel.transform.forward;
                    }
                }

                foreach (var entity in _movingBackwardFilter.Value)
                {
                    ornithopterComponent.OrnithopterView.Rigidbody.AddForce(forceVector * -10, ForceMode.VelocityChange);
                    var clampedVelocity = Vector3.ClampMagnitude(
                        ornithopterComponent.OrnithopterView.Rigidbody.velocity,
                        _configuration.Value.OrnithopterMaxSpeed);
                    ornithopterComponent.OrnithopterView.Rigidbody.velocity = clampedVelocity;
                }
            }
        }
    }
}