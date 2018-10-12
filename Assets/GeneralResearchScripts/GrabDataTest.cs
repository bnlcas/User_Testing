using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Meta;
using Meta.HandInput;

public class GrabDataTest : MonoBehaviour
{

    private System.Random rand = new System.Random();
    private GameObject _headSet;

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
    [SerializeField]
    string _subjectName;

    private bool _initialized = false;

    Research.DataLogger _dataLogger = new Research.DataLogger();

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
    }

    private void OnApplicationQuit()
    {
        _dataLogger.CloseDataLogger();
    }


    //Trail:
    private IEnumerator NewTrial()
    {
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

        string[] fields = { "TrialNum", "ElapsedTime", "TestObjType", "HandType",
            "HeadX", "HeadY", "HeadZ",
            "GazeX", "GazeY", "GazeZ", "UpX", "UpY", "UpZ",
            "PalmX", "PalmY", "PalmZ", "AnchorX", "AnchorY", "AnchorZ",
            "VelPalmX", "VelPalmY", "VelPalmZ", "VelAnchorX", "VelAnchorY", "VelAnchorZ",
            "TargetClosePalmX", "TargetClosePalmY", "TargetClosePalmZ",  "TargetCloseAnchorX", "TargetCloseAnchorY", "TargetCloseAnchorZ", "AdditionalObjects"}; 

        _dataLogger.CreateTable(_subjectName, fields);

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
            //_rootObjs[i].AddComponent<ObjectHalo_Mesh>();
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

        //for (int i = 0; i < _rootObjs.Count; i += 1)
        //{
        //    _rootObjs[i].GetComponent<GrabInteraction>().Events.Engaged.AddListener(ObjGrabbed);
        //    _rootObjs[i].GetComponent<GrabInteraction>().Events.Disengaged.AddListener(ObjDropped);
        //}


        //for (int i = 0; i < objs.Count; i += 1)
        //{
        //    objs[i].GetComponent<GrabInteraction>().Events.Engaged.AddListener(DecoyObjGrabbed);
        //    objs[i].GetComponent<GrabInteraction>().Events.Disengaged.AddListener(DecoyObjDropped);
        //}

        return objs;
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
        _handProvider.events.OnGrab.AddListener(LogGrab);
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




    private void AddHand(Hand h)
    {

        _activeHands.Add(h);
        Destroy(h.Palm.gameObject.GetComponent<HandCursor>());
        //h.Palm.OnEngagedEvent.AddListener(CheckEngagement);
        _nHands += 1;
    }


    private void RemHand(Hand h)
    {
        _activeHands.Remove(h);
        _nHands -= 1;
        //_priorGrabThresh = -1f;
    }




    //Data Logging:
    private void LogGrab(Hand h)
    {
        Transform head = Camera.main.transform;
        List<string> data = new List<string> {_trialNum.ToString(), (Time.time - _trialStartTime).ToString(), _rootObjs[_trialTestInd].name, h.HandType.ToString(),
                            head.position.x.ToString(), head.position.y.ToString(), head.position.z.ToString(),
                            head.forward.x.ToString(), head.forward.y.ToString(), head.forward.z.ToString(),
                            head.up.x.ToString(), head.up.y.ToString(), head.up.z.ToString(),
                            h.Palm.Position.x.ToString(), h.Palm.Position.y.ToString(), h.Palm.Position.z.ToString(),
                            h.Data.GrabAnchor.x.ToString(), h.Data.GrabAnchor.y.ToString(), h.Data.GrabAnchor.z.ToString(),
                        };
        //h.PalmVelocity.x.ToString(), h.PalmVelocity.y.ToString(), h.PalmVelocity.z.ToString(),
        //h.AnchorVelocity.x.ToString(), h.AnchorVelocity.y.ToString(), h.AnchorVelocity.z.ToString()
        data.AddRange(GetClosestPts(h, _rootObjs[_trialTestInd]));

        for(int i = 0; i < _testObjs.Count; i++)
        {
            data.AddRange(GetClosestPts(h, _testObjs[i]));
        }

        string[] data_arr = data.ToArray();
       _dataLogger.LogData(data_arr);
    }


    private List<string> GetClosestPts(Hand h, GameObject obj)
    {
        Collider objColl = obj.GetComponent<Collider>();
        Vector3 closestPalm = objColl.ClosestPointOnBounds(h.Palm.Position);
        Vector3 closestAnchor = objColl.ClosestPointOnBounds(h.Data.GrabAnchor);
        List<string> closetPts = new List<string> { closestPalm.x.ToString(), closestPalm.y.ToString(), closestPalm.z.ToString(),
            closestAnchor.x.ToString(), closestAnchor.y.ToString(), closestAnchor.z.ToString()};
        return closetPts;
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
