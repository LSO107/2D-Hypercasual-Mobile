﻿using TMPro;
using UnityEngine;

namespace UI
{
    internal sealed class GameOverManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_BestScore;
        [SerializeField] private TextMeshProUGUI m_CurrentScore;

        private void Start()
        {
            m_BestScore.text = PlayerPrefs.GetFloat("BestScore").ToString();
            m_CurrentScore.text = PlayerPrefs.GetFloat("CurrentScore").ToString();
        }
    }
}
