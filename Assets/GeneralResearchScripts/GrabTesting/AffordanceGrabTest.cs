using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Meta;
using Meta.HandInput;

namespace Research
{
    public class AffordanceGrabTest : MonoBehaviour
    {
        [SerializeField]
        private bool _saveFrames = false;
        private string _frameByName; // name of frame by frame file
        private StreamWriter _frameWriter;
        private float _saveFrameTimeOut = 45f; // max duration for saving frame by frame trial data


        private System.Random rand = new System.Random();
        private GameObject _headSet;

        //public DistantGrab _ProximalGrabController;
        //private float _grabThreshold = 0f;
        private bool _isGrabbing;



        //Objects
        //The relevant object is object0
        private List<GameObject> _rootObjs = new List<GameObject>();
        private List<Vector3> _rootSizes = new List<Vector3> { new Vector3(0.06f, 0.06f, 0.06f), new Vector3(0.04f, 0.04f, 0.04f), new Vector3(0.06f, 0.04f, 0.02f) };

        private int _trialTestInd = 0;

        private List<GameObject> _testObjs = new List<GameObject>();
        private List<Vector3> _testPos = new List<Vector3>();

        private float _zJitter = 0.04f;
        private float _scaleJitter = 0.5f; // maximum increase in the root size of an object



        private GameObject _frontBlock;
        private bool _missGrab = false;
        //Trials:
        [SerializeField]
        int _TrialsPerClass = 25;

        List<int> TrialClasses = new List<int> { 0, 1 };
        List<int> _trialClass;
        int _nTrials;
        int _trialNum = 0;


        //Hands
        private int _nHands = 0;
        private List<Hand> _activeHands = new List<Hand>();
        private int _falseObjGrabs = 0;
        private int _nGrabs = 0;


        //private float _priorGrabThresh = -1.0f;
        private bool _testGrabbed = false;
        //Logging:
        float _trialStartTime;
        string _dataBlock = "";
        [SerializeField]
        string _subjectName;

        private bool _initialized = false;


        // Use this for initialization
        void Start()
        {
            _headSet = GameObject.Find("MetaCameraRig");
            SetupHands();
            _headSet.GetComponent<SlamLocalizer>().onSlamMappingComplete.AddListener(InitializeExperiment);
        }

        // Update is called once per frame
        void Update()
        {
            //CheckGrabs();
            if (_initialized)
            {
                if (_saveFrames)
                {
                    LogFrameData();
                }

                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                {
                    if (_trialNum < _nTrials)
                    {
                        StartCoroutine(NewTrial());
                    }
                    else
                    {
                        EndTask();
                    }
                }
            }
            //HideProximalCursors();
        }

        private void OnApplicationQuit()
        {
            if (_saveFrames)
            {
                _frameWriter.Close();
            }
        }


        //Trail:
        private IEnumerator NewTrial()
        {
            StartCoroutine(LogDataBlock(_subjectName, _dataBlock));
            _dataBlock = "";
            _nGrabs = 0;
            _rootObjs[_trialTestInd].SetActive(false);
            ShuffleTestObjs();
            if (_trialClass[_trialNum] == 0)
            {
                //_proxyGrabOn = false;
            }
            else
            {
                //_proxyGrabOn = false;
            }


            _trialStartTime = Time.time;
            _trialNum += 1;
            yield return null;
        }



