using Abstract;

namespace Managers.GMStates
{
    public class GMContinue : BaseState
    {
        private readonly GameManager _sm;

        public GMContinue(GameManager stateMachine) : base("GMContinue", stateMachine) {
            _sm = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();
            EventManager.OnGameContinue?.Invoke();
        }
    }
}