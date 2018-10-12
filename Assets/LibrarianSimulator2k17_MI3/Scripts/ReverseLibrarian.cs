using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Meta;
using System.Linq;

namespace LibrarianMI3
{
    public class PlayerNameEntered : UnityEvent<string>
    {
    }

    public class TrialStart : UnityEvent<GameObject>
    { 
    }

    public class ReverseLibrarian : MonoBehaviour
    {
        System.Random _rand = new System.Random();


        //Constituent Classes, Objects and SubSystem Controllers:
        [SerializeField]
        Transform _headSet;

        [SerializeField]
        GameObject _pod;

        //Object Handlers:
        private Shelf _shelfScript;

        private PodController _podController;

        private BooksManager _booksManager;

        [SerializeField]
        private ScoreManager _scoreBoard;

        // Game Objects
        [SerializeField]
        private GameObject _bookShelf;
        private Vector3 _bookShelfPos = new Vector3(0.2f, -0.1f, 0.5f);

        private Vector3 _podPos = new Vector3(-0.6f, -0.3f, 0.3f);

        private GameObject _targetBook;

        private List<Vector3> _bookPositions = new List<Vector3>(); // List of possible book positions


        //Sound:
        [SerializeField]
        private bool _playSound = false;
        public AudioClip LibrarianSound;
        private AudioSource _audio;


        //Game Stuff:
        private float _bookStartTime = 0f;
        private float _placeTime = 5f;
        private bool _gameOver = false;


        //Data Logging:
        public PlayerNameEntered PlayerNamed = new PlayerNameEntered();
        private int _gameNumber;
        private string _playerName;

        private string _filename;
        private int _trialNum = 0;

        public TrialStart TrailStarted = new TrialStart();
        // Use this for initialization
        void Start()
        {
            //Slam
            _headSet.gameObject.GetComponent<SlamLocalizer>().onSlamMappingComplete.AddListener(InitiateScene);

            //Shelf
            //_shelfScript = GameObject.Find("ShelfHolder").GetComponent<Shelf>();
            //_bookShelf = _shelfScript.InstantiateBookShelf();
            _shelfScript = _bookShelf.GetComponent<Shelf>();
            _shelfScript.ResetBookPositions();

            //Books
            _booksManager = GameObject.Find("BooksHolder").GetComponent<BooksManager>();
            _booksManager.Shelf = _bookShelf;
            _booksManager.BookPositions = _shelfScript.BookPositions;
            _booksManager.LoadBooks();

            //Pod
            
            //_pod = GameObject.Find("Pod");
            _podController = _pod.GetComponent<PodController>();
            _scoreBoard.ScoreDetected.AddListener(DetectBookPlacement);
            //_podController.OnPodHit.AddListener(DetectBookPlacement);


            _audio = GetComponent<AudioSource>();

            //Score
            _scoreBoard = GameObject.Find("ScoreHolder").GetComponent<ScoreManager>();
            _scoreBoard.GameOver.AddListener(EndGame);

            //Data Logging

            //Set Game Number:
            _gameNumber = PlayerPrefs.GetInt("GameNumber", 0) + 1;

            PlayerPrefs.SetInt("GameNumber", _gameNumber);

            Debug.Log(GetHighScores());
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                NewTrial();
            }
        }



        private void InitiateScene()
        {
            StartCoroutine(ReorientScene());
            StartCoroutine(DisplayInstructions());
        }
        private IEnumerator ReorientScene()
        {
            yield return new WaitForSeconds(0.1f);
            //_bookShelf.transform.position = VectorTransform.MapVector2Transform(_headSet, _bookShelfPos);
            _booksManager.SetBooksInPlay();
            //_pod.transform.position =  VectorTransform.MapVector2Transform(_headSet, _podPos);
            yield return null;
        }
        private IEnumerator DisplayInstructions()
        {
            GameObject startUI = _headSet.Find("GlassUI/StartGame").gameObject;
            //GameObject scoreDisplay = GameObject.Find("ScoreDisplay/Instructions").gameObject;
            GameObject enterNameDisplay = _headSet.Find("GlassUI/NameDisplay").gameObject;
            Text NameDisplay = _headSet.Find("GlassUI/NameDisplay/EnterName").gameObject.GetComponent<Text>();
            startUI.SetActive(false);
            //scoreDisplay.SetActive(false);
            enterNameDisplay.SetActive(true);

            NameDisplay.text = "Enter your name:\n";
            while (!Input.GetKeyDown(KeyCode.Return))
            {
                if ((Input.inputString) == "\b" && (_playerName.Length > 0))
                {
                    _playerName = _playerName.Substring(0, (_playerName.Length - 1));
                    NameDisplay.text = "Enter your name:\n" + _playerName;
                }
                else
                {
                    _playerName += Input.inputString;
                    NameDisplay.text = "Enter your name:\n" + _playerName;
                }
                yield return null;
            }
            PlayerNamed.Invoke(_playerName);
            enterNameDisplay.SetActive(false);
            startUI.SetActive(true);
            yield return new WaitForSecondsRealtime(6f);
            //scoreDisplay.SetActive(true);
            startUI.SetActive(false);
            NewTrial();
            yield return null;
        }



