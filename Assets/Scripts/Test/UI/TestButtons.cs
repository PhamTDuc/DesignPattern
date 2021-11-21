using UnityEngine;
using Guinea.Core.Components;

namespace Guinea.Test.UI
{
    public class TestButtons : MonoBehaviour
    {
#if UNITY_EDITOR
        public void TestOpProp()
        {
            // OperatorManager.Execute<AddOperator>(IOperator.Exec.EXECUTE);
        }

        public void Undo()
        {
            OperatorManager.Undo();
        }

        public void Redo()
        {
            OperatorManager.Redo();
        }
#endif
    }
}