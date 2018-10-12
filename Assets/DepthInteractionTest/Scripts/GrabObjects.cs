using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta;

namespace DepthTest
{
    public class GrabObjects : MonoBehaviour
    {
        [SerializeField]
        private int _nObjects = 8;

        private List<GameObject> _grabObjects = new List<GameObject>();

        private bool _isTargetHovered = false;

        public GameObject TargetObject
        {
            get { return _grabObjects[0]; }
        }

        void Start()
        {
            LoadGrabObjects();
            GameObject.Find("SceneControl").GetComponent<DepthExperiment>().StartNewTrial.AddListener(NewTrial);
        }

        private void LoadGrabObjects()
        {
            for(int i = 0; i < _nObjects; i++)
            {
                GameObject obj = Instantiate(Resources.Load("DepthTarget")) as GameObject;
                obj.name = "target" + i.ToString();
                _grabObjects.Add(obj);
            }
            GrabInteraction grab = _grabObjects[0].GetComponent<GrabInteraction>();
            grab.Events.HoverStart.AddListener(TargetObjectHoverEnter);
            grab.Events.HoverEnd.AddListener(TargetObjectHoverExit);
            _grabObjects[0].GetComponent<Renderer>().material.color = new Color(0.8f, 0.14f, 0.1f);
            ShuffleObjects();
        }

        private void NewTrial()
        {
            StartCoroutine(ShuffleObjectsOnRelease());
        }

        private void TargetObjectHoverExit(MetaInteractionData m)
        {
            _grabObjects[0].GetComponent<Renderer>().material.color = new Color(0.8f, 0.14f, 0.1f);
            _isTargetHovered = false;
        }

        private void TargetObjectHoverEnter(MetaInteractionData m)
        {
            _isTargetHovered = true;
        }

        private IEnumerator ShuffleObjectsOnRelease()
        {
            while (_isTargetHovered)
            {
                yield return null;
            }
            yield return null;
            ShuffleObjects();
            yield return null;
        }

        private void ShuffleObjects()
        {
            for (int i = 0; i < _grabObjects.Count; i++)
            {
                Vector3 offset = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.15f, -0.05f), 0.2f);
                Vector3 direction = (Vector3.forward + Random.Range(-1f, 1f) * Vector3.right + Random.Range(-0.3f, 0.05f) * Vector3.up).normalized;

                _grabObjects[i].transform.position = Random.Range(0.1f, 0.8f) * direction + offset;
            }
        }


    }
}