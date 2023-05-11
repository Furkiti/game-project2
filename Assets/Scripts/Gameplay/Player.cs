using System.Collections;
using DG.Tweening;
using Managers;
using UnityEngine;

namespace Gameplay
{
    public class Player : MonoBehaviour
    {
        [SerializeField]private Transform player;
        [SerializeField]private Transform playerHolder;
        public AnimationController animationController;
        public float GroundCheckInterval;
        private LevelDatas levelDatas;
        private float playerSpeed;
        private WaitForSeconds waitForGroundCheckInterval;
        private bool running;
        private Rigidbody rb;

        private void Awake()
        {
            rb = playerHolder.GetComponent<Rigidbody>();
            waitForGroundCheckInterval = new WaitForSeconds(GroundCheckInterval);
        }

        private void OnEnable()
        {
            EventManager.OnGameStarted += OnGameStarted;
            EventManager.OnLevelReady+= OnLevelReady;
            EventManager.OnGameContinue += OnGameContinue;
            EventManager.OnGameReset += OnGameReset;
            EventManager.OnPathChange += OnPathChange;
        }
        
        private void OnDisable()
        {
            EventManager.OnGameStarted -= OnGameStarted;
            EventManager.OnLevelReady -= OnLevelReady;
            EventManager.OnGameContinue -= OnGameContinue;
            EventManager.OnGameReset -= OnGameReset;
            EventManager.OnPathChange -= OnPathChange;
        }
        private void ResetModel()
        {
            player.localPosition = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.transform.localPosition = Vector3.zero;
            animationController.CurrentAnimationState = AnimationController.AnimationState.Idle;
            player.DOKill();
        }

        private void OnLevelReady(LevelDatas levelDatas)
        {
            this.levelDatas = levelDatas;
            if (this.levelDatas.Speed < GroundCheckInterval)
            {
                GroundCheckInterval = this.levelDatas.Speed / 2f;
            }
            ResetModel();
        }
        
        private void OnPathChange(float xPosition)
        {
            player.DOLocalMoveX(xPosition, levelDatas.Speed / 2f);
        }

        private void OnGameReset()
        {
            ResetModel();
        }

        private void OnGameContinue()
        {
            ResetModel();
        }

        private void OnGameStarted()
        {
            DOVirtual.DelayedCall(levelDatas.Speed/2f, StartMoving);
        }

        private void StartMoving()
        {
            var finalPosition =  levelDatas.FinalPosition;
            var pieceSpeed= levelDatas.Length / levelDatas.Speed;
            playerSpeed = finalPosition / pieceSpeed;
            player.DOLocalMoveZ(finalPosition,playerSpeed )
                .SetEase(Ease.Linear).OnComplete(MoveComplete);
            animationController.CurrentAnimationState = AnimationController.AnimationState.Running;
            running = true;
            StartCoroutine(GroundCheck());
        }

        private void MoveComplete()
        {
            running = false;
            animationController.CurrentAnimationState = AnimationController.AnimationState.Dancing;
            GameManager.Instance.ChangeState(GameManager.Instance.gmCompletedState);
        }

        private void FailJump()
        {
            if(!running)
                return;
            StopAllCoroutines();
            running = false;
            rb.isKinematic = false;
            rb.useGravity = true;
            animationController.CurrentAnimationState = AnimationController.AnimationState.Falling;
            DOVirtual.Float(1, 7.5f,1.5f ,SlowDown)
                .SetDelay(1).OnComplete(TriggerFailEvent);
        }
        
        private void SlowDown(float val)
        {
            rb.drag = val;
        }

        private void TriggerFailEvent()
        {
            GameManager.Instance.ChangeState(GameManager.Instance.gmFailedState);
        }

        private IEnumerator GroundCheck()
        {
            while (running)
            {
                if (!Physics.Raycast(playerHolder.position+Vector3.up*levelDatas.Height/2f, Vector3.down, levelDatas.Height))
                {
                    player.DOKill();
                    FailJump();
                }
                yield return waitForGroundCheckInterval;
            }
        }
    }
}
