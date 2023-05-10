using Abstract;
using UnityEngine;

namespace Managers.GMStates
{
    public class GMStart : BaseState
    {
        private readonly GameManager _sm;

        public GMStart(GameManager stateMachine) : base("GMStart", stateMachine) {
            _sm = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();
            EventManager.OnGameStarted.Invoke();
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            
            if (Input.GetKey(KeyCode.U))
            {
                GameManager.Instance.ChangeState(GameManager.Instance.gmFailedState);
            }
            else if (Input.GetKey(KeyCode.P))
            {
                GameManager.Instance.ChangeState(GameManager.Instance.gmCompletedState);
            }
        }
    }
}
