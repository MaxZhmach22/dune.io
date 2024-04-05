using System;
using Unity.VisualScripting;

namespace Dune.IO
{
    public class ScoreService
    {
        private int _score;
        
        public Action<int> OnScoreChanged;

        public ScoreService(Configuration configuration)
        {
            _score = configuration.StartScore;
        }
        
        public void AddScore(int score)
        {
            _score += score;
            OnScoreChanged?.Invoke(_score);
        }
        
        public bool RemoveScore(int score)
        {
            if (_score < score)
            {
                return false;
            }
            _score -= score;
            OnScoreChanged?.Invoke(_score);
            return true;      
        }

        public float GetScore() => _score;
    }
}