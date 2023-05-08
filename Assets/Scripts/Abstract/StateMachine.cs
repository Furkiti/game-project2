using NaughtyAttributes;
using UnityEngine;

namespace Abstract
{
    public class StateMachine : MonoBehaviour
    {
        private BaseState _currentState;
        [ReadOnly]public string currentState;
        public BaseState CurrentState
        {
            get { return _currentState;}
            set { _currentState = value;}
        }

        public virtual void Start()
        {
            _currentState = GetInitialState();
            _currentState.Enter();
            currentState = _currentState.name;
        }

        public virtual void Update()
        {
            _currentState.UpdateLogic();
        }

        private void FixedUpdate()
        {
            _currentState.UpdatePhysics();
        }

        public virtual void LateUpdate()
        {
           
        }

        public void ChangeState(BaseState newState)
        {
            _currentState.Exit();

            _currentState = newState;
            _currentState.Enter();
            currentState = _currentState.name;
        }

        protected virtual BaseState GetInitialState()
        {
            return null;
        }
    }
}

