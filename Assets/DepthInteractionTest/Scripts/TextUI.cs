using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Meta;

namespace DepthTest
{
    public class PlayerNameEntered : UnityEvent<string>
    {
    }


    public class TextUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject _InstructionsDisplay;

        [SerializeField]
        private GameObject _NameDisplay;

        [SerializeField]
        private Text _PlayerNameReadout;
        private string _playerName;

        [SerializeField]
        private Text _ScoreDisplay;

        private int _trialNum = 1;

        [SerializeField]
        private GameObject _EndDisplay;

        public PlayerNameEntered PlayerNamed = new PlayerNameEntered();

        public UnityEvent InstructionsComplete = new UnityEvent();




        void Start()
        {
            GameObject.Find("MetaCameraRig").GetComponent<SlamLocalizer>().onSlamMappingComplete.AddListener(Initialize);
            GameObject.Find("SceneControl").GetComponent<DepthExperiment>().UpdateScore.AddListener(TrialComplete);
            GameObject.Find("SceneControl").GetComponent<DepthExperiment>().GameOver.AddListener(GameOver);
        }

        private void GameOver()
        {
            _EndDisplay.SetActive(true);
            StartCoroutine(PauseThenQuit());
        }

        private void TrialComplete()
        {
            _trialNum += 1;
            _ScoreDisplay.text = "Trial " + _trialNum.ToString();
        }

        private void Initialize()
        {
            _InstructionsDisplay.SetActive(false);
            _NameDisplay.SetActive(false);
            _EndDisplay.SetActive(false);

            StartCoroutine(DisplayInstructions());
        }


        private IEnumerator DisplayInstructions()
        {

            _NameDisplay.SetActive(true);

            _PlayerNameReadout.text = "Enter your name:\n";
            while (!Input.GetKeyDown(KeyCode.Return))
            {
                if ((Input.inputString) == "\b" && (_playerName.Length > 0))
                {
                    _playerName = _playerName.Substring(0, (_playerName.Length - 1));
                    _PlayerNameReadout.text = "Enter your name:\n" + _playerName;
                }
                else
                {
                    _playerName += Input.inputString;
                    _PlayerNameReadout.text = "Enter your name:\n" + _playerName;
                }
                yield return null;
            }
            PlayerNamed.Invoke(_playerName);

            _NameDisplay.SetActive(false);
            _InstructionsDisplay.SetActive(true);
            yield return new WaitForSecondsRealtime(4f);
            _InstructionsDisplay.SetActive(false);

            InstructionsComplete.Invoke();
            yield return null;
        }

        private IEnumerator PauseThenQuit()
        {
            float delay = 3f;
            yield return new WaitForSecondsRealtime(delay);
            Application.Quit();
        }

    }
}
