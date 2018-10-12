using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Meta;
using System.IO;

namespace LibrarianMI3
{
    public class ScoreManager : MonoBehaviour
    {
        float _nTrials = 0;
        int _score = 0;

        [SerializeField]
        private int _maxTrials = 30;
        float _timeRemaining = 25f;
        bool _gameOver = false;
        bool _gameOn = false;


        float _trialTimeBonus = 10f;
        float _decayRate = 0.08f;
        float _minTimeBonus = 1f;

        public UnityEvent GameOver = new UnityEvent();

        public UnityEvent ScoreDetected = new UnityEvent();

        [SerializeField]
        GameObject pod;

        [SerializeField]
        Text _clockViewer;
        [SerializeField]
        Text _scoreViewer;

        public int MaxTrials
        {
            get { return _maxTrials; }
        }

        // Use this for initialization
        void Start()
        {
            //GameObject.Find("Pod")
            _clockViewer.text = "";
            pod.GetComponent<PodController>().OnPodHit.AddListener(TrialSuccess);
            //GameObject.Find("MetaCameraRig").GetComponent<SlamLocalizer>().onSlamMappingComplete.AddListener(FirstTrial);
            //GameObject.Find("MetaHands").GetComponent<HandsProvider>().events.OnGrab.AddListener(FirstGrab);
        }

        //private void FirstTrial()
        //{
        //    _gameOn = true;
        //}

        private void TrialSuccess()
        {
            ScoreDetected.Invoke();
            if (_score == 0) { _gameOn = true; } //Start the game
            _nTrials += 1.0f;
            _timeRemaining += _trialTimeBonus * Mathf.Exp(-_nTrials * _decayRate) + _minTimeBonus;
            _score += 1;
            _scoreViewer.text = "Score: " + _score.ToString() + " Books";

            //if(_nTrials >= _maxTrials)
            //{
            //    _gameOn = false;
            //    _gameOver = true;
            //    GameOver.Invoke();
            //}
        }
        // Update is called once per frame
        void Update()
        {
            if (_gameOn)
            {
                float startTime = _timeRemaining;
                _timeRemaining -= Time.deltaTime;
                _clockViewer.text = "Time Remaining: " + _timeRemaining.ToString("F") + " (s)";
                if (_timeRemaining < 0f && startTime > 0f)
                {
                    _gameOn = false;
                    _gameOver = true;
                    GameOver.Invoke();

                }
            }
            else
            {
                if (_gameOver)
                {
                    _clockViewer.text = "Time Remaining: 0.0 (s)";
                }
                else
                {
                    _clockViewer.text = "Time Remaining: " + _timeRemaining.ToString("F") + " (s)";
                }

            }
            //if(_score > 200)
            //{
            //    GameOver.Invoke();
            //}
        }

        public float TimeRemaining
        {
            get { return _timeRemaining; }
        }

        public int Score
        {
            get { return _score; }
        }
    }
}
