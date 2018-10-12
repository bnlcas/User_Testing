using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using Meta;
using Meta.HandInput;

namespace Research
{
    public class HandFrameLog : MonoBehaviour
    {
        private string _directory;

        [SerializeField]
        private string _filename;

        private StreamWriter _dataWriter;

        private Transform _headset;
        private List<Hand> _activeHands = new List<Hand>();


        private Vector3 palm1;
        private bool grab1;
        private Vector3 palm2;
        private bool grab2;

        private void Start()
        {
            _headset = GameObject.Find("MetaCameraRig").transform;
            HandsProvider hp = GameObject.Find("MetaHands").GetComponent<HandsProvider>();
            hp.events.OnHandEnter.AddListener(AddHand);
            hp.events.OnHandExit.AddListener(RemHand);

            CreateTable();
        }

        private void Update()
        {
            if (_activeHands.Count > 1)
            {
                palm1 = _activeHands[0].Palm.Position;
                grab1 = _activeHands[0].IsGrabbing;
                palm2 = _activeHands[1].Palm.Position;
                grab2 = _activeHands[1].IsGrabbing;
            }
            else if (_activeHands.Count > 0)
            {
                palm1 = _activeHands[0].Palm.Position;
                grab1 = _activeHands[0].IsGrabbing;
                palm2 = Vector3.zero;
                grab2 = false;
            }
            else
            {
                palm1 = Vector3.zero;
                grab1 = false;
                palm2 = Vector3.zero;
                grab2 = false;

            }
            string[] data = {Time.time.ToString(), _headset.position.x.ToString(), _headset.position.y.ToString(), _headset.position.z.ToString(),
                _headset.forward.x.ToString(), _headset.forward.y.ToString(), _headset.forward.z.ToString(),
                _headset.up.x.ToString(), _headset.up.y.ToString(), _headset.up.z.ToString(),
                palm1.x.ToString(), palm1.y.ToString(), palm1.z.ToString(), grab1.ToString(),
                palm2.x.ToString(), palm2.y.ToString(), palm2.z.ToString(), grab2.ToString() };
            _dataWriter.Write(string.Join(", ", data) + "\n");

        }




        private void AddHand(Hand h)
        {
            _activeHands.Add(h);
        }
        private void RemHand(Hand h)
        {
            _activeHands.Remove(h);
        }

        public void CreateTable()
        {
            if (Directory.Exists(@"D:\"))
            {
                _directory =  @"D:\Hands_Data_Log";
            }
            else
            {
                _directory = Application.dataPath + @"\Hands_Data_Log";
            }
            _filename = System.DateTime.Now.ToString("hh_mm_dd_MM");

            if (!System.IO.Directory.Exists(_directory))
            {
                System.IO.Directory.CreateDirectory(_directory);
            }

            string path = _directory + @"\" + _filename + ".csv";
            _dataWriter = new StreamWriter(path);

            string fields = "Time, Head_X, Head_Y, Head_Z, Head_Forward_X, Head_Forward_Y, Head_Forward_Z, Head_Up_X, Head_Up_Y, Head_Up_Z, Palm1_X, Palm1_Y, Palm1_Z, Palm1_IsGrab, Palm2_X, Palm2_Y, Palm2_Z, Palm2_IsGrab\n";
            _dataWriter.WriteLine(fields);
        }


        private void OnApplicationQuit()
        {
            _dataWriter.Close();
        }




    }
}
