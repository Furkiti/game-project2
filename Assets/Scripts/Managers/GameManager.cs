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
        public GMWin gmWinState;
        public GMLose gmLoseState;
        public GMReset gmResetState;
        
        private void Awake()
        {
            gmLoadState = new GMLoad(this);
            gmStartState = new GMStart(this);
            gmWinState = new GMWin(this);
            gmLoseState = new GMLose(this);
            gmResetState = new GMReset(this);
        }
        

        protected override BaseState GetInitialState()
        {
            return gmLoadState;
        }
    }
}

