using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Dune.IO
{
    public class OrnithopterLandingSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<OrnithopterComponent, LandingComponent>> _filter = default;
        private TweenerCore<Vector3, Vector3, VectorOptions> _landingTween;

        private readonly EcsSharedInject<Configuration> _configuration = default;


        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var ornithopterComponent = ref _filter.Pools.Inc1.Get(entity);

                if (!ornithopterComponent.IsLanding)
                {
                    ornithopterComponent.IsLanding = true;
                    var rigidbody = ornithopterComponent.OrnithopterView.Rigidbody;
                    var stopTween = DOTween.To(() => rigidbody.velocity, x => rigidbody.velocity = x, Vector3.zero,
                        _configuration.Value.VelocityDecreasingTime);
                    stopTween.OnComplete(() =>
                    {
                        var isOnTheGround = false;
                        rigidbody.isKinematic = true;
                        _landingTween = rigidbody.transform.DOMoveY(0, 1f)
                            .SetEase(Ease.OutSine)
                            .OnStepComplete(async () =>
                            {
                                if (isOnTheGround) return;
                                isOnTheGround = true;
                                _landingTween.Pause();
                                await UniTask.Delay(_configuration.Value.LandingTime);
                                _landingTween.Play();
                            })
                            .SetLoops(2, LoopType.Yoyo)
                            .OnComplete(() =>
                            {
                                ref var ornithopterComponent = ref _filter.Pools.Inc1.Get(entity);
                                ornithopterComponent.IsLanding = false;
                                rigidbody.isKinematic = false;
                                _filter.Pools.Inc2.Del(entity);
                            });
                    });
                }
            }
        }
    }
}