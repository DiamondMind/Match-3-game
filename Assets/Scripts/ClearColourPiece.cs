using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearColourPiece : ClearablePiece
{
    private ColouredPiece.ColourType _colour;

    public ColouredPiece.ColourType Colour
    {
        get { return _colour; }
        set
        {
            _colour = value;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Clear()
    {
        base.Clear();
        
        //Clear pieces of same Colour
        piece.GridRef.ClearColour(_colour);
    }
}