        //Set Trials:
        private void InitializeExperiment()
        {
            SetTrials();
            //SetFrontStop();
            _subjectName = _subjectName + "_GrabData_" + DateTime.Now.ToString("MM_dd_hh_mm");
            string[] fields = { "TrialNum", "ElapsedTime", "TestObjType",
            "InteractionType", "GrabSuccess", "Num_Grabs", "IsWrongObjGrab", "IsMissGrab",
            "HeadX", "HeadY", "HeadZ",
            "GazeX", "GazeY", "GazeZ",
            "HandX", "HandY", "HandZ",
            "ObjX", "ObjY", "ObjZ",
            "ObjSizeX", "ObjSizeY","ObjSizeZ",
            "ObjRotX", "ObjRotY","ObjRotZ", "AffordAngle",
            "NearestObjDist" };
            CreateTable(_subjectName, fields);


            if (_saveFrames)
            {
                _frameByName = _subjectName + "_FrameByFrame_" + DateTime.Now.ToString("MM_dd_hh_mm");
                string[] framefields = { "TrialNum", "TrialTime", "TargetX", "TargetY", "TargetZ", "Target_Size", "PalmX", "PalmY", "PalmZ", "TopX", "TopY", "TopZ", "PalmX2", "PalmY2", "PalmZ2", "TopX2", "TopY2", "TopZ2", "HeadX", "HeadY", "HeadZ", "GazeX", "GazeY", "GazeZ" };
                CreateTable(_frameByName, framefields);
                _frameWriter = new StreamWriter(Application.dataPath + @"\Analytics_Results\" + _frameByName + ".csv");
                _frameWriter.WriteLine(String.Join(", ", framefields));
            }

            StartCoroutine(NewTrial());
            _initialized = true;
        }


        private void SetTrials()
        {
            _trialClass = SetTrialClass();
            _nTrials = _trialClass.Count;

            _testPos = SetPositions();
            SetRootObjs();
            _testObjs = LoadTestObjs();
        }

        private void SetRootObjs()
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "SPHERE";
            _rootObjs.Add(sphere);

            GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.name = "CYLINDER";
            _rootObjs.Add(cylinder);

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "CUBE";
            _rootObjs.Add(cube);
            // Add Components
            for (int i = 0; i < _rootObjs.Count; i++)
            {
                _rootObjs[i].transform.localScale = _rootSizes[i];
                _rootObjs[i].AddComponent<GrabInteraction>();
                _rootObjs[i].GetComponent<Renderer>().material.color = new Color(0.75f, 0.2f, 0.2f);
                _rootObjs[i].AddComponent<ObjectHalo_Mesh>();
                //_rootObjs[i].AddComponent<InteractionObjectOutlineSettings>();
                _rootObjs[i].SetActive(false);
            }
        }


        private List<GameObject> LoadTestObjs()
        {
            // Set Background Objects:
            List<GameObject> objs = new List<GameObject>();
            for (int i = 0; i < (_testPos.Count - 1); i++)
            {
                int rootInd = i % _rootObjs.Count;
                GameObject obj = Instantiate(_rootObjs[rootInd]) as GameObject;

                obj.GetComponent<Renderer>().material.color = new Color(0.7f, 0.7f, 0.7f);
                obj.SetActive(false);
                objs.Add(obj);
            }

            // Set Test Objects

            for (int i = 0; i < _rootObjs.Count; i += 1)
            {
                _rootObjs[i].GetComponent<GrabInteraction>().Events.Engaged.AddListener(ObjGrabbed);
                _rootObjs[i].GetComponent<GrabInteraction>().Events.Disengaged.AddListener(ObjDropped);
            }


            for (int i = 0; i < objs.Count; i += 1)
            {
                objs[i].GetComponent<GrabInteraction>().Events.Engaged.AddListener(DecoyObjGrabbed);
                objs[i].GetComponent<GrabInteraction>().Events.Disengaged.AddListener(DecoyObjDropped);
            }

            return objs;
        }

        private void SetFrontStop()
        {
            _frontBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _frontBlock.transform.position = new Vector3(0f, -0.05f, 0.05f);
            _frontBlock.transform.localScale = new Vector3(1f, 1f, 0.15f);
            _frontBlock.AddComponent<GrabInteraction>();


            _frontBlock.GetComponent<Renderer>().material = new Material(Shader.Find("Transparent/Diffuse"));
            _frontBlock.GetComponent<Renderer>().material.color = new Color(0f, 0f, 0f, 0f);

            _frontBlock.GetComponent<GrabInteraction>().Events.Engaged.AddListener(NoObjGrabbed);
            _frontBlock.GetComponent<GrabInteraction>().Events.Disengaged.AddListener(NoObjDropped);
        }

