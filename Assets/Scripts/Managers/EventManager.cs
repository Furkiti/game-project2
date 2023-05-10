using System;
using NaughtyAttributes;
using Scriptables;
using UnityEngine;

namespace Managers
{
    public static class EventManager
    {
        [Header("Dependencies")] 
        [Header("Game Events")] 
        public static Action OnGameLoaded;
        public static Action OnGameStarted;
        public static Action OnGameCompleted;
        public static Action OnGameFailed;
        public static Action OnGameReset;
        public static Action OnGameContinue;
        
        public static Action<int> OnLevelNumberChanged;
        public static Action<LevelDatas> OnLevelReady;
        public static Action<float> OnPathChange;


        public static Action<StackPlacement> OnStackBlockPlaced;





    }
}
    

