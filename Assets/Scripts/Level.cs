using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public enum LevelType
    {
        TIMER,
        OBSTACLE,
        MOVES
    }

    public Grid grid;
    public HUD hud;

    public int score1Star;
    public int score2Star;
    public int score3Star;

    protected LevelType type;

    public LevelType Type
    {
        get { return type; }
    }

    protected int currentScore;
    protected bool didWin;

    // Start is called before the first frame update
    void Start()
    {
        hud.SetScore(currentScore);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public virtual void GameWin()
    {
        didWin = true;
        grid.GameOver();
        StartCoroutine(WaitForGridToFill());
    }

    public virtual void GameLoss()
    {
        grid.GameOver();
        didWin = false;
        StartCoroutine(WaitForGridToFill());
    }

    public virtual void OnMove()
    {
        
    }

    protected virtual IEnumerator WaitForGridToFill()
    {
        while (grid.IsFilling)
        {
            yield return 0;
        }

        if (didWin)
        {
            Debug.Log("You Win");
            hud.OnGameWin(currentScore);
        }
        else
        {
            Debug.Log("You Lose");
            hud.OnGameLoss();

        }
    }

    public virtual void OnPieceCleared(GamePiece piece)
    {
        //Update Score
        currentScore += piece.score;
        hud.SetScore(currentScore);
    }
}