        private void HideProximalCursors()
        {
            float zThresh = 0.2f;
            if (_activeHands.Count > 0)
            {
                for (int i = 0; i < _activeHands.Count; i++)
                {
                    bool isFar = _activeHands[i].Palm.Position.z >= zThresh;
                    _activeHands[i].transform.GetComponentInChildren<HandCursor>().enabled = isFar;
                    _activeHands[i].transform.Find("PalmFeature").Find("HandIndicator").gameObject.SetActive(isFar);
                    _activeHands[i].transform.Find("PalmFeature").Find("VicinityIndicator").gameObject.SetActive(isFar);
                }
            }
        }
        //private bool _missGrab = false;


        private void ShuffleTestObjs()
        {
            List<int> shuffle = RandPerm(_testPos.Count);
            if (_trialNum < 10)
            {
                List<float> zDepth = new List<float>();
                for (int i = 0; i < _testPos.Count; i++)
                {
                    zDepth.Add(_testPos[i].z);
                }
                zDepth.Sort();
                while (_testPos[shuffle[0]].z != zDepth[0])
                {
                    shuffle = RandPerm(_testPos.Count);
                }
            }
            _trialTestInd = rand.Next(_rootObjs.Count);
            _rootObjs[_trialTestInd].transform.position = _testPos[shuffle[0]] + new Vector3(0f, 0f, Jitter(_zJitter));
            _rootObjs[_trialTestInd].transform.localScale = JitterTestScale(_rootObjs[_trialTestInd]);
            _rootObjs[_trialTestInd].transform.rotation = UnityEngine.Random.rotationUniform;// eulerAngles = RandomRotation();
            _rootObjs[_trialTestInd].SetActive(true);

            for (int i = 1; i < _testPos.Count; i++)
            {
                _testObjs[i - 1].transform.position = _testPos[shuffle[i]] + new Vector3(0f, 0f, Jitter(_zJitter));
                _testObjs[i - 1].transform.localScale = JitterTestScale(_testObjs[i - 1]);// _testSize + new Vector3(sizeJitter, sizeJitter, sizeJitter);
                _testObjs[i - 1].transform.rotation = UnityEngine.Random.rotationUniform;// RandomRotation();
                _testObjs[i - 1].SetActive(true);
            }
        }

        private List<Vector3> SetPositions()
        {
            Vector3 basePos = _headSet.transform.position;
            List<Vector3> positions = new List<Vector3>();
            float rows = 3; float cols = 3; float depths = 2;

            float minX = -0.125f;
            float maxX = 0.125f;

            float minY = 0.01f;
            float maxY = -0.2f;

            float minZ = 0.35f;
            float maxZ = 0.48f;
            for (float i = 0.0f; i < rows; i += 1.0f)
            {
                float xlerp = i / (rows - 1);
                float x = Mathf.Lerp(minX, maxX, xlerp);

                for (float j = 0; j < cols; j += 1.0f)
                {
                    float ylerp = j / (cols - 1);
                    float y = Mathf.Lerp(minY, maxY, ylerp);

                    for (float k = 0; k < depths; k += 1.0f)
                    {
                        float zlerp = k / (depths - 1);
                        float z = Mathf.Lerp(minZ, maxZ, zlerp);
                        positions.Add(basePos + new Vector3(x, y, z));
                    }


                }
            }

            return positions;
        }





        //Hands Set Up:
        private void SetupHands()
        {
            HandsProvider _handProvider = GameObject.Find("MetaHands").GetComponent<HandsProvider>();
            _handProvider.events.OnGrab.AddListener(TriggerGrab);
            //_handProvider.events.OnRelease.AddListener(GrabEnd);
            _handProvider.events.OnHandEnter.AddListener(AddHand);
            _handProvider.events.OnHandExit.AddListener(RemHand);
        }

