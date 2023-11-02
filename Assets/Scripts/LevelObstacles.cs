using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObstacles : Level
{
    public int numMoves;
    public Grid.PieceType[] obstacleTypes;
    
    private int _movesUsed = 0;
    private int _numOfObstaclesLeft;
    
    // Start is called before the first frame update
    void Start()
    {
        type = LevelType.OBSTACLE;
        hud.SetLevelType(type);
        hud.SetScore(currentScore);
        hud.SetTarget(_numOfObstaclesLeft);
        hud.SetRemaining(numMoves);
        
        for (int i=0; i < obstacleTypes.Length; i++)
        {
            _numOfObstaclesLeft += grid.GetPiecesOfType(obstacleTypes[i]).Count;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnMove()
    {
        _movesUsed++;
        hud.SetRemaining(numMoves - _movesUsed);
        if (numMoves - _movesUsed == 0 && _numOfObstaclesLeft > 0)
        {
            GameLoss();
        }
    }

    public override void OnPieceCleared(GamePiece piece)
    {
        base.OnPieceCleared(piece);

        for (int i = 0; i < obstacleTypes.Length; i++)
        {
            if (obstacleTypes[i] == piece.Type)
            {
                _numOfObstaclesLeft--;
                hud.SetTarget(_numOfObstaclesLeft);

                if (_numOfObstaclesLeft == 0)
                {
                    currentScore += 1000 * (numMoves - _movesUsed);
                    hud.SetScore(currentScore);
                    GameWin();
                }
            }
        }

    }
}
