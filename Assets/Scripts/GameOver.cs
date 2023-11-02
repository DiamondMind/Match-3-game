using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public GameObject screenParent;
    public GameObject scoreParent;

    public Text loseText;
    public Text scoreText;

    public Image[] stars;
    public int unitStar;

    public Text coinsText;

    // Start is called before the first frame update
    void Start()
    {
        screenParent.SetActive(false);
        foreach (var star in stars)
        {
            star.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ShowLose(int starCount)
    {
        screenParent.SetActive(true);
        scoreParent.SetActive(false);
        Animator animator = GetComponent<Animator>();
        if (animator)
        {
            animator.Play("GameOverShow");
        }
        
        ShowStarsAndCoins(starCount);
    }

    public void ShowWin(int score, int starCount)
    {
        screenParent.SetActive(true);
        loseText.enabled = false;

        scoreText.text = score.ToString();
        scoreText.enabled = false;

        Animator animator = GetComponent<Animator>();
        if (animator)
        {
            animator.Play("GameOverShow");
            SoundManager.Instance.PlayStarFillUpSound();
            SoundManager.Instance.PlayLevelFinishSound();
        }
        ShowStarsAndCoins(starCount);
    }

    private void ShowStarsAndCoins(int starCount)
    {
        int levelCoins = starCount * unitStar;
        int coins = PlayerPrefs.GetInt("CoinCount", 0);
        coins += levelCoins;
        PlayerPrefs.SetInt("CoinCount", coins);

        coinsText.text = $"{SceneManager.GetActiveScene().name} : {levelCoins} \nTotal : {coins}";

        StartCoroutine(ShowCoroutine(starCount));
    }

    private IEnumerator ShowCoroutine(int starCount)
    {
        yield return new WaitForSeconds(0.5f);
        if (starCount < stars.Length)
        {
            for (int i = 0; i <= starCount; i++)
            {
                stars[i].enabled = true;
                if (i > 0)
                {
                    stars[i - 1].enabled = false;
                }

                yield return new WaitForSeconds(0.5f);
            }
        }

        scoreText.enabled = true;
    }

    public void OnReplayClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnDoneClicked()
    {
        SceneManager.LoadScene("MainScene");
    }
}