        private List<int> SetTrialClass()
        {
            List<int> trialsRaw = new List<int>();
            for (int i = 0; i < _TrialsPerClass; i++)
            {
                foreach (int x in TrialClasses)
                {
                    trialsRaw.Add(x);
                }
            }
            List<int> shuffle = RandPerm(trialsRaw.Count);
            List<int> trialsShuffled = new List<int>();
            for (int i = 0; i < trialsRaw.Count; i++)
            {
                trialsShuffled.Add(trialsRaw[shuffle[i]]);
            }
            return trialsShuffled;
        }


        private void EndTask()
        {
            _rootObjs[0].SetActive(true);
            _rootObjs[0].transform.position = _headSet.transform.position + new Vector3(0f, -0.1f, 0.3f);
            for (int i = 1; i < _rootObjs.Count; i++)
            {
                _rootObjs[i].SetActive(false);
            }
            for (int i = 0; i < _testObjs.Count; i++)
            {
                _testObjs[i].SetActive(false);
            }
        }






        //Hands Stuff
        private void TriggerGrab(Hand h)
        {
            //_proximtyGrabbed = false;
            StartCoroutine(AppendDataBlock(h));
        }


        private void AddHand(Hand h)
        {

            _activeHands.Add(h);

            //h.Palm.OnEngagedEvent.AddListener(CheckEngagement);
            _nHands += 1;
        }


        private void RemHand(Hand h)
        {
            _activeHands.Remove(h);
            _nHands -= 1;
            //_priorGrabThresh = -1f;
        }

        private void ObjGrabbed(MetaInteractionData m)
        {
            _testGrabbed = true;
        }
        private void ObjDropped(MetaInteractionData m)
        {
            _testGrabbed = false;
        }
        private void DecoyObjGrabbed(MetaInteractionData m)
        {
            _falseObjGrabs += 1;
        }
        private void DecoyObjDropped(MetaInteractionData m)
        {
            _falseObjGrabs -= 1;
        }

        private void NoObjGrabbed(MetaInteractionData m)
        {
            _missGrab = true;
        }
        private void NoObjDropped(MetaInteractionData m)
        {
            _missGrab = false;
            _frontBlock.transform.position = new Vector3(0f, -0.1f, 0.15f);
        }







        //Data Logging:

        private IEnumerator AppendDataBlock(Hand h)
        {
            yield return new WaitForEndOfFrame();
            string[] data = { _trialNum.ToString(), (Time.time - _trialStartTime).ToString(), _rootObjs[_trialTestInd].name,
                _trialClass[_trialNum-1].ToString(), _testGrabbed.ToString(), _nGrabs.ToString(), (_falseObjGrabs != 0).ToString(), _missGrab.ToString(),
                _headSet.transform.position.x.ToString(), _headSet.transform.position.y.ToString(), _headSet.transform.position.z.ToString(),
            _headSet.transform.forward.x.ToString(), _headSet.transform.forward.y.ToString(), _headSet.transform.forward.z.ToString(),
            h.Palm.transform.position.x.ToString(),  h.Palm.transform.position.y.ToString(),  h.Palm.transform.position.z.ToString(),
            _rootObjs[_trialTestInd].transform.position.x.ToString(),  _rootObjs[_trialTestInd].transform.position.y.ToString(),  _rootObjs[_trialTestInd].transform.position.z.ToString(),
            _rootObjs[_trialTestInd].transform.localScale.x.ToString(), _rootObjs[_trialTestInd].transform.localScale.y.ToString(), _rootObjs[_trialTestInd].transform.localScale.z.ToString(),
            _rootObjs[_trialTestInd].transform.eulerAngles.x.ToString(), _rootObjs[_trialTestInd].transform.eulerAngles.y.ToString(), _rootObjs[_trialTestInd].transform.eulerAngles.z.ToString(), GetAffordanceAngle().ToString(),
            NearestObjDist().ToString(),
            };

            _dataBlock = _dataBlock + string.Join(",", data) + "\n";
        }

