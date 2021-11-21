using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Guinea.Core.Components
{
    public abstract class IOperator : MonoBehaviour, ICloneable
    {
        public abstract Option[] Options { get; }
        public virtual Result Invoke(Context context, InputActionMap inputActionMap) => Result.FINISHED;
        public virtual Result Modal(Context context, InputActionMap inputActionMap) => Result.FINISHED;
        public abstract Result Execute(Context context);
        public abstract Result Cancel(Context context);

        public object Clone()
        {
            Debug.Log($"Clone: {this.GetType().Name}");
            return this.MemberwiseClone();
        }

        public enum Exec
        {
            INVOKE = 0,
            EXECUTE
        }

        public enum Option
        {
            UNDO = 0,
        }

        public enum Result
        {
            RUNNING_MODAL = 0,
            FINISHED,
            CANCELLED,
            PASS_THROUGH
        }
    }
}