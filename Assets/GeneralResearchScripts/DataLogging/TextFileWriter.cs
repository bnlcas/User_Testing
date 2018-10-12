using UnityEngine;

namespace Research
{
    public class TextFileWriter
    {

        public void WriteTextFile(string filename, string content)
        {
            string _path = Application.dataPath + @"\Analytics_Results";
            if (!System.IO.Directory.Exists(_path))
            {
                System.IO.Directory.CreateDirectory(_path);
            }
            string _filename = _path + @"\" + filename;
            System.IO.File.AppendAllText(_filename, content);
            
        }
    }
}
