using Assets.Scripts.NodeGenerator.Conditions.Meta;
using UnityEngine;

namespace Assets.Scripts.NodeGenerator.Conditions
{
    //TODO - Switch to interface.
    public abstract class GenerationCondition : MonoBehaviour
    {
        public Node owner;

        public void SetOwner(Node owner) => this.owner = owner;

        public abstract bool Test(ConditionData data);
    }
}
