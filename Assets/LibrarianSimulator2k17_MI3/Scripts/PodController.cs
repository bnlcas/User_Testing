
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LibrarianMI3
{

    public class PodController : MonoBehaviour
    {
        public UnityEvent OnPodHit;

        private string _targetName;

        public string TargetName
        {
            get { return _targetName; }
            set { _targetName = value; }
        }

        private void Start()
        {
            OnPodHit = new UnityEvent();
        }

        void OnTriggerEnter(Collider collisionData)
        {
            if(collisionData.gameObject.name == _targetName)
            {
                OnPodHit.Invoke();
            }
        }
    }

}
