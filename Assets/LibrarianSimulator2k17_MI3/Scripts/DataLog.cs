using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Meta;
using Meta.HandInput;
using Research;

namespace LibrarianMI3
{
    // Class to Log all data for the Librarian Game
    public class DataLog : MonoBehaviour
    {
        // Scene Information
        [SerializeField]
        private ReverseLibrarian _reverseLibrarian;

        [SerializeField]
        private Shelf _shelfScript;

        private Transform _headset;

        private GameObject _targetBook;

        private int _trialNum = 0;
        private float _trailStartTime = 0f;

        //Meta Data:
        [SerializeField]
        private string _versionName;
        private string _playerName;
        private string _filename;


        //Data Logging
        private DataLogger _dataLogger = new DataLogger();
        private DataRow _dataRow = new DataRow();

        bool _useProjection = false;
        string _testType = "";

        // MonoBehavior Methods:
        void Awake()
        {
            _dataLogger.IsOpen = false;
            _useProjection = Random.Range(-1f, 1f) < 0f;
            if(_useProjection)
            {
                _testType = "Projective";
            }
            else
            {
                _testType = "Standard";
            }
            _reverseLibrarian.PlayerNamed.AddListener(InitiateDataLogging);
            _headset = GameObject.Find("MetaCameraRig").transform;
            SetupEventListeners();
            
        }

        private void SetupEventListeners()
        {
            _reverseLibrarian.TrailStarted.AddListener(NewTrial);
            HandsProvider hp = GameObject.Find("MetaHands").GetComponent<HandsProvider>();
            hp.events.OnGrab.AddListener(GrabDetected);
            hp.events.OnRelease.AddListener(ReleaseDetected);
            //hp.events.OnHandEnter.AddListener(SetHandFeature);
        }


        private void SetHandFeature(Hand h)
        {
            //h.Palm.IsProjective = _useProjection;
        }

        private void OnApplicationQuit()
        {
            _dataLogger.CloseDataLogger();
        }

        //Member Methods:
        private void InitiateDataLogging(string name)
        {
            _playerName = name;
            _filename = "ReverseLibrarian_" + "_" + _versionName  + "_" + _testType + "_" + _playerName + "_" + System.DateTime.Now.ToString("hh_mm_dd_MM");



            //if (Directory.Exists(@"D:\"))
            //{
            //    _dataLogger.CreateTable(_filename, _dataRow.GetFields(), @"D:\Hands_KPI_Testing");
            //}
            //else
            //{

            //}
            _dataLogger.CreateTable(_filename, _dataRow.GetFields(), Application.dataPath + @"\AnalyticsResults");
            _dataLogger.IsOpen = true;

        }

        private void NewTrial(GameObject book)
        {
            UpdateTargetBook(book);
            _trialNum += 1;
            _trailStartTime = Time.time;

            _dataRow.EventType = "NewTrial";
            _dataRow.TrialNum = _trialNum;
            _dataRow.TrialTime = 0f;
            _dataRow.TotalTime = Time.time;
            _dataRow.isCorrectObj = false;
            _dataRow.ObjectSpacing = _shelfScript.BookSpacing;
            _dataRow.ObjectPosition = _targetBook.transform.position;
            _dataRow.ClosestPoint = Vector3.zero;
            _dataRow.PalmPosition = Vector3.zero;
            _dataRow.AnchorPosition = Vector3.zero;
            _dataRow.HeadTransform = _headset;

            LogData(_dataRow);
        }

        private void UpdateTargetBook(GameObject book)
        {
            if (_targetBook != null)
            {
                _targetBook.GetComponent<GrabInteraction>().Events.HoverStart.RemoveListener(HoverStart);
            }
            _targetBook = book;
            _targetBook.GetComponent<GrabInteraction>().Events.HoverStart.AddListener(HoverStart);
        }

        private void HoverStart(MetaInteractionData m)
        {
            _dataRow.EventType = "HoverStart";
            _dataRow.TotalTime = Time.time;
            _dataRow.TrialTime = _dataRow.TotalTime - _trailStartTime;
            _dataRow.isCorrectObj = true;
            _dataRow.ObjectPosition = _targetBook.transform.position;
            _dataRow.ClosestPoint = Vector3.zero;
            _dataRow.PalmPosition = Vector3.zero;
            _dataRow.AnchorPosition = Vector3.zero;
            LogData(_dataRow);
        }

        private void GrabDetected(Hand h)
        {
            GameObject grabObject = h.Palm.NearObjects[0].gameObject;

            _dataRow.EventType = "Grab";
            _dataRow.TotalTime = Time.time;
            _dataRow.TrialTime = _dataRow.TotalTime - _trailStartTime;
            _dataRow.isCorrectObj = (grabObject.name == _targetBook.name);
            _dataRow.ObjectPosition = grabObject.transform.position;
            _dataRow.ClosestPoint = grabObject.GetComponent<Collider>().ClosestPointOnBounds(h.Data.GrabAnchor);
            _dataRow.PalmPosition = h.Palm.Position;
            _dataRow.AnchorPosition = h.Data.GrabAnchor;

            LogData(_dataRow);
        }

        private void ReleaseDetected(Hand h)
        {
            GameObject grabObject = h.Palm.NearObjects[0].gameObject;
            _dataRow.EventType = "Release";
            _dataRow.TotalTime = Time.time;
            _dataRow.TrialTime = _dataRow.TotalTime - _trailStartTime;
            _dataRow.isCorrectObj = (grabObject.name == _targetBook.name);
            _dataRow.ObjectPosition = grabObject.transform.position;
            _dataRow.ClosestPoint = grabObject.GetComponent<Collider>().ClosestPointOnBounds(h.Data.GrabAnchor);
            _dataRow.PalmPosition = h.Palm.Position;
            _dataRow.AnchorPosition = h.Data.GrabAnchor;

            LogData(_dataRow);
        }


        private void LogData(DataRow d)
        {
            if (_dataLogger.IsOpen)
            {
                _dataLogger.LogData(d.GetData());
            }
        }

    }
}
