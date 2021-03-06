﻿using System.Collections;
using UnityEngine;

namespace Player
{
    internal sealed class HealthObject : MonoBehaviour
    {
        [SerializeField] [Range(1, 3)] private int m_LivesOnStart;
        [SerializeField] private GameObject m_DamageParticle;

        private SpriteRenderer m_SpriteRenderer;
        private Animation m_Anim;

        public HealthDefinition HealthDefinition;

        private bool m_IsInvincible;
        private Color m_InvincibilityColour;
        private Color m_NormalColour;

        private AudioSource m_AudioSource;
        [SerializeField] private AudioClip m_HitAudioClip;
        [SerializeField] private AudioClip m_LifeLostAudioClip;
        [SerializeField] private AudioClip m_GameOverAudioClip;

        private void Awake()
        {
            HealthDefinition = new HealthDefinition(m_LivesOnStart);
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_Anim = GetComponent<Animation>();
            m_AudioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            m_NormalColour = m_SpriteRenderer.color;
            m_InvincibilityColour = new Color(0.5f, 0.85f, 0.9f, 1);
        }

        public void Damage()
        {
            if (m_IsInvincible)
                return;

            m_AudioSource.PlayOneShot(m_HitAudioClip);
            Damage(HealthDefinition.MaxHealth);
        }

        public void Damage(int amount)
        {
            if (m_IsInvincible)
                return;

            HealthDefinition.Damage(amount);

            if (HealthDefinition.IsDead)
            {
                m_AudioSource.PlayOneShot(m_LifeLostAudioClip);
                HandleDeathFeedback();
            }

            if (HealthDefinition.Lives <= 0)
            {
                StartCoroutine(GameOver());
            }
        }

        private IEnumerator GameOver()
        {
            GetComponent<SpriteRenderer>().color = Color.red;
            GetComponent<PlayerMovement>().DisableInput();
            m_AudioSource.PlayOneShot(m_GameOverAudioClip);
            yield return new WaitWhile(() => m_AudioSource.isPlaying);
            GameManager.Instance.GameOver();
        }

        private void HandleDeathFeedback()
        {
            m_Anim.Play();
            Instantiate(m_DamageParticle, transform.position, Quaternion.identity);
        }

        public void Heal(int amount)
        {
            HealthDefinition.Heal(amount);
        }

        public void SetInvincibility(bool isInvincible)
        {
            m_IsInvincible = isInvincible;
            m_SpriteRenderer.color = m_IsInvincible ? m_InvincibilityColour : m_NormalColour;
        }
    }
}
