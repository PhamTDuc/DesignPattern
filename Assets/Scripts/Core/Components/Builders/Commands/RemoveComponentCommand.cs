using UnityEngine;
using Guinea.Core.Inventory;
using Zenject;

namespace Guinea.Core.Components
{
    public class RemoveComponentCommand : ICommand
    {
        public void Execute()
        {
            throw new System.NotImplementedException();
        }

        public void Undo()
        {
            throw new System.NotImplementedException();
        }
    }
}