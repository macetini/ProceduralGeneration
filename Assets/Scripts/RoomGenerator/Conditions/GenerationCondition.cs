using Assets.Scripts.RoomGenerator.Conditions.Meta;
using UnityEngine;

namespace Assets.Scripts.RoomGenerator.Conditions
{
    //TODO - Switch this to interface
    public abstract class GenerationCondition : MonoBehaviour
    {
        public static readonly float ROTATION_AMOUNT = 90;

        public ConditionType type;

        protected RoomElement owner;

        public void SetOwner(RoomElement owner) => this.owner = owner;

        public abstract bool Test(ConditionData data);
    }
}