        private static void CreateTable(string filename, params string[] columns)
        {
            string path = Application.dataPath + @"\Analytics_Results";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            string file = path + @"\" + filename + ".csv";
            string input = string.Join(",", columns) + "\n";
            System.IO.File.AppendAllText(file, input);
        }

        private IEnumerator LogDataBlock(string filename, string input)
        {
            string file = Application.dataPath + @"\Analytics_Results\" + filename + ".csv";
            System.IO.File.AppendAllText(file, input);
            yield return null;
        }

        private void LogFrameData()
        {
            float t = Time.time - _trialStartTime;
            if (t < _saveFrameTimeOut)
            {
                if (_activeHands.Count > 1)
                {
                    string data = _trialNum.ToString() + ", " + t.ToString() + ", " +
                        _rootObjs[_trialTestInd].transform.position.x.ToString() + ", " + _rootObjs[_trialTestInd].transform.position.y.ToString() + ", " + _rootObjs[_trialTestInd].transform.position.z.ToString() + ", " + _rootObjs[_trialTestInd].transform.localScale.x.ToString() + ", " +
                        _activeHands[0].Palm.Position.x.ToString() + ", " + _activeHands[0].Palm.Position.y.ToString() + ", " + _activeHands[0].Palm.Position.z.ToString() + ", " +
                        _activeHands[0].Top.Position.x.ToString() + ", " + _activeHands[0].Top.Position.y.ToString() + ", " + _activeHands[0].Top.Position.z.ToString() + ", " +
                        _activeHands[1].Palm.Position.x.ToString() + ", " + _activeHands[1].Palm.Position.y.ToString() + ", " + _activeHands[1].Palm.Position.z.ToString() + ", " +
                        _activeHands[1].Top.Position.x.ToString() + ", " + _activeHands[1].Top.Position.y.ToString() + ", " + _activeHands[1].Top.Position.z.ToString() + ", " +

                        _headSet.transform.position.x.ToString() + ", " + _headSet.transform.position.y.ToString() + ", " + _headSet.transform.position.z.ToString() + ", " +
                        _headSet.transform.forward.x.ToString() + ", " + _headSet.transform.forward.y.ToString() + ", " + _headSet.transform.forward.z.ToString();


                    _frameWriter.WriteLine(data);
                }
                else if (_activeHands.Count == 1)
                {
                    string data = _trialNum.ToString() + ", " + t.ToString() + ", " +
                            _rootObjs[_trialTestInd].transform.position.x.ToString() + ", " + _rootObjs[_trialTestInd].transform.position.y.ToString() + ", " + _rootObjs[_trialTestInd].transform.position.z.ToString() + ", " + _rootObjs[_trialTestInd].transform.localScale.x.ToString() + ", " +


                            _activeHands[0].Palm.Position.x.ToString() + ", " + _activeHands[0].Palm.Position.y.ToString() + ", " + _activeHands[0].Palm.Position.z.ToString() + ", " +
                        _activeHands[0].Top.Position.x.ToString() + ", " + _activeHands[0].Top.Position.y.ToString() + ", " + _activeHands[0].Top.Position.z.ToString() + ", -1.0, -1.0, -1.0, -1.0, -1.0, -1.0" + ", " +
                        _headSet.transform.position.x.ToString() + ", " + _headSet.transform.position.y.ToString() + ", " + _headSet.transform.position.z.ToString() + ", " +
                        _headSet.transform.forward.x.ToString() + ", " + _headSet.transform.forward.y.ToString() + ", " + _headSet.transform.forward.z.ToString();

                    _frameWriter.WriteLine(data);
                }
                else
                {
                    string data = _trialNum.ToString() + ", " + t.ToString() + ", " +
                            _rootObjs[_trialTestInd].transform.position.x.ToString() + ", " + _rootObjs[_trialTestInd].transform.position.y.ToString() + ", " + _rootObjs[_trialTestInd].transform.position.z.ToString() + ", " + _rootObjs[_trialTestInd].transform.localScale.x.ToString() + ", " +
                           "-1.0, -1.0, -1.0, -1.0, -1.0, -1.0" + ", -1.0, -1.0, -1.0, -1.0, -1.0, -1.0" + ", " +
                        _headSet.transform.position.x.ToString() + ", " + _headSet.transform.position.y.ToString() + ", " + _headSet.transform.position.z.ToString() + ", " +
                        _headSet.transform.forward.x.ToString() + ", " + _headSet.transform.forward.y.ToString() + ", " + _headSet.transform.forward.z.ToString();
                    _frameWriter.WriteLine(data);

                }
            }
        }




