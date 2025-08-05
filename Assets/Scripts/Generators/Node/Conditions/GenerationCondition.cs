using Assets.Scripts.Generators.Node.Conditions.Meta;
using UnityEngine;

namespace Assets.Scripts.Generators.Node.Conditions
{
    //TODO - Switch to interface.
    public abstract class GenerationCondition : MonoBehaviour
    {
        public Node owner;

        //TODO - Implement in all conditions.
        public bool Negate = false;

        public void SetOwner(Node owner) => this.owner = owner;

        public abstract bool Test(ConditionData data);
    }
}
