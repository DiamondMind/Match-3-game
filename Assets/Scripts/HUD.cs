using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Level level;
    public GameOver gameOver;

    public Text remainingText;
    public Text remainingSubText;
    public Text targetText;
    public Text targetSubText;
    public Text scoreText;
    public Image[] stars;

    private int _previousVisibleStar = -1;

    private int _starIdx = 0;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            if (i == _starIdx)
            {
                stars[i].enabled = true;
            }
            else
            {
                stars[i].enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
        int visibleStar = 0;

        if (score >= level.score1Star && score < level.score2Star)
        {
            visibleStar = 1;
        }
        else if (score >= level.score2Star && score < level.score3Star)
        {
            visibleStar = 2;
        }
        else if (score >= level.score3Star)
        {
            visibleStar = 3;
        }

        for (int i = 0; i < stars.Length; i++)
        {
            if (i == visibleStar)
            {
                if (visibleStar != _previousVisibleStar)
                {
                    SoundManager.Instance.PlayStarFillUpSound();
                }

                _previousVisibleStar = visibleStar;
                stars[i].enabled = true;
            }
            else
            {
                stars[i].enabled = false;
            }
        }

        _starIdx = visibleStar;
    }

    public void SetTarget(int target)
    {
        targetText.text = target.ToString();
    }

    public void SetRemaining(int remaining)
    {
        remainingText.text = remaining.ToString();
    }

    public void SetRemaining(string remaining)
    {
        remainingText.text = remaining;
    }

    public void SetLevelType(Level.LevelType type)
    {
        if (type == Level.LevelType.MOVES)
        {
            remainingText.text = "moves remaining";
            targetSubText.text = "target score";
        }
        else if (type == Level.LevelType.OBSTACLE)
        {
            remainingText.text = "moves remaining";
            targetSubText.text = "bubbles score";
        }
        else if (type == Level.LevelType.TIMER)
        {
            remainingText.text = "time remaining";
            targetSubText.text = "target score";
        }
    }

    public void OnGameWin(int score)
    {
        gameOver.ShowWin(score, _starIdx);

        if (_starIdx > PlayerPrefs.GetInt(SceneManager.GetActiveScene().name, 0))
        {
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, _starIdx);
        }
    }

    public void OnGameLoss()
    {
        gameOver.ShowLose(_starIdx);
    }
}