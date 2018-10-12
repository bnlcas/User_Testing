using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LibrarianMI3
{
    public class TmpScript : MonoBehaviour
    {
        [SerializeField]
        private Color bookColor;

        //private Book b;
        // Use this for initialization
        void Start()
        {
            //b = GameObject.Find("Book2").GetComponent<Book>();
        }

        // Update is called once per frame
        void Update()
        {
            //b.SetColor(bookColor);
            Rigidbody[] rigidBodies = Resources.FindObjectsOfTypeAll<Rigidbody>();
            string[] rigidBodyParent = new string[rigidBodies.Length];
            for(int i = 0; i < rigidBodies.Length; i++)
            {
                rigidBodyParent[i] = rigidBodies[i].gameObject.name;
            }
            Debug.Log(string.Join(", ", rigidBodyParent));
        }
    }
}
