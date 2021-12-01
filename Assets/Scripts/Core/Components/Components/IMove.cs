using UnityEngine;

namespace Guinea.Core.Components
{
    public interface IMove
    {
        void Move(float steerAngle, float motorTorque, float brakeTorque);
    }
}