using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta;

namespace LibrarianMI3
{
    public class Book : MonoBehaviour
    {

        private bool _isGrabbed;

        private GrabInteraction _grab;

        private Renderer _renderer;

        private bool _isHome;

        private Vector3 _homePosition;


        public void ResetBookPosition(Transform baseTransform, Vector3 home)
        {
            _homePosition = home;
            if(_isGrabbed)
            {
                StartCoroutine(ReturnObjWhenDropped(baseTransform));
            }
            else
            {
                this.transform.parent = baseTransform;
                this.transform.localPosition = home;
                this.transform.parent = null;
            }
        }

        private IEnumerator ReturnObjWhenDropped(Transform baseTransform)
        {
            while(_isGrabbed)
            {
                yield return null;
            }
            this.transform.parent = baseTransform;
            this.transform.localPosition = _homePosition;
            this.transform.parent = null;
            yield return null;
        }

        private void Awake()
        {
            _renderer = this.gameObject.GetComponent<Renderer>();
            this.gameObject.GetComponent<GrabInteraction>().Events.Engaged.AddListener(GrabOn);
            this.gameObject.GetComponent<GrabInteraction>().Events.Disengaged.AddListener(GrabOff);
        }

        private void GrabOn(MetaInteractionData m)
        {
            _isGrabbed = true;
        }
        private void GrabOff(MetaInteractionData m)
        {
            _isGrabbed = false;
        }

        public void SetColor(Color c)
        {
            _renderer.material.color = c;
        }
    }
}
