using Abstract;

namespace Managers.GMStates
{
    public class GMLose : BaseState
    {
        private readonly GameManager _sm;

        public GMLose(GameManager stateMachine) : base("GMLose", stateMachine) {
            _sm = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();
        
            EventManager.OnGameFailed?.Invoke();
        }
    }
}
