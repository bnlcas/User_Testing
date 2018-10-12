using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

namespace Research
{
    public class DataLogger
    {
        private string _directory;
        private string _filename;
        private string _path;
        private StreamWriter _dataWriter;

        private bool _isOpen = true;

        public bool IsOpen
        {
            set { _isOpen = value; }
            get { return _isOpen; }
        }

        public string Filename
        {
            get { return _path; }
        }

        public void CreateTable(string fileName, string[] fields, string dir = "default")
        {
            if (dir == "default")
            {
                _directory = Application.dataPath + @"\Analytics_Results";
            }
            else
            {
                _directory = dir;
            }
            _filename = fileName;
            if (!System.IO.Directory.Exists(_directory))
            {
                System.IO.Directory.CreateDirectory(_directory);
            }

            _path = _directory + @"\" + _filename + ".csv";
            _dataWriter = new StreamWriter(_path);
            _dataWriter.WriteLine(string.Join(", ", fields));
        }

        public void LogData(string[] data)
        {
            _dataWriter.Write(string.Join(", ", data)+ "\n");
        }

        public IEnumerator LogDataAsynch(string[] data)
        {
            _dataWriter.Write(string.Join(", ", data) + "\n");
            yield return null;
        }

        public void CloseDataLogger()
        {
            if(_isOpen)
            {
                _dataWriter.Close();
            }
            _isOpen = false;
        }
    }
}
