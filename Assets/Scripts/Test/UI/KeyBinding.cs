using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Guinea.Core;

namespace Guinea.Test.UI
{
    public class KeyBinding : MonoBehaviour
    {
        [SerializeField] InputActionReference m_actionRef;
        [SerializeField] int m_bindingIndex;
        [SerializeField] InputBinding.DisplayStringOptions m_displayStringOptions;
        [Header("UI Fields")]
        [SerializeField] TextMeshProUGUI m_actionText;
        // [SerializeField] Button m_rebindButton;
        [SerializeField] TextMeshProUGUI m_rebindText;
        // [SerializeField] Button m_resetButton;
#if DEVELOPMENT
        [SerializeField] InputBinding m_inputBinding; // DEBUG  Using [SerializeField] for debugging only 
#else
        InputBinding m_inputBinding;
#endif
        string m_actionID; // * Save actionID from m_actionRef for later used

        void Awake()
        {
            GetBindingAndUpdateUI();
        }

        void GetBindingAndUpdateUI()
        {
            if (m_actionRef.action != null)
            {
                m_actionID = m_actionRef.action.id.ToString();
            }
            m_bindingIndex = Mathf.Min(Mathf.Max(m_bindingIndex, 0), m_actionRef.action.bindings.Count - 1); // ! Invalid when bindings is EMPTY
            m_inputBinding = m_actionRef.action.bindings[m_bindingIndex];

            // * Update UI
            if (m_actionText != null)
            {
                m_actionText.text = m_actionRef.action.name;
            }

            if (m_rebindText != null)
            {
                m_rebindText.text = m_actionRef.action.GetBindingDisplayString(m_bindingIndex, m_displayStringOptions);
            }
        }

        string GetBindingDisplayString(string actionID, int bindingIndex)
        {
            InputAction inputAction = InputManager.Map.asset.FindAction(actionID);
            return inputAction.GetBindingDisplayString(m_bindingIndex, m_displayStringOptions);
        }

        public void Rebind()
        {
            InputAction inputAction = InputManager.Map.asset.FindAction(m_actionID);
            inputAction.Rebind(m_bindingIndex, () => m_rebindText.text = "Press Any Key..", () => m_rebindText.text = GetBindingDisplayString(m_actionID, m_bindingIndex));
        }

        public void Reset()
        {
            InputAction inputAction = InputManager.Map.asset.FindAction(m_actionID);
            inputAction.RemoveAllBindingOverrides();
            m_rebindText.text = GetBindingDisplayString(m_actionID, m_bindingIndex);
        }

        // public string SaveBindingOverrideToString()
        // {
        //     InputAction inputAction = InputManager.Map.asset.FindAction(m_actionID);
        //     return inputAction.SaveBindingOverridesAsJson();
        // }

        // public void LoadBindingOverrideFromJson(string json, bool removeExisting = true)
        // {
        //     InputAction inputAction = InputManager.Map.asset.FindAction(m_actionID);
        //     inputAction.LoadBindingOverridesFromJson(json, removeExisting);
        // }
    }
}