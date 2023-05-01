namespace _Project.Scripts
{
    public class WaitingInputState : BaseBoardState
    {
        public WaitingInputState(string stateName, BoardStateMachine machine) : base(stateName, machine)
        {
        }

        public override void Enter()
        {
            SwipeManager.Instance.EnableSwipe(true);
            Machine.Board.OnBoardMoveCompleted += ChangeSpawnPieceState;
        }

        private void ChangeSpawnPieceState()
        {
            Machine.ChangeState(Machine.SpawnPieceState);
        }

        public override void Update()
        {
            Machine.Board.TryGetMoveInput();
        }

        public override void Exit()
        {
            SwipeManager.Instance.EnableSwipe(false);
            Machine.Board.OnBoardMoveCompleted -= ChangeSpawnPieceState;
        }
    }
}