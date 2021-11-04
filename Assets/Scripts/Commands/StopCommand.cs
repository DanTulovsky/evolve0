    public class StopCommand : Command
    {
        private readonly MoveObject moveObject;

        public StopCommand(MoveObject moveObject)
        {
            this.moveObject = moveObject;
        }
        public override void Execute()
        {
            moveObject.Stop();
        }

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }
    }