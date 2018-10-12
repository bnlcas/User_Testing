using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LibrarianMI3
{
    //Controll the Shelf
    public class Shelf : MonoBehaviour
    {
        //Private:
        private GameObject _shelf;
        private Color _shelfColor = new Color(0.6f, 0.6f, 0.6f);
        private int _nShelves = 3; // number of shelves
        private int _nBooksPerShelf = 8; // number of books per shelf

        private Vector3 _bookDims = new Vector3(0.05f, 0.15f, 0.1f);

        [SerializeField]
        private Vector3 _shelfDims;
        private float panelThickness = 0.03f;
        private float _bookSpacing = 0.1f;

        List<Vector3> _bookPosition;

        public int NBooksPerShelf
        {
            get { return _nBooksPerShelf; }
        }

        public Vector3 BookDims
        {
            get { return _bookDims; }
            set { _bookDims = value; }
        }

        public Vector3 ShelfDims
        {
            get { return _shelfDims; }
            set { _shelfDims = value; }
        }

        public float BookSpacing
        {
            get { return _bookSpacing; }
            set { _bookSpacing = value; }
        }


        public List<Vector3> BookPositions
        {
            get { return _bookPosition; }
        }


        public GameObject InstantiateBookShelf()
        {
            float ShelfWidth = (float)_nBooksPerShelf * _bookDims.x + ((float)_nBooksPerShelf + 1.0f) * _bookSpacing;
            float ShelfDepth = _bookDims.z * 1.5f;
            float ShelfVerticalSpacing = _bookDims.y * 1.5f;
            _shelfDims = new Vector3(ShelfWidth, ShelfVerticalSpacing, ShelfDepth);

            float nShelves = (float)_nShelves;

            _shelf = new GameObject(); _shelf.name = "Shelf";
            GameObject back = GameObject.CreatePrimitive(PrimitiveType.Cube);
            back.transform.parent = _shelf.transform;
            back.transform.localPosition = new Vector3(0f, 0f, _bookDims.z);
            back.GetComponent<Renderer>().material.color = _shelfColor;
            back.transform.localScale = new Vector3(ShelfWidth + 2f * panelThickness, ShelfVerticalSpacing * nShelves + 2f * panelThickness, panelThickness);
            

            //Create Sides:
            GameObject leftSide = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftSide.GetComponent<Renderer>().material.color = _shelfColor;
            leftSide.transform.parent = _shelf.transform;
            leftSide.transform.localScale = new Vector3(panelThickness, ShelfVerticalSpacing * nShelves + 2f * panelThickness, ShelfDepth);
            //leftSide.transform.localPosition = new Vector3(-0.5f * (panelThickness + ShelfWidth), 0.5f * (ShelfVerticalSpacing * nShelves + 2f * panelThickness), ShelfDepth * 0.5f);
            leftSide.transform.localPosition = new Vector3(-0.5f * (panelThickness + ShelfWidth),0f, ShelfDepth * 0.5f);


            GameObject rightSide = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightSide.GetComponent<Renderer>().material.color = _shelfColor;
            rightSide.transform.parent = _shelf.transform;
            rightSide.transform.localScale = new Vector3(panelThickness, ShelfVerticalSpacing * nShelves + 2f * panelThickness, ShelfDepth);
            //rightSide.transform.localPosition = new Vector3(0.5f * (panelThickness + ShelfWidth), 0.5f * (ShelfVerticalSpacing * nShelves + 2f * panelThickness), ShelfDepth * 0.5f);
            rightSide.transform.localPosition = new Vector3(0.5f * (panelThickness + ShelfWidth), 0f, ShelfDepth * 0.5f);

            //Create Panels
            _bookPosition = new List<Vector3>();
            for (int i = 0; i <= _nShelves; i++)
            {
                GameObject row = GameObject.CreatePrimitive(PrimitiveType.Cube); row.name = "row" + i.ToString();
                row.GetComponent<Renderer>().material.color = _shelfColor;
                row.transform.parent = _shelf.transform;
                row.transform.localScale = new Vector3(ShelfWidth, panelThickness, ShelfDepth);

                //Set Position:
                float shelfHeight = ((System.Convert.ToSingle(i) / nShelves) - 0.5f) * (ShelfVerticalSpacing * nShelves);
                Vector3 shelfCenter = new Vector3(0f, shelfHeight, ShelfDepth * 0.5f);
                row.transform.localPosition = shelfCenter;

                
                if (i < nShelves)
                {
                    for(int j = 0; j < _nBooksPerShelf; j++)
                    {
                        float xOffset = -ShelfWidth * 0.5f + _bookSpacing * ((float)j + 1.0f) + ((float)j + 0.5f) * _bookDims.x;
                        float yOffset = (panelThickness + _bookDims.y) * 0.5f;
                        _bookPosition.Add(shelfCenter + new Vector3(xOffset, yOffset, 0f));
                    }
                }

            }
            return _shelf;
        }

        public void ResetBookPositions()
        {
            _nBooksPerShelf = System.Convert.ToInt32(Mathf.Floor((_shelfDims.x - _bookSpacing) / (_bookSpacing + _bookDims.x)));
            _bookPosition = new List<Vector3>();
            for (int i = 0; i < _nShelves; i++)
            {
                //Set Position:
                float shelfHeight = ((System.Convert.ToSingle(i) / System.Convert.ToSingle(_nShelves)) - 0.5f) * (_shelfDims.y * _nShelves);
                Vector3 shelfCenter = new Vector3(0f, shelfHeight, _shelfDims.z * 0.5f);

                for (int j = 0; j < _nBooksPerShelf; j++)
                {
                    float xOffset = -_shelfDims.x * 0.5f + _bookSpacing * ((float)j + 1.0f) + ((float)j + 0.5f) * _bookDims.x;
                    float yOffset = (panelThickness + _bookDims.y) * 0.5f;
                    _bookPosition.Add(shelfCenter + new Vector3(xOffset, yOffset, 0f));
                }

            }
            //return _bookPosition;
        }
    }
}
