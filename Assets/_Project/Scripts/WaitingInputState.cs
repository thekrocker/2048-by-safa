namespace _Project.Scripts
{
    public class WaitingInputState : BaseBoardState
    {
        public WaitingInputState(string stateName, BoardStateMachine machine) : base(stateName, machine)
        {
        }

        public override void Enter()
        {
            Machine.Board.OnBoardMoveCompleted += ChangeSpawnState;
        }

        private void ChangeSpawnState()
        {
            Machine.ChangeState(Machine.SpawnPieceState);
        }

        public override void Update()
        {
            Machine.Board.TryGetMoveInput();
        }

        public override void Exit()
        {
            Machine.Board.OnBoardMoveCompleted -= ChangeSpawnState;
        }
    }
}