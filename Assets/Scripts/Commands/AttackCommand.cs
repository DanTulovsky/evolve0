    // The Attack command

    using UnityEngine;

    public class AttackCommand : Command
    {
        private readonly FighterObject fighterObject;
        private readonly GameObject target;

        public AttackCommand(FighterObject fighterObject, GameObject target)
        {
            this.fighterObject = fighterObject;
            this.target = target;
        }

        public override void Execute()
        {
           fighterObject.Attack(target);
        }

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }
    }