    // The Attack command

    using UnityEngine;

    public class PostureCommand : Command
    {
        private readonly FighterObject fighterObject;
        private readonly GameObject target;

        public PostureCommand(FighterObject fighterObject, GameObject target)
        {
            this.fighterObject = fighterObject;
            this.target = target;
        }

        public override void Execute()
        {
           fighterObject.Posture(target);
        }

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }
    }