using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearLinePieces : ClearablePiece
{
    public bool isRow;
    
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
        if (isRow)
        {
            //Clear Row
            piece.GridRef.ClearRow(piece.Y);
        }
        else
        {
            // Clear Column
            piece.GridRef.ClearColumn(piece.X);
        }
    }
}
