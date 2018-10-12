using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Meta;
using Meta.HandInput;

namespace DepthTest
{
    public class GrabPoseDetected : UnityEvent<Hand>
    {
    }

    public class GrabPoseDetector : MonoBehaviour
    {
        List<Hand> _activeHands = new List<Hand>();
        List<bool> _handGrabbing = new List<bool>();
        public GrabPoseDetected GrabPoseOn = new GrabPoseDetected();
        // Use this for initialization
        void Start()
        {
            HandsProvider hp = GameObject.Find("MetaHands").GetComponent<HandsProvider>();
            hp.events.OnHandEnter.AddListener(AddHand);
            hp.events.OnHandExit.AddListener(RemHand);
        }

        // Update is called once per frame
        void Update()
        {
            for(int i = 0; i < _activeHands.Count; i++)
            {
                bool isGrab = _activeHands[i].IsGrabbing;
                if(isGrab && !_handGrabbing[i])
                {
                    GrabPoseOn.Invoke(_activeHands[i]);
                }
                _handGrabbing[i] = isGrab;
            }
        }

        private void AddHand(Hand h)
        {
            _activeHands.Add(h);
            _handGrabbing.Add(false);
        }
        private void RemHand(Hand h)
        {
            int grabInd = _activeHands.IndexOf(h);
            _handGrabbing.RemoveAt(grabInd);
            _activeHands.Remove(h);
        }
    }

}