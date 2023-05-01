namespace _Game_.Scripts.SM
{
    public abstract class BaseState
    {
        public readonly string StateName;

        protected BaseState(string stateName)
        {
            StateName = stateName;
        }

        protected virtual void TryChangeState() { }
        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();
    }
}