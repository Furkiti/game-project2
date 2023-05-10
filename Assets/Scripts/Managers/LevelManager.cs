using DG.Tweening;
using UnityEngine;

namespace Managers
{
    [SerializeField]
    public class LevelDatas
    {
        public float Width;
        public float Height;
        public float Length;
        public float ToleranceWidth;
        public float Speed;
        public int PieceCount;
        public float FinalPosition;
    }
    
    public class LevelManager : MonoBehaviour
    {
        private string levelStr = "level";
        private int initialLevel = 1;
        
        public float ReplayWaitDuration;
        public float PieceWidth;
        public float PieceHeight;
        public float PieceLength;
        public float ToleranceWidth;
        public float Speed;
        public int PieceCountMin;
        public int PieceCountMax;
        public GameObject FinishLinePrefab;
        private float finishLength;
        
        private int CurrentLevel
        {
            get
            {
                if (Save.Save.CheckSavedData(levelStr))
                {
                    return Save.Save.GetSavedIntData(levelStr);
                }

                Save.Save.SetSavedData(levelStr,initialLevel);
                return initialLevel;
            }

            set
            {
                Save.Save.SetSavedData(levelStr, value);
                EventManager.OnLevelNumberChanged?.Invoke(value);
            }
        }

        private void OnEnable()
        {
            EventManager.OnGameLoaded += OnGameLoaded;
            EventManager.OnGameContinue += OnContinueNewLevel;
            EventManager.OnGameFailed += OnGameFailed;
        }

        private void OnDisable()
        {
            EventManager.OnGameLoaded -= OnGameLoaded;
            EventManager.OnGameContinue -= OnContinueNewLevel;
            EventManager.OnGameFailed -= OnGameFailed;
        }

        private void OnGameLoaded()
        {
            finishLength = FinishLinePrefab.GetComponent<MeshRenderer>().bounds.extents.z;
            LoadNewLevel();
        }
        
        private void OnContinueNewLevel()
        {
            CurrentLevel++;
            LoadNewLevel();
        }
        
        private void OnGameFailed()
        {
            DOVirtual.DelayedCall(ReplayWaitDuration, TriggerReplay);
        }
        
        private void TriggerReplay()
        {
            EventManager.OnGameReset?.Invoke();
        }
        
        private void LoadNewLevel()
        {
            LevelDatas levelParameter = GetLevelDifficulty();
            levelParameter.FinalPosition =(PieceLength * levelParameter.PieceCount)+PieceLength/2f+finishLength;
            EventManager.OnLevelReady?.Invoke(levelParameter);
        }
        
        private LevelDatas GetLevelDifficulty()
        {
            var pieceCount = Random.Range(PieceCountMin, PieceCountMax);
           
            LevelDatas levelDatas = new LevelDatas()
            {
                Width = PieceWidth,
                Height = PieceHeight,
                Length = PieceLength,
                Speed = Speed,
                ToleranceWidth =ToleranceWidth,
                PieceCount = pieceCount
            };
            
            return levelDatas;
        }

      

       
        
        
    }
}
