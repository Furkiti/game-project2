namespace Abstract
{
    
    public class BaseState
    {
        public readonly string name;
        protected StateMachine stateMachine;

        protected BaseState(string name, StateMachine stateMachine)
        {
            this.name = name;
            this.stateMachine = stateMachine;
        }

        public virtual void Enter() { }
        public virtual void UpdateLogic() { }
        public virtual void UpdatePhysics() { }
        public virtual void Exit() { }
    }

}
