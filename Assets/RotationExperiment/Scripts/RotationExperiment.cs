using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RotationTest
{
    public class RotationExperiment : MonoBehaviour
    {
        public UnityEvent EndTrial = new UnityEvent();

        public UnityEvent StartTrial = new UnityEvent();

        public UnityEvent EndExperiment = new UnityEvent();

        [SerializeField]
        private int _nTrials = 24;
        private int _trialNum = 0;

        [SerializeField]
        GameObject _testObject;
        Vector3 _startPosition = new Vector3(0f, -0.1f, 0.4f);

        void Start()
        {
            GameObject.Find("RotationTestController").GetComponent<TextUI>().InstructionsComplete.AddListener(StartTest);
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(LoadNewTrial());
                EndTrial.Invoke();

            }


        }
        private void StartTest()
        {
            _testObject.SetActive(true);
            //_testObject.transform.rotation = Random.rotationUniform;
            _testObject.transform.eulerAngles = new Vector3(Random.Range(-30f, 30f), Random.Range(-30f, 30f), Random.Range(-30f, 30f));
            _testObject.transform.position = _startPosition;
            StartTrial.Invoke();
        }

        private IEnumerator LoadNewTrial()
        {
            _testObject.SetActive(true);
            _trialNum += 1;
            yield return null;

            _testObject.transform.rotation = Random.rotationUniform;
            _testObject.transform.position = _startPosition;
            if (_trialNum >= _nTrials)
            {
                _testObject.SetActive(false);
                EndExperiment.Invoke();
            }
            else
            {
                StartTrial.Invoke();
            }

            yield return null;

        }


    }
}
