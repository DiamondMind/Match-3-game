using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    public int score;
    
    private int _x;
    private int _y;

    public int X
    {
        get { return _x; }
        set
        {
            if (IsMoveable())
                _x = value;
        }
    }

    public int Y
    {
        get { return _y; }
        set
        {
            if (IsMoveable())
                _y = value;
        }
    }

    private Grid.PieceType _type;

    public Grid.PieceType Type
    {
        get { return _type; }
    }

    private Grid _grid;

    public Grid GridRef
    {
        get { return _grid; }
    }

    private MoveablePiece _moveableComponent;

    public MoveablePiece MoveableComponent
    {
        get { return _moveableComponent; }
    }

    private ColouredPiece _colouredComponent;

    public ColouredPiece ColouredComponent
    {
        get { return _colouredComponent; }
    }
    
    private ClearablePiece _clearableComponent;

    public ClearablePiece ClearableComponent
    {
        get { return _clearableComponent; }
    }

    private void Awake()
    {
        _moveableComponent = GetComponent<MoveablePiece>();
        _colouredComponent = GetComponent<ColouredPiece>();
        _clearableComponent = GetComponent<ClearablePiece>();
    }

    private void OnMouseEnter()
    {
        _grid.EnterPiece(this);
    }

    private void OnMouseDown()
    {
        _grid.PressPiece(this);
    }

    private void OnMouseUp()
    {
        _grid.ReleasePiece();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Init(int x, int y, Grid grid, Grid.PieceType type)
    {
        _x = x;
        _y = y;
        _grid = grid;
        _type = type;
    }

    public bool IsMoveable()
    {
        return _moveableComponent != null;
    }

    public bool IsColoured()
    {
        return _colouredComponent != null;
    }

    public bool IsClearable()
    {
        return _clearableComponent != null;
    }
}