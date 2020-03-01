﻿using System.Collections.Generic;
using Ads;
using Effects;
using Player;
using Score;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

internal sealed class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerMovement PlayerMovement;
    public TextMeshProUGUI ScoreText;
    public LivesUI LivesUI;
    public HealthObject HealthObject;
    public Animation MinRestrictionLine;
    public EffectsManager EffectsManager;
    public List<Vector2> Grid;
    public ScoreManager ScoreManager;

    // Ensure only one instance of the GameManager exists
    //
    public void Awake()
    {
        if (Instance != null)
            Destroy(this);

        if (Instance == null)
            Instance = this;

        ScoreManager = new ScoreManager();
    }

    private void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        AdsManager.Initialize();
    }

    public void GameOver()
    {
        AdsManager.ShowAd();

        PlayerPrefs.SetInt("CurrentScore", ScoreManager.Score);
        var currentScore = PlayerPrefs.GetInt("CurrentScore");
        var bestScore = PlayerPrefs.GetInt("BestScore", 0);

        if (currentScore > bestScore)
        {
            PlayerPrefs.SetInt("BestScore", currentScore);
            PlayerPrefs.Save();
        }

        SceneManager.LoadScene("GameOver");
    }
}