        //Raas PsychTools:
        private List<int> Range(int n)
        {
            List<int> vals = new List<int>();
            for (int i = 0; i < n; i++)
            {
                vals.Add(i);
            }
            return vals;
        }

        private List<int> RandPerm(int n)
        {
            int ind;
            List<int> vals = Range(n);
            List<int> shuffle = new List<int>();
            while (vals.Count > 0)
            {
                ind = rand.Next(vals.Count);
                shuffle.Add(vals[ind]);
                vals.RemoveAt(ind);
            }
            return shuffle;
        }

        private float Jitter(float max, float min = -9999.9f)
        {

            if (min == -9999.9f)
            {
                min = -max;
            }
            float midpt = (max + min) / 2.0f;
            float range = Mathf.Abs(max - min);
            float x = range * Convert.ToSingle(rand.NextDouble()) - midpt;
            return x;
        }

        private Vector3 JitterTestScale(GameObject testObj)
        {
            Vector3 scaleJitter;
            switch (testObj.name)
            {
                case "SPHERE":
                    scaleJitter = _rootSizes[0] * (1.0f + _scaleJitter * Convert.ToSingle(rand.NextDouble()));
                    break;
                case "CYLINDER":
                    scaleJitter = _rootSizes[1] * (1.0f + _scaleJitter * Convert.ToSingle(rand.NextDouble()));
                    break;
                case "CUBE":
                    scaleJitter = _rootSizes[2] * (1.0f + _scaleJitter * Convert.ToSingle(rand.NextDouble()));
                    break;
                default:
                    scaleJitter = testObj.transform.localScale;
                    break;
            }
            return scaleJitter;
        }

        private Vector3 RandomRotation()
        {
            float xAng = 360f * Convert.ToSingle(rand.NextDouble());
            float yAng = 360f * Convert.ToSingle(rand.NextDouble());
            float zAng = 360f * Convert.ToSingle(rand.NextDouble());
            return new Vector3(xAng, yAng, zAng);
        }

        private float NearestObjDist()
        {
            float minDist = 10f;
            for (int i = 1; i < _testObjs.Count; i++)
            {
                if (Vector3.Distance(_testObjs[0].transform.position, _testObjs[i].transform.position) < minDist)
                {
                    minDist = Vector3.Distance(_testObjs[0].transform.position, _testObjs[i].transform.position);
                }
            }
            return minDist;
        }

        private float GetAffordanceAngle()
        {
            float affordAngle = 0f;
            if (_rootObjs[_trialTestInd].name == "SPHERE")
            {
                affordAngle = 0f;
            }
            else if (_rootObjs[_trialTestInd].name == "CYLINDER")
            {
                affordAngle = Vector3.Angle(Vector3.right, _rootObjs[_trialTestInd].transform.up);
            }
            else if (_rootObjs[_trialTestInd].name == "CUBE")
            {
                affordAngle = Vector3.Angle(Vector3.right, _rootObjs[_trialTestInd].transform.right);
            }
            return affordAngle;
        }
    }
}