
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Meta;
using Meta.HandInput;
using Research;


namespace LibrarianMI3
{
    public class LibrarianSimulatorControl : MonoBehaviour
    {

        //System Parameters
        private System.Random _rand = new System.Random();

        private UnityEvent _bookPlaced = new UnityEvent();
        //Book Parameters:
        private float _bookHeight = 0.15f;
        private float _bookWidth = 0.05f;
        private float _bookDepth = 0.1f;


        private Vector3 _bookStartPos;
        private bool _jitterBookPos = true;
        private bool _jitterBookSize = true;

        private List<Vector3> _bookPositions = new List<Vector3>(); // List of possible book positions
        private List<int> _bookTargets = new List<int>(); // randomized list of book targets


        // Game Objects
        private GameObject _bookShelf;
        private List<GameObject> _books = new List<GameObject>();
        private GameObject _target; // target for placing a book


        //Sound:
        [SerializeField]
        private bool _playSound = false;
        public AudioClip LibrarianSound;
        private AudioSource _audio;


        //Game Play Values:
        [SerializeField]
        private UnityEngine.UI.Text _scoreText;
        private int _gameScore = 0;     // Total Points in Game
        private int _placementValue = 50; // 40*50 = 2000;
        private int _levelUpValue = 500;
        private int _level = 0; // current level;


        private float _bookStartTime = 0f;
        private float _placeTime = 5f;

        private DataLogger dataLogger = new DataLogger();


        //Use this for initialization
        void Start()
        {
            _bookPlaced.AddListener(PlacementOn);
            GameObject.Find("MetaCameraRig").GetComponent<SlamLocalizer>().onSlamMappingComplete.AddListener(InitiateScene);
            InstantiateBookShelf();
            _audio = GetComponent<AudioSource>();

            string filename = "LibrarianTest_" + DateTime.Now.ToString("MM_dd_hh_mm");
            dataLogger.CreateTable(filename, new string[] { "TrailNum", "Time",
                                    "ObjX", "ObjY", "ObjZ", "CloseX", "CloseY", "CloseZ",
                                    "PalmX", "PalmY", "PalmZ", "AnchorX", "AnchorY", "AnchorZ",
                                    "VelAnchorX" , "VelAnchorY", "VelAnchorZ",
                                    "HeadX", "HeadY", "HeadZ", "GazeX", "GazeY", "GazeZ", "UpX", "UpY", "UpZ" });
            GameObject.Find("MetaHands").GetComponent<HandsProvider>().events.OnGrab.AddListener(LogGrab);
        }

        private void OnApplicationQuit()
        {
            dataLogger.CloseDataLogger();
        }

        private void InitiateScene()
        {
            Transform headSet = GameObject.Find("MetaCameraRig").transform;
            float shelfDist = 0.55f;
            float shelfSink = -0.3f;
            Vector3 bookShelfPos = headSet.position + headSet.forward * shelfDist + headSet.up * shelfSink;
            _bookShelf.transform.position = bookShelfPos;

            for (int i = 0; i < _bookPositions.Count; i++)
            {
                _bookPositions[i] = _bookPositions[i] + bookShelfPos;
            }


            _bookStartPos = headSet.position + 0.3f * headSet.right - 0.3f * headSet.up + 0.1f * headSet.forward;


            StartLevel();

        }

        // Update is called once per frame
        void Update()
        {
            DetectPlacement();
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                ResetScore();
            }

        }

        private void StartLevel()
        {
            foreach (GameObject book in _books)
            {
                Destroy(book);
            }
            _books.Clear();
            
            _bookTargets = GenerateArray.RandPerm(_bookPositions.Count);
            NewTrial();
        }

        private void NewTrial()
        {
            AddBook();
            _target.transform.position = _bookPositions[_bookTargets[_books.Count]];
        }

        // Set up the Bookshelf:
        private void InstantiateBookShelf()
        {
            Vector3 bookShelfPos = Vector3.zero;
            Color shelfColor = new Color(0.6f, 0.6f, 0.6f);
            int nShelves = 3; // number of shelves
            int nBooks = 8; // number of books per shelf
            GameObject shelf;

            Vector3 bookTarg;

            //Create BackBoard
            _bookShelf = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _bookShelf.transform.localScale = new Vector3(Convert.ToSingle(nBooks) * _bookWidth * 1.2f, Convert.ToSingle(nShelves) * _bookHeight * 1.2f, 0.001f);
            _bookShelf.transform.position = new Vector3(0f, 0f, _bookDepth * 1.2f);
            _bookShelf.GetComponent<Renderer>().material.color = shelfColor;
            //Create Sides:
            shelf = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shelf.transform.localScale = new Vector3(0.05f, Convert.ToSingle(nShelves) * _bookHeight * 1.2f, _bookDepth * 1.2f);
            shelf.transform.position = new Vector3(-1.0f * _bookShelf.transform.localScale.x / 2f, 0f, shelf.transform.localScale.z / 2f);
            shelf.transform.parent = _bookShelf.transform;
            shelf.GetComponent<Renderer>().material.color = shelfColor;

            shelf = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shelf.transform.localScale = new Vector3(0.05f, Convert.ToSingle(nShelves) * _bookHeight * 1.2f, _bookDepth * 1.2f);
            shelf.transform.position = new Vector3(_bookShelf.transform.localScale.x / 2f, 0f, shelf.transform.localScale.z / 2f);
            shelf.transform.parent = _bookShelf.transform;
            shelf.GetComponent<Renderer>().material.color = shelfColor;

            //Create Panels

            for (int i = 0; i <= nShelves; i++)
            {
                float zSpacing = _bookHeight * 1.2f;
                float zBase = -1.0f * _bookShelf.transform.localScale.y / 2f;
                float zCoord = Convert.ToSingle(i) * zSpacing + zBase;
                shelf = GameObject.CreatePrimitive(PrimitiveType.Cube);
                shelf.transform.localScale = new Vector3(_bookShelf.transform.localScale.x, 0.03f, _bookDepth * 1.2f);
                shelf.transform.position = new Vector3(0f, zCoord, shelf.transform.localScale.z / 2f);
                shelf.transform.parent = _bookShelf.transform;
                shelf.GetComponent<Renderer>().material.color = shelfColor;

                if (i < nShelves)
                {
                    for (int j = 0; j < nBooks; j++)
                    {
                        float xSpacing = _bookWidth;
                        float xBase = -1.0f * _bookWidth * Convert.ToSingle(nBooks) / 2f;
                        bookTarg = new Vector3(xBase + xSpacing * Convert.ToSingle(j), zCoord + _bookHeight * 0.6f, -_bookDepth / 2f);

                        _bookPositions.Add(bookTarg + bookShelfPos);
                    }
                }

            }

            //Create Target:
            _target = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _target.GetComponent<Renderer>().material.color = new Color(0.6f, 0.6f, 0f);
            _target.transform.localScale = new Vector3(_bookWidth, _bookHeight, _bookDepth);

            _bookShelf.transform.position = bookShelfPos;
        }


