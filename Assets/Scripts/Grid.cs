using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Grid : MonoBehaviour
{
    public int xDim;
    public int yDim;
    public float fillTime;

    public Level level;

    private bool _inverse = false;

    private bool _gameOver = false;

    public enum PieceType
    {
        EMPTY,
        NORMAL,
        BUBBLE,
        ROW_CLEAR,
        COLUMN_CLEAR,
        RAINBOW,
        COUNT
    }

    [System.Serializable]
    public struct PiecePrefab
    {
        public PieceType type;
        public GameObject prefab;
    }

    [System.Serializable]
    public struct PiecePosition
    {
        public PieceType type;
        public int x;
        public int y;
    }

    public PiecePrefab[] piecePrefabs;
    public GameObject backgroundPrefab;

    private Dictionary<PieceType, GameObject> _piecePrefabDict;

    private GamePiece[,] _pieces;

    private GamePiece _pressedPieces;
    private GamePiece _enteredPieces;

    public PiecePosition[] initialPieces;

    private bool _isFilling = false;

    public bool IsFilling
    {
        get { return _isFilling; }
    }

    // Start is called before the first frame update
    void Awake()
    {
        _piecePrefabDict = new Dictionary<PieceType, GameObject>();
        for (int i = 0; i < piecePrefabs.Length; i++)
        {
            if (!_piecePrefabDict.ContainsKey(piecePrefabs[i].type))
            {
                _piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
            }
        }

        //Instantiate each background prefab in grid
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                GameObject background = Instantiate(backgroundPrefab, GetWorldPosition(x, y), Quaternion.identity);
                background.transform.parent = transform;
            }
        }

        _pieces = new GamePiece[xDim, yDim];

        for (int i = 0; i < initialPieces.Length; i++)
        {
            if (initialPieces[i].x >= 0 && initialPieces[i].x < xDim &&
                initialPieces[i].y >= 0 && initialPieces[i].y < yDim)
            {
                SpawnNewPiece(initialPieces[i].x, initialPieces[i].y, initialPieces[i].type);
            }
        }

        //Instantiate each item prefab in grid
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                if (_pieces[x, y] == null)
                {
                    SpawnNewPiece(x, y, PieceType.EMPTY);
                }
            }
        }

        StartCoroutine(Fill());
    }

    // Update is called once per frame
    void Update()
    {
    }

    public IEnumerator Fill()
    {
        bool needsRefill = true;
        _isFilling = true;

        while (needsRefill)
        {
            yield return new WaitForSeconds(fillTime);

            while (FillStep())
            {
                _inverse = !_inverse;
                yield return new WaitForSeconds(fillTime);
                needsRefill = ClearAllValidMatches();
            }

            _isFilling = false;
        }
    }

    public bool FillStep()
    {
        bool movedPiece = false;

        for (int y = yDim - 2; y >= 0; y--)
        {
            //Grid Fillup Sound
            SoundManager.Instance.PlayFillUpSound();

            for (int loopX = 0; loopX < xDim; loopX++)
            {
                int x = loopX;
                if (_inverse)
                {
                    x = xDim - 1 - loopX;
                }

                GamePiece piece = _pieces[x, y];

                if (piece.IsMoveable())
                {
                    GamePiece pieceBelow = _pieces[x, y + 1];

                    //Move Down the grid
                    if (pieceBelow.Type == PieceType.EMPTY)
                    {
                        Destroy(pieceBelow.gameObject);
                        piece.MoveableComponent.Move(x, y + 1, fillTime);
                        _pieces[x, y + 1] = piece;
                        SpawnNewPiece(x, y, PieceType.EMPTY);
                        movedPiece = true;
                    }
                    else
                    {
                        //Move Diagonal On Grid
                        for (int diag = -1; diag <= 1; diag++)
                        {
                            if (diag != 0)
                            {
                                int diagX = x + diag;

                                if (_inverse)
                                {
                                    diagX = x - diag;
                                }

                                //Check if Outside Board Bound
                                if (diagX < 0 || diagX >= xDim) continue;

                                GamePiece diagonalPiece = _pieces[diagX, y + 1];

                                if (diagonalPiece.Type != PieceType.EMPTY) continue;

                                bool hasPieceAbove = true;

                                for (int aboveY = y; aboveY >= 0; aboveY--)
                                {
                                    GamePiece pieceAbove = _pieces[diagX, aboveY];

                                    if (pieceAbove.IsMoveable())
                                    {
                                        break;
                                    }
                                    else if (!pieceAbove.IsMoveable() && pieceAbove.Type != PieceType.EMPTY)
                                    {
                                        hasPieceAbove = false;
                                        break;
                                    }
                                }

                                //Piece Can be filled from above, so continue
                                if (hasPieceAbove) continue;

                                Destroy(diagonalPiece.gameObject);
                                piece.MoveableComponent.Move(diagX, y + 1, fillTime);
                                _pieces[diagX, y + 1] = piece;
                                SpawnNewPiece(x, y, PieceType.EMPTY);
                                movedPiece = true;
                                break;
                            }
                        }
                    }
                }
            }
        }

        for (int x = 0; x < xDim; x++)
        {
            GamePiece pieceBelow = _pieces[x, 0];

            if (pieceBelow.Type == PieceType.EMPTY)
            {
                GameObject newPiece = Instantiate(_piecePrefabDict[PieceType.NORMAL],
                    GetWorldPosition(x, -1), Quaternion.identity, this.transform);
                newPiece.transform.parent = transform;

                _pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                _pieces[x, 0].Init(x, -1, this, PieceType.NORMAL);
                _pieces[x, 0].MoveableComponent.Move(x, 0, fillTime);
                _pieces[x, 0].ColouredComponent
                    .SetColour((ColouredPiece.ColourType) Random.Range(0, _pieces[x, 0].ColouredComponent.NumColours));
                movedPiece = true;
            }
        }

        return movedPiece;
    }

    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(transform.position.x - xDim / 2.0f + x,
            transform.position.y + yDim / 2.0f - y);
    }

    public GamePiece SpawnNewPiece(int x, int y, PieceType type)
    {
        GameObject newPiece = Instantiate(_piecePrefabDict[type], GetWorldPosition(x, y), Quaternion.identity);
        newPiece.transform.parent = transform;

        _pieces[x, y] = newPiece.GetComponent<GamePiece>();
        _pieces[x, y].Init(x, y, this, type);
        return _pieces[x, y];
    }

    public bool IsAdjacent(GamePiece piece1, GamePiece piece2)
    {
        return piece1.X == piece2.X && Mathf.Abs(piece1.Y - piece2.Y) == 1
               || piece1.Y == piece2.Y && Mathf.Abs(piece1.X - piece2.X) == 1;
    }

    public void SwapPieces(GamePiece piece1, GamePiece piece2)
    {
        if (_gameOver)
            return;

        if (piece1.IsMoveable() && piece2.IsMoveable())
        {
            _pieces[piece1.X, piece1.Y] = piece2;
            _pieces[piece2.X, piece2.Y] = piece1;

            if (GetMatch(piece1, piece2.X, piece2.Y) != null || GetMatch(piece2, piece1.X, piece1.Y) != null ||
                piece1.Type == PieceType.RAINBOW || piece2.Type == PieceType.RAINBOW)
            {
                int piece1X = piece1.X;
                int piece1Y = piece1.Y;

                piece1.MoveableComponent.Move(piece2.X, piece2.Y, fillTime);
                piece2.MoveableComponent.Move(piece1X, piece1Y, fillTime);

                if (piece1.Type == PieceType.RAINBOW && piece1.IsClearable() && piece2.IsColoured())
                {
                    ClearColourPiece clearColour = piece1.GetComponent<ClearColourPiece>();
                    if (clearColour)
                    {
                        clearColour.Colour = piece2.ColouredComponent.Colour;
                    }

                    ClearPiece(piece1.X, piece1.Y);
                    SoundManager.Instance.PlaySpecialPieceSound();
                }

                if (piece2.Type == PieceType.RAINBOW && piece2.IsClearable() && piece1.IsColoured())
                {
                    ClearColourPiece clearColour = piece2.GetComponent<ClearColourPiece>();
                    if (clearColour)
                    {
                        clearColour.Colour = piece1.ColouredComponent.Colour;
                    }

                    ClearPiece(piece2.X, piece2.Y);
                    SoundManager.Instance.PlaySpecialPieceSound();
                }

                ClearAllValidMatches();

                // special pieces get cleared, event if they are not matched
                if (piece1.Type == PieceType.ROW_CLEAR || piece1.Type == PieceType.COLUMN_CLEAR)
                {
                    ClearPiece(piece1.X, piece1.Y);
                    SoundManager.Instance.PlayMatchPopSound();
                }

                if (piece2.Type == PieceType.ROW_CLEAR || piece2.Type == PieceType.COLUMN_CLEAR)
                {
                    ClearPiece(piece2.X, piece2.Y);
                    SoundManager.Instance.PlayMatchPopSound();
                }

                _pressedPieces = null;
                _enteredPieces = null;

                StartCoroutine(Fill());

                level.OnMove();
            }
            else
            {
                _pieces[piece1.X, piece1.Y] = piece1;
                _pieces[piece2.X, piece2.Y] = piece2;
            }
        }
    }

    public void PressPiece(GamePiece piece)
    {
        _pressedPieces = piece;
    }

    public void EnterPiece(GamePiece piece)
    {
        _enteredPieces = piece;
    }

    public void ReleasePiece()
    {
        if (IsAdjacent(_pressedPieces, _enteredPieces))
        {
            SwapPieces(_pressedPieces, _enteredPieces);
        }
    }

    public List<GamePiece> GetMatch(GamePiece piece, int newX, int newY)
    {
        if (piece.IsColoured())
        {
            ColouredPiece.ColourType colour = piece.ColouredComponent.Colour;
            List<GamePiece> horizontalPieces = new List<GamePiece>();
            List<GamePiece> verticalPieces = new List<GamePiece>();
            List<GamePiece> matchingPieces = new List<GamePiece>();

            //Check Horizontal
            horizontalPieces.Add(piece);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int xOffset = 1; xOffset < xDim; xOffset++)
                {
                    int x;
                    if (dir == 0)
                    {
                        //Going Left
                        x = newX - xOffset;
                    }
                    else
                    {
                        //Going right
                        x = newX + xOffset;
                    }

                    if (x < 0 || x >= xDim)
                        break;

                    if (_pieces[x, newY].IsColoured() && _pieces[x, newY].ColouredComponent.Colour == colour)
                    {
                        horizontalPieces.Add(_pieces[x, newY]);
                    }
                    else
                        break;
                }
            }

            if (horizontalPieces.Count >= 3)
            {
                for (int i = 0; i < horizontalPieces.Count; i++)
                {
                    matchingPieces.Add(horizontalPieces[i]);
                }
            }

            //Traverse vertically if we found a match (For L and T shape)
            if (horizontalPieces.Count >= 3)
            {
                for (int i = 0; i < horizontalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int yOffset = 1; yOffset < yDim; yOffset++)
                        {
                            int y;
                            if (dir == 0)
                            {
                                //Going Up
                                y = newY - yOffset;
                            }
                            else
                            {
                                //Going Down
                                y = newY + yOffset;
                            }

                            if (y < 0 || y >= yDim)
                                break;

                            if (_pieces[horizontalPieces[i].X, y].IsColoured() &&
                                _pieces[horizontalPieces[i].X, y].ColouredComponent.Colour == colour)
                            {
                                verticalPieces.Add(_pieces[horizontalPieces[i].X, y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (verticalPieces.Count < 2)
                    {
                        verticalPieces.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < verticalPieces.Count; j++)
                        {
                            matchingPieces.Add(verticalPieces[j]);
                        }

                        break;
                    }
                }
            }

            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }

            //Nothing Found Horizontal, Check Vertical
            horizontalPieces.Clear();
            verticalPieces.Clear();
            verticalPieces.Add(piece);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yOffset = 1; yOffset < yDim; yOffset++)
                {
                    int y;
                    if (dir == 0)
                    {
                        //Going Up
                        y = newY - yOffset;
                    }
                    else
                    {
                        //Going Down
                        y = newY + yOffset;
                    }

                    if (y < 0 || y >= yDim)
                        break;

                    if (_pieces[newX, y].IsColoured() && _pieces[newX, y].ColouredComponent.Colour == colour)
                    {
                        verticalPieces.Add(_pieces[newX, y]);
                    }
                    else
                        break;
                }
            }

            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    matchingPieces.Add(verticalPieces[i]);
                }
            }

            //Traverse horizontally if we found a match (For L and T shape)
            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xOffset = 1; xOffset < xDim; xOffset++)
                        {
                            int x;
                            if (dir == 0)
                            {
                                //Going Left
                                x = newY - xOffset;
                            }
                            else
                            {
                                //Going Right
                                x = newY + xOffset;
                            }

                            if (x < 0 || x >= xDim)
                                break;

                            if (_pieces[x, verticalPieces[i].Y].IsColoured() &&
                                _pieces[x, verticalPieces[i].Y].ColouredComponent.Colour == colour)
                            {
                                horizontalPieces.Add(_pieces[x, verticalPieces[i].Y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (horizontalPieces.Count < 2)
                    {
                        horizontalPieces.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < horizontalPieces.Count; j++)
                        {
                            matchingPieces.Add(horizontalPieces[j]);
                        }

                        break;
                    }
                }
            }

            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }
        }

        return null;
    }

    public bool ClearAllValidMatches()
    {
        bool needsRefill = false;
        for (int y = 0; y < yDim; y++)
        {
            for (int x = 0; x < xDim; x++)
            {
                if (_pieces[x, y].IsClearable())
                {
                    List<GamePiece> match = GetMatch(_pieces[x, y], x, y);
                    if (match != null)
                    {
                        PieceType specialPieceType = PieceType.COUNT;
                        GamePiece randomPiece = match[Random.Range(0, match.Count)];

                        int specialPieceX = randomPiece.X;
                        int specialPieceY = randomPiece.Y;

                        if (match.Count == 4)
                        {
                            if (_pressedPieces == null || _enteredPieces == null)
                            {
                                specialPieceType = (PieceType) Random.Range((int) PieceType.ROW_CLEAR,
                                    (int) PieceType.COLUMN_CLEAR);
                            }
                            else if (_pressedPieces.Y == _enteredPieces.Y)
                            {
                                specialPieceType = PieceType.ROW_CLEAR;
                            }
                            else
                            {
                                specialPieceType = PieceType.COLUMN_CLEAR;
                            }
                        }
                        else if (match.Count >= 5)
                        {
                            specialPieceType = PieceType.RAINBOW;
                        }

                        for (int i = 0; i < match.Count; i++)
                        {
                            if (ClearPiece(match[i].X, match[i].Y))
                            {
                                needsRefill = true;
                                if (match[i] == _pressedPieces || match[i] == _enteredPieces)
                                {
                                    specialPieceX = match[i].X;
                                    specialPieceY = match[i].Y;
                                    SoundManager.Instance.PlayMatchPopSound();
                                }
                            }
                        }

                        if (specialPieceType != PieceType.COUNT)
                        {
                            Destroy(_pieces[specialPieceX, specialPieceY]);
                            GamePiece newPiece = SpawnNewPiece(specialPieceX, specialPieceY, specialPieceType);

                            if ((specialPieceType == PieceType.ROW_CLEAR ||
                                 specialPieceType == PieceType.COLUMN_CLEAR) && newPiece.IsColoured() &&
                                match[0].IsColoured())
                            {
                                newPiece.ColouredComponent.SetColour(match[0].ColouredComponent.Colour);
                            }
                            else if (specialPieceType == PieceType.RAINBOW && newPiece.IsColoured())
                            {
                                newPiece.ColouredComponent.SetColour(ColouredPiece.ColourType.ANY);
                            }
                        }
                    }
                }
            }
        }

        return needsRefill;
    }

    public bool ClearPiece(int x, int y)
    {
        if (_pieces[x, y].IsClearable() && !_pieces[x, y].ClearableComponent.IsBeingCleared)
        {
            _pieces[x, y].ClearableComponent.Clear();
            SpawnNewPiece(x, y, PieceType.EMPTY);
            ClearObstacles(x, y);
            return true;
        }

        return false;
    }

    public void ClearObstacles(int x, int y)
    {
        //Check Horizontal adjacent pieces
        for (int adjacentX = x - 1; adjacentX <= x + 1; adjacentX++)
        {
            if (adjacentX != x && adjacentX >= 0 && adjacentX < xDim)
            {
                if (_pieces[adjacentX, y].Type == PieceType.BUBBLE && _pieces[adjacentX, y].IsClearable())
                {
                    _pieces[adjacentX, y].ClearableComponent.Clear();
                    SpawnNewPiece(adjacentX, y, PieceType.EMPTY);
                }
            }
        }

        // Check Vertical adjacent pieces
        for (int adjacentY = x - 1; adjacentY <= x + 1; adjacentY++)
        {
            if (adjacentY != x && adjacentY >= 0 && adjacentY < yDim)
            {
                if (_pieces[x, adjacentY].Type == PieceType.BUBBLE && _pieces[x, adjacentY].IsClearable())
                {
                    _pieces[x, adjacentY].ClearableComponent.Clear();
                    SpawnNewPiece(x, adjacentY, PieceType.EMPTY);
                }
            }
        }
    }

    public void ClearRow(int row)
    {
        SoundManager.Instance.PlayCoinSound();
        for (int x = 0; x < xDim; x++)
        {
            ClearPiece(x, row);
        }
    }

    public void ClearColumn(int column)
    {
        SoundManager.Instance.PlayCoinSound();
        for (int y = 0; y < yDim; y++)
        {
            ClearPiece(column, y);
        }
    }

    public void ClearColour(ColouredPiece.ColourType colour)
    {
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                if (_pieces[x, y].IsColoured() && (_pieces[x, y].ColouredComponent.Colour == colour) ||
                    colour == ColouredPiece.ColourType.ANY)
                {
                    ClearPiece(x, y);
                }
            }
        }
    }

    public void GameOver()
    {
        _gameOver = true;
    }

    public List<GamePiece> GetPiecesOfType(PieceType type)
    {
        List<GamePiece> piecesOfType = new List<GamePiece>();
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                if (_pieces[x, y].Type == type)
                {
                    piecesOfType.Add(_pieces[x, y]);
                }
            }
        }

        return piecesOfType;
    }
}