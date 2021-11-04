    using UnityEngine;

    public class RunAwayCommand : Command
    {
        private readonly MoveObject moveObject;
        private readonly GameObject otherObject;

        public RunAwayCommand(MoveObject moveObject, GameObject otherObject)
        {
            this.moveObject = moveObject;
            this.otherObject = otherObject;
        }
        public override void Execute()
        {
            moveObject.RunAwayFrom(otherObject);
        }

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }
    }