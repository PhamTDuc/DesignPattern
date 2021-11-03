using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Guinea.Core
{
    public static class Extensions
    {
        #region Rigidbody
        public static void AccelerateTo(this Rigidbody rb, Vector3 targetVelocity, float maxAccel)
        {
            Vector3 deltaV = targetVelocity - rb.velocity;
            Vector3 accel = deltaV / Time.deltaTime;

            if (accel.sqrMagnitude > maxAccel * maxAccel)
                accel = accel.normalized * maxAccel;

            rb.AddForce(accel, ForceMode.Acceleration);
        }
        #endregion

        #region InputAction
        public static void Rebind(this InputAction inputAction, int bindingIndex, Action onStart = null, Action onComplete = null, Action onCancel = null)
        {
            // * Validation: bindingIndex
            if (bindingIndex >= inputAction.bindings.Count)
            {
                inputAction.Enable();
                return;
            }

            inputAction.Disable();

            if (inputAction.bindings[bindingIndex].isComposite)
            {
                Rebind(inputAction, bindingIndex + 1, onStart, onComplete, onCancel);
                return;
            }


            var rebindingOperation = inputAction.PerformInteractiveRebinding(bindingIndex);

            rebindingOperation.OnComplete(operation =>
            {
                inputAction.Enable();
                operation.Dispose();

                if (inputAction.bindings[bindingIndex].isPartOfComposite)
                {
                    Rebind(inputAction, bindingIndex + 1, onStart, onComplete, onCancel);
                }
                onComplete?.Invoke();
            });

            rebindingOperation.OnCancel(operation =>
            {
                inputAction.Enable();
                operation.Dispose();
                onCancel?.Invoke();
            });

            rebindingOperation.WithCancelingThrough(@"<Keyboard>/escape"); // * Cancel interactiveRebinding when key is pressed

            onStart?.Invoke();
            rebindingOperation.Start();
        }

        public static string SaveToJson(this InputAction inputAction)
        {
            // TODO: Upgrade InputSystem package to 1.1.1
            throw new NotImplementedException();
        }

        public static void LoadFromJson(this InputAction inputAction)
        {
            // TODO: Upgrade InputSystem package to 1.1.1
            throw new NotImplementedException();
        }
        #endregion
        #region ComponentBase
        public static void SetParent(this Guinea.Core.Components.ComponentBase child, Guinea.Core.Components.ComponentBase parent, bool worldPositionStays = true)
        {
            child.transform.SetParent(parent.transform, worldPositionStays);
        }
        #endregion
    }
}