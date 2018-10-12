using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DepthTest
{
    public class DepthExperiment : MonoBehaviour
    {
        [SerializeField]
        private int _nTrials;

        private int _trialNum = 0;

        public UnityEvent GameOver = new UnityEvent();

        public UnityEvent StartNewTrial = new UnityEvent();

        public UnityEvent LogNewTrial = new UnityEvent();

        public UnityEvent UpdateScore = new UnityEvent();

        // Use this for initialization
        void Start()
        {
            GameObject.Find("TargetPlate").GetComponent<TargetPlate>().OnPlateHit.AddListener(NewTrial);
        }


        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                NewTrial();
            }
        }


        private void NewTrial()
        {
            Debug.Log("DepthExperiment New Trial");
            StartNewTrial.Invoke();
            LogNewTrial.Invoke();
            UpdateScore.Invoke();

            _trialNum += 1;
            if (_trialNum >= _nTrials)
            {
                GameOver.Invoke();
            }
        }
    }
}