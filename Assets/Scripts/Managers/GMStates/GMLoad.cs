using Abstract;
using UnityEngine;

namespace Managers.GMStates
{
    public class GMLoad : BaseState
    {
        private readonly GameManager _sm;

        public GMLoad(GameManager stateMachine) : base("GMLoad", stateMachine) {
            _sm = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();
        
            EventManager.OnGameLoaded.Invoke();
            GameManager.Instance.ChangeState(GameManager.Instance.gmStartState);
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();

            /*if (Input.GetKey(KeyCode.I))
            {
                GameManager.Instance.ChangeState(GameManager.Instance.gmStartState);
            }*/
           
        }
    }
}
