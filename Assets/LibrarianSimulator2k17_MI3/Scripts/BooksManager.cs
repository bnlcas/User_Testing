using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta;

namespace LibrarianMI3
{
    public class BooksManager : MonoBehaviour
    {
        private System.Random _rand = new System.Random();

        private List<Vector3> _bookPositions;

        private GameObject _shelf;

        private bool _booksInPlay = false;

        private List<Book> _books = new List<Book>();

        private int _targetInd = 0;

        private GameObject _targetBook;

        private Color _idleColor = new Color(0.6f, 0.6f, 0.6f);

        private Color _targetColor = new Color(0.92f, 0.3f, 0.15f);

        [SerializeField]
        private int _maxBooks = 40;

        private int _activeBooks = 0;
        public List<Vector3> BookPositions
        {
            set { _bookPositions = value; }
        }

        public GameObject Shelf
        {
            get { return _shelf; }
            set { _shelf = value; }
        }

        public GameObject TargetBook
        {
            get { return _targetBook; }
        }

        public void LoadBooks()
        {
            for(int i = _books.Count; i < _maxBooks; i++)
            {
                GameObject bookObject = Instantiate(Resources.Load("Book2")) as GameObject;
                bookObject.name = "book" + i.ToString();
                Book book = bookObject.GetComponent<Book>();

                book.SetColor(_idleColor);
                _books.Add(book);

                if (i >= _bookPositions.Count)
                {
                    book.gameObject.SetActive(false);
                }
                else
                {
                    if (!_booksInPlay)
                    {
                        bookObject.transform.parent = _shelf.transform;
                        bookObject.transform.localPosition = _bookPositions[i];
                    }
                    else
                    {
                        book.ResetBookPosition(_shelf.transform, _bookPositions[i]);
                    }
                }

                
            }
            _activeBooks = _bookPositions.Count;
        }

        public void ActivateBooks()
        {
            for (int i = _activeBooks; i < _bookPositions.Count; i++)
            {
                _books[i].gameObject.SetActive(true);
                _books[i].ResetBookPosition(_shelf.transform, _bookPositions[i]);
            }
            _activeBooks = _bookPositions.Count;
        }

        public void SetBooksInPlay()
        {
            _booksInPlay = true;
            for (int i = 0; i < _bookPositions.Count; i++)
            {
                _books[i].transform.parent = null;
            }
        }

        public void ResetBooks()
        {
            for(int i = 0; i < _bookPositions.Count; i++)
            {
                _books[i].ResetBookPosition(_shelf.transform, _bookPositions[i]);
            }
        }

        public void SetTargetBook()
        {
            _books[_targetInd].SetColor(_idleColor);
            _targetInd = _rand.Next(_bookPositions.Count);
            _books[_targetInd].SetColor(_targetColor);

            _targetBook = _books[_targetInd].gameObject;
        }
    }
}