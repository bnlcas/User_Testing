using UnityEngine;

namespace RotationTest
{
    public class DataRow_RotationTest
    {
        public int TrialNum;
        public string EventType;
        public float TrialTime;
        public float TotalTime;
        public Transform ObjectTransform;
        public Transform HeadTransform;


        public string[] GetFields()
        {
            string[] fields = new string[] {"TrialNum", "EventType","TrialTime", "TotalTime",
            "ObjPos_X", "ObjPos_Y", "ObjPos_Z",
            "Obj_Forward_X", "Obj_Forward_Y", "Obj_Forward_Z",
            "Obj_Up_X", "Obj_Up_Y", "Obj_Up_Z",
            "Head_X", "Head_Y", "Head_Z",
            "Forward_X", "Forward_Y", "Forward_Z",
            "Up_X", "Up_Y", "Up_Z"};
            return fields;

        }

        public string[] GetData()
        {
            string[] dataRow = new string[] { TrialNum.ToString(), EventType, TrialTime.ToString(), TotalTime.ToString(),

            ObjectTransform.position.x.ToString(), ObjectTransform.position.y.ToString(), ObjectTransform.position.z.ToString(),
            ObjectTransform.forward.x.ToString(), ObjectTransform.forward.y.ToString(), ObjectTransform.forward.z.ToString(),
            ObjectTransform.up.x.ToString(), ObjectTransform.up.y.ToString(), ObjectTransform.up.z.ToString(),
            HeadTransform.position.x.ToString(), HeadTransform.position.y.ToString(), HeadTransform.position.z.ToString(),
            HeadTransform.forward.x.ToString(), HeadTransform.forward.y.ToString(), HeadTransform.forward.z.ToString(),
            HeadTransform.up.x.ToString(), HeadTransform.up.y.ToString(), HeadTransform.up.z.ToString()};
            return dataRow;
        }
    }
}
