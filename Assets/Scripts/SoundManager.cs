using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;

    public AudioClip levelFinishSound;
    public AudioClip matchPopSound;
    public AudioClip specialPieceSound;
    public AudioClip coinsSound;
    public AudioClip fillUpSound;
    public AudioClip starFillSound;

    public static SoundManager Instance
    {
        get { return _instance; }
    }

    private AudioSource _audioSource;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void PlayLevelFinishSound()
    {
        _audioSource.PlayOneShot(levelFinishSound);
    }

    public void PlayMatchPopSound()
    {
        _audioSource.PlayOneShot(matchPopSound, 1.0f);
    }

    public void PlaySpecialPieceSound()
    {
        _audioSource.PlayOneShot(specialPieceSound);
    }

    public void PlayCoinSound()
    {
        _audioSource.PlayOneShot(coinsSound);
    }

    public void PlayFillUpSound()
    {
        _audioSource.PlayOneShot(fillUpSound, 0.08f);
    }

    public void PlayStarFillUpSound()
    {
        _audioSource.PlayOneShot(starFillSound, 1.0f);
    }
}