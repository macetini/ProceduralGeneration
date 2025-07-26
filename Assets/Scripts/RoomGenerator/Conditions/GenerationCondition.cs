using Assets.Scripts.RoomGenerator.Conditions;
using UnityEngine;

namespace Assets.Scripts.RoomGenerator.Conditions
{
    public abstract class GenerationCondition : MonoBehaviour
    {
        public static readonly float ROTATION_AMOUNT = 90;

        public ConditionType type;

        protected RoomElement owner;

        public void SetOwner(RoomElement owner) => this.owner = owner;

        public abstract bool Test(ConditionData data);
    }
}
