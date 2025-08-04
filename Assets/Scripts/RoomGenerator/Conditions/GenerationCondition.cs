using Assets.Scripts.RoomGenerator.Conditions.Meta;
using UnityEngine;

namespace Assets.Scripts.RoomGenerator.Conditions
{
    //TODO - Switch to interface.
    public abstract class GenerationCondition : MonoBehaviour
    {
        public RoomElement owner;

        public void SetOwner(RoomElement owner) => this.owner = owner;

        public abstract bool Test(ConditionData data);
    }
}
