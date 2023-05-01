using _Game_.Scripts.SM;

namespace _Project.Scripts
{
    public abstract class BaseBoardState : BaseState
    {
        protected BoardStateMachine Machine;
        
        public BaseBoardState(string stateName, BoardStateMachine machine) : base(stateName)
        {
            Machine = machine;
        }
        
    }
}