        private void DetectBookPlacement()
        {
            ReduceSpacing();
            _booksManager.ResetBooks();
            NewTrial();
        }

        private void ReduceSpacing()
        {
            int nBooks = _shelfScript.NBooksPerShelf;
            //_shelfScript.BookSpacing = _shelfScript.BookSpacing * 0.94f;
            float percentComplete = ((float)_trialNum) / ((float)_scoreBoard.MaxTrials);
            _shelfScript.BookSpacing = Mathf.Lerp(0.1f, 0.02f, percentComplete);
            _shelfScript.ResetBookPositions();

            if (nBooks != _shelfScript.NBooksPerShelf)
            {
                _booksManager.BookPositions = _shelfScript.BookPositions;
                //_booksManager.LoadBooks();
                _booksManager.ActivateBooks();
            }

        }

        private void NewTrial()
        {
            Shush(0.25f);
            _trialNum += 1;
            _bookStartTime = Time.time;
            ResetTargetBook();
            TrailStarted.Invoke(_targetBook);
        }

        private void ResetTargetBook()
        {
            _booksManager.SetTargetBook();
            _targetBook = _booksManager.TargetBook;
            _podController.TargetName = _targetBook.name;
        }
        private void Shush(float shushProb)
        {
            //float shushProb = 0.25f;
            if (_playSound)
            {
                if (_rand.NextDouble() < shushProb)
                {
                    _audio.PlayOneShot(LibrarianSound);
                }
            }
        }

        private void EndGame()
        {
            _gameOver = true;
            Shush(1f);

            DisplayEndScreen();
        }

        private void DisplayEndScreen()
        {
            //string[] Scores = _downloadManager.GetDownloadData().Split('\n');
            //_highScores = _downloadManager.GetDownloadData();

            Debug.Log("Over");
            string scoresMessage = GetHighScores();// "Test Completed!\nThank You.";

            GameObject.Find("ScoreHolder").SetActive(false);
            _headSet.Find("GlassUI/Endgame").gameObject.SetActive(true);
            Text Leaderboard = _headSet.Find("GlassUI/Endgame/EndgameMessage").GetComponent<Text>();


            Leaderboard.text = scoresMessage;
            StartCoroutine(PauseThenQuit());
        }

         private IEnumerator PauseThenQuit()
        {
            float delay = 3f;
            yield return new WaitForSecondsRealtime(delay);
            Application.Quit();
        }




        //Data Logging Stuff:
        private void SendData()
        {
            MetaDataJSON metaData = new MetaDataJSON();


            metaData.cpu_start_time = DateTime.Now.ToString("hh_mm_dd_MM");
            metaData.device_id = SystemInfo.deviceName;
            metaData.gpu_name = SystemInfo.graphicsDeviceName;
            metaData.cpu_name = SystemInfo.processorType;
            metaData.session_uuid = Guid.NewGuid().ToString();
            metaData.device_uuid = SystemInfo.deviceUniqueIdentifier;
            metaData.player_name = _playerName;
            metaData.score = _scoreBoard.Score;
            metaData.session_number = _gameNumber;
            metaData.projective_grab_on = "FALSE";


            //dataLogger.CloseDataLogger();

            //byte[] fileData = System.IO.File.ReadAllBytes(dataLogger.Filename);
            //string grabData = Convert.ToBase64String(fileData);
            //metaData.csv_data = grabData;

            //string metaDataJSON = JsonUtility.ToJson(metaData);
            //System.IO.File.WriteAllText(Application.dataPath + @"\Analytics_Results\metaDataJSON.txt", metaDataJSON);

        }



        private string GetHighScores()
        {
            string HighScore = "\n" + _playerName + ": " + _scoreBoard.Score.ToString();
            System.IO.File.AppendAllText(Application.dataPath + @"\highScores.txt", HighScore);
            string[] lines = System.IO.File.ReadAllLines(Application.dataPath + @"\highScores.txt");

            List<KeyValuePair<int, string>> scores = new List<KeyValuePair<int, string>>();
            //List<string> entries = new List<string>(lines.Length);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length > 1)
                {
                    string[] entry = lines[i].Split(' ');
                    int score = Convert.ToInt32(entry[entry.Length - 1]);

                    scores.Add(new KeyValuePair<int, String>(score, lines[i]));

                }
            }

            //entries = entries.OrderByDescending(x => scores.IndexOf[x]).ToList();




            string highscores = "High Scores:\n";

            int nFinalists = 5;
            int j = 0;
            foreach (KeyValuePair<int, string> entry in scores.OrderByDescending(key => key.Key))
            {
                if (j < nFinalists)
                {
                    highscores = highscores + entry.Value + "\n";
                }

                j += 1;
            }
            return highscores;
        }      

    }
}
