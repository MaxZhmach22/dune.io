using UniRx;
using UnityEngine;


namespace Dune.IO
{
    public class ScoreService
    {
        private float _score;
        private Configuration _configuration;
        
        public Subject<float> ScoreChanged { get; private set; } = new Subject<float>();
        
        public ScoreService(Configuration configuration)
        {
            _score = configuration.StartScore;
            _configuration = configuration;
        }
        
        public void AddScore(float score)
        {
            _score += score;
            ScoreChanged.OnNext(_score);
            CheckWin(_score);
            
        }

        private void CheckWin(float score)
        {
            if (score < _configuration.FinalGoal) return;
            
            Time.timeScale = 0;
            Debug.Log("WIN");
        }

        public bool RemoveScore(int score)
        {
            if (_score < score)
            {
                return false;
            }
            _score -= score;
            ScoreChanged.OnNext(_score);
            return true;      
        }

        public float GetScore() => _score;
    }
}