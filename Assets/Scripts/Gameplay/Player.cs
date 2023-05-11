using System;
using System.Collections;
using DG.Tweening;
using Managers;
using UnityEngine;

namespace Gameplay
{
    public class Player : MonoBehaviour
    {
        [SerializeField]private Transform playerTransform;
        [SerializeField]private Transform playerHolderTransform;
        [SerializeField] AnimationController animationController;
        [SerializeField]private Rigidbody playerRigidbody;
        private LevelDatas currentLevelDatas;
        private float playerSpeed;
        private bool isGrounded = true;
        
        private void OnEnable()
        {
            EventManager.OnGameStarted += OnGameStarted;
            EventManager.OnLevelReady+= OnLevelReady;
            EventManager.OnGameContinue += OnGameContinue;
            EventManager.OnGameReset += OnGameReset;
            EventManager.OnNewLineCreated += OnNewLineCreated;
        }
        
        private void OnDisable()
        {
            EventManager.OnGameStarted -= OnGameStarted;
            EventManager.OnLevelReady -= OnLevelReady;
            EventManager.OnGameContinue -= OnGameContinue;
            EventManager.OnGameReset -= OnGameReset;
            EventManager.OnNewLineCreated -= OnNewLineCreated;
        }
      

        private void OnLevelReady(LevelDatas levelDatas)
        {
            this.currentLevelDatas = levelDatas;
            PlayerModelAdjustment();
        }
        
        private void OnNewLineCreated(float newPos)
        {
            playerTransform.DOLocalMoveX(newPos, currentLevelDatas.Speed / 4f);
        }

        private void OnGameReset()
        {
            PlayerModelAdjustment();
        }

        private void OnGameContinue()
        {
            PlayerModelAdjustment();
        }

        private void OnGameStarted()
        {
            DOVirtual.DelayedCall(currentLevelDatas.Speed/2f, PlayerStartMoving);
        }
        
        private void PlayerModelAdjustment()
        {
            playerTransform.localPosition = Vector3.zero;
            playerRigidbody.isKinematic = true;
            playerRigidbody.useGravity = false;
            playerRigidbody.transform.localPosition = Vector3.zero;
            animationController.CurrentAnimationState = AnimationController.AnimationState.Idle;
            playerTransform.DOKill();
        }

        private void PlayerStartMoving()
        {
            var finalPosition =  currentLevelDatas.FinalPosition;
            var pieceSpeed= currentLevelDatas.Length / currentLevelDatas.Speed;
            playerSpeed = finalPosition / pieceSpeed;
            playerTransform.DOLocalMoveZ(finalPosition,playerSpeed )
                .SetEase(Ease.Linear).OnComplete(PlayerMovementDone);
            animationController.CurrentAnimationState = AnimationController.AnimationState.Running;
        }

        private void PlayerMovementDone()
        {
            animationController.CurrentAnimationState = AnimationController.AnimationState.Dancing;
            GameManager.Instance.ChangeState(GameManager.Instance.gmCompletedState);
        }

        private void PlayerFalling()
        {
            if(animationController.CurrentAnimationState != AnimationController.AnimationState.Running)
                return;
            
            StopAllCoroutines();
            playerRigidbody.isKinematic = false;
            playerRigidbody.useGravity = true;
            animationController.CurrentAnimationState = AnimationController.AnimationState.Falling;
            GameManager.Instance.ChangeState(GameManager.Instance.gmFailedState);
        }

        private void Update()
        {
            if (animationController.CurrentAnimationState == AnimationController.AnimationState.Running)
            {
                if (!Physics.Raycast(playerHolderTransform.position + Vector3.up * currentLevelDatas.Height / 2f, Vector3.down, currentLevelDatas.Height))
                {
                    if (isGrounded)
                    {
                        playerTransform.DOKill();
                        PlayerFalling();
                    }
                    isGrounded = false;
                }
                else
                {
                    isGrounded = true;
                }
            }
        }

        
    }
}
