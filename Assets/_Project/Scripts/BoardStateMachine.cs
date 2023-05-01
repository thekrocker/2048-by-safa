using _Game_.Scripts.SM;

namespace _Project.Scripts
{
    public class BoardStateMachine : BaseStateMachine
    {
        public Board Board { get; private set; }
        
        public WaitingInputState WaitingInputState { get; private set; }
        public SpawnPieceState SpawnPieceState { get; private set; }
        
        protected override void InitStateMachine()
        {
            // Init Board
            Board = GetComponent<Board>();

            // Init States
            WaitingInputState = new WaitingInputState("Waiting Input", this);
            SpawnPieceState = new SpawnPieceState("Spawning Pieces", this);

        }

        protected override BaseState GetInitialState()
        {
            return WaitingInputState;
        }
    }
}