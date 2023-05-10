using Abstract;

namespace Managers.GMStates
{
    public class GMCompleted : BaseState
    {
        private readonly GameManager _sm;

        public GMCompleted(GameManager stateMachine) : base("GMCompleted", stateMachine) {
            _sm = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();
            EventManager.OnGameCompleted?.Invoke();
        }
    }
}
