using Abstract;

namespace Managers.GMStates
{
    public class GMFailed : BaseState
    {
        private readonly GameManager _sm;

        public GMFailed(GameManager stateMachine) : base("GMFailed", stateMachine) {
            _sm = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();
        
            EventManager.OnGameFailed?.Invoke();
        }
    }
}
