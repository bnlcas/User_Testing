using System;
namespace LibrarianMI3
{
    [Serializable]
    public class MetaDataJSON
    {
        public string cpu_start_time;
        public string device_id;
        public string gpu_name;
        public string cpu_name;
        public string session_uuid;
        public string device_uuid;
        public string player_name;
        public int score;
        public int session_number;
        public string projective_grab_on;
        public string csv_data;
    }
}