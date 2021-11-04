    public class GoCommand : Command
    {
        private readonly MoveObject moveObject;

        public GoCommand(MoveObject moveObject)
        {
            this.moveObject = moveObject;
        }
        public override void Execute()
        {
            moveObject.Go();
        }

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }
    }