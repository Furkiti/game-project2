using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

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
        
        
        private float spawnXDistance;
        private Transform currentPiece;
        private Transform prevPiece;
        private bool inputEnabled;
        private TweenerCore<Vector3, Vector3, VectorOptions> tween;
        private LevelDatas levelDatas;
        private GameObject finish;
        private float finishLength;
        private int pieceCount;
        private float colorCount;
        //private PairedGradient targetColor;
        //private PairGradient sourceColor;
        
        private void OnEnable()
        {
            EventManager.OnGameStarted += OnGameStarted;
            EventManager.OnGameReset += OnGameReset;
            EventManager.OnGameFailed += OnGameFailed;
            EventManager.OnLevelReady += OnLevelReady;
        }

        private void OnDisable()
        {
            EventManager.OnGameStarted -= OnGameStarted;
            EventManager.OnGameReset -= OnGameReset;
            EventManager.OnGameFailed -= OnGameFailed;
            EventManager.OnLevelReady -= OnLevelReady;
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
            //first two object are finish line and starting piece respectively
            for (int i = stack.childCount - 1; i > 1; i--)
            {
                Destroy(stack.GetChild(i).gameObject);
            }

            currentPiece = stack.GetChild(1);
        }
        
        private void OnGameFailed()
        {
            inputEnabled = false;
            currentPiece.DOKill();
        }
        
        private void OnLevelReady(LevelDatas levelDatas)
        {
            pieceCount = 0;
            if (stack.childCount > 1) // previously completed a level
            {
                for (int i = stack.childCount - 1; i >= 0; i--)
                {
                    stack.GetChild(i).SetParent(stack, true);
                }

                var previousPos = stackPrevious.localPosition;
                var gap = finishLength + levelDatas.Length / 2f;
                previousPos.z -= this.levelDatas.FinalPosition + gap;
                stackPrevious.localPosition = previousPos;
                this.levelDatas = levelDatas;
                currentPiece = CreateStartingPiece(0);
                finish = Instantiate(finishLinePrefab, new Vector3(0, 0, levelDatas.FinalPosition),
                    Quaternion.identity, stack);
            }
            else // new load
            {
                this.levelDatas = levelDatas;
                finish = Instantiate(finishLinePrefab, new Vector3(0, 0, levelDatas.FinalPosition),
                    Quaternion.identity, stack);
                //GetRandomStartingGradient();
                currentPiece = CreateStartingPiece(0);
            }
        }
        
        private Transform CreateStartingPiece(float offsetZ)
        {
            var go = Instantiate(pieceBlockPrefab, stack).transform;
            go.localPosition = new Vector3(0, levelDatas.Height / -2f, levelDatas.Length * offsetZ);
            go.localScale = new Vector3(levelDatas.Width, levelDatas.Height, levelDatas.Length);
            //go.GetComponent<MeshRenderer>().material.color = GetNewColor();
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
            //piece.GetComponent<MeshRenderer>().material.color = GetNewColor();
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
            EventManager.OnPathChange?.Invoke(centeredPositionX);
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

        /*private Color GetNewColor()
        {
            colorCount++;
            if (colorCount >= targetColor.Length)
            {
                colorCount = 0;
                sourceColor = targetColor.Pair;
                targetColor = sourceColor.PairGradients[Random.Range(0, sourceColor.PairGradients.Count)];
            }

            return Color.Lerp(sourceColor.Color, targetColor.Pair.Color, colorCount / targetColor.Length);
        }

        private void GetRandomStartingGradient()
        {
            var index = Random.Range(0, Gradients.Count);
            sourceColor = Gradients[index];
            targetColor = sourceColor.PairGradients[Random.Range(0, sourceColor.PairGradients.Count)];
        }*/
        
    }
}
