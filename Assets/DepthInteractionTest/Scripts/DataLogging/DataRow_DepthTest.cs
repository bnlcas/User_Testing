
using UnityEngine;

namespace DepthTest
{
    public class DataRow_DepthTest
    {
        public int TrialNum;
        public string EventType;
        public float TrialTime;
        public float TotalTime;
        public bool isCorrectObj;
        public Vector3 ObjectPosition;
        public Vector3 ClosestPoint;
        public Vector3 PalmPosition;
        public Vector3 AnchorPosition;
        public Transform HeadTransform;


        public string[] GetFields()
        {
            string[] fields = new string[] {"TrialNum", "EventType","TrialTime", "TotalTime",
            "IsCorrectObj",
            "ObjPos_X", "ObjPos_Y", "ObjPos_Z",
            "ClosestPt_X", "ClosestPt_Y", "ClosestPt_Z",
            "Palm_X", "Palm_Y", "Palm_Z",
            "Anchor_X", "Anchor_Y", "Anchor_Z",
            "Head_X", "Head_Y", "Head_Z",
            "Forward_X", "Forward_Y", "Forward_Z",
            "Up_X", "Up_Y", "Up_Z"};
            return fields;

        }

        public string[] GetData()
        {
            string[] dataRow = new string[] { TrialNum.ToString(), EventType, TrialTime.ToString(), TotalTime.ToString(),
                isCorrectObj.ToString(),
            ObjectPosition.x.ToString(), ObjectPosition.y.ToString(), ObjectPosition.z.ToString(),
            ClosestPoint.x.ToString(), ClosestPoint.y.ToString(), ClosestPoint.z.ToString(),
            PalmPosition.x.ToString(),PalmPosition.y.ToString(),PalmPosition.z.ToString(),
            AnchorPosition.x.ToString(), AnchorPosition.y.ToString(), AnchorPosition.z.ToString(),
            HeadTransform.position.x.ToString(), HeadTransform.position.y.ToString(), HeadTransform.position.z.ToString(),
            HeadTransform.forward.x.ToString(), HeadTransform.forward.y.ToString(), HeadTransform.forward.z.ToString(),
            HeadTransform.up.x.ToString(), HeadTransform.up.y.ToString(), HeadTransform.up.z.ToString()};
            return dataRow;
        }
    }
}
