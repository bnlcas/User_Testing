using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DepthTest
{

    public class TargetPlate : MonoBehaviour
    {
        public UnityEvent OnPlateHit = new UnityEvent();

        private string _targetObjectID = "target0";

        public string TargetObjectID
        {
            get { return _targetObjectID; }
            set { _targetObjectID = value; }
        }


        void OnTriggerEnter(Collider collisionData)
        {
            if (collisionData.gameObject.name == _targetObjectID)
            {
                collisionData.gameObject.GetComponent<Renderer>().material.color = new Color(0.8f, 0.8f, 0.8f);
                OnPlateHit.Invoke();
            }
        }
    }

}
