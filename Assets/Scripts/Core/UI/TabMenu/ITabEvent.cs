using System;
using UnityEngine;

namespace Guinea.Core.UI
{
    public interface ITabEvent
    {
        event Action OnTabEnable;
        event Action OnTabDisable;
    }
}