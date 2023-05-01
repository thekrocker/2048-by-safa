using DG.Tweening;

namespace _Project.Scripts
{
    public class SpawnPieceState : BaseBoardState
    {
        public SpawnPieceState(string stateName, BoardStateMachine machine) : base(stateName, machine)
        {
        }

        public override void Enter()
        {
            Machine.Board.SpawnAfterBoardTurn().OnComplete(() =>
            {
                Machine.ChangeState(Machine.WaitingInputState);
            });
        }

        public override void Update()
        {
        }

        public override void Exit()
        {
        }
    }
}