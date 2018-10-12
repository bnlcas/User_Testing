using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Research
{
    public class WebDownloader
    {
        private byte[] _downloadData;



        public IEnumerator Download(string serverAddress)
        {
            UnityWebRequest www = UnityWebRequest.Get(serverAddress);
            yield return www.Send();
            
            if(!www.isError)
            {
                _downloadData = www.downloadHandler.data;
            }

        }

        public string ReturnHighScores(int playerScore, string playerName)
        {
            string downloadString = System.Text.Encoding.UTF8.GetString(_downloadData);

            string[] entryPattern = new string[] { "},{" };
            string[] userPattern = new string[] { "username\":" };
            string[] scorePattern = new string[] { ",\"score\":" };


            string[] entries = downloadString.Split(entryPattern, StringSplitOptions.None);

            List<string> names = new List<string>();
            List<int> scores = new List<int>();
            for (int i = 0; i < entries.Length; i++)
            {
                string name = entries[i].Split(userPattern, StringSplitOptions.None)[1];
                name = name.Split(',')[0].Replace("\"", "");
                //Debug.Log(name);
                string score = entries[i].Split(scorePattern, StringSplitOptions.None)[1];
                score = score.Split('}')[0];
                //Debug.Log(score);
                names.Add(name);
                scores.Add(Convert.ToInt32(score));
            }


            int nScores = 3;
            string HighScores = "Highscores:\n\n";
            if (playerScore > scores[2])
            {
                int playerInd = 0;
                for (int i = 0; i < nScores; i++)
                {
                    if(playerScore > scores[i] && playerInd == i)
                    {
                        HighScores += (i + 1).ToString() + ".) " + playerName + ": " + playerScore.ToString() + "\n";
                    }
                    else
                    {
                        HighScores += (i + 1).ToString() + ".) " + names[playerInd] + ": " + scores[playerInd].ToString() + "\n";
                        playerInd += 1;
                    }
                    
                }
            }
            else
            {
                for (int i = 0; i < nScores; i++)
                {
                    HighScores += (i + 1).ToString() + ".) " + names[i] + ": " + scores[i].ToString() + "\n";
                }
                HighScores += "\n...\n" + playerName + ": " + playerScore + "\n";
            }

            return HighScores;
        }

    }
}
