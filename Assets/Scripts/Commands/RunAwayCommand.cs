    using UnityEngine;

    public class RunAwayCommand : Command
    {
        private readonly MoveObject moveObject;
        private GameObject otherObject;

        public RunAwayCommand(MoveObject moveObject, GameObject otherObject)
        {
            this.moveObject = moveObject;
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