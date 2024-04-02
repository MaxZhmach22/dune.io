using Leopotam.EcsLite;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Dune.IO
{
    public class UiService
    {
        private readonly EcsWorld _world;
        
        private readonly Button _restartButton;
        private readonly TMP_Text _scoreText;
        private readonly Button _starButton;
        private readonly GameObject _startPanel;
        
        private readonly Configuration _configuration;
        private readonly ScoreService _scoreService;

        public UiService(
            EcsWorld world,
            Button restartButton,
            TMP_Text scoreText,
            Button starButton,
            GameObject startPanel,
            Configuration configuration,
            ScoreService scoreService
            )
        {
            _world = world;
            _restartButton = restartButton;
            _scoreText = scoreText;
            _starButton = starButton;
            _startPanel = startPanel;
            _configuration = configuration;
            _scoreService = scoreService;
        }


        public UiService Init(MonoBehaviour monoBehaviour)
        {
            _restartButton.OnClickAsObservable()
                .Subscribe(_ => SceneManager.LoadScene(SceneManager.GetActiveScene().name))
                .AddTo(monoBehaviour);

            _starButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _startPanel.SetActive(false);
                })
                .AddTo(monoBehaviour);
            
            _scoreService.ScoreSubject
                .DoOnSubscribe(() =>
                {
                    _scoreText.text = _scoreService.GetScore().ToString();
                }) 
                .Subscribe(score => _scoreText.text = score.ToString())
                .AddTo(monoBehaviour);
            
            return this;
        }
    }
}