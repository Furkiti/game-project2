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
        
        
        private float spawnDistanceX;
        private Transform currentBlockTransform;
        private Transform previousPieceTransform;
        private bool enableInput;
        private TweenerCore<Vector3, Vector3, VectorOptions> blockTween;
        private LevelDatas levelDatas;
        private GameObject finish;
        private float finishLength;
        private int blockCount;
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
            enableInput = true;
            spawnDistanceX = levelDatas.Width;
            CreateNewBlock();
        }
        
        private void OnGameReset()
        {
            blockCount = 0;
            
            for (int i = stack.childCount - 1; i > 1; i--)
            {
                Destroy(stack.GetChild(i).gameObject);
            }
            
            currentBlockTransform = stack.GetChild(0);
        }
        
        private void OnGameFailed()
        {
            enableInput = false;
            currentBlockTransform.DOKill();
        }
        
        private void OnLevelReady(LevelDatas levelData)
        {
            blockCount = 0;
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
                currentBlockTransform = CreateStartingBlock(0);
                finish = Instantiate(finishLinePrefab, new Vector3(0, 0, levelData.FinalPosition),
                    Quaternion.identity, stack);
            }
            else
            {
                levelDatas = levelData;
                currentBlockTransform = CreateStartingBlock(0);
                finish = Instantiate(finishLinePrefab, new Vector3(0, 0, levelData.FinalPosition),
                    Quaternion.identity, stack);
            }
        }
        
        private Transform CreateStartingBlock(float offsetZ)
        {
            var go = Instantiate(pieceBlockPrefab, stack).transform;
            go.localPosition = new Vector3(0, levelDatas.Height / -2f, levelDatas.Length * offsetZ);
            go.localScale = new Vector3(levelDatas.Width, levelDatas.Height, levelDatas.Length);
            go.GetComponent<MeshRenderer>().material.color = GetNewColor();
            return go;
        }
        
        private void CreateNewBlock()
        {
            blockCount++;
            var block = Instantiate(currentBlockTransform, stack).transform;
            block.name = "block" + blockCount;
            var direction = GetRandomDirection();
            var currentPos = currentBlockTransform.localPosition;
            var currentScale = currentBlockTransform.localScale;
            var horizontalPosition = (spawnDistanceX + currentScale.x / 2f) * direction;
            var startingPosition = new Vector3(horizontalPosition, currentPos.y, currentPos.z + currentScale.z);
            block.localPosition = startingPosition;
            block.GetComponent<MeshRenderer>().material.color = GetNewColor();
            blockTween = block.DOLocalMoveX(-startingPosition.x, levelDatas.Speed).SetEase(Ease.Linear);
            previousPieceTransform = currentBlockTransform;
            currentBlockTransform = block;
        }
        
        
        private float GetRandomDirection()
        {
            return Random.value > 0.5f ? 1 : -1;
        }

        private void OnBlockPlaced()
        {
            if (blockCount >= levelDatas.PieceCount)
            {
                return;
            }

            CreateNewBlock();
            enableInput = true;
        }

        private void Update()
        {
            if (!enableInput)
                return;
            
            if (!Input.GetMouseButtonDown(0))
                return;
            
            enableInput = false;
            var remainingTime = 0f;
            if (blockTween.active)
            {
                remainingTime = blockTween.Duration() - blockTween.Elapsed();
                blockTween.Kill();
            }

            var currentPosition = currentBlockTransform.localPosition;
            var currentScale = currentBlockTransform.localScale;
            var prevPosition = previousPieceTransform.localPosition;
            var widthDifference = currentPosition.x - prevPosition.x;
            var absoluteWidthDifference = Mathf.Abs(widthDifference);


            if (absoluteWidthDifference < levelDatas.ToleranceWidth) 
            {
                currentPosition.x = prevPosition.x;
                currentBlockTransform.localPosition = currentPosition;
                DOVirtual.DelayedCall(remainingTime, OnBlockPlaced);
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
            currentBlockTransform.localScale = new Vector3(cutWidth, currentScale.y, currentScale.z);
            var centeredPositionX = currentPosition.x - widthDifference / 2f;
            currentBlockTransform.localPosition = new Vector3(centeredPositionX, currentPosition.y, currentPosition.z);
            EventManager.OnNewLineCreated?.Invoke(centeredPositionX);
            var cutPiece = Instantiate(currentBlockTransform, stack);
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
            DOVirtual.DelayedCall(remainingTime, OnBlockPlaced);
        }

        private Color GetNewColor()
        {
            colorCount++;
            return stackColors[(colorCount - 1) % stackColors.Length].color;
        }
        
    }
}
