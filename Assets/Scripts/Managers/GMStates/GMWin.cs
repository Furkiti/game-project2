using Abstract;

namespace Managers.GMStates
{
    public class GMWin : BaseState
    {
        private readonly GameManager _sm;

        public GMWin(GameManager stateMachine) : base("GMWin", stateMachine) {
            _sm = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();
            EventManager.OnGameCompleted?.Invoke();
        }
    }
}
