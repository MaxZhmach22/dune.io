using UniRx;


namespace Dune.IO
{
    public class ScoreService
    {
        private float _score;
        
        public Subject<float> ScoreChanged { get; private set; } = new Subject<float>();
        
        public ScoreService(Configuration configuration)
        {
            _score = configuration.StartScore;
        }
        
        public void AddScore(float score)
        {
            _score += score;
            ScoreChanged.OnNext(_score);
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