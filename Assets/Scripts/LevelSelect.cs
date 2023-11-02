using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [System.Serializable]
    public struct ButtonPlayerPrefs
    {
        public GameObject gameObject;
        public string playerPrefKey;
    }

    public ButtonPlayerPrefs[] buttons;

    public Sprite[] numbers;

    public GameObject coinTexts;

    // Start is called before the first frame update
    void Start()
    {
        int coins = PlayerPrefs.GetInt("CoinCount", 0);
        CreateSpriteCoinNumbers(coins);

        foreach (var button in buttons)
        {
            int score = PlayerPrefs.GetInt(button.playerPrefKey, 0);

            for (int starIdx = 1; starIdx <= 3; starIdx++)
            {
                Transform star = button.gameObject.transform.Find("star" + starIdx);

                if (starIdx <= score)
                    star.gameObject.SetActive(true);
                else
                    star.gameObject.SetActive(false);
            }

            if (button.playerPrefKey.Contains("4") && GetCombinedStarCount() >= 9)
            {
                button.gameObject.transform.Find("Locked").gameObject.SetActive(false);
            }
        }
    }

    private void CreateSpriteCoinNumbers(int coins)
    {
        Debug.Log("The number of coins " + coins);

        char[] num = coins.ToString().ToCharArray();
        Sprite[] nums = new Sprite[num.Length];
        int i = 0;
        foreach (var c in num)
        {
            int cNum = Int32.Parse(c.ToString());
            nums[i++] = numbers[cNum];
        }

        int j = nums.Length;
        foreach (var sprite in nums)
        {
            CreateScoreGameObject(sprite, --j);
        }
    }

    private void CreateScoreGameObject(Sprite sprite, int offset)
    {
        float baseX = 614f;
        float baseY = 357f;
        float scale = 74f;
        int multiplier = 62;
        GameObject number = new GameObject
        {
            layer = coinTexts.layer
        };

        Image image = number.AddComponent<Image>();
        image.sprite = sprite;
        RectTransform rect = number.GetComponent<RectTransform>();
        rect.SetParent(coinTexts.transform);
        rect.sizeDelta = new Vector2(scale, scale);
        number.transform.localPosition = new Vector2(baseX - (multiplier * offset), baseY);
        number.transform.localScale = Vector3.one;
        number.name = sprite.name;
        number.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnButtonPress(string levelName)
    {
        if (levelName.Contains("4") && GetCombinedStarCount() < 9)
        {
            Debug.Log("You cannot play this level yet, you need " + (9 - GetCombinedStarCount()) + " more stars");
            return;
        }

        SoundManager.Instance.PlayCoinSound();
        SceneManager.LoadScene(levelName);
    }

    private int GetCombinedStarCount()
    {
        int count = 0;
        foreach (var button in buttons)
        {
            int score = PlayerPrefs.GetInt(button.playerPrefKey, 0);

            for (int starIdx = 1; starIdx <= 3; starIdx++)
            {
                if (starIdx <= score)
                {
                    count++;
                }
            }
        }

        return count;
    }
}