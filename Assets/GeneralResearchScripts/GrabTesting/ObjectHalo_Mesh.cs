using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta;
using Meta.HandInput;

namespace Research
{
    public class ObjectHalo_Mesh : MonoBehaviour
    {

        GameObject _halo;
        Renderer _rend;

        Vector3 _minSize;
        Vector3 _maxSize;

        List<Hand> _activeHands = new List<Hand>();

        bool _wasContact = false;

        private Color ObjectGrabbedColor = new Color(63 / 255f, 195 / 255f, 245 / 255f);
        private Color ObjectHoverColor = new Color(63 / 255f, 169 / 255f, 245 / 255f);
        private Color ObjectIdleColor = new Color(0f, 0f, 0f, 0f);

        // Use this for initialization
        void Start()
        {
            CreateHalo();
            SetupHands();
        }

        // Update is called once per frame
        private void Update()
        {
            bool inContact = CheckHandStatus();
            if (inContact != _wasContact)
            {
                ToggleContact(inContact);
            }
            _wasContact = inContact;
        }


        private bool CheckHandStatus()
        {
            bool inContact = false;
            for (int i = 0; i < _activeHands.Count; i++)
            {
                if ((_activeHands[i].Palm.NearObjects.Count > 0) && (_activeHands[i].Palm.NearObjects[0].transform == this.transform))
                {
                    inContact = true;
                    break;
                }
            }
            return inContact;
        }

        private void CreateHalo()
        {
            _halo = Instantiate(this.gameObject) as GameObject;
            _halo.name = "HALO";
            Component[] comps = _halo.transform.GetComponentsInChildren<Component>();
            for (int i = 0; i < comps.Length; i++)
            {
                if (!((comps[i] is Transform) || (comps[i] is MeshFilter) || (comps[i] is MeshRenderer) || (comps[i] is Renderer)))
                {
                    Destroy(comps[i]);
                }
            }

            _rend = _halo.transform.GetComponent<Renderer>();
            _rend.material.shader = Shader.Find("Transparent/Diffuse");//Shader.Find("Unlit/Color");// 
            _rend.material.color = new Color(0f, 0f, 0f, 0f);
            _rend.material.renderQueue = 1000;


            _halo.transform.SetParent(this.transform);
            _halo.transform.localRotation = Quaternion.identity;
            _halo.transform.localPosition = Vector3.zero;
            _minSize = _halo.transform.localScale;
            _maxSize = 1.08f * _minSize;
            _halo.transform.localScale = _maxSize;
        }


        private void ToggleContact(bool inContact)
        {
            if (inContact)
            {
                _rend.material.color = ObjectHoverColor;
            }
            else
            {
                _rend.material.color = ObjectIdleColor;
            }

        }

        private void SetupHands()
        {
            HandsProvider hp = GameObject.Find("MetaHands").GetComponent<HandsProvider>();
            hp.events.OnHandEnter.AddListener(AddHand);
            hp.events.OnHandExit.AddListener(RemHand);

            GrabInteraction g = this.gameObject.GetComponent<GrabInteraction>();
            g.Events.Engaged.AddListener(GrabOn);
            g.Events.Engaged.AddListener(GrabOff);
        }
        private void AddHand(Hand h)
        {
            _activeHands.Add(h);
        }
        private void RemHand(Hand h)
        {
            _activeHands.Remove(h);
        }

        private void GrabOn(MetaInteractionData m)
        {
            _rend.material.color = ObjectGrabbedColor;
        }
        private void GrabOff(MetaInteractionData m)
        {
            _rend.material.color = ObjectHoverColor;
        }


    }
}