        //Add a book
        private void AddBook()
        {
            GameObject book = Instantiate(Resources.Load("Book")) as GameObject;
            if (_jitterBookSize)
            {
                float heightJit = 0.1f * _bookHeight * (Convert.ToSingle(_rand.NextDouble()) - 0.5f);
                float depthJit = 0.1f * _bookDepth * (Convert.ToSingle(_rand.NextDouble()) - 0.5f);
                book.transform.localScale = new Vector3(_bookWidth, _bookHeight, _bookDepth) + new Vector3(0f, heightJit, depthJit);
            }

            if (_jitterBookPos)
            {
                Vector3 jitterPos = new Vector3(Convert.ToSingle(_rand.NextDouble()) - 0.5f, Convert.ToSingle(_rand.NextDouble()) - 0.5f, Convert.ToSingle(_rand.NextDouble()) - 0.5f);

                book.transform.position = _bookStartPos + 0.1f * jitterPos;
            }
            else
            {
                book.transform.position = _bookStartPos;
            }
            _books.Add(book);
            _bookStartTime = Time.time;
        }




        private void DetectPlacement()
        {
            if (_books.Count > 0)
            {
                float dist = Vector3.Distance(_books[_books.Count - 1].transform.position, _target.transform.position);
                if (dist < 0.1f)
                {
                    _bookPlaced.Invoke();
                }
            }
        }



        private void PlacementOn()
        {

            ScorePlacement();
            PlaceBook(); // Sets isPlaced to false, aligns book and freezes it, kills the target

            if (_books.Count == (_bookTargets.Count - 1))
            {
                _level += 1;
                _gameScore += _levelUpValue;
                StartLevel();
            }
            else
            {
                NewTrial();
            }

            //Shush
            if (_rand.NextDouble() < 0.3f)
            {
                if (_playSound)
                {
                    _audio.PlayOneShot(LibrarianSound);
                }
            }
        }


        private void PlaceBook()
        {
            // Sets isPlaced to false, aligns book and freezes it, kills the target
            _books[_books.Count - 1].transform.position = _bookPositions[_bookTargets[_books.Count]];
            _books[_books.Count - 1].GetComponent<GrabInteraction>().enabled = false;
        }


        private void ScorePlacement()
        {
            _placeTime = Time.time - _bookStartTime;
            int placementScore = Convert.ToInt32(Mathf.Max(Convert.ToSingle(_placementValue) / 5f, _placementValue / (0.5f * _placeTime)));
            _gameScore += placementScore;
            UpdateScore();
        }


        private void UpdateScore()
        {
            string scoreOut = "Score: " + Convert.ToString(_gameScore);
            _scoreText.text = scoreOut;
        }

        private void ResetScore()
        {
            _gameScore = 0;
            UpdateScore();
        }


        private void LogGrab(Hand h)
        {
            Vector3 objPosition = _target.transform.position;
            Vector3 closestPt = _target.GetComponent<Collider>().ClosestPointOnBounds(h.Data.GrabAnchor);
            Transform headset = Camera.main.transform;
            string[] data = new string[] { _level.ToString(), (Time.time - _bookStartTime).ToString(),
            objPosition.x.ToString(), objPosition.y.ToString(), objPosition.z.ToString(), closestPt.x.ToString(), closestPt.y.ToString(), closestPt.z.ToString(),
            h.Palm.Position.x.ToString(), h.Palm.Position.y.ToString(), h.Palm.Position.z.ToString(), h.Data.GrabAnchor.x.ToString(), h.Data.GrabAnchor.y.ToString(), h.Data.GrabAnchor.z.ToString(),
            //h.Velocity.x.ToString(), h.Velocity.y.ToString(), h.Velocity.z.ToString(),
            headset.position.x.ToString(), headset.position.y.ToString(), headset.position.z.ToString(),
            headset.forward.x.ToString(), headset.forward.y.ToString(), headset.forward.z.ToString(),
            headset.up.x.ToString(), headset.up.y.ToString(), headset.up.z.ToString()};

            dataLogger.LogData(data);
        }


    }
}