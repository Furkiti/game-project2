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
        //public Model Model;
        public bool AdaptChanges;
        public float GroundCheckInterval;
        private LevelDatas param;
        private float playerSpeed;
        private WaitForSeconds waitForGroundCheckInterval;
        private bool running;
        private Rigidbody body;

        private void Awake()
        {
            body = playerHolder.GetComponent<Rigidbody>();
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
            body.isKinematic = true;
            body.useGravity = false;
            body.transform.localPosition = Vector3.zero;
            //Model.Idle();
            player.DOKill();
        }

        private void OnLevelReady(LevelDatas levelDatas)
        {
            param = levelDatas;
            if (param.Speed < GroundCheckInterval)
            {
                GroundCheckInterval = param.Speed / 2f;
            }
            ResetModel();
        }
        
        private void OnPathChange(float xPosition)
        {
            if(!AdaptChanges)
                return;
            player.DOLocalMoveX(xPosition, param.Speed / 2f);
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
            DOVirtual.DelayedCall(param.Speed/2f, StartMoving);
        }

        private void StartMoving()
        {
            var finalPosition =  param.FinalPosition;
            var pieceSpeed= param.Length / param.Speed;
            playerSpeed = finalPosition / pieceSpeed;
            player.DOLocalMoveZ(finalPosition,playerSpeed )
                .SetEase(Ease.Linear).OnComplete(MoveComplete);
            //Model.Run();
            running = true;
            StartCoroutine(GroundCheck());
        }

        private void MoveComplete()
        {
            running = false;
            //Model.Dance();
            EventManager.OnGameCompleted?.Invoke();
        }

        private void FailJump()
        {
            if(!running)
                return;
            StopAllCoroutines();
            running = false;
            body.isKinematic = false;
            body.useGravity = true;
            //Model.Fail();
            DOVirtual.Float(1, 7.5f,1.5f ,SlowDown)
                .SetDelay(1).OnComplete(TriggerFailEvent);
        }
        
        private void SlowDown(float val)
        {
            body.drag = val;
        }

        private void TriggerFailEvent()
        {
            EventManager.OnGameFailed?.Invoke();
        }

        IEnumerator GroundCheck()
        {
            while (running)
            {
                if (!Physics.Raycast(playerHolder.position+Vector3.up*param.Height/2f, Vector3.down, param.Height))
                {
                    player.DOKill();
                    FailJump();
                }
                yield return waitForGroundCheckInterval;
            }
        }
    }
}
