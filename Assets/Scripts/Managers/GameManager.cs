using Abstract;
using Managers.GMStates;
using Singleton;
using UnityEngine;

namespace Managers
{
    public class GameManager : SMSingleton<GameManager>
    {
        //Game States
        public GMLoad gmLoadState;
        public GMStart gmStartState;
        public GMCompleted gmCompletedState;
        public GMFailed gmFailedState;
        public GMReset gmResetState;
        
        private void Awake()
        {
            gmLoadState = new GMLoad(this);
            gmStartState = new GMStart(this);
            gmCompletedState = new GMCompleted(this);
            gmFailedState = new GMFailed(this);
            gmResetState = new GMReset(this);
        }
        

        protected override BaseState GetInitialState()
        {
            return gmLoadState;
        }
    }
}

