﻿using UnityEngine;

namespace Player
{
    internal sealed class PlayerMovement : MonoBehaviour
    {
        private float m_FurthestDistance;
        private float m_MinimumOffsetY = 3;
        private float m_MinHorizontalMovement = 0;
        private float m_MaxHorizontalMovement = 3;

        private Vector3 m_TargetPos;
        [SerializeField] private float m_MovementSpeed = 6;
        [SerializeField] private float m_MoveSpeed = 1;

        [SerializeField] private AudioClip m_MoveAudioClip;
        private AudioSource m_AudioSource;

        private bool m_ReversedControls;
        private float m_NextBonusRow = 47;
        private TrailRenderer m_TrailRenderer;

        private bool m_DisabledInput;

        private void Start()
        {
            var startHeight = transform.position.y;
            m_FurthestDistance = startHeight;
            m_TargetPos = transform.position;
            m_TrailRenderer = GetComponent<TrailRenderer>();
            m_AudioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (m_DisabledInput)
                return;

            AutoscrollPlayer();

            CalculateRowProgression(transform.position.y);

            if (transform.position != m_TargetPos)
            {
                transform.position = Vector3.MoveTowards(transform.position, m_TargetPos, m_MovementSpeed * Time.deltaTime);
            }
        }

        public void DisableInput()
        {
            m_DisabledInput = true;
        }

        /// <summary>
        /// Modifies the constant move speed on the Y-axis
        /// </summary>
        public void EditMoveSpeed(float speed)
        {
            m_MoveSpeed = speed;

            if (m_MoveSpeed > 1)
            {
                m_TrailRenderer.startColor = Color.red;
                m_TrailRenderer.endColor = Color.white;
            }
            else
            {
                m_TrailRenderer.startColor = Color.white;
            }
        }

        /// <summary>
        /// Rebounds the player in the direction, unless the player is at min or max X-axis
        /// to prevent the player being pushed outside of the boundaries
        /// </summary>
        public void Rebound(Vector3 direction)
        {
            if (m_TargetPos.x <= m_MinHorizontalMovement || m_TargetPos.x >= m_MaxHorizontalMovement)
            {
                m_TargetPos += Vector3.down;
                return;
            }

            m_TargetPos += direction;
        }

        /// <summary>
        /// Change target position based on direction of swipe
        /// </summary>
        public void HandleSwipeGesture(Vector3 direction)
        {
            if (m_ReversedControls)
            {
                direction *= -1;
            }

            var targetPos = m_TargetPos + direction;

            if (targetPos.x > m_MaxHorizontalMovement || targetPos.x < m_MinHorizontalMovement)
                return;

            if (targetPos.y < m_FurthestDistance - m_MinimumOffsetY)
            {
                GameManager.Instance.MinRestrictionLine.Play();
                return;
            }

            m_AudioSource.PlayOneShot(m_MoveAudioClip);
            m_TargetPos = targetPos;
        }

        /// <summary>
        /// Reverses the input controls
        /// </summary>
        public void ReverseInput(bool reversed)
        {
            m_ReversedControls = reversed;
        }

        /// <summary>
        /// Scroll the player upwards at a rate determined
        /// by the move speed field
        /// </summary>
        private void AutoscrollPlayer()
        {
            var myPos = transform.position;
            var scrollToPosition = new Vector3(myPos.x, myPos.y + m_MoveSpeed * Time.deltaTime, myPos.z);

            m_TargetPos = new Vector3(m_TargetPos.x, m_TargetPos.y + m_MoveSpeed * Time.deltaTime, m_TargetPos.z);
            transform.Translate(scrollToPosition - myPos);
        }

        /// <summary>
        /// Check that the player has progressed past the furthest distance
        /// then reward the player with a point per row
        /// </summary>
        private void CalculateRowProgression(float myPosY)
        {
            if (myPosY > m_FurthestDistance)
            {
                var currentY = Mathf.Floor(myPosY);
                var furthestY = Mathf.Floor(m_FurthestDistance);

                if (currentY > furthestY)
                {
                    GameManager.Instance.ScoreManager.IncreaseScore(1);
                }

                CalculateBonusRow(currentY);

                m_FurthestDistance = myPosY;
            }
        }

        /// <summary>
        /// Increase score each time the bonus row is passed,
        /// bonus points are incremented each bonus row
        /// </summary>
        private void CalculateBonusRow(float currentY)
        {
            if (currentY < m_NextBonusRow)
                return;

            var bonusScore = (int)(currentY / 5) + 1;

            GameManager.Instance.ScoreManager.IncreaseScore(bonusScore);
            m_NextBonusRow += 50;
        }
    }
}
