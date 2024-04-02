using Leopotam.EcsLite;
using UniRx;

namespace Dune.IO
{
    public class ScoreService
    {
        private float _score;

        public Subject<float> ScoreSubject { get; } = new Subject<float>();

        public ScoreService(Configuration configuration)
        {
            _score = configuration.StartScore;
        }
        
        public void AddScore(float score)
        {
            _score += score;
            ScoreSubject.OnNext(score);
        }
        
        public bool RemoveScore(float score)
        {
            if (_score < score)
            {
                return false;
            }
            else
            {
                _score -= score;
                ScoreSubject.OnNext(score);
                return true;               
            }
        }

        public float GetScore() => _score;
    }
}