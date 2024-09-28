using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _transform;
    [SerializeField] private Transform _prefab;

    private List<Transform> _pieces;
    private int _emptyLocation;
    private int _size = 3;
    private bool _isShuffle = false;

    private void Start()
    {
        _pieces = new List<Transform>();
        CreateGamePieces(0.01f);
    }

    private void Update()
    {
        if (!_isShuffle && IsCompletion())
        {
            _isShuffle = true;
            StartCoroutine(WaitShuffle(0.5f));
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit)
            {
                for (int i = 0; i < _pieces.Count; i++)
                {
                    if (_pieces[i] == hit.transform)
                    {
                        if (IsSwapValid(i, -_size, _size))
                            break;
                        if (IsSwapValid(i, _size, _size))
                            break;
                        if (IsSwapValid(i, -1, 0))
                            break;
                        if (IsSwapValid(i, 1, _size - 1))
                            break;
                    }
                }
            }
        }
    }
    
    private IEnumerator WaitShuffle(float duration)
    {
        yield return new WaitForSeconds(duration);
        Shuffle();
        _isShuffle = false;
    }

    private void Shuffle()
    {
        int count = 0;
        int last = 0;

        while (count < (_size * _size * _size))
        {
            int rnd = Random.Range(0, _size * _size);

            if (rnd == last)
            {
                continue;
            }

            last = _emptyLocation;

            if (IsSwapValid(rnd, -_size, _size))
                count++;
            else if (IsSwapValid(rnd, _size, _size))
                count++;
            else if (IsSwapValid(rnd, -1, 0))
                count++;
            else if (IsSwapValid(rnd, 1, _size - 1))
                count++;
        }
    }

    private bool IsCompletion()
    {
        for (int i = 0; i < _pieces.Count; ++i)
        {
            if (_pieces[i].name != $"{i}")
            {
                return false;
            }
        }

        return true;
    }

    private bool IsSwapValid(int i, int offset, int colCheck)
    {
        if (((i % _size) != colCheck) && ((i + offset) == _emptyLocation))
        {
            (_pieces[i], _pieces[i + offset]) = (_pieces[i + offset], _pieces[i]);
            (_pieces[i].localPosition, _pieces[i + offset].localPosition) = (_pieces[i + offset].localPosition, _pieces[i].localPosition);
            _emptyLocation = i;

            return true;
        }

        return false;
    }

    private void CreateGamePieces(float gapThickness)
    {
        float width = 1 / (float)_size;

        for (int row = 0; row < _size; row++)
        {
            for (int col = 0; col < _size; col++)
            {
                Transform piece = Instantiate(_prefab, _transform);
                _pieces.Add(piece);

                piece.localPosition = new Vector3(-1 + (2 * width * col) + width, 1 - (2 * width * row) - width, 0);
                piece.localScale = ((2 * width) - gapThickness) * Vector3.one;
                piece.name = $"{(row * _size) + col}";

                if ((row == _size - 1) && (col == _size - 1))
                {
                    _emptyLocation = (_size * _size) - 1;
                    piece.gameObject.SetActive(false);
                }
                else
                {
                    float gap = gapThickness / 2;
                    Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
                    Vector2[] uv = new Vector2[4];

                    uv[0] = new Vector2((width * col) + gap, 1 - ((width * (row + 1)) - gap));
                    uv[1] = new Vector2((width * (col + 1)) - gap, 1 - ((width * (row + 1)) - gap));
                    uv[2] = new Vector2((width * col) + gap, 1 - ((width * row) + gap));
                    uv[3] = new Vector2((width * (col + 1)) - gap, 1 - ((width * row) + gap));

                    mesh.uv = uv;
                }
            }
        }
    }
}