using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Meta;
using Meta.HandInput;
using Research;


namespace DepthTest
{
    public class DepthTestDataLog : MonoBehaviour
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

        private GameObject _targetObject;


        //Data Logging
        private DataLogger _dataLogger = new DataLogger();
        private DataRow_DepthTest _dataRow = new DataRow_DepthTest();



        // MonoBehavior Methods:
        void Awake()
        {
            _dataLogger.IsOpen = false;
            //_reverseLibrarian.PlayerNamed.AddListener(InitiateDataLogging);
            _headset = GameObject.Find("MetaCameraRig").transform;
            StartCoroutine(SetupEventListeners());

        }

        private IEnumerator SetupEventListeners()
        {
            yield return null;
            //_reverseLibrarian.TrailStarted.AddListener(NewTrial);
            HandsProvider hp = GameObject.Find("MetaHands").GetComponent<HandsProvider>();
            hp.events.OnGrab.AddListener(GrabDetected);
            hp.events.OnRelease.AddListener(ReleaseDetected);
            GameObject.Find("SceneControl").GetComponent<TextUI>().PlayerNamed.AddListener(InitiateDataLogging);

            GrabObjects objs = GameObject.Find("SceneControl").GetComponent<GrabObjects>();
            _targetObject = objs.TargetObject;
            _targetObject.GetComponent<GrabInteraction>().Events.HoverStart.AddListener(HoverStart);

            GameObject.Find("SceneControl").GetComponent<DepthExperiment>().LogNewTrial.AddListener(LogNewTrial);

            GameObject.Find("SceneControl").GetComponent<GrabPoseDetector>().GrabPoseOn.AddListener(LogGrabPose);
        }
        private void OnApplicationQuit()
        {
            _dataLogger.CloseDataLogger();
        }

        //Member Methods:
        private void InitiateDataLogging(string name)
        {
            _playerName = name;
            _filename = "DepthTest_" + _versionName + "_" + _playerName + "_" + System.DateTime.Now.ToString("hh_mm_dd_MM");

            if (Directory.Exists(@"D:\"))
            {
                _dataLogger.CreateTable(_filename, _dataRow.GetFields(), @"D:\Hands_KPI_Testing");
            }
            else
            {
                _dataLogger.CreateTable(_filename, _dataRow.GetFields(), @"C:\Hands_KPI_Testing");
            }
            _dataLogger.IsOpen = true;

            LogNewTrial();

        }

        private void LogNewTrial()
        {
            Debug.Log("DataLog New Trial");
            _trialNum += 1;
            _trailStartTime = Time.time;

            _dataRow.EventType = "NewTrial";
            _dataRow.TrialNum = _trialNum;
            _dataRow.TrialTime = 0f;
            _dataRow.TotalTime = Time.time;
            _dataRow.isCorrectObj = false;
            //_dataRow.ObjectSpacing = _shelfScript.BookSpacing;
            _dataRow.ObjectPosition = _targetObject.transform.position;
            _dataRow.ClosestPoint = Vector3.zero;
            _dataRow.PalmPosition = Vector3.zero;
            _dataRow.AnchorPosition = Vector3.zero;
            _dataRow.HeadTransform = _headset;

            LogData(_dataRow);
        }

        private void LogGrabPose(Hand h)
        {
            _dataRow.EventType = "HoverStart";
            _dataRow.TotalTime = Time.time;
            _dataRow.TrialTime = _dataRow.TotalTime - _trailStartTime;
            _dataRow.isCorrectObj = false;
            if(h.Palm.NearObjects.Count > 0)
            {
                GameObject obj = h.Palm.NearObjects[0].gameObject;
                _dataRow.isCorrectObj = (obj.name == _targetObject.name);
            }

            _dataRow.ObjectPosition = _targetObject.transform.position;
            _dataRow.ClosestPoint = Vector3.zero;
            _dataRow.PalmPosition = Vector3.zero;
            _dataRow.AnchorPosition = Vector3.zero;
            _dataRow.HeadTransform = _headset;
        }

        private void HoverStart(MetaInteractionData m)
        {
            _dataRow.EventType = "HoverStart";
            _dataRow.TotalTime = Time.time;
            _dataRow.TrialTime = _dataRow.TotalTime - _trailStartTime;
            _dataRow.isCorrectObj = true;
            //_dataRow.ObjectPosition = _targetBook.transform.position;
            _dataRow.ObjectPosition = _targetObject.transform.position;
            _dataRow.ClosestPoint = Vector3.zero;
            _dataRow.PalmPosition = Vector3.zero;
            _dataRow.AnchorPosition = Vector3.zero;
            _dataRow.HeadTransform = _headset;


            LogData(_dataRow);
        }

        private void GrabDetected(Hand h)
        {
            GameObject grabObject = h.Palm.NearObjects[0].gameObject;

            _dataRow.EventType = "Grab";
            _dataRow.TotalTime = Time.time;
            _dataRow.TrialTime = _dataRow.TotalTime - _trailStartTime;
            _dataRow.isCorrectObj = (grabObject.name == _targetObject.name);
            _dataRow.ObjectPosition = grabObject.transform.position;
            _dataRow.ClosestPoint = grabObject.GetComponent<Collider>().ClosestPointOnBounds(h.Data.GrabAnchor);
            _dataRow.PalmPosition = h.Palm.Position;
            _dataRow.AnchorPosition = h.Data.GrabAnchor;
            _dataRow.HeadTransform = _headset;

            LogData(_dataRow);
        }

        private void ReleaseDetected(Hand h)
        {
            GameObject grabObject = h.Palm.NearObjects[0].gameObject;
            _dataRow.EventType = "Release";
            _dataRow.TotalTime = Time.time;
            _dataRow.TrialTime = _dataRow.TotalTime - _trailStartTime;
            _dataRow.isCorrectObj = (grabObject.name == _targetObject.name);
            _dataRow.ObjectPosition = grabObject.transform.position;
            _dataRow.ClosestPoint = grabObject.GetComponent<Collider>().ClosestPointOnBounds(h.Data.GrabAnchor);
            _dataRow.PalmPosition = h.Palm.Position;
            _dataRow.AnchorPosition = h.Data.GrabAnchor;
            _dataRow.HeadTransform = _headset;

            LogData(_dataRow);
        }


        private void LogData(DataRow_DepthTest d)
        {
            if (_dataLogger.IsOpen)
            {
                _dataLogger.LogData(d.GetData());
            }
        }
    }
}
