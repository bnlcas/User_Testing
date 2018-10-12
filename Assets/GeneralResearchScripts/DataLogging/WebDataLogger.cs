using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Research
{
    public class WebDataLogger
    {
        private string _serverAddress = "www.internet.com";

        public IEnumerator Upload(string filename, string webAddress, string jsonData)
        {
            _serverAddress = webAddress;
            WWWForm data = Text2Webform(filename, jsonData);

            WWWForm test = new WWWForm();
            test.AddField("test", jsonData);

            UnityWebRequest www = UnityWebRequest.Post(_serverAddress, test);// data);
            www.SetRequestHeader("Content-Type", "application/json");
            www.uploadHandler.contentType = "application/json";
            yield return www.Send();
            yield return null;
        }




        private WWWForm Text2Webform(string pathFilename, string jsonData)
        {
            
            string fileName = Path.GetFileName(pathFilename);
            //byte[] fileData = File.ReadAllBytes(pathFilename);
            byte[] fileData = System.Text.Encoding.UTF8.GetBytes(jsonData);
            WWWForm data = new WWWForm();

            data.AddBinaryData("data", fileData, fileName,  "application/json");

            return data;
        }


        public IEnumerator PostJSON(string url, string bodyJsonString)
        {
            var request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(bodyJsonString);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.Send();

            //Debug.Log("Status Code: " + request.responseCode);
        }

        public IEnumerator PostCSV(string url, byte[] data)
        {
            var request = new UnityWebRequest(url, "POST");
            //byte[] fileData = File.ReadAllBytes(pathFilename);
            //byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(bodyJsonString);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.Send();

            //Debug.Log("Status Code: " + request.responseCode);
        }


    }
}
