using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions
{
    [Description("Returns true if someone is within the defined distance")]
    public class HaveSomeoneNearby : ConditionTask<Bird>
    {

        public BBParameter<GameObject> nearbyTarget;

        //Use for initialization. This is called only once in the lifetime of the task.
        //Return null if init was successful. Return an error string otherwise
        protected override string OnInit()
        {
            return null;
        }

        //Called whenever the condition gets enabled.
        protected override void OnEnable()
        {
        }

        //Called whenever the condition gets disabled.
        protected override void OnDisable()
        {
        }

        //Called once per frame while the condition is active.
        //Return whether the condition is success or failure.
        protected override bool OnCheck()
        {
            var nearBy = agent.GetNearby();

            if (nearBy.Count > 0)
            {
                GameObject randomEnemy = nearBy[Random.Range(0, nearBy.Count - 1)].gameObject;
                nearbyTarget.value = randomEnemy;
                Debug.LogFormat("nearby target: {0}", nearbyTarget.value);
                return true;
            }

            return false;
        }
    }
}