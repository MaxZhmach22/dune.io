using System;
using Leopotam.EcsLite;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Dune.IO
{
    public class UiService
    {
        private readonly EcsWorld _world;

        private readonly TMP_Text _scoreText;
        
        private readonly Button _restartButton;
        private readonly Button _starButton;
        private readonly Button _buyHarvesterButton;
        private readonly Button _fireButton;
        private readonly Button _landingButton;
        
        private readonly GameObject _startPanel;
        private readonly Configuration _configuration;
        private readonly ScoreService _scoreService;

        public UiService(
            EcsWorld world,
            Button restartButton,
            TMP_Text scoreText,
            Button starButton,
            Button buyHarvesterButton,
            Button fireButton,
            Button landingButton,
            GameObject startPanel,
            Configuration configuration,
            ScoreService scoreService
        )
        {
            _world = world;
            _restartButton = restartButton;
            _starButton = starButton;
            _buyHarvesterButton = buyHarvesterButton;
            _fireButton = fireButton;
            _landingButton = landingButton;
            _startPanel = startPanel;
            _configuration = configuration;
            _scoreService = scoreService;
            _scoreText = scoreText;
        }

        public UiService Init(MonoBehaviour monoBehaviour)
        {
            var ornithopter = Object.FindObjectOfType<Ornithopter>();
            
            _restartButton.OnClickAsObservable()
                .Subscribe(_ => SceneManager.LoadScene(SceneManager.GetActiveScene().name))
                .AddTo(monoBehaviour);

            _starButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _startPanel.SetActive(false);
                    ref var harvesterComponent = ref _world.GetPool<BuyHarvesterComponent>().Add(_world.NewEntity());
                    harvesterComponent.Price = _configuration.StartHarvesterPrice;
                })
                .AddTo(monoBehaviour);

            _fireButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    Debug.Log("Fire button clicked");
                })
                .AddTo(monoBehaviour);
            
            _landingButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    var pool = _world.GetPool<LandingComponent>();
                    if (!pool.Has(ornithopter.EntityId))
                    {
                        Debug.Log("Landing button clicked");
                        pool.Add(ornithopter.EntityId);
                    }
                    else
                    {
                        Debug.Log("Landing component is already added to the entity");
                    }
                })
                .AddTo(monoBehaviour);

            _buyHarvesterButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    ref var harvesterComponent = ref _world.GetPool<BuyHarvesterComponent>().Add(_world.NewEntity());
                    harvesterComponent.Price = _configuration.StartHarvesterPrice;
                })
                .AddTo(monoBehaviour);
            
            _scoreService.ScoreChanged
                .DoOnSubscribe(() => { _scoreText.text = Math.Round(_scoreService.GetScore()).ToString(); })
                .Subscribe(score => { _scoreText.text = Math.Round(score).ToString(); })
                .AddTo(monoBehaviour);

            return this;
        }
    }
}