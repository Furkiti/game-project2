using Abstract;
using Task = System.Threading.Tasks.Task;

namespace Managers.GMStates
{
    public class GMReset : BaseState
    {
        private readonly GameManager _sm;

        public GMReset(GameManager stateMachine) : base("GMReset", stateMachine) {
            _sm = stateMachine;
        }

        public override void Enter()
        {
            base.Enter();
            
            EventManager.OnGameReset?.Invoke();
            
        }

        private async void PrepareScene()
        {
            await Task.Delay(1500);
        
            // reset all game data
      
            _sm.ChangeState(_sm.gmLoadState);
        }
    }
}
