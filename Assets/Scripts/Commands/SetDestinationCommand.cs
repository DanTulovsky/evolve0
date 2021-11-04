    using UnityEngine;

    public class SetDestinationCommand : Command
    {
        private readonly MoveObject moveObject;
        private readonly GameObject destination;

        public SetDestinationCommand(MoveObject moveObject, GameObject destination)
        {
            this.moveObject = moveObject;
            this.destination = destination;
        }
        public override void Execute()
        {
            moveObject.SetDestination(destination);
        }

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }
    }