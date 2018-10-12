using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Meta;
using Meta.HandInput;
using Research;

namespace RotationTest
{
    public class RotationTestDataLog : MonoBehaviour
    {
        //User Data:
        [SerializeField]
        private string _versionName;
        private string _playerName;
        private string _filename;


        private int _trialNum = 0;

        private float _trailStartTime = 0f;

        // Scene Information
        //[SerializeField]
        //private ReverseLibrarian _reverseLibrarian;

        private Transform _headset;

        [SerializeField]
        private GameObject _targetObject;


        //Data Logging
        private DataLogger _dataLogger = new DataLogger();
        private DataRow_RotationTest _dataRow = new DataRow_RotationTest();



        // MonoBehavior Methods:
        void Awake()
        {
            _dataLogger.IsOpen = false;
            //_reverseLibrarian.PlayerNamed.AddListener(InitiateDataLogging);
            _headset = GameObject.Find("MetaCameraRig").transform;
            //_targetObject = GameObject.Find("RotationTestObject");
            GameObject.Find("RotationTestController").GetComponent<RotationExperiment>().StartTrial.AddListener(LogTrialStart);
            GameObject.Find("RotationTestController").GetComponent<RotationExperiment>().EndTrial.AddListener(LogTrialEnd);
            GameObject.Find("RotationTestController").GetComponent<TextUI>().PlayerNamed.AddListener(InitiateDataLogging);
        }


        private void OnApplicationQuit()
        {
            _dataLogger.CloseDataLogger();
        }

        //Member Methods:
        private void InitiateDataLogging(string name)
        {
            _playerName = name;
            _filename = "RotationTest_" + _versionName + "_" + _playerName + "_" + System.DateTime.Now.ToString("hh_mm_dd_MM");

            if (Directory.Exists(@"D:\"))
            {
                _dataLogger.CreateTable(_filename, _dataRow.GetFields(), @"D:\Hands_KPI_Testing");
            }
            else
            {
                _dataLogger.CreateTable(_filename, _dataRow.GetFields(), @"C:\Hands_KPI_Testing");
            }
            _dataLogger.IsOpen = true;


        }


        private void LogTrialStart()
        {
            _trialNum += 1;
            _trailStartTime = Time.time;

            _dataRow.EventType = "TrialStart";
            _dataRow.TrialNum = _trialNum;
            _dataRow.TrialTime = 0f;
            _dataRow.TotalTime = Time.time;

            _dataRow.ObjectTransform = _targetObject.transform;
            _dataRow.HeadTransform = _headset;

            LogData(_dataRow);
        }


        private void LogTrialEnd()
        {

            _dataRow.EventType = "TrialEnd";
            _dataRow.TrialNum = _trialNum;
            _dataRow.TrialTime = Time.time - _trailStartTime;
            _dataRow.TotalTime = Time.time;

            _dataRow.ObjectTransform = _targetObject.transform;
            _dataRow.HeadTransform = _headset;


            LogData(_dataRow);
        }

            private void LogData(DataRow_RotationTest d)
        {
            if (_dataLogger.IsOpen)
            {
                _dataLogger.LogData(d.GetData());
            }
        }
    }
}
