using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public enum StackPlacement 
    {
        Perfect,
        Cutted,
    }
    public class StackManager : MonoBehaviour
    {
        public GameObject pieceBlockPrefab;
        public GameObject finishLinePrefab;
        public Transform stack;
        public Transform stackPrevious;
        public float cuttedBlockKillTime;
        [SerializeField] private Material[] stackColors;
        
        
        private float spawnXDistance;
        private Transform currentPiece;
        private Transform prevPiece;
        private bool inputEnabled;
        private TweenerCore<Vector3, Vector3, VectorOptions> tween;
        private LevelDatas levelDatas;
        private GameObject finish;
        private float finishLength;
        private int pieceCount;
        private int colorCount;
       
        
        private void OnEnable()
        {
            EventManager.OnGameLoaded += OnGameLoad;
            EventManager.OnGameStarted += OnGameStarted;
            EventManager.OnGameReset += OnGameReset;
            EventManager.OnGameFailed += OnGameFailed;
            EventManager.OnLevelReady += OnLevelReady;
        }

        private void OnDisable()
        {
            EventManager.OnGameLoaded -= OnGameLoad;
            EventManager.OnGameStarted -= OnGameStarted;
            EventManager.OnGameReset -= OnGameReset;
            EventManager.OnGameFailed -= OnGameFailed;
            EventManager.OnLevelReady -= OnLevelReady;
        }

        private void OnGameLoad()
        {
            finishLength = finishLinePrefab.GetComponent<MeshRenderer>().bounds.extents.z;
        }
        
        private void OnGameStarted()
        {
            inputEnabled = true;
            spawnXDistance = levelDatas.Width;
            CreateNewPiece();
        }
        
        private void OnGameReset()
        {
            pieceCount = 0;
            
            for (int i = stack.childCount - 1; i > 1; i--)
            {
                Destroy(stack.GetChild(i).gameObject);
            }
            
            currentPiece = stack.GetChild(0);
        }
        
        private void OnGameFailed()
        {
            inputEnabled = false;
            currentPiece.DOKill();
        }
        
        private void OnLevelReady(LevelDatas levelData)
        {
            pieceCount = 0;
            if (stack.childCount > 1)
            {
                for (int i = stack.childCount - 1; i >= 0; i--)
                {
                    stack.GetChild(i).SetParent(stackPrevious, true);
                }

                var previousPos = stackPrevious.localPosition;
                var gap = finishLength + levelData.Length / 2f;
                previousPos.z -= levelDatas.FinalPosition + gap;
                stackPrevious.localPosition = previousPos;
                levelDatas = levelData;
                currentPiece = CreateStartingPiece(0);
                finish = Instantiate(finishLinePrefab, new Vector3(0, 0, levelData.FinalPosition),
                    Quaternion.identity, stack);
            }
            else
            {
                levelDatas = levelData;
                currentPiece = CreateStartingPiece(0);
                finish = Instantiate(finishLinePrefab, new Vector3(0, 0, levelData.FinalPosition),
                    Quaternion.identity, stack);
            }
        }
        
        private Transform CreateStartingPiece(float offsetZ)
        {
            var go = Instantiate(pieceBlockPrefab, stack).transform;
            go.localPosition = new Vector3(0, levelDatas.Height / -2f, levelDatas.Length * offsetZ);
            go.localScale = new Vector3(levelDatas.Width, levelDatas.Height, levelDatas.Length);
            go.GetComponent<MeshRenderer>().material.color = GetNewColor();
            return go;
        }
        
        private void CreateNewPiece()
        {
            pieceCount++;
            var piece = Instantiate(currentPiece, stack).transform;
            piece.name = "p" + pieceCount;
            var direction = GetRandomDirection();
            var currentPos = currentPiece.localPosition;
            var currentScale = currentPiece.localScale;
            var horizontalPosition = (spawnXDistance + currentScale.x / 2f) * direction;
            var startingPosition = new Vector3(horizontalPosition, currentPos.y, currentPos.z + currentScale.z);
            piece.localPosition = startingPosition;
            piece.GetComponent<MeshRenderer>().material.color = GetNewColor();
            tween = piece.DOLocalMoveX(-startingPosition.x, levelDatas.Speed).SetEase(Ease.Linear);
            prevPiece = currentPiece;
            currentPiece = piece;
        }
        
        
        private float GetRandomDirection()
        {
            return Random.value > 0.5f ? 1 : -1;
        }

        private void OnPiecePlaced()
        {
            if (pieceCount >= levelDatas.PieceCount)
            {
                return;
            }

            CreateNewPiece();
            inputEnabled = true;
        }

        private void Update()
        {
            if (!inputEnabled)
                return;
            
            if (!Input.GetMouseButtonDown(0))
                return;
            
            inputEnabled = false;
            var remainingTime = 0f;
            if (tween.active)
            {
                remainingTime = tween.Duration() - tween.Elapsed();
                tween.Kill();
            }

            var currentPosition = currentPiece.localPosition;
            var currentScale = currentPiece.localScale;
            var prevPosition = prevPiece.localPosition;
            var widthDifference = currentPosition.x - prevPosition.x;
            var absoluteWidthDifference = Mathf.Abs(widthDifference);


            if (absoluteWidthDifference < levelDatas.ToleranceWidth) 
            {
                currentPosition.x = prevPosition.x;
                currentPiece.localPosition = currentPosition;
                DOVirtual.DelayedCall(remainingTime, OnPiecePlaced);
                EventManager.OnStackBlockPlaced?.Invoke(StackPlacement.Perfect);
            }
            else
            {
                EventManager.OnStackBlockPlaced?.Invoke(StackPlacement.Cutted);
                CutPiece(currentScale, absoluteWidthDifference, currentPosition, widthDifference, remainingTime);
            }
        }

        private void CutPiece(Vector3 currentScale, float absWidthDifference, Vector3 currentPosition,
            float widthDifference, float remainingTime)
        {
            var cutWidth = currentScale.x - absWidthDifference;
            currentPiece.localScale = new Vector3(cutWidth, currentScale.y, currentScale.z);
            var centeredPositionX = currentPosition.x - widthDifference / 2f;
            currentPiece.localPosition = new Vector3(centeredPositionX, currentPosition.y, currentPosition.z);
            EventManager.OnNewLineCreated?.Invoke(centeredPositionX);
            var cutPiece = Instantiate(currentPiece, stack);
            var cutScale = cutPiece.localScale;
            cutScale.x = absWidthDifference;
            var cutDirection = Mathf.Sign(widthDifference);
            cutPiece.localScale = cutScale;
            var cutCenteredPositionX = centeredPositionX + cutDirection * ((cutWidth + cutScale.x) / 2f);
            cutPiece.localPosition = new Vector3(cutCenteredPositionX, currentPosition.y, currentPosition.z);
            var body = cutPiece.GetComponent<Rigidbody>();
            body.useGravity = true;
            body.isKinematic = false;
            body.AddForce(new Vector3(cutDirection * levelDatas.Length, 0, 0), ForceMode.Impulse);
            Destroy(cutPiece.gameObject, cuttedBlockKillTime);
            DOVirtual.DelayedCall(remainingTime, OnPiecePlaced);
        }

        private Color GetNewColor()
        {
            colorCount++;
            return stackColors[(colorCount - 1) % stackColors.Length].color;
        }
        
    }
}
